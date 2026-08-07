namespace TheMathAndScienceAcademy.Application.Features.RolePermissions.Dtos;

public class RolePermissionDto
{
    public Guid PermissionId { get; set; }
    public string PermissionName { get; set; } = default!;
    public string? PermissionDescription { get; set; }
    public string ModuleName { get; set; } = default!;
    public bool IsGranted { get; set; }
}
