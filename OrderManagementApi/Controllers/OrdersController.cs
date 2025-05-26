using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrderManagementApi.Data;
using OrderManagementApi.Models;
using System.Security.Claims;

namespace OrderManagementApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("orders")]
    public class OrdersController : ControllerBase
    {
        private readonly AppDbContext _context;

        public OrdersController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> Create(Order order)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            order.UserId = userId;
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();
            return Ok(order);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var order = await _context.Orders.Include(o => o.User).FirstOrDefaultAsync(o => o.Id == id);
            if (order == null) return NotFound();
            return Ok(order);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, Order orderUpdate)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null) return NotFound();

            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var userRole = User.FindFirstValue(ClaimTypes.Role);

            if (order.UserId != userId && userRole != "Admin")
                return Forbid();

            order.Title = orderUpdate.Title;
            order.Description = orderUpdate.Description;
            await _context.SaveChangesAsync();
            return Ok(order);
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null) return NotFound();

            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
