using MagacinERP.Core.Models;

namespace MagacinERP.Core.Interfaces;

public interface IAuditLogRepository
{
    void Add(AuditLog log);
    IEnumerable<AuditLog> GetByDateRange(DateTime from, DateTime to);
    IEnumerable<AuditLog> GetRecent(int days);
}
