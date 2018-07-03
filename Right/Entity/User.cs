using PetaPoco;
using System;

namespace Right.Entity
{
    [TableName("Right_User")]
    [PrimaryKey("UserId", AutoIncrement = true)]
    public class User 
    {


        public int UserId { get; set; }
        public string UserName { get; set; }
        public int EmployeeId { get; set; }

    }
}
