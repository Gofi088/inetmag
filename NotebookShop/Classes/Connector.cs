using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Diagnostics;
using System.Data;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using Npgsql;

namespace NotebookShop.Classes
{
    public class Connector
    {
        static object obj = null;

        private static IHostingEnvironment _environment;

        public static void SetHostingEnvironment(IHostingEnvironment environment) => _environment = environment;
        
        static string SQLValue { get; set; }

        static string NpgsqlConnectionstring => string.Format(@"Server=localhost;Port=5432;Database=NotebookShop;User Id=postgres;Password={0};", Encryption.Decrypt("iHZEDqXUUJ8Q91gY3LsPxmOWhRYqucvyO26cCl3LH6s="));
        
        static BindingFlags BindingFlags => BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.Default;

        static Type TableName { get; set; }

        static List<string> Columns
        {
            get
            {
                return TableName.GetProperties(BindingFlags).Select(c => c.Name).ToList();
            }
            set
            {
                Columns.Clear();
                Columns.AddRange(value);
            }
        }

        static object ExecuteQuery(string query, string method)
        {
            IDbConnection connection = null;
            IDbCommand dbCommand = null;

            try
            {
                connection = new NpgsqlConnection(NpgsqlConnectionstring);

                connection.Open();

                dbCommand = new NpgsqlCommand(query, (NpgsqlConnection)connection);

                using (var reader = dbCommand.ExecuteReader())
                {
                    switch (method)
                    {
                        #region Get method that return table records
                        case "Get":
                            var list = CreateList(TableName);

                            while (reader.Read())
                            {
                                //Create new object for current record (current table)
                                var resObj = Activator.CreateInstance(TableName);

                                foreach (var column in Columns)
                                {
                                    //Get data from column
                                    PropertyInfo info = TableName.GetProperty(column);
                                    try
                                    {
                                        //Add new value for object
                                        info.SetValue(resObj, Convert.ChangeType(reader[column], info.PropertyType, null));
                                    }
                                    catch (Exception exc)
                                    {
                                        Debug.WriteLine(method + ": " + exc.Message);
                                    }
                                }
                                list.Add(resObj);
                            }

                            obj = list;
                            break;
                        #endregion

                        #region GetValue method that return one value after query
                        case "GetValue":
                            if (reader.FieldCount != 0)
                            {
                                try
                                {
                                    reader.Read();
                                    obj = reader[0].ToString();
                                }
                                catch (Exception exc)
                                {
                                    Debug.WriteLine(method + ": " + exc.Message);
                                }
                            }
                            break;
                        #endregion

                        #region SQLQuery method that just execute query without return values
                        case "SQLQuery":
                            obj = true;
                            break;
                        #endregion

                        default:
                            obj = false;
                            break;
                    }

                    dbCommand.Dispose();
                    connection.Close();

                    return obj;
                }
            }
            catch (Exception exc)
            {
                try
                {
                    connection.Close();
                }
                catch (Exception dbExc)
                {
                    Debug.WriteLine(method + ": " + dbExc.Message);
                }

                try
                {
                    dbCommand.Dispose();
                }
                catch (Exception dbExc)
                {
                    Debug.WriteLine(method + ": " + dbExc.Message);
                }

                Debug.WriteLine(method + ": " + exc.Message);

                return null;
            }
        }

        static bool CheckObj(params object[] data)
        {
            if (data == null || data.Count() < 1)
                return false;
            if (data != null)
                foreach (var item in data)
                    if (item == null)
                        return false;
            TableName = data[0].GetType();
            return true;
        }

        public static bool Insert(params object[] data)
        {
            if (CheckObj(data) == false)
                return false;

            SQLValue = "INSERT INTO " + TableName.Name;

            var strWithColumns = string.Format(" ({0}) values ", (string.Join(",", Columns.ToArray())).TrimEnd(','));
            var strWithValues = string.Empty;

            foreach (var table in data)
            {
                strWithValues += '(';
                foreach (var column in Columns)
                {
                    if (column.Length <= 2 && column.ToLower().IndexOf("id") != -1)
                    {
                        var IdValue = TableName.GetProperty(column).GetValue(table, null);

                        if ((int)IdValue <= 0)
                        {
                            string idValue = GetValue("SELECT MAX(Id) FROM " + TableName.Name);

                            int maxId = 1;

                            if (!string.IsNullOrEmpty(idValue))
                            {
                                try
                                {
                                    maxId = int.Parse(idValue);

                                    if (maxId <= 0)
                                        maxId = 1;
                                    else
                                        maxId++;
                                }
                                catch
                                {
                                    maxId = 1;
                                } 
                            }

                            strWithValues += string.Format("'{0}',", maxId.ToString());

                            continue;
                        }
                    }

                    var value = TableName.GetProperty(column).GetValue(table, null);

                    if (value == null)
                        value = " ";

                    strWithValues += string.Format("'{0}',", value);
                }
                strWithValues = strWithValues.TrimEnd(',') + "),";
            }
            strWithValues = strWithValues.TrimEnd(',') + ";";

            SQLValue = string.Format("{0}{1}{2}", SQLValue, strWithColumns, strWithValues);

            if (ExecuteQuery(SQLValue, "SQLQuery") == null)
                return false;
            else
                return (bool)obj;
        }

        public static List<T> Get<T>()
        {
            TableName = typeof(T);
            return (List<T>)ExecuteQuery("SELECT * FROM " + TableName.Name, "Get");
        }

        public static List<object> Get(string table, string condition)
        {
            try
            {
                TableName = Activator.CreateInstance(Type.GetType("NotebookShop.Models.Database." + table)).GetType();
            }
            catch
            {
                return null;
            }

            string Query = "SELECT * FROM " + table;

            if (!string.IsNullOrEmpty(condition))
                Query += " " + condition;

            var obj = ExecuteQuery(Query, "Get");

            if (obj == null)
                return null;

            if (obj is IEnumerable)
                return ((IEnumerable)obj).Cast<object>().ToList();
            return new List<object>() { obj };
        }
       
        public static string GetValue(string query)
        {
            try
            {
                return ExecuteQuery(query, "GetValue") == null ? null : (string)obj;
            }
            catch
            {
                return null;
            }
        }

        public static bool Update(int oldId, params object[] data)
        {
            if (CheckObj(data) == false)
                return false;

            if (oldId <= 0)
                return false;

            SQLValue = "UPDATE " + TableName.Name + " SET ";

            foreach (var table in data)
            {
                foreach (var column in Columns)
                {
                    SQLValue += column + " = '" + TableName.GetProperty(column).GetValue(table, null) + "',";
                }
                SQLValue = SQLValue.TrimEnd(',') + " WHERE Id = " + oldId + ";";
            }

            if (ExecuteQuery(SQLValue, "SQLQuery") == null)
                return false;
            else
                return (bool)obj;
        }

        public static bool Delete(params object[] data)
        {
            SQLValue = "DELETE FROM " + TableName.Name + " WHERE Id = ";

            foreach (var id in data)
            {
                try
                {
                    SQLValue += TableName.GetProperty("Id").GetValue(id, null);
                }
                catch (Exception)
                {
                    return false;
                }                
            }

            if (ExecuteQuery(SQLValue, "SQLQuery") == null)
                return false;
            else
                return (bool)obj;
        }

        public static bool SQLQuery(string query) => ExecuteQuery(query, "SQLQuery") == null ? false : (bool)obj;

        public static IList CreateList(Type listItemType) => (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(listItemType));

        public static string GetFkValue(string field, int fkId)
        {
            string ReturnField = "Name";

            if (string.IsNullOrEmpty(field))
                return null;

            string Table = field;
            Table = Table.Substring(2);

            if (field.Equals("FkAdmins") || field.Equals("FkUsers"))
                ReturnField = "Email";

            return GetValue(string.Format("SELECT {0} FROM {1} WHERE Id = {2}", ReturnField, Table, fkId));
        }

        public static string GetFkComboBox(string field, int fkId)
        {
            string HtmlComboBox = "<select class=\"form-control adminFormControl\" name = \"" + field + "\" data-validation=\"required\">";

            string ReturnField = "Name";

            if (field.Equals("FkAdmins") || field.Equals("FkUsers"))
                ReturnField = "Login";

            if (string.IsNullOrEmpty(field))
                return null;

            var listData = Get(field.Substring(2).ToLower(), null);

            if (listData == null)
                return null;

            foreach (var item in listData)
            {
                int id = 0;
                string value = "";

                foreach (PropertyInfo property in item.GetType().GetProperties())
                {
                    if (property.Name.Equals("Id"))
                    {
                        id = (int)property.GetValue(item, null);
                        continue;
                    }
                    else if (property.Name.Equals(ReturnField))
                    {
                        value = (string)property.GetValue(item, null);
                    }
                    else
                    {
                        continue;
                    }

                    if (id == fkId)
                    {
                        HtmlComboBox += string.Format("<option value = \"{0}\" selected>{1} ({0})</option>", id, value.TrimEnd());
                        break;
                    }
                    else
                    {
                        HtmlComboBox += string.Format("<option value = \"{0}\">{1} ({0})</option>", id, value.TrimEnd());
                        break;
                    }
                }
            }

            return HtmlComboBox += "</select>";
        }

        public static void CheckAndDeleteFolders(IHostingEnvironment _hostingEnvironment, string table, string searchField)
        {
            string isExists = "";

            try
            {
                string[] folders = Directory.GetDirectories(_hostingEnvironment.WebRootPath + "\\images\\user\\" + table);

                foreach (var folder in folders)
                {
                    var folderName = folder.Substring(folder.LastIndexOf("\\") + 1);

                    isExists = GetValue(string.Format("SELECT {0} FROM {1} WHERE {0} LIKE '%{2}%'", searchField, table, folderName));

                    if (string.IsNullOrEmpty(isExists))
                        Directory.Delete(folder, true);
                }
            }
            catch (Exception exc)
            {
                Debug.WriteLine(exc.Message);
            }
        }
    }
}