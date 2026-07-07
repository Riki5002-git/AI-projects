using Microsoft.AspNetCore.Mvc;
using NotificationService.Data;
using NotificationService.Models;

namespace NotificationService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NotificationsController : ControllerBase
    {
        private readonly NotificationDbContext _context;

        public NotificationsController(NotificationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult GetAll() => Ok(_context.Notifications.ToList());

        [HttpPost]
        public IActionResult Create(Notification notification)
        {
            notification.SentAt = DateTime.UtcNow;
            _context.Notifications.Add(notification);
            _context.SaveChanges();
            return CreatedAtAction(nameof(GetAll), new { id = notification.Id }, notification);
        }
    }
}