using Calidad.Dtos;
using DocumentFormat.OpenXml.Drawing.Diagrams;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Rechazos.Models;

namespace Calidad.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DefectConditionController : ControllerBase
    {
        private readonly AppDbContext _context;

        public DefectConditionController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Route("GetCondition")]
        public async Task<IActionResult> GetCondition()
        {
            var list = await _context.RjCondition
                            .AsNoTracking()
                            .ToListAsync();

            if(list == null)
            {
                return BadRequest("Lista vacia");
            }

            return Ok(list);
        }

        [HttpGet]
        [Route("GetConditionByDefect")]
        public async Task<IActionResult> GetConditionByDefect(int idDefect)
        {
            var IdDefect = await _context.RjCondition
                                    .Where(c => c.IdDefects == idDefect)
                                    .AsNoTracking()
                                    .ToListAsync();

            return Ok(IdDefect);
        }

        [HttpPost]
        [Route("RegisterDefectCondition")]
        public async Task<IActionResult> RegisterDefectCondition([FromBody] DefectConditionDto dto)
        {
            if(dto == null)
            {
                return BadRequest("Campos vacios");
            }

            var newDefectCondition = new RjCondition
            {
                IdDefects = dto.IdDefects,
                Name = dto.Name,
            };

            _context.RjCondition.Add(newDefectCondition);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                success = true,
                message = "Condicion registrada",
                IdCondition = newDefectCondition.Id
            });
        }

        [HttpPut]
        [Route("UpdateDefectCondition/{id}")]
        public async Task<IActionResult> UpdateDefectConditon([FromBody] DefectConditionDto dto, int id)
        {
            var IdDefectCondition = await _context.RjCondition.FindAsync(id);

            if(IdDefectCondition == null)
            {
                return BadRequest("Id no encontrado");
            }

            IdDefectCondition.Name = dto.Name;

            await _context.SaveChangesAsync();

            return Ok(new
            {
                success = true,
                message = "Condicion actualizada"
            });
        }

        [HttpDelete]
        [Route("DeleteDefectCondition/{id}")]
        public async Task<IActionResult> DeleteDefectCondition(int id)
        {
            var IdDefectCondition = await _context.RjCondition.FindAsync(id);

            if( IdDefectCondition == null)
            {
                return BadRequest("Id no encontrado");
            }

            _context.RjCondition.Remove(IdDefectCondition);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                success = true,
                message = "Condicion eliminada"
            });
        }
    }
}