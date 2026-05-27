using DataObject;
using MySql.Data.MySqlClient;
using Org.BouncyCastle.Pqc.Crypto.Hqc;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessModel
{
    public class RoleDetailsBL
    {
        protected string UserId = null;

        private string DBName = ConfigurationManager.AppSettings["DBName"];
        private static string MySqlconnection = ConfigurationManager.ConnectionStrings["MysqlConnection"].ConnectionString;

        public List<roleDO> SaveRoleDetails(roleDO role)
        {
            List<roleDO> listdata = new List<roleDO>();
            getDrtolist getDrtolistParam = new getDrtolist();
            List<MySqlParameter> mysqlParameters = new List<MySqlParameter>();
            try
            {
                mysqlParameters.Add(DataClass.GetParameter("@p_roledescription", role.Roledescription));
                mysqlParameters.Add(DataClass.GetParameter("@p_insertedby", role.Insertedby));
                mysqlParameters.Add(DataClass.GetParameter("@p_type", "InsertRoleM"));
                listdata = getDrtolistParam.getdatafromreder<roleDO>(DataClass.GetDataReaderFromSpWithParam(mysqlParameters, DBName, "Sp_insertRole"));
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog("RoleDetailsBL", "SaveRoleDetails", "Exception Message" + ex.Message + "Strace=" + ex.StackTrace, UserId);
            }
            return listdata;
        }
        public List<roleDO> AdvanceSearch(roleDO role)
        {
            List<roleDO> listdata = new List<roleDO>();
            getDrtolist getDrtolistParam = new getDrtolist();
            List<MySqlParameter> mysqlParameters = new List<MySqlParameter>();
            try
            {
                //mysqlParameters.Add(DataClass.GetParameter("@p_roledescription", role.Roledescription));
                mysqlParameters.Add(DataClass.GetParameter("@p_searchValue", role.searchValue));
                listdata = getDrtolistParam.getdatafromreder<roleDO>(DataClass.GetDataReaderFromSpWithParam(mysqlParameters, DBName, "sp_SearchAdvRole"));
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog("RoleDetailsBL", "AdvanceSearch", "Exception Message" + ex.Message + "Strace=" + ex.StackTrace, UserId);
            }
            return listdata;
        }
 
        public List<roleDO> GetRoleDetails(int Roleid)
        {
            List<roleDO> listdata = new List<roleDO>();
            getDrtolist getDrtolistParam = new getDrtolist();
            List<MySqlParameter> mysqlParameters = new List<MySqlParameter>();
            try
            {
                mysqlParameters.Add(DataClass.GetParameter("@p_roleid", Roleid));
                mysqlParameters.Add(DataClass.GetParameter("@p_type", "getrole"));
                listdata = getDrtolistParam.getdatafromreder<roleDO>(DataClass.GetDataReaderFromSpWithParam(mysqlParameters, DBName, "Sp_getRole"));
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog("RoleDetailsBL", "GetRoleDetails", "Exception Message" + ex.Message + "Strace=" + ex.StackTrace, UserId);
            }
            return listdata;
        }
        public List<roleDO> ViewAllRoles()
        {
            List<roleDO> listdata = new List<roleDO>();
            getDrtolist getDrtolistParam = new getDrtolist();
            List<MySqlParameter> mysqlParameters = new List<MySqlParameter>();
            try
            {
                mysqlParameters.Add(DataClass.GetParameter("@p_type", "GetAllRole"));
                listdata = getDrtolistParam.getdatafromreder<roleDO>(DataClass.GetDataReaderFromSpWithParam(mysqlParameters, DBName, "Sp_getAllRole"));
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog("RoleDetailsBL", "ViewAllRoles", "Exception Message" + ex.Message + "Strace=" + ex.StackTrace, UserId);
                throw ex;
            }
            return listdata;
        }
        public List<roleDO> UpdateRoleDetails(roleDO role)
        {
            List<roleDO> listdata = new List<roleDO>();
            getDrtolist getDrtolistParam = new getDrtolist();
            List<MySqlParameter> mysqlParameters = new List<MySqlParameter>();
            try
            {
                mysqlParameters.Add(DataClass.GetParameter("@p_roleid", role.Roleid));
                mysqlParameters.Add(DataClass.GetParameter("@p_roledescription", role.Roledescription));
                mysqlParameters.Add(DataClass.GetParameter("@p_updatedby", role.Insertedby));
                mysqlParameters.Add(DataClass.GetParameter("@p_type", "UpdateRole"));
                listdata = getDrtolistParam.getdatafromreder<roleDO>(DataClass.GetDataReaderFromSpWithParam(mysqlParameters, DBName, "Sp_updaterole"));
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog("RoleDetailsBL", "UpdateRoleDetails", "Exception Message" + ex.Message + "Strace=" + ex.StackTrace, UserId);
            }
            return listdata;
        }

        public roleDO DeactivateRole(int Roleid)
        {
            roleDO result = new roleDO();

            using (MySqlConnection con = new MySqlConnection(MySqlconnection))
            {
                MySqlCommand cmd = new MySqlCommand("Sp_deleteRole", con); // ✅ Changed here
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@p_type", "DeleteRole");
                cmd.Parameters.AddWithValue("@p_Roleid", Roleid);
             

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
