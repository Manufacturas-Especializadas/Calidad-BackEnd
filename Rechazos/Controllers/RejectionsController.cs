using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Rechazos.Dtos;
using Rechazos.Models;
using Rechazos.Services;

namespace Rechazos.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RejectionsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly string _container = "rechazos";
        private readonly AzureStorageService _azureStorageService;

        public RejectionsController(AppDbContext context, AzureStorageService azureStorageService)
        {
            _context = context;
            _azureStorageService = azureStorageService;
        }

        [HttpGet]
        [Route("GetLines")]
        public async Task<IActionResult> GetLinesAsync()
        {
            var list = await _context.Lines
                            .AsNoTracking()
                            .ToListAsync();

            if(list == null)
            {
                BadRequest("No hay datos en esta lista");
            }

            return Ok(list);
        }

        [HttpGet]
        [Route("GetClients")]
        public async Task<IActionResult> GetClientsAsync()
        {
            var list = await _context.Clients
                            .AsNoTracking()
                            .ToListAsync();

            if(list == null)
            {
                BadRequest("No hay datos en esta lista");
            }

            return Ok(list);
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
                BadRequest("No hay datos en esta lista");
            }

            return Ok(list);
        }

        [HttpGet]
        [Route("GetContainmentAction")]
        public async Task<IActionResult> GetContainmentAction()
        {
            var list = await _context.RjContainmentaction
                                .AsNoTracking()
                                .ToListAsync();

            if(list == null)
            {
                BadRequest("No hay datos en esta lista");
            }

            return Ok(list);
        }

        [HttpGet]
        [Route("GetConditionByDefect")]
        public async Task<IActionResult> GetConditionByDefect(int defectId)
        {
            var list = await _context.RjCondition
                                    .Where(c => c.IdDefects == defectId)
                                    .AsNoTracking()
                                    .ToListAsync();

            return Ok(list);
        }

        [HttpGet]
        [Route("GetNextFolio")]
        public async Task<IActionResult> GetNextFolio()
        {
            var nextFolio = await _context.Rejections
                .Select(r => (int?)r.Folio)
                .MaxAsync() ?? 0;

            return Ok(nextFolio + 1);
        }

        [HttpGet]
        [Route("GetRejections")]
        public async Task<IActionResult> GetRejections()
        {
            var list = await _context.Rejections
                                .OrderByDescending(r => r.Id)
                                .Select(r => new
                                {
                                    r.Id,
                                    r.Insepector,
                                    r.PartNumber,
                                    r.NumberOfPieces,
                                    r.OperatorPayroll,
                                    r.Description,
                                    r.Image,
                                    r.RegistrationDate,
                                    r.Folio,
                                    Clients = r.IdClientNavigation.Name,
                                    Defects = r.IdDefectNavigation.Name,
                                    Lines = r.IdLineNavigation.Name,
                                    Condition = r.IdConditionNavigation.Name,
                                    Action = r.IdContainmentactionNavigation.Name,
                                })
                                .AsNoTracking()
                                .ToListAsync();

            if(list == null)
            {
                NotFound("Lista vacia");
            }

            return Ok(list);
        }

        [HttpDelete]
        [Route("Delete/{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var rejection = await _context.Rejections.FindAsync(id);

            if (rejection == null) return NotFound("Rejection not found");

            _context.Rejections.Remove(rejection);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                success = true,
                message = "Rejection deleted successfully"
            });
        }

        [HttpPost]
        [Route("Create")]
        public async Task<IActionResult> Create([FromForm] RejectionDto rejection)
        {
            if (rejection == null)
            {
                return BadRequest("Rejection no tiene datos");
            }

            if (rejection.Photos != null && rejection.Photos.Count > 5)
            {
                return BadRequest("Se permite un máximo de 4 fotos");
            }

            var urls = new List<string>();
            string informedSignatureUrl = null;

            if (rejection.Photos != null && rejection.Photos.Any())
            {
                foreach (var photo in rejection.Photos)
                {
                    if (photo.Length > 0)
                    {
                        var url = await _azureStorageService.StoragePhotos(_container, photo);

                        var fileName = photo.FileName.ToLower();
                        if (fileName.Contains("signature"))
                        {
                            informedSignatureUrl = url;
                        }
                        else
                        {
                            urls.Add(url); 
                        }
                    }
                }

                var photoUrl = urls.Count > 0 ? string.Join(";", urls) : null;

                var newRejection = new Rejections
                {
                    Insepector = rejection.Insepector,
                    PartNumber = rejection.PartNumber,
                    NumberOfPieces = rejection.NumberOfPieces,
                    IdDefect = rejection.IdDefect,
                    IdCondition = rejection.IdCondition,
                    Description = rejection.Description,
                    Image = photoUrl,
                    IdLine = rejection.IdLine,
                    IdClient = rejection.IdClient,
                    OperatorPayroll = rejection.OperatorPayroll,
                    IdContainmentaction = rejection.IdContainmentaction,
                    InformedSignature = informedSignatureUrl,
                    RegistrationDate = DateTime.Now,
                    Folio = rejection.Folio
                };

                _context.Rejections.Add(newRejection);
                await _context.SaveChangesAsync();

                return Ok(new
                {
                    success = true,
                    message = "Registro exitosamente creado",
                    rejectionId = newRejection.Id
                });
            }

            if (!string.IsNullOrEmpty(rejection.InformedSignature) && rejection.InformedSignature.StartsWith("data:image"))
            {
                return BadRequest("Firma no puede ser base64. Debe subirse como archivo");
            }

            var newRejectionNoPhotos = new Rejections
            {
                Insepector = rejection.Insepector,
                PartNumber = rejection.PartNumber,
                NumberOfPieces = rejection.NumberOfPieces,
                IdDefect = rejection.IdDefect,
                IdCondition = rejection.IdCondition,
                Description = rejection.Description,
                Image = null,
                IdLine = rejection.IdLine,
                IdClient = rejection.IdClient,
                OperatorPayroll = rejection.OperatorPayroll,
                IdContainmentaction = rejection.IdContainmentaction,
                InformedSignature = "",
                RegistrationDate = DateTime.Now,
                Folio = rejection.Folio
            };

            _context.Rejections.Add(newRejectionNoPhotos);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                success = true,
                message = "Registro exitosamente creado",
                rejectionId = newRejectionNoPhotos.Id
            });
        }
    }
}