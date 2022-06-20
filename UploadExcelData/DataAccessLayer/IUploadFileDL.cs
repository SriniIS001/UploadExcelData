using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using UploadExcelData.Model;

namespace UploadExcelData.DataAccessLayer
{
    public interface IUploadFileDL
    {
        public Task<UploadXMLFileResponse> UploadXMLFile(UploadXMLFileRequest request, string Path);

      
        
    }
}
