using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Entity
{
    [PetaPoco.TableName("valueSet")]
    [PetaPoco.PrimaryKey("ID", AutoIncrement = true)]
    public class ValueSet 
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public string Value { get; set; }
        public string Description { get; set; }

        public string Creator { get; set; }
        public string CreatorID { get; set; }
        public DateTime? CreateTime { get; set; }

        public bool IsEnabled { get; set; }
        public bool IsDelete { get; set; }

    }
}
