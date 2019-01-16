using Log.Entity;
using PetaPoco;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Log.Service
{
    public class RightLogService
    {
        private Database db;

        public RightLogService(Database para )
        {
            this.db = para;
        }

        /// <summary>
        /// 记录【新增】操作日志
        /// </summary>
        /// <param name="user"></param>
        /// <param name="module"></param>
        /// <param name="objId"></param>
        /// <param name="obj"></param>
        public void WriteInsertOperateLog(KeyValuePair<string,string> user, string applicationId, object objId, object obj)
        {
            var groupId = Guid.NewGuid().ToString();
            var type = obj.GetType();
            var module = type.GetCustomAttribute<DisplayNameAttribute>();
            var propList = type.GetProperties();
            foreach (var prop in propList)
            {
                var displayName = prop.GetCustomAttribute<DisplayNameAttribute>();
                if (displayName != null)
                {
                    var value = prop.GetValue(obj);
                    var log = new RightOperateLog
                    {
                        ApplicationId = applicationId,
                        Module = module.DisplayName,
                        ObjId = objId.ToString(),
                        GroupId = groupId,
                        OperateType = OperateTypeEnum.增加,
                        OperateBy = user.Key,
                        OperateByName = user.Value,
                        OperateOn = DateTime.Now,
                        FieldName = prop.Name,
                        FieldDisplayName = displayName.DisplayName,
                        NewValue = value == null ? null : value.ToString()
                    };
                    db.Insert(log);
                }
            }
        }

        /// <summary>
        /// 记录【修改】操作日志
        /// </summary>
        /// <param name="user"></param>
        /// <param name="module"></param>
        /// <param name="objId"></param>
        /// <param name="oldObj"></param>
        /// <param name="newObj"></param>
        public void WriteUpdateOperateLog(KeyValuePair<string, string> user, string applicationId, object objId, object oldObj, object newObj)
        {
            var groupId = Guid.NewGuid().ToString();
            var type = newObj.GetType();
            var module = type.GetCustomAttribute<DisplayNameAttribute>();
            var propList = type.GetProperties();
            foreach (var prop in propList)
            {
                var displayName = prop.GetCustomAttribute<DisplayNameAttribute>();
                if (displayName != null)
                {
                    var oldValue = prop.GetValue(oldObj);
                    var newValue = prop.GetValue(newObj);

                    if (!IsEqual(oldValue, newValue))
                    {
                        var log = new RightOperateLog
                        {
                            ApplicationId = applicationId,
                            Module = module.DisplayName,
                            ObjId = objId.ToString(),
                            GroupId = groupId,
                            OperateType = OperateTypeEnum.修改,
                            OperateBy = user.Key,
                            OperateByName = user.Value,
                            OperateOn = DateTime.Now,
                            FieldName = prop.Name,
                            FieldDisplayName = displayName.DisplayName,
                            OldValue = oldValue == null ? null : oldValue.ToString(),
                            NewValue = newValue == null ? null : newValue.ToString()
                        };
                        db.Insert(log);
                    }
                }
            }
        }

        /// <summary>
        /// 记录【删除】操作日志
        /// </summary>
        /// <param name="user"></param>
        /// <param name="module"></param>
        /// <param name="objId"></param>
        /// <param name="obj"></param>
        public void WriteDeleteOperateLog(KeyValuePair<string, string> user, string applicationId, object objId, object obj, string desc = null)
        {
            var groupId = Guid.NewGuid().ToString();
            var type = obj.GetType();
            var module = type.GetCustomAttribute<DisplayNameAttribute>();
            var log = new RightOperateLog
            {
                ApplicationId = applicationId,
                Module = module.DisplayName,
                ObjId = objId.ToString(),
                GroupId = groupId,
                OperateType = OperateTypeEnum.修改,
                OperateBy = user.Key,
                OperateByName = user.Value,
                OperateOn = DateTime.Now,
                NewValue = desc
            };
            db.Insert(log);
        }

        /// <summary>
        /// 用GetHashCode函数的返回值判断两个字段值是否相等
        /// 以下例外：DateTime
        /// </summary>
        /// <param name="oldValue"></param>
        /// <param name="newValue"></param>
        /// <returns></returns>
        public bool IsEqual(object oldValue, object newValue)
        {
            if (oldValue == null && newValue == null)
            {
                return true;
            }
            else if (oldValue != null && newValue != null)
            {
                var valueType = oldValue.GetType().Name;

                if (oldValue is DateTime)
                {
                    return DateTime.Equals((DateTime)oldValue, (DateTime)newValue);
                }
                return oldValue.GetHashCode() == newValue.GetHashCode();
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 查询日志
        /// </summary>
        /// <param name="applicationId"></param>
        /// <param name="module"></param>
        /// <param name="operateType"></param>
        /// <param name="objId"></param>
        /// <param name="groupId"></param>
        /// <param name="displayName"></param>
        /// <param name="operateOnStart"></param>
        /// <param name="operateOnEnd"></param>
        /// <param name="orderBy"></param>
        /// <param name="orderSort"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="total"></param>
        /// <returns></returns>
        public List<RightOperateLog> SearchOperateLog(string applicationId, string module, int? operateType, string objId, string groupId, string displayName, string operateOnStart, string operateOnEnd, string orderBy, string orderSort, long? pageIndex, long? pageSize, out long total)
        {
            var sql = new Sql(@"WHERE 1=1");
            if (!string.IsNullOrEmpty(applicationId))
            {
                sql.Append("AND ApplicationId = @0", applicationId);
            }
            if (!string.IsNullOrEmpty(module))
            {
                sql.Append("AND Module = @0", module);
            }
            if (operateType.HasValue)
            {
                sql.Append("AND OperateType = @0", operateType);
            }
            if (!string.IsNullOrEmpty(objId))
            {
                sql.Append("AND ObjeId = @0", objId);
            }
            if (!string.IsNullOrEmpty(groupId))
            {
                sql.Append("AND GroupId = @0", groupId);
            }
            if (!string.IsNullOrEmpty(displayName))
            {
                sql.Append("AND DisplayName = @0", displayName);
            }
            if (!string.IsNullOrEmpty(operateOnStart))
            {
                sql.Append("AND OperateOn >= @0", DateTime.Parse(operateOnStart).ToString("yyyy-MM-dd 00:00:00"));
            }
            if (!string.IsNullOrEmpty(operateOnEnd))
            {
                sql.Append("AND OperateOn < @0", DateTime.Parse(operateOnEnd).AddDays(1).ToString("yyyy-MM-dd 00:00:00"));
            }
            if (string.IsNullOrEmpty(orderBy))
            {
                orderBy = "OperateOn";
            }
            orderSort = string.IsNullOrEmpty(orderSort) || orderSort.ToUpper() == "desc" ? "desc" : "asc";
            if (pageIndex.HasValue && pageSize.HasValue)
            {
                var totalSql = new Sql(string.Format(@"SELECT COUNT(0) FROM ({0})t1", sql.SQL), sql.Arguments);
                sql.Append(string.Format("ORDER BY {0} {1}", orderBy, orderSort));
                total = db.ExecuteScalar<int>(totalSql);
                var result = db.Fetch<RightOperateLog>(pageIndex.Value, pageSize.Value, sql);
                return result;
            }
            else
            {
                sql.Append(string.Format("ORDER BY {0} {1}", orderBy, orderSort));
                var result = db.Fetch<RightOperateLog>(sql);
                total = result.Count;
                return result;
            }
        }

        /// <summary>
        /// 获取日志模块列表
        /// </summary>
        /// <returns></returns>
        public List<string> GetOperateLogModules(string applicationId)
        {
            var sql = @"
                    SELECT DINTINCT Module
                    FROM 
                    RM_T_OperateLog
                    WHERE ApplicationId = @0";
            return db.Fetch<string>(sql, applicationId);
        }
    }
}
