#nullable disable

using System.Reflection;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TurnDigital.Domain.Features.Categories.Entities;
using TurnDigital.Domain.Features.Products.Entities;
using TurnDigital.Domain.Logging.Entities;
using TurnDigital.Domain.Security.Entities;

namespace TurnDigital.Infrastructure.DataAccess;

public class TurnDigitalDbContext : IdentityDbContext<User,
    Role,
    int,
    UserClaim,
    UserRole,
    UserLogin,
    RoleClaim,
    UserToken>
{
    public TurnDigitalDbContext(DbContextOptions<TurnDigitalDbContext> options) : base(options)
    {

    }

    internal DbSet<LoginAudit> LoginAudits { get; set; }

    internal DbSet<ActionAudit> ActionsAudits { get; set; }
    
    internal DbSet<Category> Categories { get; set; }
    
    internal DbSet<Product> Products { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(InfrastructureModule).Assembly);
    }
}