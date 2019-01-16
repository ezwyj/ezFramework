using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Right.Model
{
    public class UserRightModel
    {
        /// <summary>
        /// 角色ID
        /// </summary>
        public int RoleId { get; set; }
        /// <summary>
        /// 角色名称
        /// </summary>
        public string RoleName { get; set; }
        /// <summary>
        /// 角色描述
        /// </summary>
        public string RoleDesc { get; set; }
        /// <summary>
        /// 是否为外部接口返回的
        /// </summary>
        public bool IsOuter { get; set; }
        /// <summary>
        /// 操作ID
        /// </summary>
        public int OperationId { get; set; }
        /// <summary>
        /// 操作名称
        /// </summary>
        public string OperationName { get; set; }
        /// <summary>
        /// 操作描述
        /// </summary>
        public string OperationDesc { get; set; }
        /// <summary>
        /// 权限对象ID
        /// </summary>
        public int ObjectId { get; set; }
        /// <summary>
        /// 权限对象
        /// </summary>
        public string ObjectName { get; set; }
    }
}
