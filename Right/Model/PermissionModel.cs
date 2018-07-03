using Right.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Right.Model
{
    public class PermissionModel

    {
        public RightObject Resource { get; set; }


        public List<PermissionOperationModel> Operations = new List<PermissionOperationModel>();




    }
    public class PermissionOperationModel
    {
        public int OperationId { get; set; }
        public string OperationCode { get; set; }
        public bool IsHaveRight { get; set; }
    }
}
