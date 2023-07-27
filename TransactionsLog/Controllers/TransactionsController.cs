using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TransactionsLog.Services.TransactionsLogger;
using TransactionsLog.Services.TransactionsReportGenerator;

namespace TransactionsLog.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionsController : ControllerBase
    {

        private readonly ITransactionsLogger _transactionsLogger;
        private readonly ITransactionsReportGenerator _transactionsReportGenerator;

        public TransactionsController(ITransactionsLogger transactionsLogger, ITransactionsReportGenerator transactionsReportGenerator)
        {
            _transactionsLogger = transactionsLogger;
            _transactionsReportGenerator = transactionsReportGenerator;
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

        [HttpGet("report")]
        public async Task<IActionResult> GenerateReport()
        {
            return Ok(await _transactionsReportGenerator.Generate());
        }
    }
}
