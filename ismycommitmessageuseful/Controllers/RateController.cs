using ismycommitmessageuseful.Database;
using ismycommitmessageuseful.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ismycommitmessageuseful.Controllers
{
    [Route("api")]
    [ApiController]
    public class RateController : Controller
    {
        private readonly Context _context;

        public RateController(Context context)
        {
            _context = context;
        }

        [HttpGet("commits")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<CommitDto>>> Get()
        {
            // I use raw SQL, because there is no suitable random function to use with LINQ
            return await _context
                .Commits
                .AsNoTracking()
                .FromSql("SELECT * FROM " +
                    "(SELECT \"Id\",\"Message\",\"UsefulCount\",\"NotUsefulCount\",\"DontKnowCount\",\"xmin\"" +
                    "FROM \"Commits\"" +
                    "ORDER BY \"UsefulCount\" + \"NotUsefulCount\"" +
                    "LIMIT 50) AS \"x\"" +
                    "ORDER BY random()")
                .Select(x => new CommitDto
                {
                    Id = x.Id,
                    Message = x.Message
                })
                .ToListAsync()
                .ConfigureAwait(false);
        }
    }
}
