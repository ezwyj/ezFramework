using PetaPoco;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Log.Entity
{

    /// <summary>
    /// 角色操作
    /// </summary>
    [TableName("RM_T_OperateLog")]
    [PrimaryKey("Id", autoIncrement =  true)]
    public class RightOperateLog
    {
        /// <summary>
        /// Id
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// 应用Id
        /// </summary>
        public string ApplicationId { get; set; }
        /// <summary>
        /// 模块
        /// </summary>
        public string Module { get; set; }
        /// <summary>
        /// 对象Id
        /// </summary>
        public string ObjId { get; set; }
        /// <summary>
        /// 组Id
        /// </summary>
        public string GroupId { get; set; }
        /// <summary>
        /// 操作类型
        /// </summary>
        public OperateTypeEnum OperateType { get; set; }
        /// <summary>
        /// 操作类型名称
        /// </summary>
        [Ignore]
        public string OperateTypeName
        {
            get
            {
                return OperateType.ToString();
            }
        }
        /// <summary>
        /// 操作人工号
        /// </summary>
        public string OperateBy { get; set; }
        /// <summary>
        /// 操作人姓名
        /// </summary>
        public string OperateByName { get; set; }
        /// <summary>
        /// 操作时间
        /// </summary>
        public DateTime OperateOn { get; set; }
        /// <summary>
        /// 字段名
        /// </summary>
        public string FieldName { get; set; }
        /// <summary>
        /// 字段显示名
        /// </summary>
        public string FieldDisplayName { get; set; }
        /// <summary>
        /// 旧值
        /// </summary>
        public string OldValue { get; set; }
        /// <summary>
        /// 新值
        /// </summary>
        public string NewValue { get; set; }
    }

    /// <summary>
    /// 日志操作类型
    /// </summary>
    public enum OperateTypeEnum
    {
        增加 = 1,
        删除 = 2,
        修改 = 3,
        查询 = 4
    }
}
