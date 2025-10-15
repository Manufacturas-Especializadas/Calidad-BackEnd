using Calidad.Dtos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Rechazos.Models;

namespace Calidad.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClientsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ClientsController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Route("GetClients")]
        public async Task<IActionResult> GetClients()
        {
            var list = await _context.Clients
                    .AsNoTracking()
                    .ToListAsync();

            if(list == null)
            {
                return BadRequest("Lista vacia");
            }

            return Ok(list);
        }

        [HttpGet]
        [Route("GetClientById/{id}")]
        public async Task<IActionResult> GetClientById(int id)
        {
            var client = await _context.Clients.FirstOrDefaultAsync(c => c.Id == id);

            if(client == null)
            {
                return BadRequest("Id no encotrado");
            }

            return Ok(client);
        }

        [HttpPost]
        [Route("RegisterClient")]
        public async Task<IActionResult> RegisterClient([FromBody] ClientsDto dto)
        {
            if(dto == null)
            {
                return BadRequest("No se acepta datos vacios");
            }

            var newClient = new Clients
            {
                Name = dto.Name,
            };

            _context.Clients.Add(newClient);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                success = true,
                message = "Cliente registrado",
                clientId = newClient.Id
            });
        }

        [HttpPut]
        [Route("UpdateClient/{id}")]
        public async Task<IActionResult> UpdateClient([FromBody] ClientsDto dto, int id)
        {
            var IdClient = await _context.Clients.FindAsync(id);

            if(IdClient == null)
            {
                return NotFound("Id no encontrado");
            }

            IdClient.Name = dto.Name;

            await _context.SaveChangesAsync();

            return Ok(new
            {
                success = true,
                message = "Cliente actualizada"
            });
        }

        [HttpDelete]
        [Route("DeleteClient/{id}")]
        public async Task<IActionResult> DeleteClient(int id)
        {
            var IdClient = await _context.Clients.FindAsync(id);

            if (IdClient == null)
            {
                return NotFound("Id no encontrado");
            }

            _context.Clients.Remove(IdClient);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                success = true,
                message = "Cliente eliminado"
            });
        }
    }
}