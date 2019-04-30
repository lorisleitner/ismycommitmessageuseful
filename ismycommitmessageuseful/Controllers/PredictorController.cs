using ismycommitmessageuseful.ML;
using ismycommitmessageuseful.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;

namespace ismycommitmessageuseful.Controllers
{
    [ApiController]
    [Route("api")]
    public class PredictorController : Controller
    {
        private readonly IPooledPredictionEngine<CommitInput, CommitPrediction> _predictionEngine;

        public PredictorController(IPooledPredictionEngine<CommitInput, CommitPrediction> predictionEngine)
        {
            _predictionEngine = predictionEngine;
        }

        [HttpGet("predict")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<CommitPredictionDto> Predict([FromQuery]CommitInputDto commitInputDto)
        {
            var commitInput = new CommitInput()
            {
                Message = commitInputDto.Message
            };

            var prediction = _predictionEngine.Predict(commitInput);

            return Ok(new CommitPredictionDto()
            {
                Usefulness = Math.Clamp(prediction.Usefulness, 0.0f, 100.0f)
            });
        }
    }
}
