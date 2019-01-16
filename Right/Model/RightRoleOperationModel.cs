using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Right.Model
{
    public class RightRoleOperationModel
    {
        /// <summary>
        /// 对象操作ID
        /// </summary>
        public int OperationId { get; set; }
        /// <summary>
        /// 对象操作
        /// </summary>
        public string OperationName { get; set; }
        /// <summary>
        /// 权限对象ID
        /// </summary>
        public int ObjectId { get; set; }
        /// <summary>
        /// 权限对象
        /// </summary>
        public string ObjectName { get; set; }
        /// <summary>
        /// 是否有权限
        /// </summary>
        public bool IsHaveRight { get; set; }
    }
}
