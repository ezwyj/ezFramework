using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Organization.Entity
{
    public class Employee
    {
        public int Id { get; set; }

        public string Badge { get; set; }
        public string Name { get; set; }

        public string Email { get; set; }

        public int Status { get; set; }

        public int JobId { get; set; }

        public string JobName { get; set; }

        public int DepId { get; set; }

        public string DepName { get; set; }

        public int ZeroDepId { get; set; }

        public string ZeroDepName { get; set; }

        public bool IsEnabled { get; set; }

        public DateTime? CreateTime { get; set; }
    }
}
