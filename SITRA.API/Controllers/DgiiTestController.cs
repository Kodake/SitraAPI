using Microsoft.AspNetCore.Mvc;
using SITRA.Core.Interfaces;
using System.Threading.Tasks;

namespace SITRA.API.Controllers
{
    [ApiController]
    [Route("api/dgiiTests")]
    public class DgiiTestController : ControllerBase
    {
        private readonly IDgiiTestService _dgiiTestService;

        public DgiiTestController(IDgiiTestService dgiiTestService)
        {
            _dgiiTestService = dgiiTestService;
        }

        [HttpPost("run-tests")]
        public async Task<IActionResult> RunDgiiTests([FromQuery] string? ecfType = null)
        {
            try
            {
                var results = await _dgiiTestService.RunTestsAsync(ecfType);
                return Ok(new 
                { 
                    Message = "Ronda de pruebas completada", 
                    Results = results 
                });
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, new 
                { 
                    Error = ex.Message, 
                    Stack = ex.StackTrace 
                });
            }
        }
    }
}
