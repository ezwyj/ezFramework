using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Account.Model.Organization
{
    public class Employee
    {

        /// <summary>
        /// 工号
        /// </summary>
        public string Id
        {
            get; set;
        }
        /// <summary>
        /// 姓名
        /// </summary>

        public string Name { get; set; }
        /// <summary>
        /// 邮箱
        /// </summary>

        public string Email { get; set; }

        public Department Department { get; set; }

        public string JobId { get; set; }
        public string Job { get; set; }

    }
}
