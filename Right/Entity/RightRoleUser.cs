using PetaPoco;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Right.Entity
{
    [DisplayName("角色用户")]
    [TableName("RM_T_RightRoleUser")]
    [PrimaryKey("Id", autoIncrement = true)]
    public class RightRoleUser
    {
        /// <summary>
        /// Id
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// 角色ID
        /// </summary>
        [DisplayName("角色ID")]
        public int RoleId { get; set; }
        /// <summary>
        /// 工号
        /// </summary>
        [DisplayName("工号")]
        public string Badge { get; set; }
        /// <summary>
        /// 姓名
        /// </summary>
        [DisplayName("姓名")]
        public string Name { get; set; }
        /// <summary>
        /// 部门
        /// </summary>
        [DisplayName("部门")]
        public string Department { get; set; }
        /// <summary>
        /// 是否为外部url返回的用户
        /// </summary>
        [Ignore]
        public bool IsOuter { get; set; }
    }
}
