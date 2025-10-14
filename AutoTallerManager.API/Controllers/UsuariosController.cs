using System;
using Microsoft.AspNetCore.RateLimiting;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoTallerManager.Application.Abstractions;
using AutoTallerManager.Application.Abstractions.Auth;
using AutoTallerManager.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AutoTallerManager.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsuariosController : ControllerBase
    {
        [HttpGet]
        [Authorize]
        public IActionResult GetAll()
        {
            return Ok(Array.Empty<object>());
        }

        [HttpGet("{id}")]
        [Authorize]
        public IActionResult GetById(int id)
        {
            return Ok(new { id });
        }

        [HttpPost]
        [Authorize]
        public IActionResult Create()
        {
            return Created(string.Empty, new { });
        }

        [HttpPut("{id}")]
        [Authorize]
        public IActionResult Update(int id)
        {
            return Ok(new { id });
        }

        [HttpPut("{id}/cambiar-password")]
        [Authorize]
        public IActionResult ChangePassword(int id)
        {
            return Ok(new { id });
        }

        [HttpPut("{id}/activar")]
        [Authorize]
        public IActionResult Activate(int id)
        {
            return Ok(new { id });
        }

        [HttpPut("{id}/desactivar")]
        [Authorize]
        public IActionResult Deactivate(int id)
        {
            return Ok(new { id });
        }
    }
}
