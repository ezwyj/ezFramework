using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Model
{
    public class JobModel
    {
        public int Id { get; set; }

        public string JobName { get; set; }

        public List<EmployeeModel> Employees { get; set; }


    }
}
