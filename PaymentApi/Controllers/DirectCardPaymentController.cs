using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PaymentApi.Models;
using PaymentApi.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PaymentApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DirectCardPaymentController : ControllerBase
    {
        private readonly ILogger<DirectCardPaymentController> _logger;
        private readonly IRabbitmqDirectClient _rabbitmqClient;

        public DirectCardPaymentController(ILogger<DirectCardPaymentController> logger, IRabbitmqDirectClient rabbitmqClient)
        {
            _logger = logger;
            _rabbitmqClient = rabbitmqClient;
        }
        
        [HttpPost]
        public IActionResult MakePayment([FromBody] CardPayment payment)
        {
            try
            {
                var result = _rabbitmqClient.MakePayment(payment);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return BadRequest("Error during the operation");
            }
            
        }
    }
}
