using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Mt.Contracts;

namespace publisher_api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class OrderController : ControllerBase
    {
        private readonly ILogger<OrderController> _logger;
        private readonly IRequestClient<IProcessOrder> _requestClient;

        public OrderController(ILogger<OrderController> logger, IRequestClient<IProcessOrder> requestClient)
        {
            _logger = logger;
            _requestClient = requestClient;
        }

        [HttpPost]
        public async Task<IActionResult> Post(int orderId)
        {
            var respuesta = await _requestClient.GetResponse<OrderProcessedOk>(new
            {
                OrderId = orderId,
                Date = DateTime.Now
            });
            return Ok(respuesta);
        }
    }
}
