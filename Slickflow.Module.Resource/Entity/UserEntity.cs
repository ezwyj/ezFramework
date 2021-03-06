﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Slickflow.Module.Resource
{
    /// <summary>
    /// 用户对象
    /// </summary>
    [Table("SysUser")]
    public class UserEntity
    {
        public int ID { get; set; }
        public string UserName { get; set; }

        public string Login { get; set; }

        public string Password { get; set; }
    }
}
