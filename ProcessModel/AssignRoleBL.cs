using DataObject;
using MySql.Data.MySqlClient;
using Org.BouncyCastle.Pqc.Crypto.Hqc;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessModel
{
    public class AssignRoleBL
    {
        protected string UserId = null;
        private string DBName = ConfigurationManager.AppSettings["DBName"];
        private static string MySqlconnection = ConfigurationManager.ConnectionStrings["MysqlConnection"].ConnectionString;

        public List<AssignRoleDO> SaveAssignRoleDetails(AssignRoleDO assignrole)
        {
            List<AssignRoleDO> listdata = new List<AssignRoleDO>();
            getDrtolist getDrtolistParam = new getDrtolist();
            List<MySqlParameter> mysqlParameters = new List<MySqlParameter>();
            try
            {
                mysqlParameters.Add(DataClass.GetParameter("@p_userloginid", assignrole.UserName));
                mysqlParameters.Add(DataClass.GetParameter("@p_roleid", assignrole.roledescription));
                mysqlParameters.Add(DataClass.GetParameter("@p_insertedby", assignrole.Insertedby));
                mysqlParameters.Add(DataClass.GetParameter("@p_type", "Insertassignroles"));

                listdata = getDrtolistParam.getdatafromreder<AssignRoleDO>(DataClass.GetDataReaderFromSpWithParam(mysqlParameters, DBName, "Sp_insertassignroles"));
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog("AssignRoleBL", "SaveAssignRoleDetails", "Exception Message" + ex.Message + "Strace=" + ex.StackTrace, UserId);
            }
            return listdata;
        }
        public List<AssignRoleDO> AdvanceSearch(AssignRoleDO assRole)
        {
            List<AssignRoleDO> listdata = new List<AssignRoleDO>();
            getDrtolist getDrtolistParam = new getDrtolist();
            List<MySqlParameter> mysqlParameters = new List<MySqlParameter>();

            try
            {
                mysqlParameters.Add(DataClass.GetParameter("@p_usernameId", assRole.usernameId));
                mysqlParameters.Add(DataClass.GetParameter("@p_roleId", assRole.roleId));

                listdata = getDrtolistParam.getdatafromreder<AssignRoleDO>(DataClass.GetDataReaderFromSpWithParam(mysqlParameters, DBName, "sp_SearchAdvAssignRoles"));
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog("AssignRoleBL", "AdvanceSearch", "Exception Message" + ex.Message + "Strace=" + ex.StackTrace, UserId);
            }
            return listdata;
        }
  
        public AssignRoleDO DeactivateAssignedRole(int Userroledetailsid)
        {
            AssignRoleDO result = new AssignRoleDO();

            using (MySqlConnection con = new MySqlConnection(MySqlconnection))
            {
                MySqlCommand cmd = new MySqlCommand("Sp_DeleteAssignedRole", con); 
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@p_type", "DeleteAssignedRole");
                cmd.Parameters.AddWithValue("@p_userroledetailsid", Userroledetailsid);

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

        //public List<AssignRoleDO> GetAssignedRole(int Userroledetailsid)
        //{
        //    List<AssignRoleDO> listdata = new List<AssignRoleDO>();
        //    getDrtolist getDrtolistParam = new getDrtolist();
        //    List<MySqlParameter> mysqlParameters = new List<MySqlParameter>();
        //    try
        //    {
        //        mysqlParameters.Add(DataClass.GetParameter("@p_type", "GetAssignedRole"));
        //        mysqlParameters.Add(DataClass.GetParameter("@p_userroledetailsid", Userroledetailsid));

        //        //listdata = getDrtolistParam.getdatafromreder<AssignRoleDO>(DataClass.GetDataReaderFromSpWithParam(mysqlParameters, DBName, "Sp_getAssignedRole"));
        //        listdata = getDrtolistParam.getdatafromreder<AssignRoleDO>(DataClass.GetDataReaderFromSpWithParam(mysqlParameters, DBName, "Sp_getAssignedRole"));

        //    }
        //    catch (Exception ex)
        //    {
        //        CommonBL errorlog = new CommonBL();
        //        errorlog.fnStoreErrorLog("assignRole", "GetAssignedRole", "Exception Message" + ex.Message + "Strace=" + ex.StackTrace, UserId);
        //    }
        //    return listdata;
        //}
        public List<AssignRoleDO> GetAssignedRole(int Userroledetailsid)
        {
            List<AssignRoleDO> roledetaillist = new List<AssignRoleDO>();
            try
            {
                using (MySqlConnection con = new MySqlConnection(MySqlconnection))
                {
                    using (MySqlCommand cmd = new MySqlCommand("Sp_getAssignedRole", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add(new MySqlParameter("@p_userroledetailsid", MySqlDbType.Int32)).Value = Userroledetailsid;
                        cmd.Parameters.Add(new MySqlParameter("@p_type", MySqlDbType.VarChar)).Value = "GetAssignedRole";

                        con.Open();
                        using (MySqlDataReader rdr = cmd.ExecuteReader())
                        {
                            while (rdr.Read())
                            {
                                AssignRoleDO assignedrole = new AssignRoleDO
                                {
                                    Userroledetailsid = rdr["userroledetailsid"] is DBNull ? 0 : Convert.ToInt32(rdr["userroledetailsid"]),
                                    Userloginid = rdr["userloginid"] is DBNull ? 0 : Convert.ToInt32(rdr["userloginid"]),
                                    Roleid = rdr["roleid"] is DBNull ? 0 : Convert.ToInt32(rdr["roleid"]),
                                  
                                };
                                roledetaillist.Add(assignedrole);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog("AssignRoleBL", "GetAssignedRole", "Exception Message: " + ex.Message + " StackTrace: " + ex.StackTrace, UserId);

            }
            return roledetaillist;
        }
        public List<AssignRoleDO> ViewAllAssignRoles()
        {
            List<AssignRoleDO> listdata = new List<AssignRoleDO>();
            getDrtolist getDrtolistParam = new getDrtolist();
            List<MySqlParameter> mysqlParameters = new List<MySqlParameter>();
            try
            {
                mysqlParameters.Add(DataClass.GetParameter("@p_type", "GetAllAssignedRole"));

                listdata = getDrtolistParam.getdatafromreder<AssignRoleDO>(DataClass.GetDataReaderFromSpWithParam(mysqlParameters, DBName, "Sp_getAllAssignedRole"));

            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog("assignRole", "ViewAllAssignRoles", "Exception Message" + ex.Message + "Strace=" + ex.StackTrace, UserId);
            }
            return listdata;
        }
        public List<AssignRoleDO> UpdateRoleDetails(AssignRoleDO assignedRole)
        {
            List<AssignRoleDO> listdata = new List<AssignRoleDO>();
            getDrtolist getDrtolistParam = new getDrtolist();
            List<MySqlParameter> mysqlParameters = new List<MySqlParameter>();

            try
            {
                mysqlParameters.Add(DataClass.GetParameter("@p_userroledetailsid", assignedRole.Userroledetailsid));
                mysqlParameters.Add(DataClass.GetParameter("@p_userloginid", assignedRole.UserName));
                mysqlParameters.Add(DataClass.GetParameter("@p_roleid", assignedRole.roledescription));
                mysqlParameters.Add(DataClass.GetParameter("@p_updatedby", assignedRole.Insertedby));
                mysqlParameters.Add(DataClass.GetParameter("@p_type", "Updateassignroles"));

                listdata = getDrtolistParam.getdatafromreder<AssignRoleDO>(DataClass.GetDataReaderFromSpWithParam(mysqlParameters, DBName, "Sp_updateassignroles"));
            }

            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog("assignRole", "ViewAllAssignRoles", "Exception Message" + ex.Message + "Strace=" + ex.StackTrace, UserId);
            }
            return listdata;
        }
    }
}
