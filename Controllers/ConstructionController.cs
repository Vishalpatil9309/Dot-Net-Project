using Microsoft.AspNetCore.Mvc;
using ConstructionAPI.Models;
using ConstructionAPI.Services;

namespace ConstructionAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ConstructionController : ControllerBase
    {
        private readonly IConstructionDataService _constructionDataService;
        private readonly ILogger<ConstructionController> _logger;

        public ConstructionController(IConstructionDataService constructionDataService, ILogger<ConstructionController> logger)
        {
            _constructionDataService = constructionDataService;
            _logger = logger;
        }

        /// <summary>
        /// Get all construction observation data
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<ConstructionObservation>> GetConstructionData()
        {
            try
            {
                var data = await _constructionDataService.GetConstructionDataAsync();
                if (data == null)
                {
                    return NotFound("Construction data not found");
                }
                return Ok(data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving construction data");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Get construction data by sampling time
        /// </summary>
        [HttpGet("by-sampling-time")]
        public async Task<ActionResult<ConstructionData>> GetConstructionDataBySamplingTime([FromQuery] DateTime samplingTime)
        {
            try
            {
                var data = await _constructionDataService.GetConstructionDataBySamplingTimeAsync(samplingTime);
                if (data == null)
                {
                    return NotFound($"Construction data not found for sampling time {samplingTime}");
                }
                return Ok(data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving construction data by sampling time");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Update construction data for a specific sampling time
        /// </summary>
        [HttpPut("update")]
        public async Task<ActionResult> UpdateConstructionData([FromBody] UpdateConstructionDataRequest request)
        {
            try
            {
                if (request == null || request.Properties == null || !request.Properties.Any())
                {
                    return BadRequest("Invalid request data");
                }

                var success = await _constructionDataService.UpdateConstructionDataAsync(request.SamplingTime, request.Properties);
                if (success)
                {
                    return Ok(new { message = "Construction data updated successfully" });
                }
                else
                {
                    return StatusCode(500, "Failed to update construction data");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating construction data");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Get all unique sampling times
        /// </summary>
        [HttpGet("sampling-times")]
        public async Task<ActionResult<List<DateTime>>> GetSamplingTimes()
        {
            try
            {
                var data = await _constructionDataService.GetConstructionDataAsync();
                if (data == null)
                {
                    return NotFound("Construction data not found");
                }

                var samplingTimes = data.Datas.Select(d => d.SamplingTime).OrderBy(t => t).ToList();
                return Ok(samplingTimes);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving sampling times");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}