using Microsoft.AspNetCore.Mvc;
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
        private readonly IRabbitmqClient _rabbitmqClient;

        public DirectCardPaymentController(IRabbitmqClient rabbitmqClient)
        {
            _rabbitmqClient = rabbitmqClient;
        }
        
        [HttpPost]
        public IActionResult MakePayment([FromBody] CardPayment payment)
        {
            _rabbitmqClient.SendPayment(payment);
            return Ok(payment);
        }
    }
}
