using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Organization.Entity
{
    /// <summary>
    /// 岗位
    /// </summary>
    public class Job
    {
        public int Id { get; set; }

        public string JobName { get; set; }

        public DateTime? CreateTime { get; set; }
    }
}
