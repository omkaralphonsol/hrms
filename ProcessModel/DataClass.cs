using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using MySql.Data.MySqlClient;

namespace ProcessModel
{
    public static class DataClass
    {
        static string DBfixName = ConfigurationManager.AppSettings["DBName"];
        static string SQLstr = ConfigurationManager.ConnectionStrings["MysqlConnection"].ConnectionString;

        public static MySqlDataReader GetDataReaderFromSpWithParam(List<MySqlParameter> sqlParamList, string DBname, string SPName)
        {
            MySqlDataReader dr = null;
            if (string.IsNullOrEmpty(DBname))
            {
                DBname = DBfixName;
            }

            MySqlConnection Con = new MySqlConnection(string.Format(SQLstr, DBname));
            try
            {
                Con.Open();
            }
            catch (Exception)
            {
                return dr;
            }

            using (MySqlCommand cmd = new MySqlCommand())
            {
                cmd.Connection = Con;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = SPName;
                cmd.CommandTimeout = 0;

                if (sqlParamList != null)
                {
                    foreach (MySqlParameter sp in sqlParamList)
                    {
                        cmd.Parameters.Add(sp);
                    }
                }

                dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                return dr;
            }
        }

        public static MySqlParameter GetOutputParameter(string parametername, MySqlDbType type, int size)
        {
            MySqlParameter param = new MySqlParameter(parametername, type, size);
            param.Direction = ParameterDirection.Output;
            return param;
        }

        public static MySqlDataReader GetDataReaderFromSp(string DBname, string SPName)
        {
            MySqlDataReader dr = null;
            if (string.IsNullOrEmpty(DBname))
            {
                DBname = DBfixName;
            }

            MySqlConnection Con = new MySqlConnection(string.Format(SQLstr, DBname));
            try
            {
                Con.Open();
            }
            catch (Exception)
            {
                return dr;
            }

            using (MySqlCommand cmd = new MySqlCommand())
            {
                cmd.Connection = Con;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = SPName;
                cmd.CommandTimeout = 0;

                dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                return dr;
            }
        }

        public static MySqlParameter GetParameter(string parametername, object value)
        {
            MySqlParameter param = new MySqlParameter(parametername, value);
            return param;
        }

        public static int ExecuteNonQuerySp(string SPName, List<MySqlParameter> sqlParamList, string DBname)
        {
            if (string.IsNullOrEmpty(DBname))
            {
                DBname = DBfixName;
            }
            int rowsAffected = 0;

            using (MySqlConnection Con = new MySqlConnection(string.Format(SQLstr, DBname)))
            {
                using (MySqlCommand cmd = new MySqlCommand())
                {
                    cmd.Connection = Con;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = SPName;
                    cmd.CommandTimeout = 0;

                    if (sqlParamList != null)
                    {
                        foreach (MySqlParameter sp in sqlParamList)
                        {
                            cmd.Parameters.Add(sp);
                        }
                    }

                    try
                    {
                        Con.Open();
                        rowsAffected = cmd.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        CommonBL errorlog = new CommonBL();
                        errorlog.fnStoreErrorLog("DataClass", "ExecuteNonQuerySp", "Exception Message: " + ex.Message + " StackTrace: " + ex.StackTrace, "0");
                    }
                }
            }

            return rowsAffected;
        }
    }

    public class getDrtolist
    {
        public List<T> getdatafromreder<T>(MySqlDataReader dr) where T : new()
        {
            List<T> list = new List<T>();
            try
            {
                Type type = typeof(T);
                PropertyInfo[] pi = type.GetProperties();
                object[] values = null;

                if (dr.HasRows)
                {
                    var fieldnames = Enumerable.Range(0, dr.FieldCount).Select(i => dr.GetName(i).ToUpper()).ToArray();
                    while (dr.Read())
                    {
                        T objectofclass = new T();
                        foreach (PropertyInfo pp in pi)
                        {
                            try
                            {
                                if (fieldnames.Contains(pp.Name.ToUpper()))
                                {
                                    values = pp.GetCustomAttributes(true);
                                    if ((dr[pp.Name] != DBNull.Value) && (values.Length == 0))
                                    {
                                        if (pp.PropertyType == typeof(string))
                                        {
                                            pp.SetValue(objectofclass, dr[pp.Name].ToString(), null);
                                        }
                                        else
                                        {
                                            pp.SetValue(objectofclass, Convert.ChangeType(dr[pp.Name], pp.PropertyType), null);
                                        }
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                CommonBL log = new CommonBL();
                                log.insertlog("DataClass.cs", ex.Message, "list count", "Process End", "GetTaskDatabyId");
                            }
                        }
                        list.Add(objectofclass);
                    }
                }
            }
            catch (Exception ee)
            {
                CommonBL log = new CommonBL();
                log.insertlog("DataClass.cs", ee.Message, "list count", "Process End", "GetTaskDatabyId");
                dr.Close();
            }
            dr.Close();
            return list;
        }

        public List<T> getdatafromrederwithrepeat<T>(MySqlDataReader dr) where T : new()
        {
            List<T> list = new List<T>();
            try
            {
                Type type = typeof(T);
                PropertyInfo[] pi = type.GetProperties();
                object[] values = null;

                if (dr.HasRows)
                {
                    var fieldnames = Enumerable.Range(0, dr.FieldCount).Select(i => dr.GetName(i).ToUpper()).ToArray();
                    while (dr.Read())
                    {
                        T objectofclass = new T();
                        foreach (PropertyInfo pp in pi)
                        {
                            try
                            {
                                if (fieldnames.Contains(pp.Name.ToUpper()))
                                {
                                    values = pp.GetCustomAttributes(true);
                                    if ((dr[pp.Name] != DBNull.Value) && (values.Length == 0))
                                    {
                                        if (pp.PropertyType == typeof(string))
                                        {
                                            pp.SetValue(objectofclass, dr[pp.Name].ToString(), null);
                                        }
                                        else
                                        {
                                            pp.SetValue(objectofclass, Convert.ChangeType(dr[pp.Name], pp.PropertyType), null);
                                        }
                                    }
                                }
                            }
                            catch (IndexOutOfRangeException) { }
                        }
                        list.Add(objectofclass);
                    }
                }
            }
            catch
            {
                dr.Close();
            }
            return list;
        }
    }
//    public static class DataClass
//    {
//        static string DBfixName = ConfigurationManager.AppSettings["DBName"];
//        static string SQLstr = ConfigurationManager.ConnectionStrings["MysqlConnection"].ConnectionString;
//        public static MySqlDataReader GetDataReaderFromSpWithParam(List<MySqlParameter> sqlParamList, string DBname, string SPName)
//        {
//            string mess = null;
//            MySqlDataReader dr = null;
//            if (DBname == "")
//            {
//                DBname = DBfixName;
//            }
//            MySqlConnection Con = new MySqlConnection(string.Format(SQLstr, DBname));
//#pragma warning disable CS0168 // The variable 'ex' is declared but never used
//            try
//            {
//                Con.Open();
//            }
//            catch (Exception ex)
//            {
//                return dr;
//            }
//#pragma warning restore CS0168 // The variable 'ex' is declared but never used

//            using (MySqlCommand cmd = new MySqlCommand())
//            {
//                cmd.Connection = Con;
//                cmd.CommandType = CommandType.StoredProcedure;
//                cmd.CommandText = SPName;
//                cmd.CommandTimeout = 0;
//                if (sqlParamList != null)
//                {
//                    foreach (MySqlParameter sp in sqlParamList)
//                    {
//                        cmd.Parameters.Add(sp);
//                        cmd.Parameters["@result"].Direction = ParameterDirection.Output;
//                    }
//                }

//                dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);


//                return dr;
//            }
//        }

//        public static MySqlParameter GetOutputParameter(string parametername, MySqlDbType type, int size)
//        {
//            MySqlParameter param = new MySqlParameter(parametername, type, size);
//            param.Direction = ParameterDirection.Output;
//            return param;
//        }

//        public static MySqlDataReader GetDataReaderFromSp(string DBname, string SPName)
//        {
//            MySqlDataReader dr = null;
//            if (DBname == "")
//            {
//                DBname = DBfixName;
//            }
//            MySqlConnection Con = new MySqlConnection(string.Format(SQLstr, DBname));
//#pragma warning disable CS0168 // The variable 'ex' is declared but never used
//            try
//            {
//                Con.Open();
//            }
//            catch (Exception ex)
//            {
//                return dr;
//            }
//#pragma warning restore CS0168 // The variable 'ex' is declared but never used

//            using (MySqlCommand cmd = new MySqlCommand())
//            {
//                cmd.Connection = Con;
//                cmd.CommandType = CommandType.StoredProcedure;
//                cmd.CommandText = SPName;
//                cmd.CommandTimeout = 0;
//                dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
//                return dr;
//            }
//        }

//        public static MySqlParameter GetParameter(string parametername, object value)
//        {
//            MySqlParameter param = new MySqlParameter(parametername, value);
//            return param;
//        }

//        public static int ExecuteNonQuerySp(string SPName, List<MySqlParameter> sqlParamList, string DBname)
//        {
//            if (string.IsNullOrEmpty(DBname))
//            {
//                DBname = DBfixName;
//            }

//            int rowsAffected = 0;

//            using (MySqlConnection Con = new MySqlConnection(string.Format(SQLstr, DBname)))
//            {
//                using (MySqlCommand cmd = new MySqlCommand())
//                {
//                    cmd.Connection = Con;
//                    cmd.CommandType = CommandType.StoredProcedure;
//                    cmd.CommandText = SPName;
//                    cmd.CommandTimeout = 0;

//                    if (sqlParamList != null)
//                    {
//                        foreach (MySqlParameter sp in sqlParamList)
//                        {
//                            cmd.Parameters.Add(sp);
//                        }
//                    }

//                    try
//                    {
//                        Con.Open();
//                        rowsAffected = cmd.ExecuteNonQuery();
//                    }
//                    catch (Exception ex)
//                    {
//                        CommonBL errorlog = new CommonBL();
//                        errorlog.fnStoreErrorLog("DataClass", "ExecuteNonQuerySp", "Exception Message: " + ex.Message + " StackTrace: " + ex.StackTrace, "0");
//                    }
//                }
//            }

//            return rowsAffected;
//        }
//    }

//    public class getDrtolist
//    {
//        public List<T> getdatafromreder<T>(MySqlDataReader dr) where T : new()
//        {
//            List<T> list = new List<T>();
//#pragma warning disable CS0168 // The variable 'ee' is declared but never used
//            try
//            {
//                Type type = typeof(T);
//                PropertyInfo[] pi = type.GetProperties();
//                object[] values = null;
//                if (dr.HasRows)
//                {
//                    var fieldnames = Enumerable.Range(0, dr.FieldCount).Select(i => dr.GetName(i).ToUpper()).ToArray();
//                    while (dr.Read())
//                    {
//                        T objectofclass = new T();
//                        foreach (PropertyInfo pp in pi)
//                        {
//                            try
//                            {
//                                if (fieldnames.Contains(pp.Name.ToUpper()))
//                                {
//                                    values = pp.GetCustomAttributes(true);
//                                    if ((dr[pp.Name] != System.DBNull.Value) && (values.Length == 0))
//                                    {
//                                        if (pp.PropertyType.ToString() == "System.String")
//                                        {
//                                            pp.SetValue(objectofclass, dr[pp.Name].ToString(), null);
//                                        }
//                                        else
//                                        {
//                                            pp.SetValue(objectofclass, Convert.ChangeType(dr[pp.Name], pp.PropertyType), null);
//                                        }
//                                    }
//                                }
//                            }
//                            catch (Exception ex)
//                            {
//                                CommonBL log = new CommonBL();
//                                log.insertlog("DataClass.cs", ex.Message, "list count", "Process End", "GetTaskDatabyId");

//                            }
//                        }
//                        list.Add(objectofclass);
//                    }
//                }
//            }
//            catch (Exception ee)
//            {
//                CommonBL log = new CommonBL();
//                log.insertlog("DataClass.cs", ee.Message, "list count", "Process End", "GetTaskDatabyId");

//                dr.Close();
//            }
//#pragma warning restore CS0168 // The variable 'ee' is declared but never used
//            dr.Close();
//            return list;
//        }

//        public List<T> getdatafromrederwithrepeat<T>(MySqlDataReader dr) where T : new()
//        {
//            List<T> list = new List<T>();
//#pragma warning disable CS0168 // The variable 'ee' is declared but never used
//            try
//            {
//                Type type = typeof(T);
//                PropertyInfo[] pi = type.GetProperties();
//                object[] values = null;
//                if (dr.HasRows)
//                {
//                    var fieldnames = Enumerable.Range(0, dr.FieldCount).Select(i => dr.GetName(i).ToUpper()).ToArray();
//                    while (dr.Read())
//                    {
//                        T objectofclass = new T();
//                        foreach (PropertyInfo pp in pi)
//                        {
//                            try
//                            {
//                                if (fieldnames.Contains(pp.Name.ToUpper()))
//                                {
//                                    values = pp.GetCustomAttributes(true);
//                                    if ((dr[pp.Name] != System.DBNull.Value) && (values.Length == 0))
//                                    {
//                                        if (pp.PropertyType.ToString() == "System.String")
//                                        {
//                                            pp.SetValue(objectofclass, dr[pp.Name].ToString(), null);
//                                        }
//                                        else
//                                        {
//                                            pp.SetValue(objectofclass, Convert.ChangeType(dr[pp.Name], pp.PropertyType), null);
//                                        }
//                                    }
//                                }
//                            }
//                            catch (IndexOutOfRangeException) { }
//                        }
//                        list.Add(objectofclass);
//                    }
//                }
//            }
//            catch (Exception ee)
//            {
//                dr.Close();
//            }
//#pragma warning restore CS0168 // The variable 'ee' is declared but never used
//            return list;
//        }



//    }
}
