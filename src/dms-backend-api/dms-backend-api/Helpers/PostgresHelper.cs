
using dms_backend_api.Domain.Postgres;

namespace dms_backend_api.Helpers
{
    public static class PostgresHelper
    {
        public static DatabaseConnection TransferPostgreUrlToConnection(string url)
        {
            var _url = url.Replace("postgres://", "");
            var user = _url.Substring(0, _url.IndexOf(':'));
            _url = _url.Remove(0, _url.IndexOf(':') + 1);

            var password = _url.Substring(0, _url.IndexOf('@'));
            _url = _url.Remove(0, _url.IndexOf('@') + 1);

            var hostname = _url.Substring(0, _url.IndexOf('/'));
            _url = _url.Remove(0, _url.IndexOf('/') + 1);

            var databaseName = _url.Substring(0, _url.Length);
            /*TODO: ADD port parse*/
            return new DatabaseConnection()
            {
                User = user,
                Password = password,
                Hostname = hostname,
                DatabaseName = databaseName
            };
        }
    }
}
