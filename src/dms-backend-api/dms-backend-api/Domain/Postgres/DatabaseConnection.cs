
namespace dms_backend_api.Domain.Postgres
{
    public class DatabaseConnection
    {
        /*=postgres://{user}:{password}@{hostname}:{port}/{database-name}*/
        public string User { get; set; }
        public string Password { get; set; }
        public string Hostname { get; set; }
        public int Port { get; set; } = 5432;
        public string DatabaseName { get; set; }
    }
}