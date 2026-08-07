namespace TheMathAndScienceAcademy.Domain.Entities;

public class RolePermission
{
    public string RoleId { get; set; } = default!;
    public string PermissionId { get; set; } = default!;
    public bool IsGranted { get; set; }

    public Role Role { get; set; } = default!;
    public Permission Permission { get; set; } = default!;
}
 
