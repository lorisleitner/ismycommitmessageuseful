using AspNetCoreRateLimit;
using ismycommitmessageuseful.Configuration;
using ismycommitmessageuseful.Database;
using ismycommitmessageuseful.ML;
using ismycommitmessageuseful.Models;
using ismycommitmessageuseful.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ismycommitmessageuseful
{
    public class Startup
    {
        private readonly ILogger _logger;

        public Startup(IConfiguration configuration, ILogger<Startup> logger)
        {
            Configuration = configuration;

            _logger = logger;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<Context>(options =>
            {
                var databaseUri = new Uri(Configuration["DATABASE_URL"]);

                var connectionStringBuilder = new NpgsqlConnectionStringBuilder();

                connectionStringBuilder.Host = databaseUri.Host;
                connectionStringBuilder.Port = databaseUri.Port;

                var userInfo = databaseUri.UserInfo.Split(":");
                connectionStringBuilder.Username = userInfo[0];
                connectionStringBuilder.Password = userInfo[1];

                connectionStringBuilder.Database = databaseUri.Segments[1];

                connectionStringBuilder.ApplicationName = "ismycommitmessageuseful-api";

                // Heroku only allows 20 concurrent connection, but 10 are enough for us
                connectionStringBuilder.MaxPoolSize = 10;

                options.UseNpgsql(connectionStringBuilder.ToString());
            });

            services.AddScoped<IPooledPredictionEngine<CommitInput, CommitPrediction>>(ctx =>
            {
                var memoryCache = ctx.GetRequiredService<IMemoryCache>();

                // A prediction request can arrive before model generation was completed (model is generated on every startup)
                // so we need to spin here and wait until the model is ready to predict

                IPooledPredictionEngine<CommitInput, CommitPrediction> engine;

                do
                {
                    engine = memoryCache.Get<IPooledPredictionEngine<CommitInput, CommitPrediction>>(CacheKeys.PredictionEngine);
                }
                while (engine == null);

                return engine;
            });

            services.AddMemoryCache();

            services.AddSingleton<IHostedService, UpdateModelService>();

            services.Configure<IpRateLimitOptions>(options =>
            {
                options.EnableEndpointRateLimiting = true;
                options.QuotaExceededResponse = new QuotaExceededResponse
                {
                    Content = "{{\"message\":\"API quota exceeded\",\"retryAfter\":\"{2}\"}}",
                    ContentType = "application/json",
                    StatusCode = 429
                };
                options.GeneralRules = new List<RateLimitRule>
                {
                    new RateLimitRule
                    {
                        Endpoint = "*:/api/predict",
                        Limit = 1,
                        Period = "1s"
                    },
                    new RateLimitRule
                    {
                        Endpoint = "*:/api/commits/",
                        Limit = 1,
                        Period = "5s"
                    },
                    new RateLimitRule
                    {
                        Endpoint = "*:/api/commits/*/useful",
                        Limit = 1,
                        Period = "5s"
                    },
                    new RateLimitRule
                    {
                        Endpoint = "*:/api/commits/*/notuseful",
                        Limit = 1,
                        Period = "5s"
                    },
                    new RateLimitRule
                    {
                        Endpoint = "*:/api/commits/*/dontknow",
                        Limit = 1,
                        Period = "5s"
                    },
                };
            });

            services.AddSingleton<IRateLimitConfiguration, EndpointRateLimiterConfiguration>();

            services.AddSingleton<IIpPolicyStore, MemoryCacheIpPolicyStore>();
            services.AddSingleton<IRateLimitCounterStore, MemoryCacheRateLimitCounterStore>();

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            services.AddCors();

            services.AddMvc()
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
        }

        public void Configure(IApplicationBuilder app, Microsoft.AspNetCore.Hosting.IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler(exceptionApp =>
                {
                    exceptionApp.Run(context =>
                    {
                        context.Response.StatusCode = StatusCodes.Status500InternalServerError;

                        var contextFeature = context.Features.Get<IExceptionHandlerFeature>();
                        if (contextFeature != null)
                        {
                            _logger.LogError(contextFeature.Error, "Unhandled exception occurred");
                        }

                        return Task.CompletedTask;
                    });
                });

                app.UseHsts();
                app.UseIpRateLimiting();
            }
            
            app.UseHttpsRedirection();
            app.UseCors(builder =>
            {
                builder
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader();
            });
            app.UseMvc();
        }
    }
}
