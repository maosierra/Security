using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace SecurityPage.Controllers
{
    public class UploadController : Controller
    {
        // GET: Upload
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Encrypt([FromForm] FileEncryptForm fileForm)
        {
            try
            {
                var fileNameSplit = fileForm.File.FileName.Split('.');
                if (fileNameSplit[fileNameSplit.Length - 1] != "txt")
                {
                    ViewBag.Message = "Only txt files allowed!";
                    return View("~/Views/Upload/Index.cshtml");
                }

                if (fileForm.Password.Length != 16)
                {
                    ViewBag.Message = "Password length should be 16 characters";
                    return View("~/Views/Upload/Index.cshtml");
                }

                var fileName = fileNameSplit[0];
                var fileContent = await ReadAsStringAsync(fileForm.File);
                var fileContentEncrypt = EncryptString(fileContent, fileForm.Password);
                return new FileStreamResult(GenerateStreamFromString(fileContentEncrypt), "text/plain") { FileDownloadName = $"{fileName}.txt" };
            }
            catch (Exception)
            {
                ViewBag.Message = "File Upload Failed";
                return View("~/Views/Upload/Index.cshtml");
            }
        }

        [HttpPost]
        public async Task<ActionResult> Decrypt([FromForm] FileEncryptForm fileForm)
        {
            try
            {
                var fileNameSplit = fileForm.File.FileName.Split('.');
                if (fileNameSplit[fileNameSplit.Length - 1] != "txt")
                {
                    ViewBag.Message = "Only txt files allowed!";
                    return View("~/Views/Upload/Index.cshtml");
                }

                if (fileForm.Password.Length != 16)
                {
                    ViewBag.Message = "Password length should be 16 characters";
                    return View("~/Views/Upload/Index.cshtml");
                }

                var fileName = fileNameSplit[0];
                var fileContentEncrypt = await ReadAsStringAsync(fileForm.File);
                var fileContentDecrypt = DecryptString(fileContentEncrypt, fileForm.Password);
                return new FileStreamResult(GenerateStreamFromString(fileContentDecrypt), "text/plain") { FileDownloadName = $"{fileName}.txt" };
            }
            catch (Exception)
            {
                ViewBag.Message = "File Upload Failed";
                return View("~/Views/Upload/Index.cshtml");
            }
        }

        public class FileEncryptForm
        {
            public IFormFile File { get; set; }
            public string Password { get; set; }
            public string Extension { get; set; }
        }

        private async Task<string> ReadAsStringAsync(IFormFile file)
        {
            var result = new StringBuilder();
            using (var reader = new StreamReader(file.OpenReadStream()))
            {
                while (reader.Peek() >= 0)
                    result.AppendLine(await reader.ReadLineAsync());
            }
            return result.ToString();
        }

        private Stream GenerateStreamFromString(string s)
        {
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(s);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }

        private string EncryptString(string text, string keyString)
        {
            var key = Encoding.UTF8.GetBytes(keyString);

            using (var aesAlg = Aes.Create())
            {
                using (var encryptor = aesAlg.CreateEncryptor(key, aesAlg.IV))
                {
                    using (var msEncrypt = new MemoryStream())
                    {
                        using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                        using (var swEncrypt = new StreamWriter(csEncrypt))
                        {
                            swEncrypt.Write(text);
                        }

                        var iv = aesAlg.IV;

                        var decryptedContent = msEncrypt.ToArray();

                        var result = new byte[iv.Length + decryptedContent.Length];

                        Buffer.BlockCopy(iv, 0, result, 0, iv.Length);
                        Buffer.BlockCopy(decryptedContent, 0, result, iv.Length, decryptedContent.Length);

                        return Convert.ToBase64String(result);
                    }
                }
            }
        }

        private string DecryptString(string cipherText, string keyString)
        {
            var fullCipher = Convert.FromBase64String(cipherText);

            var iv = new byte[16];
            var cipher = new byte[16];

            Buffer.BlockCopy(fullCipher, 0, iv, 0, iv.Length);
            Buffer.BlockCopy(fullCipher, iv.Length, cipher, 0, iv.Length);
            var key = Encoding.UTF8.GetBytes(keyString);

            using (var aesAlg = Aes.Create())
            {
                using (var decryptor = aesAlg.CreateDecryptor(key, iv))
                {
                    string result;
                    using (var msDecrypt = new MemoryStream(cipher))
                    {
                        using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                        {
                            using (var srDecrypt = new StreamReader(csDecrypt))
                            {
                                result = srDecrypt.ReadToEnd();
                            }
                        }
                    }

                    return result;
                }
            }
        }
    }
}

