using ClosedXML.Excel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Rechazos.Dtos;
using Rechazos.Models;
using Rechazos.Services;
using System.Drawing;
using System.Net.Security;
using System.Security.Claims;

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

        [HttpGet]
        [Route("GetRejectionsById/{id}")]
        public async Task<IActionResult> GetRejectionsById(int id)
        {
            var rejection = await _context.Rejections
                .Where(r => r.Id == id)
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
                    r.InformedSignature,
                    r.IdClient,
                    r.IdDefect,
                    r.IdLine,
                    r.IdCondition,
                    r.IdContainmentaction,
                    ClientName = r.IdClientNavigation.Name,
                    DefectName = r.IdDefectNavigation.Name,
                    LineName = r.IdLineNavigation.Name,
                    ConditionName = r.IdConditionNavigation.Name,
                    ActionName = r.IdContainmentactionNavigation.Name,
                })
                .FirstOrDefaultAsync();

            if (rejection == null)
            {
                return NotFound("Id no encontrado");
            }

            return Ok(rejection);
        }


        [HttpPost]
        [Route("DownloadExcel")]
        public async Task<IActionResult> ExportToExcel()
        {
            var rejection = await _context.Rejections
                .Select(r => new
                {
                    Inspector = r.Insepector,
                    PartNumber = r.PartNumber,
                    Amount = r.NumberOfPieces,
                    OperatorPayroll = r.OperatorPayroll,
                    Description = r.Description,
                    Date = r.RegistrationDate,
                    Defect = r.IdDefectNavigation != null ? r.IdDefectNavigation.Name : null,
                    Condition = r.IdConditionNavigation != null ? r.IdConditionNavigation.Name : null,
                    Line = r.IdLineNavigation != null ? r.IdLineNavigation.Name : null,
                    Client = r.IdContainmentactionNavigation != null ? r.IdContainmentactionNavigation.Name : null,
                    ImageUrls = r.Image,
                    SignatureUrl = r.InformedSignature
                })
                .AsNoTracking()
                .ToListAsync();

            using var workBook = new XLWorkbook();
            var workSheet = workBook.Worksheets.Add("Rechazos");
            using var httpClient = new HttpClient();

            workSheet.Cell(1, 1).Value = "Inspector";
            workSheet.Cell(1, 2).Value = "Número de parte";
            workSheet.Cell(1, 3).Value = "Cantidad de piezas";
            workSheet.Cell(1, 4).Value = "Nómina";
            workSheet.Cell(1, 5).Value = "Descripción";
            workSheet.Cell(1, 6).Value = "Fecha";
            workSheet.Cell(1, 7).Value = "Defecto";
            workSheet.Cell(1, 8).Value = "Condición";
            workSheet.Cell(1, 9).Value = "Línea";
            workSheet.Cell(1, 10).Value = "Cliente";
            workSheet.Cell(1, 11).Value = "Imagen 1";
            workSheet.Cell(1, 12).Value = "Imagen 2";
            workSheet.Cell(1, 13).Value = "Imagen 3";
            workSheet.Cell(1, 14).Value = "Imagen 4";
            workSheet.Cell(1, 15).Value = "Firma";

            for (int i = 0; i < rejection.Count; i++)
            {
                var row = i + 2;

                workSheet.Cell(row, 1).Value = rejection[i].Inspector ?? string.Empty;
                workSheet.Cell(row, 2).Value = rejection[i].PartNumber ?? string.Empty;
                workSheet.Cell(row, 3).Value = rejection[i].Amount ?? 0;
                workSheet.Cell(row, 4).Value = rejection[i].OperatorPayroll?.ToString() ?? string.Empty;
                workSheet.Cell(row, 5).Value = rejection[i].Description ?? string.Empty;
                workSheet.Cell(row, 6).Value = rejection[i].Date?.ToString("dd/MM/yyyy") ?? string.Empty;
                workSheet.Cell(row, 7).Value = rejection[i].Defect ?? string.Empty;
                workSheet.Cell(row, 8).Value = rejection[i].Condition ?? string.Empty;
                workSheet.Cell(row, 9).Value = rejection[i].Line ?? string.Empty;
                workSheet.Cell(row, 10).Value = rejection[i].Client ?? string.Empty;

                string[] imageUrls = (rejection[i].ImageUrls ?? "")
                    .Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(u => u.Trim())
                    .Where(u => !string.IsNullOrEmpty(u))
                    .ToArray();

                for (int imgIndex = 0; imgIndex < 4; imgIndex++)
                {
                    if (imgIndex < imageUrls.Length && !string.IsNullOrWhiteSpace(imageUrls[imgIndex]))
                    {
                        try
                        {
                            byte[] imageBytes = await httpClient.GetByteArrayAsync(imageUrls[imgIndex].Trim());
                            using var imageStream = new MemoryStream(imageBytes);
                            using var image = Image.FromStream(imageStream);

                            double targetWidth = 150;
                            double targetHeight = 100;

                            double scaleX = targetWidth / image.Width;
                            double scaleY = targetHeight / image.Height;
                            double scale = Math.Min(scaleX, scaleY);

                            var picture = workSheet.AddPicture(imageStream)
                                .MoveTo(workSheet.Cell(row, 11 + imgIndex))
                                .Scale(scale);

                            workSheet.Row(row).Height = 100;
                            workSheet.Column(11 + imgIndex).Width = 20;
                        }
                        catch (Exception ex)
                        {
                            workSheet.Cell(row, 11 + imgIndex).Value = "Error al cargar imagen";
                        }
                    }
                }

                if (!string.IsNullOrWhiteSpace(rejection[i].SignatureUrl))
                {
                    try
                    {
                        byte[] signatureBytes = await httpClient.GetByteArrayAsync(rejection[i].SignatureUrl);
                        using var signatureStream = new MemoryStream(signatureBytes);
                        using var signatureImage = Image.FromStream(signatureStream);

                        double sigTargetWidth = 150;
                        double sigTargetHeight = 60;

                        double sigScaleX = sigTargetWidth / signatureImage.Width;
                        double sigScaleY = sigTargetHeight / signatureImage.Height;
                        double sigScale = Math.Min(sigScaleX, sigScaleY);

                        var signature = workSheet.AddPicture(signatureStream)
                            .MoveTo(workSheet.Cell(row, 15))
                            .Scale(sigScale);

                        workSheet.Row(row).Height = Math.Max(workSheet.Row(row).Height, 60);
                        workSheet.Column(15).Width = 25;
                    }
                    catch (Exception ex)
                    {
                        workSheet.Cell(row, 15).Value = "Error al cargar firma";
                    }
                }
            }

            workSheet.Columns(1, 10).AdjustToContents();

            var stream = new MemoryStream();
            workBook.SaveAs(stream);
            stream.Position = 0;

            var fileName = $"Rechazos_{DateTime.Now:ddMMyyyy_HHmmss}.xlsx";

            return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
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

        [Authorize]
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

            var userClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userClaim == null || !int.TryParse(userClaim.Value, out int userId))
            {
                return Unauthorized(new { message = "Usuario no autenticado" });
            }

            var userExists = await _context.Users.AnyAsync(u => u.Id == userId);
            if (!userExists)
            {
                return Unauthorized(new { message = "Usuario no válido" });
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
                    Folio = rejection.Folio,
                    IdUser = userId
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

        [Authorize]
        [HttpPut]
        [Route("Edit/{id}")]
        public async Task<IActionResult> Edit(int id, [FromForm] RejectionDto rejection)
        {
            if (rejection == null)
            {
                return BadRequest("Rejection no tiene datos");
            }

            if (rejection.Photos != null && rejection.Photos.Count > 5)
            {
                return BadRequest("Se permite un máximo de 5 fotos");
            }

            var userClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userClaim == null || !int.TryParse(userClaim.Value, out int userId))
            {
                return Unauthorized(new { message = "Usuario no autenticado" });
            }

            var existingRejection = await _context.Rejections.FindAsync(id);
            if (existingRejection == null)
            {
                return NotFound("Rechazo no encontrado");
            }

            var orignalUrls = existingRejection.Image?.Split(";", StringSplitOptions.RemoveEmptyEntries) ?? Array.Empty<string>();
            var remainingUrls = rejection.ExistingImageUrls?.Split(";", StringSplitOptions.RemoveEmptyEntries) ?? Array.Empty<string>();

            var urlsToDelete = orignalUrls.Except(remainingUrls);

            foreach(var url in urlsToDelete)
            {
                await _azureStorageService.DeleteFileAsync(_container, url);
            }

            var urls = new List<string>();
            string newSignatureUrl = existingRejection.InformedSignature!;

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
                            if (!string.IsNullOrEmpty(existingRejection.InformedSignature))
                            {
                                await _azureStorageService.DeleteFileAsync(_container, existingRejection.InformedSignature);
                            }
                            newSignatureUrl = url;
                        }
                        else
                        {
                            urls.Add(url);
                        }
                    }
                }
            }

            var finalUrls = remainingUrls.Concat(urls).ToList();
            existingRejection.Image = finalUrls.Any() ? string.Join(";", finalUrls) : null;
            existingRejection.InformedSignature = newSignatureUrl;

            existingRejection.Insepector = rejection.Insepector;
            existingRejection.PartNumber = rejection.PartNumber;
            existingRejection.NumberOfPieces = rejection.NumberOfPieces;
            existingRejection.IdDefect = rejection.IdDefect;
            existingRejection.IdCondition = rejection.IdCondition;
            existingRejection.Description = rejection.Description;
            existingRejection.IdLine = rejection.IdLine;
            existingRejection.IdClient = rejection.IdClient;
            existingRejection.OperatorPayroll = rejection.OperatorPayroll;
            existingRejection.IdContainmentaction = rejection.IdContainmentaction;
            existingRejection.Folio = rejection.Folio;

            _context.Rejections.Update(existingRejection);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                success = true,
                message = "Registro actualizado exitosamente",
                rejectionId = existingRejection.Id
            });
        }      
    }
}