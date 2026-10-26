using TheMathAndScienceAcademy.Domain.Common;

namespace TheMathAndScienceAcademy.Domain.Entities;

public class AuditLog : BaseEntity
{
    public string? ActorUserId { get; set; }
    public string Action { get; set; } = default!;
    public string Entity { get; set; } = default!;
    public string? EntityId { get; set; }
    public string? AcademyId { get; set; }
    public string Result { get; set; } = default!;
    public int StatusCode { get; set; }
}
