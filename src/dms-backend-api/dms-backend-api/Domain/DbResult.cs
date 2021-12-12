using dms_backend_api.Model;
using System.Collections.Generic;

namespace dms_backend_api.Domain
{
    public class DbResult
    {
        private static readonly DbResult _success = new()
        {
            Succeeded = true
        };

        private readonly List<ErrorModel> _errors = new();

        public bool Succeeded
        {
            get;
            protected set;
        }

        public IEnumerable<ErrorModel> Errors => _errors;

        public static DbResult Success => _success;
        public static DbResult Failed(params ErrorModel[] errors)
        {
            DbResult DbResult = new()
            {
                Succeeded = false
            };
            if (errors != null)
            {
                DbResult._errors.AddRange(errors);
            }

            return DbResult;
        }
    }
}

