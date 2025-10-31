using Microsoft.Data.SqlClient;

namespace Identity.Presentation.Initializer
{
    public interface IDatabaseInitializer
    {
        Task EnsureDatabaseCreatedAsync();
    }

   
}
