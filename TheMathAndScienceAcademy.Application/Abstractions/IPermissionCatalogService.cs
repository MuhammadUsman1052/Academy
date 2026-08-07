namespace TheMathAndScienceAcademy.Application.Abstractions;

public interface IPermissionCatalogService
{
    IReadOnlyCollection<string> GetPermissionNames();
}
