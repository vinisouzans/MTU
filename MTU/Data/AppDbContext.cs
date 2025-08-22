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
        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<Pedido> Pedidos { get; set; }
        public DbSet<ItemPedido> ItensPedido { get; set; }
        public DbSet<Entrega> Entregas { get; set; }

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

            // Cliente
            modelBuilder.Entity<Cliente>()
                .HasIndex(c => c.Email)
                .IsUnique();

            // Pedido
            modelBuilder.Entity<Pedido>()
                .HasOne(p => p.Cliente)
                .WithMany()
                .HasForeignKey(p => p.ClienteId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Pedido>()
                .HasMany(p => p.Itens)
                .WithOne(i => i.Pedido)
                .HasForeignKey(i => i.PedidoId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Pedido>()
                .Property(p => p.ValorTotal)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Pedido>()
                .Property(p => p.TaxaEntrega)
                .HasColumnType("decimal(18,2)");

            // ItemPedido
            modelBuilder.Entity<ItemPedido>()
                .Property(i => i.PrecoUnitario)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<ItemPedido>()
                .Ignore(i => i.Subtotal);

            // Entrega
            modelBuilder.Entity<Entrega>()
                .HasOne(e => e.Pedido)
                .WithMany()
                .HasForeignKey(e => e.PedidoId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Entrega>()
                .HasOne(e => e.Entregador)
                .WithMany()
                .HasForeignKey(e => e.EntregadorId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}