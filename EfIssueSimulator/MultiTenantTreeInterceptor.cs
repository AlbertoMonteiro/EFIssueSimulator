using System.Data.Entity.Core.Common.CommandTrees;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Infrastructure.Interception;
using System.Linq;

namespace EfIssueSimulator
{
    public class MultiTenantTreeInterceptor : IDbCommandTreeInterceptor
    {
        private const string COLUMN_NAME = "TenantId";

        public void TreeCreated(DbCommandTreeInterceptionContext interceptionContext)
        {
            if (interceptionContext.OriginalResult.DataSpace == DataSpace.SSpace)
                if (CanHandle(interceptionContext.Result))
                {
                    var dbCommandTree = Handle(interceptionContext.Result);
                    if (dbCommandTree != null)
                        interceptionContext.Result = dbCommandTree;
                }
        }

        private bool CanHandle(DbCommandTree command)
            => command is DbInsertCommandTree;

        public DbCommandTree Handle(DbCommandTree command)
        {
            var insertCommandTree = command as DbInsertCommandTree;

            var dbSetClauses = insertCommandTree.SetClauses.Cast<DbSetClause>().ToList();

            var setClause = dbSetClauses.FirstOrDefault(HasColumn);
            if (setClause != null)
            {
                dbSetClauses.RemoveAll(HasColumn);
                dbSetClauses.Add(CreateSetClause(setClause.Property as DbPropertyExpression));

                return new DbInsertCommandTree(insertCommandTree.MetadataWorkspace,
                    insertCommandTree.DataSpace,
                    insertCommandTree.Target,
                    dbSetClauses.Cast<DbModificationClause>().ToList().AsReadOnly(),
                    insertCommandTree.Returning);
            }
            return insertCommandTree;
        }

        private static DbSetClause CreateSetClause(DbPropertyExpression propertyExpression)
            => DbExpressionBuilder.SetClause(propertyExpression, propertyExpression.ResultType.Parameter("tenantId"));

        private static bool HasColumn(DbSetClause setClause)
            => (setClause.Property as DbPropertyExpression).Property.Name == COLUMN_NAME;
    }
}