using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using System.Web;
using MySql.Data.MySqlClient;
using System.Text;
using System.Data;

using System.Reflection;

namespace Odp.Data.Sql
{
    public class sqlServerConnection
    {
        private string connStr = string.Empty;

        public sqlServerConnection(string connStr)
        {
            this.connStr = connStr;
        }

        public int Version
        {
            get
            {
                DataTable reader = GetReader("SELECT SERVERPROPERTY('productversion') AS version, SERVERPROPERTY ('productlevel') AS level, SERVERPROPERTY ('edition') AS edition");
                int ver = -1;

                foreach (DataRow row in reader.Rows)
                {
                    string fullversion = row[0].ToString();
                    string[] verparts = fullversion.Split('.');
                    ver = Convert.ToInt32(verparts[0]);
                }

                return ver;
            }
        }

        //TODO CHANGE, this is not the way to get a connection to SQL server but it is the fastest to get some work done quickly
        public static sqlServerConnection GetDataServiceInstance()
        {
            sqlServerConnection sql = new sqlServerConnection(sqlAppSettings.DataServiceConnectionString);
            return sql;
        }

        //TODO CHANGE, this is not the way to get a connection to SQL server but it is the fastest to get some work done quickly
        public static sqlServerConnection GetAppMetadataInstance()
        {
            sqlServerConnection sql = new sqlServerConnection(sqlAppSettings.AppMetadataConnectionString);
            return sql;
        }

        //conversion between SQL server and edm. data types (entity data model)
        private string sqlToEdm(string sql)
        {
            //see http://www.odata.org/developers/protocols/overview for refence
            var ct = new string[] { "char", "varchar", "text", "nchar", "nvarchar", "ntext" };
            var it = new string[] { "int", "smallint", "tinyint" };
            var nt = new string[] { "decimal", "numeric" };
            var dt = new string[] { "date", "datetime2", "smalldatetime", "datetime", "time" };
            var bt = new string[] { "binary", "varbinary", "image" };

            if (sql == "text")
            {
                return "Edm.String";
            }
            else
            {
                sql = sql.Substring(0, sql.IndexOf("("));

                if (ct.Any(s => s.Equals(sql)))
                    return "Edm.String";
                else
                    if (it.Any(s => s.Equals(sql)))
                        return "Edm.Int32";
                    else
                        if (sql == "bigint")
                            return "Edm.Int64";
                        else
                            if (sql == "float")
                                return "Edm.Double";
                            else
                                if (sql == "real")
                                    return "Edm.Single";
                                else
                                    if (nt.Any(s => s.Equals(sql)))
                                        return "Edm.Decimal";
                                    else
                                        if (dt.Any(s => s.Equals(sql)))
                                            return "Edm.DateTime";
                                        else
                                            if (sql == "datetimeoffset")
                                                return "Edm.DateTimeOffset";
                                            else
                                                if (bt.Any(s => s.Equals(sql)))
                                                    return "Edm.Binary";
                                                else
                                                    if (sql == "uniqueidentifier")
                                                        return "Edm.Guid";
                                                    else
                                                        return sql;
            }
        }

        public static string WcfToSqlFilter(string wcf)
        {
            String where = "";
            if (!string.IsNullOrEmpty(wcf))
            {
                //and or not is OK
                where = wcf;
                where = ReplaceString(where, " Eq ", " = ", StringComparison.CurrentCultureIgnoreCase);
                where = ReplaceString(where, " Ne ", " <> ", StringComparison.CurrentCultureIgnoreCase);
                where = ReplaceString(where, " Gt ", " > ", StringComparison.CurrentCultureIgnoreCase);
                where = ReplaceString(where, " Ge ", " >= ", StringComparison.CurrentCultureIgnoreCase);
                where = ReplaceString(where, " Lt ", " < ", StringComparison.CurrentCultureIgnoreCase);
                where = ReplaceString(where, " Le ", " <= ", StringComparison.CurrentCultureIgnoreCase);
                where = ReplaceString(where, " Add ", " + ", StringComparison.CurrentCultureIgnoreCase);
                where = ReplaceString(where, " Sub ", " - ", StringComparison.CurrentCultureIgnoreCase);
                where = ReplaceString(where, " Mul ", " * ", StringComparison.CurrentCultureIgnoreCase);
                where = ReplaceString(where, " Div ", " / ", StringComparison.CurrentCultureIgnoreCase);
                //sql = ReplaceString(sql, " Mod ", "<=", StringComparison.CurrentCultureIgnoreCase);???
            }

            // Correctly format oData data types by splitting out all elements
            int i = 0;
            string[] spaceSplit = where.Split(' ');
            foreach (string item in spaceSplit)
            {
                if (item.Contains("datetime'"))
                {
                    //Take datetime'2011-06-07T13:18:25.0348565-07:00' & Convert to yyyy-mm-dd hh:mi:ss
                    DateTime dtNewDate = DateTime.Parse(item.Replace("datetime", "").Replace("'", "").Replace(")", "").Replace("(", ""));
                    string strNewDate = dtNewDate.Year + "-" + dtNewDate.Month.ToString().PadLeft(2, '0') + "-" + dtNewDate.Day.ToString().PadLeft(2, '0') + " " + dtNewDate.Hour.ToString().PadLeft(2, '0') + ":" + dtNewDate.Minute.ToString().PadLeft(2, '0') + ":" + dtNewDate.Second.ToString().PadLeft(2, '0');

                    if (item.EndsWith(")"))
                        spaceSplit[i] = String.Format("convert(datetime,'{0}',120))", strNewDate);
                    else
                        spaceSplit[i] = String.Format("convert(datetime,'{0}',120)", strNewDate);
                }
                i++;
            }

            // Recombine all elements
            where = "";
            foreach (string item in spaceSplit)
            {
                where += ' ' + item;
            }

            return where.Trim();
        }

        //this function serves interactive SDK
        //interactive SDK uses more elaborate metadata
        public DataTable GetEntitySetsMetaData(string SQLquery)
        {
            string sWhere = string.IsNullOrEmpty(SQLquery) ? "" : " WHERE " + SQLquery;
            return GetReader(
                @"SELECT
						odpappmetadata.id, EntityId, odpcategories.Name AS Category, odpdatasources.Name AS Datasource, EntitySet, DatasetName,
						odpdatasources.Description AS DatasourceDescription, downloadlink, datasvclink, datasvckmllink, datasvckmltype,
						LastUpdateDate, ReleasedDate, ExpiredDate, ImageLink, odpappmetadata.Description
					FROM odpappmetadata
					INNER JOIN odpdatasources ON datasource_id = odpdatasources.id
					INNER JOIN odpcategories ON category_id = odpcategories.id" + sWhere + ";");
        }

        //this function serves interactive SDK, gets download links for EntitySeth
        //interactive SDK uses more elaborate metadata
        public DataTable GetEntitySetsLinks(string SQLquery)
        {
            string sWhere = string.IsNullOrEmpty(SQLquery) ? "" : " WHERE " + SQLquery;
            return GetReader(
                @"SELECT
                    odpdownloads.id as ID, odpdownloads.entityset_id as EntitySetID, Name, Description, Link,
                    IconLink, FileType, AdditionalInfoLink, download_count as DownloadCount
                    FROM odpdownloads
                    LEFT JOIN odpanalyticinfodownloads_agg on odpdownloads.id = odpanalyticinfodownloads_agg.download_id
                    " + sWhere + ";");
        }

        public string ObjToString(object obj)
        {
            if (obj != null && obj.GetType() == typeof(System.DateTime))
                return ((System.DateTime)obj).ToString("yyyy-MM-dd HH:mm:ss");
            if (obj != null && obj.GetType() != typeof(System.DBNull))
                return obj.ToString();
            else
                return "";
        }

        //this function serves data service
        //OData feed has only rudimentary SQL server tables/views metadata
        public sqlEntitySetMetadata sqlGetEntitySetMetaData(string entitySet)
        {
            DataTable reader = GetReader(@"SELECT * FROM odpdataservicemetadata WHERE EntitysetName='" + entitySet + "';");
            sqlEntitySetMetadata md = new sqlEntitySetMetadata();

            if (reader.Rows.Count == 0)
                throw (new Exception(string.Format("Entity {0} is not confirgured in meta data table.", entitySet)));

            foreach (DataRow row in reader.Rows)
            {
                md.EntitysetName = ObjToString(entitySet);
                md.TableName = ObjToString(row["TableName"]);
                md.RowId = ObjToString(row["RowId"]);
                md.Longitude = ObjToString(row["Longitude"]);
                md.Latitude = ObjToString(row["Latitude"]);
                md.KmlSnippet = ObjToString(row["KmlSnippet"]);
                md.KmlName = ObjToString(row["KmlName"]);
                md.KmlDescription = ObjToString(row["KmlDescription"]);
            }

            return md;
        }


        //this is SQL server specific
        //excludes columns containing KML relevant data
        public List<sqlTableMetadata> GetTablesMetadata(string entitySet, bool excludeKml)
        {
            var tables = new List<sqlTableMetadata>();
            DataTable entitysets = GetReader(string.IsNullOrEmpty(entitySet) ? "select * from odpdataservicemetadata;" : "select * from odpdataservicemetadata where EntitysetName='" + entitySet + "';");

            foreach(DataRow row in entitysets.Rows)
            {
                var entityset = row["EntitySetName"].ToString();
                var table = row["TableName"].ToString();

                //TODO can we recycle this, see gskhasdfsfdsagf
                //exclude this from the OData feed
                var lon = row["Longitude"].ToString();
                var lat = row["Latitude"].ToString();
                var kml_ = row["KmlSnippet"].ToString();
                bool longlat = lon.GetType() != typeof(System.DBNull) && lat.GetType() != typeof(System.DBNull);
                bool kmlsnip = !longlat && kml_.GetType() != typeof(System.DBNull);
                string longitude = "";
                string latitude = "";
                string kml = "";
                if (longlat)
                {
                    longitude = lon.ToString().ToLower();
                    latitude = lat.ToString().ToLower();
                }
                if (kmlsnip)
                    kml = kml_.ToString().ToLower();

                var props = new List<sqlPropertyMetadata>();
                MySqlConnection conn = new MySqlConnection(connStr);

                string e = string.Empty;
                try
                {
                    conn.Open();
                    var columns = new MySqlCommand("DESCRIBE `" + table + "`", conn);
                    columns.CommandType = CommandType.Text;
                    var creader = columns.ExecuteReader();
                    if (creader.HasRows)
                    {
                        while (creader.Read())
                        {
                            //exclude kml from OData feed
                            if (excludeKml)
                            {
                                if (longlat)
                                {
                                    var col = creader["Field"].ToString().ToLower();
                                    //if(col == longitude || col == latitude)
                                    //continue;//exclude
                                }
                                else
                                    if (kmlsnip)
                                    {
                                        var col = creader["Field"].ToString().ToLower();
                                        if (col == kml)
                                            continue;//exclude
                                    }
                            }

                            //ok to publish the column
                            e = creader["Type"].ToString();
                            props.Add(new sqlPropertyMetadata() { name = creader["Field"].ToString(), dataType = sqlToEdm(creader["Type"].ToString()), nullable = (creader["Type"].ToString() == "1" ? "true" : "false") });
                        }
                    }
                    tables.Add(new sqlTableMetadata() { name = entityset, properties = props });
                }
                catch (Exception ex)
                {
                    Odp.Data.ErrorLog.WriteError(ex.Message);
                }
                finally
                {
                    conn.Close();
                }
            }

            return tables;
        }

        static public string ReplaceString(string str, string oldValue, string newValue, StringComparison comparison)
        {
            StringBuilder sb = new StringBuilder();

            int previousIndex = 0;
            int index = str.IndexOf(oldValue, comparison);
            while (index != -1)
            {
                sb.Append(str.Substring(previousIndex, index - previousIndex));
                sb.Append(newValue);
                index += oldValue.Length;

                previousIndex = index;
                index = str.IndexOf(oldValue, index, comparison);
            }
            sb.Append(str.Substring(previousIndex));

            return sb.ToString();
        }

        public string GetTableName(string dataSource, string entitySet)
        {
            DataTable tables = GetReader("select TableName from odpdataservicemetadata WHERE EntitysetName='" + entitySet + "';");
            string tableName = string.Empty;

            foreach (DataRow row in tables.Rows)
            {
                tableName = row["TableName"].ToString();
            }

            return tableName;
        }

        public string getHashPassword(string userName)
        {
            DataTable users = GetReader("select password from odplogin WHERE username = '" + userName + "';");
            string password = string.Empty;

            foreach (DataRow row in users.Rows)
            {
                password = row["password"].ToString();
            }

            return password;
        }

        public DataTable GetReader(string sql)
        {
            MySqlConnection connection = null;
            DataTable retVal = new DataTable();

            try
            {
                MySqlDataAdapter adapt = new MySqlDataAdapter();
                connection = new MySqlConnection(connStr);

                var query = new MySqlCommand();
                query.Connection = connection;
                query.CommandText = sql;

                adapt.SelectCommand = query;
                adapt.Fill(retVal);
            }
            catch (Exception ex)
            {
                Odp.Data.ErrorLog.WriteError(ex.Message);
            }
            finally
            {
                connection.Close();
            }

            return retVal;
        }

        public int Execute(string sql)
        {
            MySqlConnection connection = null;
            int retVal = -1;

            try
            {
                MySqlDataAdapter adapt = new MySqlDataAdapter();
                connection = new MySqlConnection(connStr);
                connection.Open();

                var query = new MySqlCommand();
                query.Connection = connection;
                query.CommandText = sql;

                retVal = (int)query.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Odp.Data.ErrorLog.WriteError(ex.Message);
            }
            finally
            {
                connection.Close();
            }

            return retVal;
        }

        public DataTable GetReader(string table, string columns, string where)
        {
            MySqlConnection connection = null;
            DataTable retVal = new DataTable();

            try
            {
                MySqlDataAdapter adapt = new MySqlDataAdapter();
                connection = new MySqlConnection(connStr);

                var query = new MySqlCommand();
                query.Connection = connection;
                string sColumns = (columns == "") ? "*" : columns;
                string sWhere = "";
                if (!string.IsNullOrEmpty(sWhere))
                    sWhere = " WHERE " + sWhere;
                query.CommandText = "select " + sColumns + " from " + table + sWhere + ";";

                adapt.SelectCommand = query;
                adapt.Fill(retVal);
            }
            catch (Exception ex)
            {
                Odp.Data.ErrorLog.WriteError(ex.Message);
            }
            finally
            {
                connection.Close();
            }

            return retVal;
        }


        public string getColumnNames(string entity)
        {
            MySqlConnection connection = null;
            string query = string.Format("SELECT * FROM {0} WHERE 1 = 0;", entity);
            string columns = string.Empty;

            try
            {
                connection = new MySqlConnection(connStr);
                connection.Open();

                MySqlCommand sqlComm = new MySqlCommand();
                sqlComm.CommandText = query;
                sqlComm.Connection = connection;

                MySqlDataReader reader = sqlComm.ExecuteReader();

                for (int i = 0; i < reader.FieldCount; i++)
                {
                    columns += String.Format("{0},", reader.GetName(i));
                }
            }
            catch (Exception ex)
            {
                Odp.Data.ErrorLog.WriteError(ex.Message);
            }
            finally
            {
                connection.Close();
            }

            return columns.Substring(0, columns.Length - 1);
        }

        public DataTable GetReader(sqlEntitySetMetadata md, string columns, string WCFquery, int ntop, string nextRowKey, int skip, string order)
        {
            MySqlConnection connection = null;
            DataTable retVal = new DataTable();
            string sql = string.Empty;

            string sColumns = string.Empty;
            string sOrder = string.Empty;
            string sWhere = WcfToSqlFilter(WCFquery); // inner where statement for the filter from the url, odprownumber clause will be in outer query

            if (!string.IsNullOrEmpty(sWhere))
                sWhere = " WHERE (" + sWhere + ")";

            if (!string.IsNullOrEmpty(columns))
                sColumns = columns;
            else
                sColumns = getColumnNames(md.TableName);

            if (!string.IsNullOrEmpty(order))
                sOrder = order;
            else
                sOrder = md.RowId;

            bool isRowId = !string.IsNullOrEmpty(md.RowId);
            if (!isRowId)
                throw new Exception("RowId not defined for entity set '" + md.EntitysetName + "'.");

            if (ntop == -1)
            {
                sql = string.Format(
                @"
                SELECT {0}
                FROM {1}
                {2}   
                ORDER BY {3};
                ", sColumns, md.TableName, sWhere, sOrder);
            }
            else
            {
                sql = string.Format(
                @"
                SELECT {0}
                FROM {1}
                {2}   
                ORDER BY {3}
                LIMIT {4}, {5};
                ", sColumns, md.TableName, sWhere, sOrder, skip, ntop);
            }

            try
            {
                MySqlDataAdapter adapt = new MySqlDataAdapter();
                connection = new MySqlConnection(connStr);

                MySqlCommand query = new MySqlCommand();
                query.Connection = connection;
                query.CommandText = sql;

                adapt.SelectCommand = query;
                adapt.Fill(retVal);
            }
            catch (Exception ex)
            {
                Odp.Data.ErrorLog.WriteError(ex.Message);
            }
            finally
            {
                connection.Close();
            }

            return retVal;
        }

        public bool InsertMetaData(string name, string table, string dataSource, string category, string imageLink, string description)
        {
            return InsertMetaData(name, table, dataSource, category, description, imageLink, string.Empty, string.Empty);
        }

        public bool InsertMetaData(string name, string table, string dataSource, string category, string imageLink, string description, string path, string fType)
        {
            MySqlConnection connection = null;
            bool retVal = false;

            try
            {
                MySqlDataAdapter adapt = new MySqlDataAdapter();
                connection = new MySqlConnection(connStr);
                connection.Open();

                MySqlCommand query = new MySqlCommand();
                query.Connection = connection;
                query.CommandText += "INSERT INTO odpappmetadata (DatasetName, EntityID, EntitySet, Description, category_id, datasource_id, LastUpdateDate, ReleasedDate, downloadLink, ImageLink) VALUES (@name, @EntityID, @table, @description, @category, @dataSource, @LastUpdateDate, @ReleasedDate, @downloadLink, @ImageLink);";

                if (path != string.Empty)
                    query.CommandText += "INSERT INTO odpdownloads (Entityset_id, Name, Description, Link, IconLink, FileType) VALUES (@EntityID, @name, @description, @downloadLink, @ImageLink, @fType)";
                else
                    query.CommandText += "INSERT INTO odpdataservicemetadata (EntitySetName, TableName) VALUES (@table, @table);";

                query.Parameters.AddWithValue("@name", name); 
                query.Parameters.AddWithValue("@EntityID", Guid.NewGuid().ToString()); 
                query.Parameters.AddWithValue("@table", table);
                query.Parameters.AddWithValue("@category", GetCategoryID(category));
                query.Parameters.AddWithValue("@dataSource", GetDataSourceID(dataSource));
                query.Parameters.AddWithValue("@description", description);
                query.Parameters.AddWithValue("@downloadLink", path);
                query.Parameters.AddWithValue("@LastUpdateDate", DateTime.Now);
                query.Parameters.AddWithValue("@ReleasedDate", DateTime.Now);
                query.Parameters.AddWithValue("@ImageLink", imageLink);
                query.Parameters.AddWithValue("@fType", fType);

                query.ExecuteNonQuery();
                retVal = true;
            }
            catch (Exception ex)
            {
                Odp.Data.ErrorLog.WriteError(ex.Message);
            }
            finally
            {
                connection.Close();
            }

            return retVal;
        }

        public bool UpdateMetaData(string name, string table, string dataSource, string category, string description, string imageLink)
        {
            MySqlConnection connection = null;
            bool retVal = false;

            try
            {
                MySqlDataAdapter adapt = new MySqlDataAdapter();
                connection = new MySqlConnection(connStr);
                connection.Open();

                MySqlCommand query = new MySqlCommand();
                query.Connection = connection;
                query.CommandText += "UPDATE odpappmetadata SET DatasetName = @name, Description = @description, category_id = @category, datasource_id = @dataSource, LastUpdateDate = @LastUpdateDate, ImageLink = @ImageLink WHERE EntitySet = @table;";
                //query.CommandText += "UPDATE odpservicemetadata SET **** WHERE EntitySetName = @table";

                query.Parameters.AddWithValue("@name", name);
                query.Parameters.AddWithValue("@table", table);
                query.Parameters.AddWithValue("@category", GetCategoryID(category));
                query.Parameters.AddWithValue("@dataSource", GetDataSourceID(dataSource));
                query.Parameters.AddWithValue("@description", description);
                query.Parameters.AddWithValue("@LastUpdateDate", DateTime.Now);
                query.Parameters.AddWithValue("@ImageLink", imageLink);

                query.ExecuteNonQuery();
                retVal = true;
            }
            catch (Exception ex)
            {
                Odp.Data.ErrorLog.WriteError(ex.Message);
            }
            finally
            {
                connection.Close();
            }

            return retVal;
        }

        public bool InsertCategory(string name)
        {
            MySqlConnection connection = null;
            bool retVal = false;

            try
            {
                MySqlDataAdapter adapt = new MySqlDataAdapter();
                connection = new MySqlConnection(connStr);
                connection.Open();

                MySqlCommand query = new MySqlCommand();
                query.Connection = connection;
                query.CommandText += "INSERT INTO odpcategories (name) VALUES (@name);";
                query.Parameters.AddWithValue("@name", name);

                query.ExecuteNonQuery();
                retVal = true;
            }
            catch (Exception ex)
            {
                Odp.Data.ErrorLog.WriteError(ex.Message);
            }
            finally
            {
                connection.Close();
            }

            return retVal;
        }

        public bool UpdateCategory(int id, string name)
        {
            MySqlConnection connection = null;
            bool retVal = false;

            try
            {
                MySqlDataAdapter adapt = new MySqlDataAdapter();
                connection = new MySqlConnection(connStr);
                connection.Open();

                MySqlCommand query = new MySqlCommand();
                query.Connection = connection;
                query.CommandText += "UPDATE odpcategories SET name = @name WHERE id = @id;";
                query.Parameters.AddWithValue("@id", id);
                query.Parameters.AddWithValue("@name", name);

                query.ExecuteNonQuery();
                retVal = true;
            }
            catch (Exception ex)
            {
                Odp.Data.ErrorLog.WriteError(ex.Message);
            }
            finally
            {
                connection.Close();
            }

            return retVal;
        }

        public bool InsertContributor(string name, string description, string disclaimer)
        {
            MySqlConnection connection = null;
            bool retVal = false;

            try
            {
                MySqlDataAdapter adapt = new MySqlDataAdapter();
                connection = new MySqlConnection(connStr);
                connection.Open();

                MySqlCommand query = new MySqlCommand();
                query.Connection = connection;
                query.CommandText += "INSERT INTO odpdatasources (name, description, disclaimer) VALUES (@name, @description, @disclaimer);";
                query.Parameters.AddWithValue("@name", name);
                query.Parameters.AddWithValue("@description", description);
                query.Parameters.AddWithValue("@disclaimer", disclaimer);

                query.ExecuteNonQuery();
                retVal = true;
            }
            catch (Exception ex)
            {
                Odp.Data.ErrorLog.WriteError(ex.Message);
            }
            finally
            {
                connection.Close();
            }

            return retVal;
        }

        public bool UpdateContributor(int id, string name, string description, string disclaimer)
        {
            MySqlConnection connection = null;
            bool retVal = false;

            try
            {
                MySqlDataAdapter adapt = new MySqlDataAdapter();
                connection = new MySqlConnection(connStr);
                connection.Open();

                MySqlCommand query = new MySqlCommand();
                query.Connection = connection;
                query.CommandText += "UPDATE odpdatasources SET name = @name, description = @description, disclaimer = @disclaimer WHERE id = @id;";
                query.Parameters.AddWithValue("@id", id);
                query.Parameters.AddWithValue("@name", name);
                query.Parameters.AddWithValue("@description", description);
                query.Parameters.AddWithValue("@disclaimer", disclaimer);

                query.ExecuteNonQuery();
                retVal = true;
            }
            catch (Exception ex)
            {
                Odp.Data.ErrorLog.WriteError(ex.Message);
            }
            finally
            {
                connection.Close();
            }

            return retVal;
        }

        public int GetCategoryID(string category)
        {
            MySqlConnection connection = null;
            int id = -1;

            try
            {
                MySqlDataAdapter adapt = new MySqlDataAdapter();
                connection = new MySqlConnection(connStr);
                connection.Open();

                MySqlCommand query = new MySqlCommand();
                query.Connection = connection;
                query.CommandText = "SELECT id FROM odpcategories WHERE name = @name";
                query.Parameters.AddWithValue("@name", category);

                MySqlDataReader reader = query.ExecuteReader();
                while(reader.Read())
                {
                    id = Convert.ToInt16(reader["id"]);
                }
            }
            catch (Exception ex)
            {
                Odp.Data.ErrorLog.WriteError(ex.Message);
            }
            finally
            {
                connection.Close();
            }

            return id;
        }

        public int GetDataSourceID(string dataSource)
        {
            MySqlConnection connection = null;
            int id = -1;

            try
            {
                MySqlDataAdapter adapt = new MySqlDataAdapter();
                connection = new MySqlConnection(connStr);

                MySqlCommand query = new MySqlCommand();
                query.Connection = connection;
                query.CommandText = "SELECT id FROM odpdatasources WHERE name = @name";
                query.Parameters.AddWithValue("@name", dataSource);
                connection.Open();

                MySqlDataReader reader = query.ExecuteReader();
                while (reader.Read())
                {
                    id = Convert.ToInt16(reader["id"]);
                }
            }
            catch (Exception ex)
            {
                Odp.Data.ErrorLog.WriteError(ex.Message);
            }
            finally
            {
                connection.Close();
            }

            return id;
        }
    }

    public class sqlEntitySetMetadata
    {
        public string EntitysetName { get; set; }
        public string TableName { get; set; }
        public string RowId { get; set; }
        public string Longitude { get; set; }
        public string Latitude { get; set; }
        public string KmlSnippet { get; set; }
        public string KmlName { get; set; }
        public string KmlDescription { get; set; }
    }
}
