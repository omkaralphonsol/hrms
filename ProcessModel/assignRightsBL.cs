using DataObject;
using MySql.Data.MySqlClient;
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
    public class assignRightsBL
    {
        protected string UserId = null;

private string DBName = ConfigurationManager.AppSettings["DBName"];
        private static string MySqlconnection = ConfigurationManager.ConnectionStrings["MysqlConnection"].ConnectionString;
        public List<rightsDO> SaveRights(rightsDO rights)
        {
            List<rightsDO> listdata = new List<rightsDO>();
            getDrtolist getDrtolistParam = new getDrtolist();
            List<MySqlParameter> mysqlParameters = new List<MySqlParameter>();
            try
            {
                mysqlParameters.Add(DataClass.GetParameter("@p_roleid", rights.roleid));
                mysqlParameters.Add(DataClass.GetParameter("@p_menuid", rights.menuid));
                mysqlParameters.Add(DataClass.GetParameter("@p_submenuid", rights.submenuid));

                mysqlParameters.Add(DataClass.GetParameter("@p_insertedby", rights.Insertedby));
                mysqlParameters.Add(DataClass.GetParameter("@p_type", "AssignRights"));
                listdata = getDrtolistParam.getdatafromreder<rightsDO>(DataClass.GetDataReaderFromSpWithParam(mysqlParameters, DBName, "Sp_insertAssignRights"));
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog("assignRightsBL", "SaveRights", "Exception Message" + ex.Message + "Strace=" + ex.StackTrace, UserId);
            }
            return listdata;
        }

        public List<rightsDO> AdvanceSearch(rightsDO assRghts)
        {
            List<rightsDO> listdata = new List<rightsDO>();
            getDrtolist getDrtolistParam = new getDrtolist();
            List<MySqlParameter> mysqlParameters = new List<MySqlParameter>();
            try
            {
                mysqlParameters.Add(DataClass.GetParameter("@p_roleId", assRghts.roleId));
                mysqlParameters.Add(DataClass.GetParameter("@p_menuId", assRghts.menuId));
                mysqlParameters.Add(DataClass.GetParameter("@p_submenuId", assRghts.submenuId));
                listdata = getDrtolistParam.getdatafromreder<rightsDO>(DataClass.GetDataReaderFromSpWithParam(mysqlParameters, DBName, "sp_SearchAdvAssignRightss"));
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog("assignRightsBL", "AdvanceSearch", "Exception Message" + ex.Message + "Strace=" + ex.StackTrace, UserId);
            }
            return listdata;
        }

        //public List<rightsDO> ViewAllAssignRights()
        //{
        //    List<rightsDO> listdata = new List<rightsDO>();
        //    getDrtolist getDrtolistParam = new getDrtolist();
        //    List<MySqlParameter> mysqlParameters = new List<MySqlParameter>();
        //    try
        //    {
        //        mysqlParameters.Add(DataClass.GetParameter("@p_type", "getrightdata"));
        //        listdata = getDrtolistParam.getdatafromreder<rightsDO>(DataClass.GetDataReaderFromSpWithParam(mysqlParameters, DBName, "Sp_getassignrightdata"));
        //    }
        //    catch (Exception ex)
        //    {
        //        CommonBL errorlog = new CommonBL();
        //        errorlog.fnStoreErrorLog("assignRightsBL", "ViewAllAssignRights", "Exception Message" + ex.Message + "Strace=" + ex.StackTrace, UserId);
        //    }
        //    return listdata;
        //}
  
        public List<rightsDO> ViewAllAssignRights()
        {
            List<rightsDO> listdata = new List<rightsDO>();

            try
            {
                using (MySqlConnection conn = new MySqlConnection(MySqlconnection))
                {
                    conn.Open();

                    using (MySqlCommand cmd = new MySqlCommand("Sp_getassignrightdata", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("@p_type", "getrightdata");

                        using (MySqlDataReader dr = cmd.ExecuteReader())
                        {
                            while (dr.Read())
                            {
                                rightsDO obj = new rightsDO
                                {
                                    userrightid = dr["userrightid"] != DBNull.Value ? dr["userrightid"].ToString() : string.Empty,
                                    roleid = dr["roleid"] != DBNull.Value ? Convert.ToInt32(dr["roleid"]) : 0,
                                    roledescription = dr["roledescription"] != DBNull.Value ? dr["roledescription"].ToString() : string.Empty,
                                    menuid = dr["menuid"] != DBNull.Value ? Convert.ToInt32(dr["menuid"]) : 0,
                                    menu = dr["menu"] != DBNull.Value ? dr["menu"].ToString() : string.Empty,
                                    submenuid = dr["submenuid"] != DBNull.Value ? Convert.ToInt32(dr["submenuid"]) : 0,
                                    submenu = dr["submenu"] != DBNull.Value ? dr["submenu"].ToString() : string.Empty
                                };


                                listdata.Add(obj);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog("assignRightsBL", "ViewAllAssignRights",
                    "Exception Message: " + ex.Message + " StackTrace: " + ex.StackTrace, UserId);
            }

            return listdata;
        }

        
        public rightsDO DeactivateRights(int userrightsid)
        {
            rightsDO result = new rightsDO();

            using (MySqlConnection con = new MySqlConnection(MySqlconnection))
            {
                MySqlCommand cmd = new MySqlCommand("Sp_deleteassignright", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@p_type", "DeleteRights");
                cmd.Parameters.AddWithValue("@p_userrightsid", userrightsid);

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
