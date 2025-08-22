using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace TransmissionStockApp.Helpers
{
    public static class DbExceptionHelper
    {
        public static bool IsForeignKeyViolation(DbUpdateException ex)
        {
            if (ex.InnerException is SqlException sqlEx)
                return sqlEx.Number == 547; // FK violation

            var msg = ex.InnerException?.Message ?? ex.Message;
            return msg.Contains("FOREIGN KEY", StringComparison.OrdinalIgnoreCase)
                || msg.Contains("REFERENCE constraint", StringComparison.OrdinalIgnoreCase);
        }

        public static bool IsUniqueViolation(DbUpdateException ex)
        {
            if (ex.InnerException is SqlException sql)
                return sql.Number is 2601 or 2627;
            var msg = ex.InnerException?.Message ?? ex.Message;
            return msg.Contains("UNIQUE", StringComparison.OrdinalIgnoreCase)
                || msg.Contains("duplicate", StringComparison.OrdinalIgnoreCase);
        }
    }
}
