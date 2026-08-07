namespace TheMathAndScienceAcademy.Application.Features.Academies.Dtos;

public class AcademyDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string PhoneNumber { get; set; } = default!;
    public string Address { get; set; } = default!;
    public Guid? AdminRoleId { get; set; }
    public Guid? AdminUserId { get; set; }
}
