using Newtonsoft.Json;
using PetaPoco;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class Util
    {
        /// <summary>
        /// 将枚举值转换为JSON对象，键为数字值
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static List<KeyValuePair<string, int>> EnumToJson(Type type)
        {
            var list = new List<KeyValuePair<string, int>>();
            foreach (var value in Enum.GetValues(type))
            {
                list.Add(new KeyValuePair<string, int>(value.ToString(), (int)value));
            }

            return list;
        }

        /// <summary>
        /// 数据表转换成list
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static List<T> TableToList<T>(DataTable dt) where T : class, new()
        {
            Type type = typeof(T);
            List<T> list = new List<T>();
            Dictionary<string, PropertyInfo> colNames = new Dictionary<string, PropertyInfo>();
            Dictionary<string, TypeConverter> colTypeConverters = new Dictionary<string, TypeConverter>();
            PropertyInfo[] pArray = type.GetProperties();
            foreach (DataColumn col in dt.Columns)
            {
                var q = from c in pArray where c.Name.ToUpper() == col.ColumnName.ToUpper() select c;
                if (q.Count() > 0)
                {
                    colNames.Add(col.ColumnName, q.First());
                    colTypeConverters[col.ColumnName] = TypeDescriptor.GetConverter(colNames[col.ColumnName].PropertyType);
                }
            }

            foreach (DataRow row in dt.Rows)
            {

                T entity = new T();
                foreach (string key in colNames.Keys)
                {
                    try
                    {
                        if (row[key] != System.DBNull.Value)
                        {
                            var tc = colTypeConverters[key];
                            if (tc.CanConvertFrom(row[key].GetType()))
                            {
                                colNames[key].SetValue(entity, tc.ConvertFrom(row[key]), null);
                            }
                            else
                            {
                                var vStr = row[key].ToString();
                                if (!String.IsNullOrWhiteSpace(vStr))
                                {
                                    colNames[key].SetValue(entity, tc.ConvertFromString(vStr), null);
                                }
                            }
                        }

                    }
                    catch
                    {

                    }
                }
                list.Add(entity);
            }
            return list;
        }

        /// <summary>
        /// 序列化成字符串
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string ToJson<T>(T obj)
        {
            var str = JsonConvert.SerializeObject(obj);
            return str;
        }

        /// <summary>
        /// 反序列化成对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="json"></param>
        /// <param name="isNeedTrim">如果为true则去掉string类型的前后空格</param>
        /// <returns></returns>
        public static T ToObject<T>(string json, bool isNeedTrim = false)
        {
            var obj = JsonConvert.DeserializeObject<T>(json);

            if (isNeedTrim)
            {
                TrimObject<T>(obj);
            }

            return obj;
        }

        /// <summary>
        /// 为Trim特性去除空格
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        public static void TrimObject<T>(T obj)
        {
            var t = typeof(T);
            var props = t.GetProperties();

            foreach (var prop in props)
            {
                if (prop.PropertyType == typeof(string) && prop.GetCustomAttribute<DisplayNameAttribute>() != null)
                {
                    var val = prop.GetValue(obj);
                    if (val != null)
                    {
                        prop.SetValue(obj, val.ToString().Trim());
                    }
                }
            }
        }

        /// <summary>
        /// petapoco分页
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="db"></param>
        /// <param name="sql"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="orderBy"></param>
        /// <param name="orderSort"></param>
        /// <param name="total"></param>
        /// <returns></returns>
        public static List<T> FetchPage<T>(Database db, Sql sql, int pageIndex, int pageSize, string orderBy, out int total)
        {
            if (string.IsNullOrEmpty(orderBy))
            {
                throw new Exception("orderby is required");
            }
            var totalSql = new Sql(string.Format(@"SELECT COUNT(0) FROM ({0})t1", sql.SQL), sql.Arguments);
            sql = new Sql(string.Format(@"
                        SELECT * FROM (
                            SELECT t1.*
                             ,Row_Number() OVER ({3}) AS Rowid
                            FROM (
                             {0}
                            )t1
                        )t2
                        WHERE t2.Rowid BETWEEN {1} AND {2}", sql.SQL, (pageIndex - 1) * pageSize + 1, pageIndex * pageSize, orderBy), sql.Arguments);
            total = db.ExecuteScalar<int>(totalSql);
            return db.Fetch<T>(sql);
        }

        /// <summary>
        /// 发送http get 请求
        /// </summary>
        /// <param name="url"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public static string HttpGet(string url, int timeout = 60000)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "GET";
            request.ContentType = "text/html;charset=UTF-8";
            request.UserAgent = null;
            request.Timeout = timeout;

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Stream myResponseStream = response.GetResponseStream();
            StreamReader myStreamReader = new StreamReader(myResponseStream, Encoding.GetEncoding("utf-8"));
            string retStr = myStreamReader.ReadToEnd();
            myStreamReader.Close();
            myResponseStream.Close();

            return retStr;
        }

        /// <summary>
        /// Http请求并序列化
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public static T HttpGet<T>(string url, int timeout = 60000)
        {
            var retStr = HttpGet(url, timeout);
            return JsonConvert.DeserializeObject<T>(retStr);
        }
    }
}
