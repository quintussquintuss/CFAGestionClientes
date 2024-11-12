using Microsoft.EntityFrameworkCore;
using CFAGestionClientes.Models;

namespace CFAGestionClientes.Data
{
    public class CFAContext : DbContext
    {
        public CFAContext(DbContextOptions<CFAContext> options) : base(options) { }

        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<Direccion> Direcciones { get; set; }
        public DbSet<Telefono> Telefonos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // Configuración adicional puede ir aquí si es necesario
        }
    }
}