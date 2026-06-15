using TheMathAndScienceAcademy.Domain.Common;

namespace TheMathAndScienceAcademy.Domain.Entities;

public class Academy : BaseEntity
{
    public string Name { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string PhoneNumber { get; set; } = default!;
    public string Address { get; set; } = default!;
}
