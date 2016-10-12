namespace EfIssueSimulator
{
    public class Blog
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public long TenantId { get; set; }
        public virtual Tenant Tenant { get; set; }
    }
}