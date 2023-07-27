using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TransactionsLog.Services.TransactionsLogger;

namespace TransactionsLog.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionsController : ControllerBase
    {

        private readonly ITransactionsLogger _transactionsLogger;

        public TransactionsController(ITransactionsLogger transactionsLogger)
        {
            _transactionsLogger = transactionsLogger;
        }

        [HttpPost("upload")]
        public async Task<IActionResult> UploadFile(IFormFile file)
        {
            if (file == null)
                return BadRequest("Arquivo não informado");

            var loggingResult = await _transactionsLogger.FromFile(file.OpenReadStream());
            if (!loggingResult.Success)
                return BadRequest(loggingResult.ErrorMessages);
            return NoContent();
        }
    }
}
