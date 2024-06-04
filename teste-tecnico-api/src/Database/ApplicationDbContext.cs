using Microsoft.EntityFrameworkCore;
using teste_tecnico_api.src.Models;

namespace teste_tecnico_api.src.Database;
public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{

    public DbSet<BillToPay> BillsToPay { get; set; }
}
