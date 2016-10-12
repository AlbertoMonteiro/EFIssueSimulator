using System;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Infrastructure.Interception;
using System.Linq;
using EfIssueSimulator;
using Xunit;

namespace EfIssueSimulatorTests
{
    public class IssueContextTest : TestClassBase, IDisposable
    {
        public void Dispose()
        {
            using (var context = new IssueContext())
            {
                context.Database.ExecuteSqlCommand("DELETE FROM Blog");
                context.Database.ExecuteSqlCommand("DELETE FROM Tenant");
            }
        }

        [Fact, Order(1)]
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
                Assert.True(context.Blogs.Any(b => b.Name == "Some blog about C#" && b.TenantId == tenant.Id));
        }

        [Fact, Order(2)]
        public void ThereIsIssueWhenInsertARecordWithIDbCommandTreeInterceptor()
        {
            var tenant = new Tenant { Name = "First Tenant" };
            DbInterception.Add(new MultiTenantTreeInterceptor());
            DbInterception.Add(new MultiTenantInterceptor());

            using (var context = new IssueContext())
            {
                //Arrange
                context.Database.CreateIfNotExists();
                context.Tenants.Add(tenant);
                context.SaveChanges();

                MultiTenantInterceptor.TentantId = tenant.Id;

                //Act
                context.Blogs.Add(new Blog { Name = "Some blog about C#", Tenant = tenant, TenantId = tenant.Id });
                try
                {
                    context.SaveChanges();
                    Assert.True(false, "it should fail");
                }
                catch (DbUpdateException ex)
                {
                    const string expected = "The variable name '@tenantId' has already been declared. Variable names must be unique within a query batch or stored procedure.";
                    Assert.Equal(expected, ex.InnerException.InnerException.Message);
                }
            }
        }
    }
}
