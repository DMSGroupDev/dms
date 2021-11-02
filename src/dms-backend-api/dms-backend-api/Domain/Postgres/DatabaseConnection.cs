
namespace dms_backend_api.Domain.Postgres
{
    public partial class DatabaseConnection
    {
        /*=postgres://{user}:{password}@{hostname}:{port}/{database-name}*/
        public string User { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string Hostname { get; set; } = null!;
        public int Port { get; set; } = 5432;
        public string DatabaseName { get; set; } = null!;
    }
}