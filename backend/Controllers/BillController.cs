using backend.Service.implementations;
using backend.Service.interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BillController : ControllerBase
    {
        private readonly IBillService _billService;
        public BillController(IBillService billService)
        {
            _billService = billService;
        }
        [HttpGet]
        public async Task<IActionResult> GetAllBills()
        {
            var response = await _billService.GetAll();
            return StatusCode(response.statusCode, response);
        }
    }
}
