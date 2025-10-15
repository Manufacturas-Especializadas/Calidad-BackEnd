using Calidad.Dtos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Rechazos.Models;

namespace Calidad.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LinesController : ControllerBase
    {


        private readonly AppDbContext _context;

        public LinesController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Route("GetLines")]
        public async Task<IActionResult> GetLines()
        {
            var list = await _context.Lines
                            .AsNoTracking()
                            .ToListAsync();
            
            if(list == null)
            {
                return BadRequest("Lista vacia");
            }

            return Ok(list);
        }

        [HttpGet]
        [Route("GetLineById/{id}")]
        public async Task<IActionResult> GetLineById(int id)
        {
            var IdLine = await _context.Lines.FirstOrDefaultAsync(l => l.Id == id);

            if(IdLine == null)
            {
                return BadRequest("Linea no encontrada");
            }

            return Ok(IdLine);
        }

        [HttpPost]
        [Route("RegisterLine")]
        public async Task<IActionResult> RegisterLine([FromBody] LinesDto dto)
        {
            if(dto == null)
            {
                return BadRequest("No se acepta datos vacios");
            }

            var newLine = new Lines
            {
                Name = dto.Name,
            };

            _context.Lines.Add(newLine);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                success = true,
                message = "Linea registrada",
                newLine = newLine.Id
            });
        }

        [HttpPut]
        [Route("UpdateLine/{id}")]
        public async Task<IActionResult> UpdateLine([FromBody] LinesDto dto, int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var IdLine = await _context.Lines.FindAsync(id);
            if(IdLine == null)
            {
                return NotFound("Id no encontrado");
            }

            IdLine.Name = dto.Name;

            await _context.SaveChangesAsync();

            return Ok(new
            {
                success = true,
                message = "Linea actualizada"
            });
        }

        [HttpDelete]
        [Route("DeleteLine/{id}")]
        public async Task<IActionResult> DeleteLine(int id)
        {
            var IdLine = await _context.Lines.FindAsync(id);

            if(IdLine == null)
            {
                return NotFound("Id no encontrado");
            }
            
            _context.Lines.Remove(IdLine);
            await _context.SaveChangesAsync();


            return Ok(new
            {
                success = true,
                message = "Linea eliminada"
            });
        }
    }
}