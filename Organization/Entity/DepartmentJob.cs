using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Organization.Entity
{
    public class DepartmentJob
    {
        public int Id { get; set; }

        public int DepartmentId { get; set; }

        public int JobId { get; set; }
    }
}
