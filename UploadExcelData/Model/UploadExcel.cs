
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace UploadExcelData.Model
{
    


    public class UploadXMLFileRequest
    {
        public IFormFile File { get; set; }
    }

    public class UploadXMLFileResponse
    {
        public bool IsSuccess { get; set; }
        public string? Message { get; set; }
    }

    public class ExcelBulkUploadParameter
    {

        public int ID { get; set; }
        public string? Name { get; set; }
        public string? Dept { get; set; }
        public string? Loc { get; set; }


    }

}
