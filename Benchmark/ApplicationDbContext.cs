using SQLite.CodeFirst;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Benchmark
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(string path) : base(DbServices.PrepareConnectionString(path), true)
        {
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            Database.SetInitializer(new DbInitializer(modelBuilder));
        }

        public virtual DbSet<SpeedTestResultHeader> SpeedTestResultHeaders { get; set; }
        public virtual DbSet<SpeedTestResult> SpeedTestResults { get; set; }

        public static void Seed(ApplicationDbContext db)
        {
            
        }
    }

    public class DbInitializer : SqliteCreateDatabaseIfNotExists<ApplicationDbContext>
    {
        public DbInitializer(DbModelBuilder modelBuilder) : base(modelBuilder)
        {
        }
        protected override void Seed(ApplicationDbContext context)
        {
            base.Seed(context);
            ApplicationDbContext.Seed(context);
        }
    }
    public class DbServices
    {
        private static readonly string dbPassword = "";
        public static bool CheckDbPath(string path)
        {
            if (!string.IsNullOrEmpty(path)
                || Path.HasExtension(path)
                || Path.GetExtension(path) == "db3"
                || File.Exists(path)
                )
                return true;

            return false;
        }

        public static SQLiteConnection PrepareConnectionString(string path)
        {
          
            return new SQLiteConnection($"data source={path};");
        }

    }
}
