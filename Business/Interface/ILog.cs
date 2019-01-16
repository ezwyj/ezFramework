using Business.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Interface
{
    public interface ILog
    {
        void WriteCfgLog<T>(EmployeeModel employee, string log, T oldObj, T newObj);

        void WriteOperateLog(EmployeeModel employee, string log);

        void WriteErrorLog(string log);

    }
}
