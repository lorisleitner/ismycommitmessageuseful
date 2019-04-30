using ismycommitmessageuseful.Database;
using ismycommitmessageuseful.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ismycommitmessageuseful.Controllers
{
    [Route("api/commits")]
    [ApiController]
    public class RateController : Controller
    {
        private readonly Context _context;

        private readonly ILogger _logger;

        public RateController(Context context, ILogger<RateController> logger)
        {
            _context = context;

            _logger = logger;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<CommitDto>>> Get()
        {
            // We use raw SQL, because there is no suitable random function to use with LINQ
            return await _context
                .Commits
                .AsNoTracking()
                .FromSql("SELECT" +
                "\"Id\"" +
                ",\"Message\"" +
                ",\"UsefulCount\"" +
                ",\"NotUsefulCount\"" +
                ",\"DontKnowCount\"" +
                ",\"xmin\"" +
                "FROM\"Commits\"" +
                "ORDER BY random()" +
                "LIMIT 50")
                .Select(x => new CommitDto
                {
                    Id = x.Id,
                    Message = x.Message
                })
                .ToListAsync()
                .ConfigureAwait(false);
        }

        [HttpPost("{id}/useful")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> Useful(int id)
        {
            var commit = _context.Commits.SingleOrDefault(x => x.Id == id);
            if (commit == null)
                return NotFound();

            while (true)
            {
                try
                {
                    ++commit.UsefulCount;

                    await _context.SaveChangesAsync().ConfigureAwait(false);

                    _logger.LogInformation("UsefulCount of commit {CommidId} was updated to {UsefulCount}", commit.Id, commit.UsefulCount);

                    break;
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    _logger.LogError(ex, "Concurrent update of commit {CommitId} occurred. Retrying...", commit.Id);

                    foreach (var entry in ex.Entries)
                        await entry.ReloadAsync().ConfigureAwait(false);
                }
            }

            return Ok();
        }

        [HttpPost("{id}/notuseful")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> NotUseful(int id)
        {
            var commit = _context.Commits.SingleOrDefault(x => x.Id == id);
            if (commit == null)
                return NotFound();

            while (true)
            {
                try
                {
                    ++commit.NotUsefulCount;

                    await _context.SaveChangesAsync().ConfigureAwait(false);

                    _logger.LogInformation("NotUsefulCount of commit {CommidId} was updated to {NotUsefulCount}", commit.Id, commit.NotUsefulCount);

                    break;
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    _logger.LogError(ex, "Concurrent update of commit {CommitId} occurred. Retrying...", commit.Id);

                    foreach (var entry in ex.Entries)
                        await entry.ReloadAsync().ConfigureAwait(false);
                }
            }

            return Ok();
        }

        [HttpPost("{id}/dontknow")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> DontKnow(int id)
        {
            var commit = _context.Commits.SingleOrDefault(x => x.Id == id);
            if (commit == null)
                return NotFound();

            while (true)
            {
                try
                {
                    ++commit.DontKnowCount;

                    await _context.SaveChangesAsync().ConfigureAwait(false);

                    _logger.LogInformation("DontKnowCount of commit {CommidId} was updated to {DontKnowCount}", commit.Id, commit.DontKnowCount);

                    break;
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    _logger.LogError(ex, "Concurrent update of commit {CommitId} occurred. Retrying...", commit.Id);

                    foreach (var entry in ex.Entries)
                        await entry.ReloadAsync().ConfigureAwait(false);
                }
            }

            return Ok();
        }
    }
}
