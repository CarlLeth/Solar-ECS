using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Solar.Data.EntityFramework.Sql
{
    public class StandardSolarSqlSettings : ISolarSqlSettings
    {
        public int CommandTimeout => 600;
        public bool UseBulkUploading => true;
        public SqlDatabaseMode DatabaseMode => SqlDatabaseMode.SqlServer;
    }
}
