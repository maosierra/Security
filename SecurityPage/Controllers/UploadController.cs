using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SecurityPage.Services;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace SecurityPage.Controllers
{
    public class UploadController : Controller
    {
        private readonly IAesService aesService;

        public UploadController(IAesService aesService)
        {
            this.aesService = aesService;
        }

        // GET: Upload
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Encrypt([FromForm] FileEncryptForm file)
        {
            try
            {
                ViewBag.Message = "File Upload Successful1234567890";
                var fileName = file.File.FileName.Split('.')[0] ?? "aes";
                return new FileStreamResult(await EncriptFile(file), "application/octet-stream") { FileDownloadName = $"{fileName}.aes" };
            }
            catch (Exception ex)
            {
                //Log ex
                ViewBag.Message = "File Upload Failed";
            }
            return View("~/Views/Upload/Index.cshtml");
        }

        [HttpPost]
        public async Task<ActionResult> Decrypt([FromForm] FileEncryptForm file)
        {
            try
            {
                ViewBag.Message = "File Upload Successful1234567890";
                var fileName = file.File.FileName.Split('.')[0] ?? "aes";
                return new FileStreamResult(await DecriptFile(file), "application/octet-stream") { FileDownloadName = $"{fileName}.{file.Extension}" };
            }
            catch (Exception ex)
            {
                //Log ex
                ViewBag.Message = "File Upload Failed";
            }
            return View("~/Views/Upload/Index.cshtml");
        }

        public class FileEncryptForm
        {
            public IFormFile File { get; set; }
            public string Password { get; set; }
            public string Extension { get; set; }
        }

        private async Task<MemoryStream> EncriptFile(FileEncryptForm file)
        {
            using var memory = new MemoryStream();
            await file.File.CopyToAsync(memory);
            return aesService.FileEncrypt(memory, Encoding.ASCII.GetBytes(file.Password));
        }

        private async Task<MemoryStream> DecriptFile(FileEncryptForm file)
        {
            using var memory = new MemoryStream();
            await file.File.CopyToAsync(memory);
            return aesService.FileDecrypt(memory, Encoding.ASCII.GetBytes(file.Password));
        }

    }
}

