using System.Data;

namespace Ponce.Templates.APITemplate.Domain.Contracts;

public interface IRepository
{
    Task SaveChanges(CancellationToken ct = default);
    IDbTransaction BeginTransaction();
    Task CloseTransaction();
}
