using Calidad.Dtos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Rechazos.Models;

namespace Calidad.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DefectsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public DefectsController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Route("GetDefects")]
        public async Task<IActionResult> GetDefects()
        {
            var list = await _context.RjDefects
                            .AsNoTracking()
                            .ToListAsync();

            if(list == null)
            {
                return BadRequest("Lista vacia");
            }

            return Ok(list);
        }

        [HttpGet]
        [Route("GetDefectById/{id}")]
        public async Task<IActionResult> GetDefectById(int id)
        {
            var IdDefect = await _context.RjDefects.FirstOrDefaultAsync(d => d.Id == id);

            if(IdDefect == null)
            {
                return BadRequest("Id no encontrado");
            }

            return Ok(IdDefect);
        }

        [HttpPost]
        [Route("RegisterDefect")]
        public async Task<IActionResult> RegisterDefect([FromBody] DefectsDto dto)
        {
            if(dto == null)
            {
                return BadRequest("Campos vacios");
            }

            var newDefect = new RjDefects
            {
                Name = dto.Name,
            };

            _context.RjDefects.Add(newDefect);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                success = true,
                message = "Defecto agregado correctamente",
                IdDefect = newDefect.Id
            });
        }

        [HttpPut]
        [Route("UpdateDefect/{id}")]
        public async Task<IActionResult> UpdateDefect([FromBody] DefectsDto dto, int id)
        {
            var IdDefect = await _context.RjDefects.FindAsync(id);

            if( IdDefect == null)
            {
                return BadRequest("Id no encontrado");
            }

            IdDefect.Name = dto.Name;

            await _context.SaveChangesAsync();

            return Ok(new
            {
                success = true,
                message = "Dato actualizado"
            });
        }

        [HttpDelete]
        [Route("DeleteDefect/{id}")]
        public async Task<IActionResult> DeleteDefect(int id)
        {
            var IdDefect = await _context.RjDefects.FindAsync(id);

            if (IdDefect == null)
            {
                return BadRequest("Id no encontrado");
            }

            _context.RjDefects.Remove(IdDefect);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                success = true,
                message = "Dato eliminado"
            });
        }
    }
}