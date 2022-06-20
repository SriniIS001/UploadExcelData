using ExcelDataReader;
using LumenWorks.Framework.IO.Csv;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using UploadExcelData.Model;
using UploadExcelData.CommonUtility;

namespace UploadExcelData.DataAccessLayer
{
    public class UploadFileDL : IUploadFileDL
    {
        public readonly IConfiguration _configuration;
        public readonly MySqlConnection _mySqlConnection;
        public UploadFileDL(IConfiguration configuration)
        {
            _configuration = configuration;
            _mySqlConnection = new MySqlConnection(_configuration["ConnectionStrings:MySqlDBConnectionString"]);
        }

        

        

        

        public async Task<UploadXMLFileResponse> UploadXMLFile(UploadXMLFileRequest request, string path)
        {
            UploadXMLFileResponse response = new UploadXMLFileResponse();
            List<ExcelBulkUploadParameter> Parameters = new List<ExcelBulkUploadParameter>();
            DataSet dataSet;
            response.IsSuccess = true;
            response.Message = "Successful";

            try
            {

                if (request.File.FileName.ToLower().Contains(".xlsx"))
                {
                    FileStream stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                    System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
                    IExcelDataReader reader = ExcelReaderFactory.CreateReader(stream);
                    dataSet = reader.AsDataSet(
                        new ExcelDataSetConfiguration()
                        {
                            UseColumnDataType = false,
                            ConfigureDataTable = (tableReader) => new ExcelDataTableConfiguration()
                            {
                                UseHeaderRow = true
                            }

                        });

                    for(int i=0; i<dataSet.Tables[0].Rows.Count; i++)
                    {
                        ExcelBulkUploadParameter rows = new ExcelBulkUploadParameter();
                        rows.ID = dataSet.Tables[0].Rows[i].ItemArray[0] != null ? Convert.ToInt32(dataSet.Tables[0].Rows[i].ItemArray[0]) : -1;
                        rows.Name = dataSet.Tables[0].Rows[i].ItemArray[1] != null ? Convert.ToString(dataSet.Tables[0].Rows[i].ItemArray[1]) : "-1";
                        rows.Dept = dataSet.Tables[0].Rows[i].ItemArray[2] != null ? Convert.ToString(dataSet.Tables[0].Rows[i].ItemArray[2]) : "-1";
                        rows.Loc = dataSet.Tables[0].Rows[i].ItemArray[3] != null ? Convert.ToString(dataSet.Tables[0].Rows[i].ItemArray[3]) : "-1";
                        
                        Parameters.Add(rows);
                    }

                    stream.Close();

                    if (Parameters.Count > 0)
                    {
                        if (ConnectionState.Open != _mySqlConnection.State) 
                        {
                            await _mySqlConnection.OpenAsync();
                        }

                        foreach (ExcelBulkUploadParameter rows in Parameters)
                        {
                            using (MySqlCommand sqlCommand = new MySqlCommand(SqlQueries.InsertBulkUploadData, _mySqlConnection))
                            {
                                //UserName, EmailID, MobileNumber, Age, Salary, Gender
                                sqlCommand.CommandType = CommandType.Text;
                                sqlCommand.CommandTimeout = 180;
                                sqlCommand.Parameters.AddWithValue("@ID", rows.ID);
                                sqlCommand.Parameters.AddWithValue("@Name", rows.Name);
                                sqlCommand.Parameters.AddWithValue("@Dept", rows.Dept);
                                sqlCommand.Parameters.AddWithValue("@Loc", rows.Loc);
                                
                                int Status = await sqlCommand.ExecuteNonQueryAsync();
                                if(Status <=0)
                                {
                                    response.IsSuccess = false;
                                    response.Message = "Query Not Executed";
                                    return response;
                                }
                            }
                        }
                    }

                }
                else
                {
                    response.IsSuccess = false;
                    response.Message = "Invalid File";
                }

            }catch(Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ex.Message;
            }
            finally
            {
                await _mySqlConnection.CloseAsync();
                await _mySqlConnection.DisposeAsync();
            }

            return response;
        }
    }
}
