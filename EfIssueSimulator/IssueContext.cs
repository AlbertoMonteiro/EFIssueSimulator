using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace EfIssueSimulator
{
    public class IssueContext : DbContext
    {
        public IssueContext()
            : base(@"Data source=.\sqlexpress;Initial catalog=IssueDatabase;Integrated security=true")
        {

        }

        public DbSet<Tenant> Tenants { get; set; }
        public DbSet<Blog> Blogs { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }
    }
}
