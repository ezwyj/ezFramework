using PetaPoco;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Right.Entity
{
    [DisplayName("权限应用")]
    [TableName("RM_T_RightApplication")]
    [PrimaryKey("Id", autoIncrement = true)]
    public class RightApplication
    {
        /// <summary>
        /// Id
        /// 自增，主要用于排序
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// 应用ID
        /// GUID的形式
        /// </summary>
        public string ApplicationId { get; set; }
        /// <summary>
        /// 应用名称
        /// </summary>
        [DisplayName("应用名称")]
        public string ApplicationName { get; set; }
    }
}
