using Business.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Interface
{


    public interface IDepartment
    {
        void SaveDepartment(DepartmentModel model);
        void DeleteDepartment(DepartmentModel model);
        List<DepartmentModel> GetDepartments(int parentDeptId, string keyword, bool isEnabled);

        DepartmentModel GetDepartment(int Id);

    }

    public interface IEmployee
    {
        void SaveEmployee(EmployeeModel model);
        void DeleteEmployee(EmployeeModel model);

        List<EmployeeModel> GetEmployee(string name, bool isEnabled);
        List<EmployeeModel> GetEmployee(DepartmentModel dep, bool isEnabled);
        List<EmployeeModel> GetEmployee(JobModel job, bool isEnabled);


    }

    public interface IJob
    {
        void SaveJob(JobModel job);
        void DeleteJob(JobModel job);

        List<JobModel> GetJobs(string name, bool isEnabled);
    }

}
