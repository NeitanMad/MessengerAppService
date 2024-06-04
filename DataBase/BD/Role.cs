namespace DataBase.BD;

public class Role
{
    public RoleId RoleId { get; set; }

    public string Name { get; set; } = null!;

    public virtual List<User> Users { get; set; }
}