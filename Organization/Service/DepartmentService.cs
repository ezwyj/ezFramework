using Organization.Entity;
using PetaPoco;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Organization.Service
{
    public class DepartmentService : Business.Interface.i
    {
        private Database db;
        public DepartmentService(Database v)
        {
            db = v;
        }

        public List<Department > GetDepResult(string parentDepId, string keyword,bool isEnabled)
        {
            
            var list = new List<Department>();
            var sql = new Sql("SELECT * from org_t_department WHERE IsEnabled ="+isEnabled);

            if (parentDepId == "")
            {
                sql.Append("AND parentDepId IS NULL");
            }
            else if (parentDepId != null)
            {
                sql.Append("AND parentDepId = @0", parentDepId);
            }
            if (!string.IsNullOrEmpty(keyword))
            {
                sql.AppendLike("AND DepName LIKE @0", keyword);
            }
            sql.Append("ORDER BY DepName collate Chinese_PRC_CS_AS_KS_WS ASC");
            list = db.Fetch<Department>(sql);
            foreach (var item in list)
            {
                item.NodeList = GetDepNodeList(item.ID);
                item.ParentDepartment = GetById(item.ParentDepID);
            }

            return list;
        }

        public  List<Department> GetDepNodeList(string depId)
        {
            return db.Fetch<Department>("SELECT * from org_t_department WHERE IsEnabled = 1 and parentDepId=@0", depId);
        }

        public  Department GetById(string depId)
        {
            return db.SingleOrDefault<Department>("SELECT * from org_t_department WHERE IsEnabled = 1 and id=@0", depId);
        }


    }
}
