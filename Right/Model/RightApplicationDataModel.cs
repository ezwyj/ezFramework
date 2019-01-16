using Right.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Right.Model
{
    public class RightApplicationDataModel
    {
        public RightApplication Application { get; set; }
        public List<RightObject> ObjectList { get; set; }
        public List<RightObjectOperation> OperationList { get; set; }
        public List<RightRole> RoleList { get; set; }
        public List<RightRoleUser> RoleUserList { get; set; }
        public List<RightRoleOperation> RoleOperationList { get; set; }
    }
}
