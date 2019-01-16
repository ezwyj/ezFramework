using PetaPoco;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Right.Entity
{
    [DisplayName("对象操作")]
    [TableName("RM_T_RightObjectOperation")]
    [PrimaryKey("Id", autoIncrement = true)]
    public class RightObjectOperation
    {
        /// <summary>
        /// ID
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// 对象ID
        /// </summary>
        [DisplayName("对象ID")]
        public int ObjectId { get; set; }
        /// <summary>
        /// 权限操作
        /// </summary>
        [DisplayName("权限操作")]
        public string OperationName { get; set; }
        /// <summary>
        /// 权限操作说明
        /// </summary>
        [DisplayName("权限操作说明")]
        public string OperationDesc { get; set; }
    }
}
