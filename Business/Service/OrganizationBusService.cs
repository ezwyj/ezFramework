using Business.Interface;
using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Service
{


    public class OrganizationBusService
    {
        private readonly ILog _logService = new UnityContainerHelp().GetServer<ILog>();
        private readonly IDepartment _departmentService = new UnityContainerHelp().GetServer<IDepartment>();

    }
}
