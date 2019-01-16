using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Model
{
    public  class DepartmentModel
    {
        public string ID { get; set; }
        public string DepName { get; set; }
        public string ParentDepID { get; set; }

        public DateTime? CreateTime { get; set; }

        public bool IsEnabled { get; set; }
        /// <summary>
        /// 下属部门
        /// </summary>

        public List<DepartmentModel> NodeList { get; set; }


        public DepartmentModel ParentDepartment { get; set; }
    }
}
