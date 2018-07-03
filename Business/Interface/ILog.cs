using Account.Model.Organization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Interface
{
    public interface ILog
    {
        void WriteCfgLog<T>(Employee employee, string log, T oldObj, T newObj);

        void WriteOperateLog(Employee employee, string log, int projectId);

    }
}
