using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Organization.Entity
{
    public class DepartmentEmployee
    {
        public int Id { get; set; }

        public int DepartmentId { get; set; }

        public int EmployeeId { get; set; }
    }
}
