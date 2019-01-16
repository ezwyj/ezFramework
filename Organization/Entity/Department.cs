using PetaPoco;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Organization.Entity
{
    public class Department
    {
        public string ID { get; set; }
        public string DepName { get; set; }
        public string ParentDepID { get; set; }

        public DateTime? CreateTime { get; set; }

        public bool IsEnabled { get; set; }
    }
}
