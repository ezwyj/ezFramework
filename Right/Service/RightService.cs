using Common;
using Log.Service;
using log4net;
using PetaPoco;
using Right.Entity;
using Right.Model;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Right.Service
{
    public class RightService
    {
        private Database db;
        private RightLogService logService;

        public RightService(Database v)
        {
            this.db = v;
            logService = new RightLogService(this.db);
        }

        /// <summary>
        /// 获取应用列表
        /// </summary>
        /// <param name="applicationName"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="total"></param>
        /// <returns></returns>
        public List<RightApplication> GetApplicationList(string applicationName, int? pageIndex, int? pageSize, out int total)
        {
            var sql = new Sql(@"
                        SELECT Id,
                            ApplicationId,
                            ApplicationName
                        FROM RM_T_RightApplication
                        WHERE 1=1");
            if (!string.IsNullOrEmpty(applicationName))
            {
                sql.AppendLike("AND ApplicationName LIKE @0", applicationName);
            }
            List<RightApplication> result;
            if (pageIndex.HasValue && pageSize.HasValue)
            {
                result = Util.FetchPage<RightApplication>(db, sql, pageIndex.Value, pageSize.Value, "ORDER BY Id", out total);
            }
            else
            {
                result = db.Fetch<RightApplication>(sql);
                total = result.Count;
            }
            return result;
        }

        /// <summary>
        /// 根据应用id获取应用
        /// </summary>
        /// <returns></returns>
        public RightApplication GetApplication(string applicationId)
        {
            return db.SingleOrDefault<RightApplication>("WHERE applicationId = @0", applicationId);
        }

        /// <summary>
        /// 保存应用
        /// </summary>
        /// <param name="badge"></param>
        /// <param name="item"></param>
        public void SaveApplication(KeyValuePair<string,string> user, RightApplication item)
        {
            if (item.ApplicationId == ConfigurationManager.AppSettings["RightApplicationId"])
            {
                throw new InfoException("不能编辑权限管理应用");
            }
           
            db.BeginTransaction();
            try
            {
                if (string.IsNullOrEmpty(item.ApplicationId))
                {
                    if (db.Exists<RightApplication>("ApplicationName = @0", item.ApplicationName))
                    {
                        throw new InfoException("应用名称【{0}】已存在", item.ApplicationName);
                    }
                    item.ApplicationId = Guid.NewGuid().ToString();
                    db.Insert(item);
                    // 新建应用时，初始化创建权限对象【权限角色】、【权限分配】【权限查询】及其对象操作
                    var object1 = new RightObject
                    {
                        ApplicationId = item.ApplicationId,
                        ObjectName = "权限分配"
                    };
                    var objectOperation1 = new RightObjectOperation
                    {
                        ObjectId = object1.Id,
                        OperationName = "查看"
                    };
                    var objectOperation2 = new RightObjectOperation
                    {
                        ObjectId = object1.Id,
                        OperationName = "保存"
                    };
                    db.Insert(object1);
                    db.Insert(objectOperation1);
                    db.Insert(objectOperation2);
                    var object2 = new RightObject
                    {
                        ApplicationId = item.ApplicationId,
                        ObjectName = "权限角色"
                    };
                    var objectOperation3 = new RightObjectOperation
                    {
                        ObjectId = object2.Id,
                        OperationName = "查看"
                    };
                    var objectOperation4 = new RightObjectOperation
                    {
                        ObjectId = object2.Id,
                        OperationName = "新增&编辑"
                    };
                    var objectOperation5 = new RightObjectOperation
                    {
                        ObjectId = object2.Id,
                        OperationName = "删除"
                    };
                    db.Insert(object2);
                    db.Insert(objectOperation3);
                    db.Insert(objectOperation4);
                    db.Insert(objectOperation5);
                    var object3 = new RightObject
                    {
                        ApplicationId = item.ApplicationId,
                        ObjectName = "权限查询"
                    };
                    var objectOperation6 = new RightObjectOperation
                    {
                        ObjectId = object2.Id,
                        OperationName = "查询"
                    };
                    db.Insert(object3);
                    db.Insert(objectOperation6);
                    logService.WriteInsertOperateLog(user, item.ApplicationId, item.Id, item);
                }
                else
                {
                    var oldItem = GetApplication(item.ApplicationId);
                    if (oldItem == null)
                    {
                        throw new InfoException("【{0}】记录不存在", item.Id);
                    }
                    item.Id = oldItem.Id;
                    db.Update(item);
                    logService.WriteUpdateOperateLog(user, item.ApplicationId, item.Id, oldItem, item);
                }
                db.CompleteTransaction();
            }
            catch (Exception e)
            {
                db.AbortTransaction();
                throw e;
            }
        }

        /// <summary>
        /// 删除应用
        /// </summary>
        /// <param name="badge"></param>
        /// <param name="applicationId"></param>
        public void DeleteApplication(KeyValuePair<string, string> user, string applicationId)
        {
            var item = GetApplication(applicationId);
            if (applicationId == ConfigurationManager.AppSettings["RightApplicationId"])
            {
                throw new InfoException("不能删除权限管理应用");
            }
            if (item == null)
            {
                throw new InfoException("【{0}】记录不存在", applicationId);
            }
           
            db.BeginTransaction();
            try
            {
                // 删除角色操作
                db.Execute(@"
                        DELETE t1 
						FROM RM_T_RightRoleOperation t1 
                        WHERE EXISTS (
                            SELECT *
                            FROM RM_T_RightRole t2
                            WHERE t2.ApplicationId = @0
                                AND t2.Id = t1.RoleId
                        )", applicationId);
                // 删除角色用户
                db.Execute(@"
                        DELETE t1
						FROM RM_T_RightRoleUser t1
                        WHERE EXISTS (
                            SELECT *
                            FROM RM_T_RightRole t2
                            WHERE t2.ApplicationId = @0
                                AND t2.Id = t1.RoleId
                        )", applicationId);
                // 删除对象操作
                db.Execute(@"
                        DELETE t1
						FROM RM_T_RightObjectOperation t1
                        WHERE EXISTS (
                            SELECT *
                            FROM RM_T_RightObject t2
                            WHERE t2.ApplicationId = @0
                                AND t2.Id = t1.ObjectId
                        )", applicationId);
                // 删除角色
                db.Execute(@"DELETE RM_T_RightRole WHERE ApplicationId = @0", applicationId);
                // 删除权限对象
                db.Execute(@"DELETE RM_T_RightObject WHERE ApplicationId = @0", applicationId);
                // 删除应用
                db.Execute(@"DELETE RM_T_RightApplication WHERE ApplicationId = @0", applicationId);
                logService.WriteDeleteOperateLog(user, applicationId, item.Id, item);
                db.CompleteTransaction();
            }
            catch (Exception e)
            {
                db.AbortTransaction();
                throw e;
            }
        }

        /// <summary>
        /// 导出应用的所有数据
        /// </summary>
        /// <param name="applicationId"></param>
        /// <returns></returns>
        public RightApplicationDataModel ExportApplicationData(string applicationId)
        {
            var appData = new RightApplicationDataModel();
            appData.Application = GetApplication(applicationId);
            if (appData.Application == null)
            {
                throw new InfoException("应用不存在");
            }
            appData.ObjectList = GetObjectList(applicationId);
            if (appData.ObjectList != null)
            {
                appData.OperationList = new List<RightObjectOperation>();
                foreach (var obj in appData.ObjectList)
                {
                    appData.OperationList.AddRange(GetObjectOperationList(obj.Id));
                }
            }
            appData.RoleList = GetRoleList(applicationId);
            if (appData.RoleList != null)
            {
                appData.RoleUserList = new List<RightRoleUser>();
                foreach (var role in appData.RoleList)
                {
                    appData.RoleUserList.AddRange(GetRoleUserList(role.Id));
                }
            }
            appData.RoleOperationList = db.Fetch<RightRoleOperation>(@"
                                        SELECT t1.Id,
											t1.RoleId,
											t1.OperationId
                                        FROM RM_T_RightRoleOperation t1
                                        JOIN RM_T_RightRole t2
                                            ON t2.Id = t1.RoleId
                                        JOIN RM_T_RightObjectOperation t3
                                            ON t3.Id = t1.OperationId
                                        JOIN RM_T_RightObject t4
                                            ON t4.Id = t3.ObjectId
                                            AND t4.ApplicationId = t2.ApplicationId
                                        WHERE t2.ApplicationId = @0", applicationId);

            return appData;
        }

        /// <summary>
        /// 导入应用数据
        /// </summary>
        public void ImportApplicationData(KeyValuePair<string, string> user, RightApplicationDataModel appData)
        {
           
            db.BeginTransaction();
            try
            {
                if (db.Exists<RightApplication>("ApplicationId = @0 OR ApplicationName = @1", appData.Application.ApplicationId, appData.Application.ApplicationName))
                {
                    throw new InfoException("应用已存在");
                }
                appData.Application.Id = 0;
                db.Insert(appData.Application);
                logService.WriteInsertOperateLog(user, appData.Application.ApplicationId, appData.Application.Id, appData.Application);

                // 新增对象和对象操作
                if (appData.ObjectList != null)
                {
                    foreach (var obj in appData.ObjectList)
                    {
                        var oldObjId = obj.Id;
                        obj.Id = 0;
                        db.Insert(obj);
                        logService.WriteInsertOperateLog(user, appData.Application.ApplicationId, obj.Id, obj);
                        if (appData.OperationList != null)
                        {
                            var operationList = appData.OperationList.FindAll(a => a.ObjectId == oldObjId);
                            foreach (var operation in operationList)
                            {
                                var oldOperationId = operation.Id;
                                operation.Id = 0;
                                operation.ObjectId = obj.Id;
                                db.Insert(operation);
                                logService.WriteInsertOperateLog(user, appData.Application.ApplicationId, operation.Id, operation);
                                if (appData.RoleOperationList != null)
                                {
                                    var roleOperationList = appData.RoleOperationList.FindAll(a => a.OperationId == oldOperationId);
                                    foreach (var roleOperation in roleOperationList)
                                    {
                                        roleOperation.OperationId = operation.Id;
                                    }
                                }
                            }
                        }
                    }
                }

                // 新增角色和角色用户
                if (appData.RoleList != null)
                {
                    foreach (var role in appData.RoleList)
                    {
                        var oldRoleId = role.Id;
                        role.Id = 0;
                        db.Insert(role);
                        logService.WriteInsertOperateLog(user, appData.Application.ApplicationId, role.Id, role);
                        if (appData.RoleUserList != null)
                        {
                            var userList = appData.RoleUserList.FindAll(a => a.RoleId == oldRoleId);
                            foreach (var rolUser in userList)
                            {
                                rolUser.Id = 0;
                                rolUser.RoleId = role.Id;
                                db.Insert(rolUser);
                                logService.WriteInsertOperateLog(user, appData.Application.ApplicationId, rolUser.Id, rolUser);
                            }
                        }
                        if (appData.RoleOperationList != null)
                        {
                            var roleOperationList = appData.RoleOperationList.FindAll(a => a.RoleId == oldRoleId);
                            foreach (var roleOperation in roleOperationList)
                            {
                                roleOperation.Id = 0;
                                roleOperation.RoleId = role.Id;
                                db.Insert(roleOperation);
                                logService.WriteInsertOperateLog(user, appData.Application.ApplicationId, roleOperation.Id, roleOperation);
                            }
                        }
                    }
                }

                db.CompleteTransaction();
            }
            catch (Exception e)
            {
                db.AbortTransaction();
                throw e;
            }
        }

        /// <summary>
        /// 获取权限对象列表
        /// </summary>
        /// <param name="applicationId"></param>
        /// <returns></returns>
        public List<RightObject> GetObjectList(string applicationId)
        {
            var list = db.Fetch<RightObject>("WHERE ApplicationId = @0 ORDER BY SortId DESC", applicationId);
            return list;
        }

        /// <summary>
        /// 保存权限对象
        /// </summary>
        /// <param name="badge"></param>
        /// <param name="applicationId"></param>
        /// <param name="item"></param>
        public void SaveObject(KeyValuePair<string, string> user, string applicationId, RightObject item)
        {
            if (applicationId == ConfigurationManager.AppSettings["RightApplicationId"])
            {
                throw new InfoException("不能新增/编辑权限管理应用的权限对象");
            }
           
            db.BeginTransaction();
            try
            {
                if (db.Exists<RightObject>("ApplicationId = @0 AND ObjectName = @1 AND Id != @2", applicationId, item.ObjectName, item.Id))
                {
                    throw new InfoException("权限对象【{0}】已存在", item.ObjectName);
                }
                if (db.IsNew(item))
                {
                    db.Insert(item);
                    logService.WriteInsertOperateLog(user, applicationId, item.Id, item);
                }
                else
                {
                    var oldItem = db.SingleOrDefault<RightObject>(item.Id);
                    if (oldItem == null)
                    {
                        throw new InfoException("【{0}】记录不存在", item.Id);
                    }
                    if (oldItem.ObjectName == "权限分配" || oldItem.ObjectName == "权限角色" || oldItem.ObjectName == "权限查询")
                    {
                        throw new InfoException("不能编辑保留权限对象【{0}】", oldItem.ObjectName);
                    }
                    db.Update(item);
                    logService.WriteUpdateOperateLog(user, applicationId, item.Id, oldItem, item);
                }
                db.CompleteTransaction();
            }
            catch (Exception e)
            {
                db.AbortTransaction();
                throw e;
            }
        }

        /// <summary>
        /// 删除权限对象
        /// </summary>
        /// <param name="badge"></param>
        /// <param name="id"></param>
        public void DeleteObject(KeyValuePair<string, string> user, string applicationId, int id)
        {
            if (applicationId == ConfigurationManager.AppSettings["RightApplicationId"])
            {
                throw new InfoException("不能删除权限管理应用的权限对象");
            }
           
            db.BeginTransaction();
            try
            {
                var item = db.SingleOrDefault<RightObject>(id);
                if (item == null)
                {
                    throw new InfoException("【{0}】记录不存在", id);
                }
                if (item.ObjectName == "权限分配" || item.ObjectName == "权限角色" || item.ObjectName == "权限查询")
                {
                    throw new InfoException("不能删除保留权限对象【{0}】", item.ObjectName);
                }
                // 删除角色操作
                db.Execute(@"
                        DELETE t1
						FROM RM_T_RightRoleOperation t1
                        WHERE EXISTS (
                            SELECT *
                            FROM RM_T_RightObjectOperation t2
                            WHERE t2.ObjectId = @0
                                AND t2.Id = t1.OperationId
                        )", id);
                // 删除对象操作
                db.Execute(@"DELETE FROM RM_T_RightObjectOperation WHERE ObjectId = @0", id);
                // 删除对象
                db.Execute(@"DELETE FROM RM_T_RightObject WHERE Id = @0", id);
                logService.WriteDeleteOperateLog(user, item.ApplicationId, item.Id, item);
                db.CompleteTransaction();
            }
            catch (Exception e)
            {
                db.AbortTransaction();
                throw e;
            }
        }

        /// <summary>
        /// 获取对象操作列表
        /// </summary>
        /// <param name="objectId"></param>
        /// <returns></returns>
        public List<RightObjectOperation> GetObjectOperationList(int objectId)
        {
            var list = db.Fetch<RightObjectOperation>("WHERE ObjectId = @0", objectId);
            return list;
        }

        /// <summary>
        /// 保存对象操作
        /// </summary>
        /// <param name="badge"></param>
        /// <param name="applicationId"></param>
        /// <param name="item"></param>
        public void SaveObjectOperation(KeyValuePair<string, string> user, string applicationId, RightObjectOperation item)
        {
            if (applicationId == ConfigurationManager.AppSettings["RightApplicationId"])
            {
                throw new InfoException("不能新增/编辑权限管理应用的权限对象操作");
            }
            
            db.BeginTransaction();
            try
            {
                if (db.Exists<RightObjectOperation>("ObjectId = @0 AND OperationName = @1 and Id != @2", item.ObjectId, item.OperationName, item.Id))
                {
                    throw new InfoException("对象操作【{0}】已存在", item.OperationName);
                }
                if (db.IsNew(item))
                {
                    db.Insert(item);
                    logService.WriteInsertOperateLog(user, applicationId, item.Id, item);
                }
                else
                {
                    var oldItem = db.SingleOrDefault<RightObjectOperation>(item.Id);
                    if (oldItem == null)
                    {
                        throw new InfoException("【{0}】记录不存在", item.Id);
                    }
                    var objectItem = db.SingleOrDefault<RightObject>(item.ObjectId);
                    if (objectItem.ObjectName == "权限分配" || objectItem.ObjectName == "权限角色" || objectItem.ObjectName == "权限查询")
                    {
                        throw new InfoException("不能编辑保留权限对象【{0}】的操作", objectItem.ObjectName);
                    }
                    db.Update(item);
                    logService.WriteUpdateOperateLog(user, applicationId, item.Id, oldItem, item);
                }
                db.CompleteTransaction();
            }
            catch (Exception e)
            {
                db.AbortTransaction();
                throw e;
            }
        }

        /// <summary>
        /// 删除对象操作
        /// </summary>
        /// <param name="badge"></param>
        /// <param name="applicationId"></param>
        /// <param name="id"></param>
        public void DeleteObjectOperation(KeyValuePair<string, string> user, string applicationId, int id)
        {
            if (applicationId == ConfigurationManager.AppSettings["RightApplicationId"])
            {
                throw new InfoException("不能删除权限管理应用的权限对象操作");
            }
           
            db.BeginTransaction();
            try
            {
                var item = db.SingleOrDefault<RightObjectOperation>(id);
                if (item == null)
                {
                    throw new InfoException("【{0}】记录不存在", id);
                }
                var objectItem = db.SingleOrDefault<RightObject>(item.ObjectId);
                if (objectItem.ObjectName == "权限分配" || objectItem.ObjectName == "权限角色" || objectItem.ObjectName == "权限查询")
                {
                    throw new InfoException("不能删除保留权限对象【{0}】的操作", objectItem.ObjectName);
                }
                db.Delete<RightRoleOperation>("WHERE OperationId = @0", id);
                db.Delete(item);
                logService.WriteDeleteOperateLog(user, applicationId, item.Id, item);
                db.CompleteTransaction();
            }
            catch (Exception e)
            {
                db.AbortTransaction();
                throw e;
            }
        }

        /// <summary>
        /// 获取权限角色列表
        /// </summary>
        /// <param name="applicationId"></param>
        /// <returns></returns>
        public List<RightRole> GetRoleList(string applicationId)
        {
            var list = db.Fetch<RightRole>("WHERE ApplicationId = @0", applicationId);
            return list;
        }

        /// <summary>
        /// 保存权限角色
        /// </summary>
        /// <param name="badge"></param>
        /// <param name="item"></param>
        public void SaveRole(KeyValuePair<string, string> user, string applicationId, RightRole item)
        {
            if (applicationId == ConfigurationManager.AppSettings["RightApplicationId"])
            {
                throw new InfoException("不能新增/编辑权限管理应用的权限角色");
            }
            if (!string.IsNullOrEmpty(item.OuterUrl))
            {
                try
                {
                    Util.HttpGet<List<string>>(item.OuterUrl);
                }
                catch
                {
                    throw new InfoException("外部接口无效");
                }
            }
            
            db.BeginTransaction();
            try
            {
                if (db.Exists<RightRole>("ApplicationId = @0 AND RoleName = @1 and Id != @2", applicationId, item.RoleName, item.Id))
                {
                    throw new InfoException("角色名称【{0}】已存在", item.RoleName);
                }
                if (db.IsNew(item))
                {
                    db.Insert(item);
                    logService.WriteInsertOperateLog(user, applicationId, item.Id, item);
                }
                else
                {
                    var oldItem = db.SingleOrDefault<RightRole>(item.Id);
                    if (oldItem == null)
                    {
                        throw new InfoException("【{0}】记录不存在", item.Id);
                    }
                    db.Update(item);
                    logService.WriteUpdateOperateLog(user, applicationId, item.Id, oldItem, item);
                }
                db.CompleteTransaction();
            }
            catch (Exception e)
            {
                db.AbortTransaction();
                throw e;
            }
        }

        /// <summary>
        /// 删除权限角色
        /// </summary>
        /// <param name="badge"></param>
        /// <param name="applicationId"></param>
        /// <param name="id"></param>
        public void DeleteRole(KeyValuePair<string, string> user, string applicationId, int id)
        {
            if (applicationId == ConfigurationManager.AppSettings["RightApplicationId"])
            {
                throw new InfoException("不能删除权限管理应用的权限角色");
            }
           
            db.BeginTransaction();
            try
            {
                var item = db.SingleOrDefault<RightRole>(id);
                if (item == null)
                {
                    throw new InfoException("【{0}】记录不存在", id);
                }
                db.Delete<RightRoleOperation>("WHERE RoleId = @0", id);
                db.Delete<RightRoleUser>("WHERE RoleId = @0", id);
                db.Delete(item);
                logService.WriteDeleteOperateLog(user, applicationId, id, item);
                db.CompleteTransaction();
            }
            catch (Exception e)
            {
                db.AbortTransaction();
                throw e;
            }
        }

        /// <summary>
        /// 获取数据
        /// </summary>
        /// <param name="outerUrl"></param>
        /// <returns></returns>
        public T GetOuterData<T>(string outerUrl)
        {
            T result = default(T);
            try
            {
                result = Util.HttpGet<T>(outerUrl);
            }
            catch (Exception e)
            {
                var logger = LogManager.GetLogger("LogError");
                logger.Error(e);
            }
            return result;
        }

        /// <summary>
        /// 获取角色用户列表
        /// </summary>
        /// <returns></returns>
        public List<RightRoleUser> GetRoleUserList(int roleId)
        {
            var item = db.SingleOrDefault<RightRole>(roleId);
            if (item == null)
            {
                throw new InfoException("数据错误");
            }
            var list = db.Fetch<RightRoleUser>("WHERE RoleId = @0", roleId);

            if (!string.IsNullOrEmpty(item.OuterUrl))
            {
                var badges = GetOuterData<List<string>>(item.OuterUrl);
                if (badges != null)
                {
                    var outerList = GetRoleUserByBadges(badges);
                    list.AddRange(outerList.Select(a =>
                    {
                        a.RoleId = roleId;
                        a.IsOuter = true;
                        return a;
                    }));
                }
            }

            return list.OrderBy(a => a.Badge).ToList();
        }

        /// <summary>
        /// 获取某个角色的用户列表
        /// </summary>
        /// <param name="roleName"></param>
        /// <returns></returns>
        public List<RightRoleUser> GetRoleUserList(string applicationId, string roleName)
        {
            var item = db.SingleOrDefault<RightRole>("WHERE ApplicationId = @0 AND RoleName = @1", applicationId, roleName);
            if (item == null)
            {
                throw new InfoException("角色【{0}】不存在", roleName);
            }
            var list = db.Fetch<RightRoleUser>("WHERE RoleId = @0", item.Id);

            if (!string.IsNullOrEmpty(item.OuterUrl))
            {
                var badges = GetOuterData<List<string>>(item.OuterUrl);
                if (badges != null)
                {
                    var outerList = GetRoleUserByBadges(badges);
                    list.AddRange(outerList.Select(a =>
                    {
                        a.RoleId = item.Id;
                        a.IsOuter = true;
                        return a;
                    }));
                }
            }

            return list.OrderBy(a => a.Badge).ToList();
        }

        /// <summary>
        /// 根据工号获取用户
        /// </summary>
        /// <param name="badges"></param>
        /// <returns></returns>
        public List<RightRoleUser> GetRoleUserByBadges(List<string> badges)
        {
            var sql = string.Format(@"
                        SELECT badge,
	                        xingming [Name],
	                        t3.mingcheng Department
                        FROM yuangong t1
                        JOIN yg_bm t2
	                        ON t2.ygid = t1.ygid
                        JOIN bm t3
	                        ON t3.id = t2.bmid
                        WHERE t1.badge IN ('{0}')", string.Join("','", badges));
            return db.Fetch<RightRoleUser>(sql);
        }

        /// <summary>
        /// 增加角色用户
        /// </summary>
        /// <param name="badge"></param>
        /// <param name="applicationId"></param>
        /// <param name="item"></param>
        public void AddRoleUser(KeyValuePair<string, string> user, string applicationId, List<RightRoleUser> list)
        {
            
            db.BeginTransaction();
            try
            {
                foreach (var item in list)
                {
                    // 角色下已存在该用户则跳过
                    if (db.Exists<RightRoleUser>("RoleId = @0 AND Badge = @1", item.RoleId, item.Badge))
                    {
                        continue;
                    }
                    db.Insert(item);
                    logService.WriteInsertOperateLog(user, applicationId, item.Id, item);
                }
                db.CompleteTransaction();
            }
            catch (Exception e)
            {
                db.AbortTransaction();
                throw e;
            }
        }

        /// <summary>
        /// 删除角色用户
        /// </summary>
        /// <param name="badge"></param>
        /// <param name="applicationId"></param>
        /// <param name="id"></param>
        public void DeleteRoleUser(KeyValuePair<string, string> user, string applicationId, int id)
        {
            
            db.BeginTransaction();
            try
            {
                var item = db.SingleOrDefault<RightRoleUser>(id);
                if (item == null)
                {
                    throw new InfoException("【{0}】记录不存在", id);
                }
                db.Delete(item);
                logService.WriteDeleteOperateLog(user, applicationId, id, item);
                db.CompleteTransaction();
            }
            catch (Exception e)
            {
                db.AbortTransaction();
                throw e;
            }
        }

        /// <summary>
        /// 获取角色操作列表
        /// 以权限对象和操作为维度，标识用户是否有权限
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        public List<RightRoleOperationModel> GetRoleOperationList(string applicationId, int roleId)
        {
            var roleOperationList = db.Fetch<RightRoleOperationModel>(@"
                                        SELECT 
	                                        t3.Id,
	                                        t1.Id ObjectId,
	                                        t1.ObjectName,
	                                        t2.Id OperationId,
                                            t2.OperationName,
	                                        (
		                                        CASE
			                                        WHEN t3.Id IS NULL
			                                        THEN 0
			                                        ELSE 1
		                                        END
	                                        ) IsHaveRight
                                        FROM RM_T_RightObject t1
                                        JOIN RM_T_RightObjectOperation t2
	                                        ON t2.ObjectId = t1.Id
                                        LEFT JOIN RM_T_RightRoleOperation t3
	                                        ON t3.OperationId = t2.Id
	                                        AND t3.RoleId = @1
                                        WHERE t1.ApplicationId = @0
                                        ORDER BY t1.SortId DESC, t2.Id", applicationId, roleId);
            return roleOperationList;
        }

        /// <summary>
        /// 保存角色操作
        /// </summary>
        /// <param name="badge"></param>
        /// <param name="roleId"></param>
        /// <param name="list"></param>
        public void SaveRoleOperation(KeyValuePair<string, string> user, string applicationId, int roleId, List<RightRoleOperation> list)
        {
           
            db.BeginTransaction();
            try
            {
                var oldList = db.Fetch<RightRoleOperation>("WHERE RoleId = @0", roleId);
                // 删除的
                foreach (var item in oldList)
                {
                    if (list.Exists(a => a.OperationId == item.OperationId))
                    {
                        continue;
                    }
                    db.Delete(item);
                    logService.WriteDeleteOperateLog(user, applicationId, item.Id, item);
                }
                // 增加的
                foreach (var item in list)
                {
                    if (oldList.Exists(a => a.OperationId == item.OperationId))
                    {
                        continue;
                    }
                    item.RoleId = roleId;
                    db.Insert(item);
                    logService.WriteInsertOperateLog(user, applicationId, item.Id, item);
                }
                db.CompleteTransaction();
            }
            catch (Exception e)
            {
                db.AbortTransaction();
                throw e;
            }
        }

        /// <summary>
        /// 获取具有某个操作权限的用户列表
        /// </summary>
        /// <param name="objectName"></param>
        /// <param name="operationName"></param>
        /// <returns></returns>
        public List<RightRoleUser> GetRightUserList(string applicationId, string objectName, string operationName)
        {
            var sql = new Sql(@"
                        SELECT t1.RoleId
                        FROM RM_T_RightRoleOperation t1
                        JOIN RM_T_RightObjectOperation t2
                            ON t2.Id = t1.OperationId
                        JOIN RM_T_RightObject t3
                            ON t3.Id = t2.ObjectId
                        WHERE t3.ApplicationId = @0
                            AND t3.ObjectName = @1
							AND t2.OperationName = @2", applicationId, objectName, operationName);
            var roleIds = db.Fetch<int>(sql);
            var result = new List<RightRoleUser>();

            foreach (var roleId in roleIds)
            {
                result.AddRange(GetRoleUserList(roleId));
            }

            return result;
        }

        /// <summary>
        /// 按用户查询权限列表
        /// 由于用户可能存在角色的外部接口返回的结果中，无法从数据库中直接查询，因此需要拼接数据
        /// </summary>
        /// <param name="applicationId"></param>
        /// <param name="badge"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="total"></param>
        /// <returns></returns>
        public List<UserRightModel> GetUserRightList(string applicationId, string badge)
        {
            var result = new List<UserRightModel>();
            var roles = db.Fetch<RightRole>("WHERE ApplicationId = @0", applicationId);
            var inRole = false; // 标识用户是否属于该角色
            var isOuter = false; // 标识用户是否为外部接口返回的且数据该角色

            foreach (var role in roles)
            {
                inRole = false;
                isOuter = false;
                if (!string.IsNullOrEmpty(role.OuterUrl))
                {
                    var badges = GetOuterData<List<string>>(role.OuterUrl);
                    isOuter = badges != null && badges.Contains(badge);
                }
                if (!isOuter)
                {
                    inRole = db.Exists<RightRoleUser>("RoleId = @0 AND Badge = @1", role.Id, badge);
                }
                // 如果用户属于该角色，则查找该角色具有的权限
                if (isOuter || inRole)
                {
                    var temp = GetRoleOperationListByRole(applicationId, role.Id);
                    foreach (var item in temp)
                    {
                        item.RoleId = role.Id;
                        item.RoleName = role.RoleName;
                        item.RoleDesc = role.RoleDesc;
                        item.IsOuter = isOuter;
                        result.Add(item);
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// 获取角色具有的操作列表
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        public List<UserRightModel> GetRoleOperationListByRole(string applicationId, int roleId)
        {
            var roleOperationList = db.Fetch<UserRightModel>(@"
                                        SELECT 
	                                        t2.Id OperationId,
                                            t2.OperationName,
	                                        t2.OperationDesc,
	                                        t3.Id ObjectId,
	                                        t3.ObjectName
                                        FROM RM_T_RightRoleOperation t1
                                        JOIN RM_T_RightObjectOperation t2
	                                        ON t2.Id = t1.OperationId
                                        JOIN RM_T_RightObject t3
	                                        ON t3.Id = t2.ObjectId
	                                        AND t3.ApplicationId = @0
                                        WHERE t1.RoleId = @1", applicationId, roleId);
            return roleOperationList;
        }

        /// <summary>
        /// 按操作查询权限
        /// </summary>
        /// <param name="applicationId"></param>
        /// <param name="operationId"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="total"></param>
        /// <returns></returns>
        public List<OperationRightModel> GetOperationRightList(string applicationId, int operationId, int? pageIndex, int? pageSize, out int total)
        {
            var result = new List<OperationRightModel>();
            var roleOperations = db.Fetch<RightRoleOperation>("WHERE OperationId = @0", operationId);

            foreach (var roleOperation in roleOperations)
            {
                var role = db.Single<RightRole>("WHERE Id = @0", roleOperation.RoleId);
                var users = GetRoleUserList(role.Id);
                result.AddRange(users.Select(a =>
                {
                    return new OperationRightModel
                    {
                        RoleId = a.RoleId,
                        RoleName = role.RoleName,
                        RoleDesc = role.RoleDesc,
                        Badge = a.Badge,
                        Name = a.Name,
                        Department = a.Department,
                        IsOuter = a.IsOuter
                    };
                }));
            }
            total = result.Count;
            if (pageIndex.HasValue && pageSize.HasValue)
            {
                result = result.Skip((pageIndex.Value - 1) * pageSize.Value).Take(pageSize.Value).ToList();
            }

            return result;
        }

        /// <summary>
        /// 获取操作的信息，包括权限对象名称
        /// </summary>
        /// <param name="operationId"></param>
        /// <returns></returns>
        public RightRoleOperationModel GetOperationInfo(int operationId)
        {
            return db.SingleOrDefault<RightRoleOperationModel>(@"
                    SELECT 
	                    t1.Id OperationId,
                        t1.OperationName,
	                    t1.OperationDesc,
	                    t2.Id ObjectId,
	                    t2.ObjectName
                    FROM RM_T_RightObjectOperation t1
                    JOIN RM_T_RightObject t2
	                    ON t2.Id = t1.ObjectId
                    WHERE t1.Id = @0", operationId);
        }

        /// <summary>
        /// 用户是否有某个操作权限
        /// </summary>
        /// <param name="badge"></param>
        /// <param name="objectName"></param>
        /// <param name="operationName"></param>
        /// <returns></returns>
        public bool IsHaveRight(string applicationId, string badge, string objectName, string operationName)
        {
            var userRighs = GetUserRightList(applicationId, badge);
            return userRighs.Exists(a => a.ObjectName == objectName && operationName == a.OperationName);
        }

        /// <summary>
        /// 获取用户所在的角色列表
        /// 由于用户可能存在角色的外部接口返回的结果中，无法从数据库中直接查询，因此需要拼接数据
        /// </summary>
        /// <param name="applicationId"></param>
        /// <param name="badge"></param>
        /// <returns></returns>
        public List<RightRole> GetUserRoleList(string applicationId, string badge)
        {
            var result = new List<RightRole>();
            var roles = db.Fetch<RightRole>("WHERE ApplicationId = @0", applicationId);

            foreach (var role in roles)
            {
                if (db.Exists<RightRoleUser>("RoleId = @0 AND Badge = @1", role.Id, badge))
                {
                    result.Add(role);
                }
                else if (!string.IsNullOrEmpty(role.OuterUrl))
                {
                    var badges = GetOuterData<List<string>>(role.OuterUrl);
                    if (badges != null && badges.Contains(badge))
                    {
                        result.Add(role);
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// 获取所有权限列表，并标识用户是否具有该权限
        /// </summary>
        /// <param name="badge"></param>
        /// <param name=""></param>
        /// <returns></returns>
        public List<RightRoleOperationModel> GetAllRightList(string applicationId, string badge)
        {
            var roles = GetUserRoleList(applicationId, badge);
            var sql = string.Format(@"
                        SELECT DISTINCT 
							t1.Id ObjectId,
							t1.ObjectName,
							t2.Id OperationId,
                            t2.OperationName,
							(
								CASE
									WHEN t4.OperationId IS NULL
									THEN 0
									ELSE 1
								END
							) IsHaveRight
                        FROM RM_T_RightObject t1
                        JOIN RM_T_RightObjectOperation t2
                            ON t2.ObjectId = t1.Id
						LEFT JOIN (
							SELECT DISTINCT t3.OperationId
							FROM RM_T_RightRoleOperation t3
							WHERE {1}
						) t4
							ON t4.OperationId = t2.Id
                        WHERE t1.ApplicationId = '{0}'",
                        applicationId,
                        roles.Count > 0
                            ? "t3.RoleId IN (" + string.Join(",", roles.Select(a => a.Id)) + ")"
                            : "1 = 0");
            return db.Fetch<RightRoleOperationModel>(sql);
        }

        public void FixData(string applicationId)
        {
            db.BeginTransaction();
            try
            {
                var qxfpObjectId = db.Single<int>("SELECT Id FROM RM_T_RightObject WHERE ApplicationId = @0 AND ObjectName = '权限角色'", applicationId);
                var qxjsObjectId = db.Single<int>("SELECT Id FROM RM_T_RightObject WHERE ApplicationId = @0 AND ObjectName = '角色操作'", applicationId);
                db.Execute("UPDATE RM_T_RightObject SET ObjectName = '权限分配', SortId = 3 WHERE Id = @0", qxfpObjectId);
                db.Execute("UPDATE RM_T_RightObject SET ObjectName = '权限角色', SortId = 2 WHERE Id = @0", qxjsObjectId);
                db.Execute("INSERT INTO RM_T_RightObject (ApplicationId, ObjectName, SortId) VALUES (@0, @1, 1)", applicationId, "权限查询");
                var qxcxObjectId = db.Single<int>("SELECT Id FROM RM_T_RightObject WHERE ApplicationId = @0 AND ObjectName = '权限查询'", applicationId);

                db.Execute("UPDATE RM_T_RightObjectOperation SET OperationName = '查询' WHERE ObjectId = @0 AND OperationName = '管理'", qxfpObjectId);
                db.Execute("UPDATE RM_T_RightObjectOperation SET OperationName = '查询' WHERE ObjectId = @0 AND OperationName = '管理'", qxjsObjectId);
                db.Execute("INSERT INTO RM_T_RightObjectOperation (ObjectId, OperationName) VALUES (@0, @1)", qxfpObjectId, "保存");
                db.Execute("INSERT INTO RM_T_RightObjectOperation (ObjectId, OperationName) VALUES (@0, @1)", qxjsObjectId, "新增&编辑");
                db.Execute("INSERT INTO RM_T_RightObjectOperation (ObjectId, OperationName) VALUES (@0, @1)", qxjsObjectId, "删除");
                db.Execute("INSERT INTO RM_T_RightObjectOperation (ObjectId, OperationName) VALUES (@0, @1)", qxcxObjectId, "查询");
                db.Execute("INSERT INTO RM_T_RightObjectOperation (ObjectId, OperationName) VALUES (@0, @1)", qxcxObjectId, "导出");

                db.CompleteTransaction();

                if (false)
                {
                    db.AbortTransaction();
                }
            }
            catch (Exception e)
            {
                db.AbortTransaction();
                throw e;
            }
        }
    }
}
