using PetaPoco;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Right.Entity
{
    [DisplayName("角色操作")]
    [TableName("RM_T_RightRoleOperation")]
    [PrimaryKey("Id", autoIncrement = true)]
    public class RightRoleOperation
    {
        /// <summary>
        /// ID
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// 权限角色ID
        /// </summary>
        [DisplayName("权限角色ID")]
        public int RoleId { get; set; }
        /// <summary>
        /// 对象操作ID
        /// </summary>
        [DisplayName("对象操作ID")]
        public int OperationId { get; set; }
    }
}
