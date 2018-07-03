using PetaPoco;

namespace Right.Entity
{
    [TableName("ProjectArchive_Right_Role")]
    [PrimaryKey("RoleId", AutoIncrement = true)]
    public class Role 
    {
        

        [Column]
        public string Name { get; set; }

        [Column("RoleId")]
        public int RoleId { get; set; }

        [Column]
        public int Sort { get; set; }
    }
}
