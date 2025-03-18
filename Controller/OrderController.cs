using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using OrderService.Data;
using OrderService.Models;
using OrderService.Services;

namespace OrderService.Controller
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrderController : ControllerBase
    {
        private readonly OrderDbContext _context;
        private readonly OrderPublisher _publisher;

        public OrderController(OrderDbContext context, OrderPublisher publisher)
        {
            _context = context;
            _publisher = publisher;
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrder([FromBody] Order order)
        {
            if (order == null || order.Quantity <= 0 || order.ProductId <= 0)
            {
                return BadRequest("Invalid order details.");
            }

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            // Publish to RabbitMQ
            _publisher.PublishOrder(order);

            return Ok(order);
        }

        [HttpGet]
        public IActionResult GetOrders()
        {
            var orders = _context.Orders.ToList();
            return Ok(orders);
        }
    }
}