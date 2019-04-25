﻿using ismycommitmessageuseful.ML;
using ismycommitmessageuseful.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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
        public ActionResult<CommitPredictionDto> Predict([FromBody]CommitInputDto commitInputDto)
        {
            var commitInput = new CommitInput()
            {
                Message = commitInputDto.Message
            };

            var prediction = _predictionEngine.Predict(commitInput);

            return Ok(new CommitPredictionDto()
            {
                Usefulness = prediction.Usefulness
            });
        }
    }
}