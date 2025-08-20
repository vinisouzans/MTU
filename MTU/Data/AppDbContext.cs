using Microsoft.EntityFrameworkCore;
using MTU.Model;

namespace MTU.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<Moto> Motos { get; set; }
        public DbSet<Entregador> Entregadores { get; set; }
        public DbSet<Locacao> Locacoes { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<NotificacaoMoto> NotificacoesMotos { get; set; }


        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Moto
            modelBuilder.Entity<Moto>()
                .HasIndex(m => m.Placa)
                .IsUnique();

            // Entregador
            modelBuilder.Entity<Entregador>()
                .HasIndex(e => e.Cnpj)
                .IsUnique();

            modelBuilder.Entity<Entregador>()
                .HasIndex(e => e.NumeroCNH)
                .IsUnique();

            // Usuário/Admin
            modelBuilder.Entity<Usuario>()
                .HasIndex(u => u.Email)
                .IsUnique();

            modelBuilder.Entity<Usuario>()
                .Property(u => u.SenhaHash)
                .IsRequired();

            modelBuilder.Entity<Usuario>()
                .Property(u => u.EhAdmin)
                .HasDefaultValue(false);
        }
    }
}
