using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Account.Entity
{
    public class Employee 
    {
        
        public int Id { get; set; }
       
        
       
        /// <summary>
        /// 姓名
        /// </summary>
        public string Name { get; set; }

        public string Password { get; set; }
        /// <summary>
        /// 邮箱
        /// </summary>
        
        public string Email
        {
            get; set;
        }

        public string Mobile { get; set; }

        public string Remark { get; set; }

        public DateTime? LastLoginDate { get; set; }
        

    }
}
