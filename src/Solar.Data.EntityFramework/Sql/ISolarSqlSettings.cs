using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Solar.Data.EntityFramework.Sql
{
    public interface ISolarSqlSettings
    {
        int CommandTimeout { get; }
        bool UseBulkUploading { get; }
        SqlDatabaseMode DatabaseMode { get; }
    }
}
