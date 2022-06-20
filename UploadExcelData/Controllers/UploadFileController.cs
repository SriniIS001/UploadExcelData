using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
//using System.IO;
using System.Threading.Tasks;

using UploadExcelData.Model;
using UploadExcelData.DataAccessLayer;

namespace UploadExcelData.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UploadFileController : ControllerBase
    {
        public readonly IUploadFileDL _uploadFileDL;
        public UploadFileController(IUploadFileDL uploadFileDL)
        {
            _uploadFileDL = uploadFileDL;
        }

        [HttpPost]
        [Route("UploadExcelFile")]
        public async Task<IActionResult> UploadExcelFile([FromForm] UploadXMLFileRequest request)
        {
            UploadXMLFileResponse response = new UploadXMLFileResponse();
            string path = "UploadFileFolder/"+request.File.FileName;
            try
            {
                
                using (FileStream stream = new FileStream(path, FileMode.CreateNew))
                {
                    await request.File.CopyToAsync(stream);
                }

                response = await _uploadFileDL.UploadXMLFile(request, path);

                string[] files = Directory.GetFiles("UploadFileFolder/");
                foreach (string file in files)
                {
                    System.IO.File.Delete(file);
                    Console.WriteLine($"{file} is deleted.");
                }

            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ex.Message;
                
            }

            return Ok(response);
        }

       

       
    }
}
