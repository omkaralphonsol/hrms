using DataObject;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using System.Xml.Linq;
using System.Configuration;
using System.Web;


namespace ProcessModel
{
    public  class userDocumentBL
    {
        string UserId = Convert.ToString(HttpContext.Current.Session["userId"]);
        private string DBName = ConfigurationManager.AppSettings["DBName"];
        private static string MySqlconnection = ConfigurationManager.ConnectionStrings["MysqlConnection"].ConnectionString;
        public List<userDocumentsDO> SaveUserDocument(int userId, FileAttachment file, string fileExt, string basePath)
        {
            List<userDocumentsDO> listdata = new List<userDocumentsDO>();
            getDrtolist getDrtolistParam = new getDrtolist();
            List<MySqlParameter> mysqlParameters = new List<MySqlParameter>();

            try
            {
                mysqlParameters.Add(DataClass.GetParameter("@p_user_id", userId));
                mysqlParameters.Add(DataClass.GetParameter("@p_document_master_id", file.DocumentMasterId));
                mysqlParameters.Add(DataClass.GetParameter("@p_base_path", basePath)); // folder only
                mysqlParameters.Add(DataClass.GetParameter("@p_file_name", file.FileName)); // filename without extension
                mysqlParameters.Add(DataClass.GetParameter("@p_file_extension", fileExt));
                mysqlParameters.Add(DataClass.GetParameter("@p_reference_number", file.ReferenceNumber));
                mysqlParameters.Add(DataClass.GetParameter("@p_email_id", file.EmailId));
                mysqlParameters.Add(DataClass.GetParameter("@p_inserted_by", UserId));

                listdata = getDrtolistParam.getdatafromreder<userDocumentsDO>(
                    DataClass.GetDataReaderFromSpWithParam(mysqlParameters, DBName, "sp_saveUserDocument")
                );
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog("userDocumentBL", "SaveUserDocument",
                    "Exception Message: " + ex.Message + " StackTrace=" + ex.StackTrace, UserId);
            }

            return listdata;
        }

        public List<userDocumentsDO> GetUserDocuments(int userId)
        {
            List<userDocumentsDO> listdata = new List<userDocumentsDO>();
            getDrtolist getDrtolistParam = new getDrtolist();
            List<MySqlParameter> mysqlParameters = new List<MySqlParameter>();

            try
            {
                mysqlParameters.Add(DataClass.GetParameter("@p_user_id", userId));

                listdata = getDrtolistParam.getdatafromreder<userDocumentsDO>(
                    DataClass.GetDataReaderFromSpWithParam(mysqlParameters, DBName, "sp_getUserDocuments")
                );
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog("userDocumentBL", "GetUserDocuments",
                    "Exception Message: " + ex.Message + " StackTrace=" + ex.StackTrace, UserId);
            }

            return listdata;
        }
        public userDocumentsDO GetUserDocumentById(int docId)
        {
            userDocumentsDO doc = null;
            try
            {
                List<MySqlParameter> mysqlParameters = new List<MySqlParameter>
        {
            DataClass.GetParameter("@p_user_doc_det_id", docId)
        };

                var list = new getDrtolist().getdatafromreder<userDocumentsDO>(
                    DataClass.GetDataReaderFromSpWithParam(mysqlParameters, DBName, "sp_getUserDocumentById")
                );

                // Since SP returns only one record, assign first item if exists
                if (list.Count > 0)
                    doc = list[0];
            }
            catch (Exception ex)
            {
                new CommonBL().fnStoreErrorLog("userDocumentBL", "GetUserDocumentById", ex.Message + ex.StackTrace, UserId);
            }

            return doc;
        }

        public userDocumentsDO DeactivateDocument(int UserDocDetId)
        {
            userDocumentsDO result = new userDocumentsDO();

            using (MySqlConnection con = new MySqlConnection(MySqlconnection))
            {
                MySqlCommand cmd = new MySqlCommand("Sp_deleteDocument", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@p_type", "DeleteDocument");
                cmd.Parameters.AddWithValue("@p_UserDocDetId", UserDocDetId);

                con.Open();
                using (MySqlDataReader dr = cmd.ExecuteReader())
                {
                    if (dr.Read())
                    {
                        result.Status = dr["Status"].ToString();
                        result.Remarks = dr["Remarks"].ToString();
                    }
                }
            }
            return result;
        }



    }
}
