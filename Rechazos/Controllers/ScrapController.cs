using Calidad.Dtos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Rechazos.Models;

namespace Calidad.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ScrapController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ScrapController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Route("GetShifts")]
        public async Task<IActionResult> GetShiftsAsync()
        {
            var list = await _context.Shifts
                    .AsNoTracking()
                    .ToListAsync();

            if(list == null)
            {
                return BadRequest("Lista vacia");
            }

            return Ok(list);
        }

        [HttpGet]
        [Route("GetMaterial")]
        public async Task<IActionResult> GeMaterialAsync()
        {
            var list = await _context.ScMaterial
                            .AsNoTracking()
                            .ToListAsync();

            if (list == null)
            {
                return BadRequest("Lista vacia");
            }

            return Ok(list);
        }

        [HttpGet]
        [Route("GetLines")]
        public async Task<IActionResult> GetLines()
        {
            var list = await _context.Lines
                        .AsNoTracking()
                        .ToListAsync();

            if( list == null)
            {
                return BadRequest("Lista vacia");
            }

            return Ok(list);
        }

        [HttpGet]
        [Route("GetTypeScrap")]
        public async Task<IActionResult> GetTypeScrapAsync()
        {
            var list = await _context.ScTypeScrap
                        .AsNoTracking()
                        .ToListAsync();

            if (list == null)
            {
                return BadRequest("Lista vacia");
            }

            return Ok(list);
        }

        [HttpGet]
        [Route("GetScrap")]
        public async Task<IActionResult> GetScrapAsync()
        {
            var list = await _context.Scrap
                        .Select(s => new
                        {
                            s.Id,
                            Line = s.Line.Name,
                            TypeScrap = s.TypeScrap.Name,
                            Kg = s.Weight
                        })
                        .AsNoTracking()
                        .ToListAsync();

            if(list == null)
            {
                return BadRequest("Lista vacia");
            }

            return Ok(list);
        }

        [HttpGet]
        [Route("GetProcessByLine/{linesId}")]
        public async Task<IActionResult> GetProcess(int linesId)
        {
            var list = await _context.MesaProcess
                        .Where(p => p.LineId == linesId)
                        .AsNoTracking()
                        .ToListAsync();

            if(list == null)
            {
                return BadRequest("Lista vacia");
            }

            return Ok(list);
        }

        [HttpGet]
        [Route("GetMachineCodeByProcess/{processId}")]
        public async Task<IActionResult> GetMachineCodeByProcess(int processId)
        {
            var list = await _context.MachineCodes
                            .Where(m => m.ProcessId == processId)
                            .AsNoTracking()
                            .ToListAsync();

            if(list == null)
            {
                return BadRequest("Lista vacia");
            }

            return Ok(list);
        }

        [HttpGet]
        [Route("GetDefectByTypeScrap/{scrapId}")]
        public async Task<IActionResult> GetDefectByTypeScrap(int scrapId)
        {
            var list = await _context.ScDefects
                        .Where(d => d.TypeScrapId == scrapId)
                        .AsNoTracking()
                        .ToListAsync();

            if(list == null)
            {
                return BadRequest("Lista vacia");
            }

            return Ok(list);
        }

        [HttpPost]
        [Route("Create")]
        public async Task<IActionResult> Create([FromBody] ScrapDto scrap)
        {
            if (scrap == null)
            {
                return BadRequest("Campos vacios");
            }

            var newScrap = new Scrap
            {
                ShiftId = scrap.ShiftId,
                LineId = scrap.LineId,
                ProcessId = scrap.ProcessId,                
                PayRollNumber = scrap.PayRollNumber,
                MaterialId = scrap.MaterialId,
                Alloy = scrap.Alloy,
                Diameter = scrap.Diameter,
                Wall = scrap.Wall,
                TypeScrapId = scrap.TypeScrapId,
                DefectId = scrap.DefectId,
                MachineId = scrap.MachineId,
                Weight = scrap.Weight,
                Rdm = scrap.Rdm,
            };

            _context.Scrap.Add(newScrap);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                success = true,
                message = "Registro creado",
                scrapId = newScrap.Id
            });
        }
    }
}