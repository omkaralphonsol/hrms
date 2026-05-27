using DataObject;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace ProcessModel
{
    public class userBAL
    {
        protected string UserId = null;
        private string DBName = ConfigurationManager.AppSettings["DBName"];
        private static string MySqlconnection = ConfigurationManager.ConnectionStrings["MysqlConnection"].ConnectionString;
        public List<MenuData> GetMenuHierarchy(MenuDO menuDO)
        {
            List<MenuData> menuDataList = new List<MenuData>();

            try
            {
                using (MySqlConnection con = new MySqlConnection(MySqlconnection))
                {
                    using (MySqlCommand cmd = new MySqlCommand("Sp_GetRoleWiseMenu", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@p_type", menuDO.type);
                        cmd.Parameters.AddWithValue("@p_userid", menuDO.userId);

                        con.Open();
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                string menuId = reader["menuid"].ToString();
                                string menuName = reader["menu"].ToString();
                                string menuLink = reader["menulink"].ToString();
                                string icon = reader["Icon"].ToString();

                                MenuData existingMenuData = menuDataList.Find(m => m.MenuId == menuId);
                                if (existingMenuData == null)
                                {
                                    MenuData menuData = new MenuData
                                    {
                                        MenuId = menuId,
                                        Menu = menuName,
                                        MenuLink = menuLink,
                                        Icon = icon,
                                        SubMenus = new List<SubMenuData>()
                                    };
                                    menuDataList.Add(menuData);
                                    existingMenuData = menuData;
                                }

                                string submenuId = reader["submenuid"].ToString();
                                string submenu = reader["SubMenu"].ToString();
                                string submenuLink = reader["submenu_link"].ToString();

                                SubMenuData existingSubMenuData = existingMenuData.SubMenus.Find(s => s.SubMenuId == submenuId);
                                if (existingSubMenuData == null && !string.IsNullOrEmpty(submenu))
                                {
                                    SubMenuData subMenuData = new SubMenuData
                                    {
                                        SubMenuId = submenuId,
                                        SubMenu = submenu,
                                        SubMenuLink = submenuLink
                                    };
                                    existingMenuData.SubMenus.Add(subMenuData);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog("userBAL", "GetMenuHierarchy", "Exception Message: " + ex.Message + " Strace=" + ex.StackTrace, UserId);
            }

            return menuDataList;
        }

        public List<MenuDO> GetUser(MenuDO menuDO)
        {
            List<MenuDO> listdata = new List<MenuDO>();
            try
            {
                getDrtolist getDrtolistParam = new getDrtolist();
                List<MySqlParameter> mysqlParameters = new List<MySqlParameter>();
                mysqlParameters.Add(DataClass.GetParameter("@p_userId", menuDO.userId));
                listdata = getDrtolistParam.getdatafromreder<MenuDO>(DataClass.GetDataReaderFromSpWithParam(mysqlParameters, DBName, "Sp_getUserName")).ToList();
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog("userBAL", "GetUser", "Exception Message" + ex.Message + "Strace=" + ex.StackTrace, UserId);
            }
            return listdata;
        }

      
    }
}
