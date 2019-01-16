using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Slickflow.Module.Resource
{
    /// <summary>
    /// 角色对象
    /// </summary>
    [Table("SysRole")]
    public class RoleEntity
    {
        public int ID { get; set; }
        public string RoleCode { get; set; }
        public string RoleName { get; set; }
        /// <summary>
        /// 权限角色ID
        /// </summary>
        public int RightRoleId { get; set; }
    }
}
