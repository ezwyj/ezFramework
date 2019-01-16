using PetaPoco;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Right.Entity
{
    [DisplayName("权限对象")]
    [TableName("RM_T_RightObject")]
    [PrimaryKey("Id", autoIncrement = true)]
    public class RightObject
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
        /// 权限对象
        /// </summary>
        [DisplayName("权限对象")]
        public string ObjectName { get; set; }
        /// <summary>
        /// 排序ID
        /// </summary>        
        public int SortId { get; set; }
    }
}
