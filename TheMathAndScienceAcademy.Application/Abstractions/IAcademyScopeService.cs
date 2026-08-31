namespace TheMathAndScienceAcademy.Application.Abstractions;

public interface IAcademyScopeService
{
    Task<AcademyScope> GetScopeAsync();
}

public sealed record AcademyScope(
    bool IsSuperAdmin,
    string? AcademyId,
    bool HasSelectedAcademy,
    bool IsValid,
    bool IsSelectionRejected = false);
