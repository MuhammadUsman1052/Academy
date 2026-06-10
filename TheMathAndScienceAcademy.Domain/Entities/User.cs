
using TheMathAndScienceAcademy.Domain.Common;
namespace TheMathAndScienceAcademy.Domain.Entities;
public class User : BaseEntity
{
    public string Name { get; set; } = default!;
    public string Email { get; set; } = default!;
}
