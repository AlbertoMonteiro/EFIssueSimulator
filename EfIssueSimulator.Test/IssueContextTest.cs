using System.Data.Entity.Infrastructure;
using System.Data.Entity.Infrastructure.Interception;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EfIssueSimulator.Tests
{
    [TestClass]
    public class IssueContextTest
    {
        [TestCleanup]
        public void TearDown()
        {
            using (var context = new IssueContext())
            {
                context.Database.ExecuteSqlCommand("DELETE FROM Blog");
                context.Database.ExecuteSqlCommand("DELETE FROM Tenant");
            }
        }

        [TestMethod]
        public void ThereIsNoIssueWhenInsertARecordWithoutIDbCommandTreeInterceptor()
        {
            var tenant = new Tenant { Name = "First Tenant" };
            using (var context = new IssueContext())
            {
                //Arrange
                context.Database.CreateIfNotExists();
                context.Tenants.Add(tenant);
                context.SaveChanges();

                //Act
                context.Blogs.Add(new Blog { Name = "Some blog about C#", TenantId = tenant.Id });
                context.SaveChanges();
            }
            //Assert 
            using (var context = new IssueContext())
                Assert.IsTrue(context.Blogs.Any(b => b.Name == "Some blog about C#" && b.TenantId == tenant.Id));
        }

        [TestMethod]
        public void ThereIsIssueWhenInsertARecordWithIDbCommandTreeInterceptor()
        {
            var tenant = new Tenant { Name = "First Tenant" };
            using (var context = new IssueContext())
            {
                DbInterception.Add(new MultiTenantTreeInterceptor());
                DbInterception.Add(new MultiTenantInterceptor());
                //Arrange
                context.Database.CreateIfNotExists();
                context.Tenants.Add(tenant);
                context.SaveChanges();

                MultiTenantInterceptor.TentantId = tenant.Id;

                //Act
                context.Blogs.Add(new Blog { Name = "Some blog about C#" });
                try
                {
                    context.SaveChanges();
                }
                catch (DbUpdateException ex)
                {
                    const string expected = "The variable name '@tenantId' has already been declared. Variable names must be unique within a query batch or stored procedure.";
                    Assert.AreEqual(expected, ex.InnerException.InnerException.Message);
                }
            }
        }
    }
}
