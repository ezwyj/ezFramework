using Right.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Right
{
    interface IRight
    {
        List<RightRoleUser> GetRoleUserByBadges(string badge);
        bool HaveRight(string badge,  string ResourceName, string OperationCode);
    }
}
