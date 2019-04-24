using ismycommitmessageuseful.Database;
using ismycommitmessageuseful.ML;
using ismycommitmessageuseful.Models;
using ismycommitmessageuseful.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Npgsql;
using System;

namespace ismycommitmessageuseful
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
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

                options.UseNpgsql(connectionStringBuilder.ToString());
            });

            services.AddScoped<IPooledPredictionEngine<CommitInput, CommitPrediction>>(ctx =>
            {
                return new PooledPredictionEngine<CommitInput, CommitPrediction>(null, -1);
            });

            services.AddMemoryCache();

            services.AddSingleton<IHostedService, UpdateModelService>();

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
        }

        public void Configure(IApplicationBuilder app, Microsoft.AspNetCore.Hosting.IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}
