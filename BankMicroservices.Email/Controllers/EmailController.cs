using BankMicroservices.Email.Data.ValueObjects;
using BankMicroservices.Email.Repository;
using BankMicroservices.Email.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BankMicroservices.Email.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class EmailController : ControllerBase
    {
        private IEmailRepository _repository;

        public EmailController(IEmailRepository repository)
        {
            _repository = repository ?? throw new
                ArgumentNullException(nameof(repository));
        }

        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IEnumerable<EmailVO>>> FindAll()
        {
            var userIsAdmin = User.Claims.Where(u => u.Type == "role" && u.Value == Role.Admin)?.FirstOrDefault() != null;
            if (!userIsAdmin) return Forbid();
            var products = await _repository.FindAll();
            return Ok(products);
        }
    }
}
