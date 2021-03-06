﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Slickflow.Module.Resource
{
    /// <summary>
    /// 部门实体对象
    /// </summary>
    [Table("SysDepartment")]
    public class DeptEntity
    {
        public int ID { get; set; }
        public string DeptCode { get; set; }
        public string DeptName { get; set; }
        public int ParentID { get; set; }
        public string Description { get; set; }

        public bool IsEnabled { get; set; }
    }
}
