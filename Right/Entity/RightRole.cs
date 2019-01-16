using PetaPoco;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Right.Entity
{
    [DisplayName("权限角色")]
    [TableName("RM_T_RightRole")]
    [PrimaryKey("Id", autoIncrement = true)]
    public class RightRole
    {
        /// <summary>
        /// Id
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// 应用ID
        /// </summary>
        [DisplayName("应用ID")]
        public string ApplicationId { get; set; }
        /// <summary>
        /// 角色名称
        /// </summary>
        [DisplayName("角色名称")]
        public string RoleName { get; set; }
        /// <summary>
        /// 角色描述
        /// </summary>
        [DisplayName("角色描述")]
        public string RoleDesc { get; set; }
        /// <summary>
        /// 外部获取用户的Url
        /// 调用之后返回工号列表的JSON
        /// </summary>
        [DisplayName("外部Url")]
        public string OuterUrl { get; set; }
    }
}
