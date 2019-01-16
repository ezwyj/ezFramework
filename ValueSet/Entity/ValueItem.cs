using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ValueSet.Entity
{
    [PetaPoco.TableName("sys_T_valueSet")]
    [PetaPoco.PrimaryKey("ID", autoIncrement = true)]
    public class ValueItem
    {

        public ValueItem()
        {

        }
        public int Id { get; set; }
        public int SetId { get; set; }
        public string Value { get; set; }
        public string Text { get; set; }



        public bool IsEnabled { get; set; }
        public int? Sort { get; set; }

        public bool IsDelete { get; set; }
        public string Creator { get; set; }
        public string CreatorId { get; set; }
        public DateTime? CreateTime { get; set; }



    }
}
