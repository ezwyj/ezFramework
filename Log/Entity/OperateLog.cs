using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Log.Entity
{
    public class OperateLog
    {
        public int Id { get; set; }

        public DateTime CreateTime { get; set; }

        public string Operate { get; set; }

    }
}
