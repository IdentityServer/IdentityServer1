using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Configuration;

[assembly: WebActivator.PreApplicationStartMethod(typeof(Thinktecture.IdentityServer.Web.App_Start.EntityFramework_SqlServerCompact), "Start")]

namespace Thinktecture.IdentityServer.Web.App_Start
{
    public static class EntityFramework_SqlServerCompact
    {
        public static void Start()
        {
            var useSqlString = ConfigurationManager.AppSettings["UseSqlServerForConfiguration"];
            if (!string.IsNullOrWhiteSpace(useSqlString))
            {
                bool useSql = false;
                if (bool.TryParse(useSqlString, out useSql))
                {
                    if (useSql)
                    {
                        Database.DefaultConnectionFactory = new SqlConnectionFactory(string.Empty);
                        return;
                    }
                }
            }

            Database.DefaultConnectionFactory = new SqlCeConnectionFactory("System.Data.SqlServerCe.4.0");
        }
    }
}
