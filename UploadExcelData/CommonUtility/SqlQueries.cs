using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UploadExcelData.CommonUtility
{
    public class SqlQueries
    {

        static IConfiguration _configuration = new ConfigurationBuilder()
            .AddXmlFile("SqlQueries.xml", true, true)
            .Build();

        public static string InsertBulkUploadData { get { return _configuration["InsertBulkUploadData"]; } }
      

    }
}
