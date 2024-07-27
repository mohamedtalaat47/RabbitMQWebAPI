using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using RabbitMQWebAPI.Services.RabbitMQServices;

namespace RabbitMQWebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MessageController : ControllerBase
    {
        private readonly RabbitMQService _rabbitMQService;
        private static string? _latestMessage;

        public MessageController(RabbitMQService rabbitMQService)
        {
            _rabbitMQService = rabbitMQService;
        }

        [HttpPost]
        public IActionResult Post([FromBody] string message)
        {
            _rabbitMQService.PublishMessage(message, "myQueue");
            return Ok("Hello from myQ");
        }

        [HttpGet("consume")]
        public IActionResult Consume()
        {
            _rabbitMQService.ConsumeMessages("myQueue", message =>
            {
                _latestMessage = message;
                Console.WriteLine($"Received message: {message}");
            });

            return Ok("Consumer started");
        }

        [HttpGet("latest")]
        public IActionResult GetLatestMessage()
        {
            return Ok(_latestMessage);
        }
    }
}
