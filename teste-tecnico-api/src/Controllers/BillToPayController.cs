using Microsoft.AspNetCore.Mvc;
using teste_tecnico_api.src.Dtos;
using teste_tecnico_api.src.Models;
using teste_tecnico_api.src.Services;

namespace teste_tecnico_api.src.Controllers
{
    [ApiController]
    [Route("api")]
    public class BillToPayController(BillToPayService billToPayService) : ControllerBase
    {
        private readonly BillToPayService _billToPayService = billToPayService;

        [HttpGet("bills_to_pay")]
        public IActionResult GetAllBillsToPay()
        {
            try
            {
                var billsToPay = _billToPayService.GetAllBillsToPay();
                return Ok(billsToPay);
            }
            catch (Exception e)
            {
                return StatusCode(500, new { Message = $"Erro: {e.Message}" });
            }
        }

        [HttpPost("new_bill_to_pay")]
        public IActionResult CreateBillToPay([FromBody] CreateBillToPayDto billToPay)
        {
            try
            {
                _billToPayService.CreateBillToPay(billToPay);
                
                return Ok(new { Message = "Conta a pagar inserido com sucesso" });
            }
            catch (Exception e)
            {
                return StatusCode(500, new { Message = $"{e.Message}" });
            }
        }
    }
}
