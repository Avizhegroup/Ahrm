using Avhrm.Domains;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Avhrm.Persistence.Services;
public class AvhrmDbContext : IdentityDbContext<ApplicationUser>
{
    private readonly IHttpContextAccessor httpContextAccessor;

    public AvhrmDbContext(DbContextOptions<AvhrmDbContext> options
        , IHttpContextAccessor httpContextAccessor) : base(options)
    {
        Database.Migrate();

        this.httpContextAccessor = httpContextAccessor;
    }
  
    public DbSet<VacationRequest> VacationRequests { get; set; }
    public DbSet<WorkReport> WorkingReports { get; set; }
    public DbSet<WorkType> WorkTypes { get; set; }
    public DbSet<Project> Projects { get; set; }
    public DbSet<Customer> Customers { get; set; }
    public DbSet<Department> Departments { get; set; }
    public DbSet<WorkChallenge> WorkChallenges { get; set; }
    public DbSet<UserPointChangeLog> PointChangeLogs { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(Project).Assembly);

        base.OnModelCreating(modelBuilder);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var entries = ChangeTracker.Entries()
                                                       .Where(e => e.Entity is IBaseEntity);

        var userId = httpContextAccessor.HttpContext.User.GetUserId();

        foreach (var entry in entries)
        {
            var entity = (IBaseEntity)entry.Entity;

            if (entry.State == EntityState.Added)
            {
                entity.CreateDateTime = DateTime.UtcNow;
                entity.CreatorUserId = userId;
            }
            else if (entry.State == EntityState.Modified)
            {
                entity.LastUpdateDateTime = DateTime.UtcNow;
                entity.LastUpdateUserId = userId;
            }
        }

        return await base.SaveChangesAsync(cancellationToken);
    }
}
