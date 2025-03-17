using BankMicroservices.Notification.Data.ValueObjects;
using BankMicroservices.Notification.Repository;
using BankMicroservices.Notification.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BankMicroservices.Notification.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class NotificationController : ControllerBase
    {
        private INotificationRepository _repository;

        public NotificationController(INotificationRepository repository)
        {
            _repository = repository ?? throw new
                ArgumentNullException(nameof(repository));
        }

        [HttpPatch("{id}")]
        [Authorize]
        public async Task<ActionResult> SetAsRead(long id)
        {
            var userId = User.Claims.Where(u => u.Type == "sub")?.FirstOrDefault()?.Value;
            bool done = await _repository.SetAsRead(id, userId);
            return done? Ok() : BadRequest();
        }

        [HttpGet]
        [Authorize(Roles = Role.Admin)]
        public async Task<ActionResult<IEnumerable<NotificationVO>>> FindAll()
        {
            var notifications = await _repository.FindAll();
            return Ok(notifications);
        }
    }
}
