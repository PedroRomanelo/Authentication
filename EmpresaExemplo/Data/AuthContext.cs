using System;
using System.Collections.Generic;
using EmpresaExemplo.Models.AuthDB;
using Microsoft.EntityFrameworkCore;

namespace EmpresaExemplo.Data;

public partial class AuthContext : DbContext
{
    public AuthContext()
    {
    }

    public AuthContext(DbContextOptions<AuthContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Cliente> Clientes { get; set; }

    public virtual DbSet<ItensPedido> ItensPedidos { get; set; }

    public virtual DbSet<Pedido> Pedidos { get; set; }

    public virtual DbSet<Produto> Produtos { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=pedro-ybera;Database=AuthDB;Trusted_Connection=True;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Cliente>(entity =>
        {
            entity.HasIndex(e => e.Documento, "UQ__Clientes__AF73706D0089A3CA").IsUnique();

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Cidade).HasMaxLength(100);
            entity.Property(e => e.Documento)
                .HasMaxLength(11)
                .IsUnicode(false);
            entity.Property(e => e.Estado).HasMaxLength(100);
            entity.Property(e => e.NomeCompleto).HasMaxLength(100);

            entity.HasOne(d => d.IdNavigation).WithOne(p => p.Cliente)
                .HasForeignKey<Cliente>(d => d.Id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Clientes_Users");
        });

        modelBuilder.Entity<ItensPedido>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__ItensPed__3214EC07C5238CE0");

            entity.Property(e => e.Quantidade).HasDefaultValue(1);

            entity.HasOne(d => d.Pedido).WithMany(p => p.ItensPedidos)
                .HasForeignKey(d => d.PedidoId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ItensPedi__Pedid__5FB337D6");

            entity.HasOne(d => d.Produto).WithMany(p => p.ItensPedidos)
                .HasForeignKey(d => d.ProdutoId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ItensPedi__Produ__5EBF139D");
        });

        modelBuilder.Entity<Pedido>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Pedidos__3214EC07F3C7EB4B");

            entity.Property(e => e.Frete).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.Horario).HasColumnType("datetime");
            entity.Property(e => e.ValorTotal).HasColumnType("decimal(10, 2)");

            entity.HasOne(d => d.Cliente).WithMany(p => p.Pedidos)
                .HasForeignKey(d => d.ClienteId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Pedidos_Clientes");
        });

        modelBuilder.Entity<Produto>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Produtos__3214EC073B1C35E0");

            entity.HasIndex(e => e.Sku, "UQ__Produtos__CA1ECF0D600F36FE").IsUnique();

            entity.Property(e => e.NomeDoProduto).HasMaxLength(100);
            entity.Property(e => e.Sku)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("SKU");
            entity.Property(e => e.ValorUnitario).HasColumnType("decimal(10, 2)");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
