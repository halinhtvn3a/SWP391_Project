using Microsoft.AspNetCore.Mvc;
using Services;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TrainingController : ControllerBase
    {
        private readonly ModelTrainingService _modelTrainingService;

        public TrainingController(ModelTrainingService modelTrainingService)
        {
            _modelTrainingService = modelTrainingService;
        }

        [HttpPost("train-model")]
        public IActionResult TrainModel()
        {
            try
            {
                _modelTrainingService.TrainAndSaveModel();
                return Ok("Model trained and saved successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
