using Microsoft.EntityFrameworkCore;
using SysSoniaInventory.Models;


namespace SysSoniaInventory.DataAccess
{
    public partial class DBContext : DbContext
    {
        public DBContext()
        {
        }

        public DBContext(DbContextOptions<DBContext> options)
            : base(options)
        {
        }
        public virtual DbSet<ModelRol> modelRol { get; set; }
        public virtual DbSet<ModelSucursal> modelSucursal { get; set; }
        public virtual DbSet<ModelUser> modelUser { get; set; }

        public virtual DbSet<ModelCategory> modelCategory { get; set; }
        public virtual DbSet<ModelProveedor> modelProveedor { get; set; }
        public virtual DbSet<ModelProduct> modelProduct { get; set; }


        public virtual DbSet<ModelFactura> modelFactura { get; set; }
        public virtual DbSet<ModelMarca> modelMarca { get; set; }
        public virtual DbSet<ModelDetalleFactura> modelDetalleFactura { get; set; }

        public virtual DbSet<ModelDevolucion> modelDevolucion { get; set; }
        public virtual DbSet<ModelDetalleDevolucion> modelDetalleDevolucion { get; set; }

        public virtual DbSet<ModelCompra> modelCompra { get; set; }
        public virtual DbSet<ModelDetalleCompra> modelDetalleCompra { get; set; }

        public virtual DbSet<ModelHistorialProduct> modelHistorialProduct { get; set; }

        public DbSet<ModelReport> modelReport { get; set; }
       
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ModelRol>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK__ModelRol__3214EC0773A63FCB");
                entity.Property(e => e.Name).IsRequired().HasMaxLength(30).IsUnicode(false);
                entity.Property(e => e.AccessTipe).IsRequired().HasMaxLength(30).IsUnicode(false);
            });

            modelBuilder.Entity<ModelUser>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK__ModelUser__3214EC078152B9BB");

                entity.Property(e => e.Tel);
                entity.Property(e => e.Password).IsFixedLength();

                entity.HasOne(d => d.IdRolNavigation).WithMany(p => p.User)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_User_Rol");

                entity.HasOne(d => d.IdSucursalNavigation).WithMany(p => p.User)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_User_Sucursal");
            });

            modelBuilder.Entity<ModelSucursal>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK__ModelSucursal__3214EC0773A63FD2");
                entity.Property(e => e.Name).IsRequired().HasMaxLength(75).IsUnicode(false);
                entity.Property(e => e.Address).HasMaxLength(100).IsUnicode(false);
            });

            modelBuilder.Entity<ModelCategory>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK__ModelCategory__3214EC0773A63FE3");
                entity.Property(e => e.Name).IsRequired().HasMaxLength(60).IsUnicode(false);
            });

            modelBuilder.Entity<ModelProveedor>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK__ModelProveedor__3214EC0773A63FF4");
                entity.Property(e => e.Name).IsRequired().HasMaxLength(75).IsUnicode(false);
                entity.Property(e => e.Description).HasMaxLength(250).IsUnicode(false);
                entity.Property(e => e.Tel);
                entity.Property(e => e.Email).HasMaxLength(75).IsUnicode(false);
            });
            modelBuilder.Entity<ModelMarca>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK__ModelMarca__3214EC0773A63FE3");
                entity.Property(e => e.Name).IsRequired().HasMaxLength(75).IsUnicode(false);
                entity.Property(e => e.Description).HasMaxLength(250).IsUnicode(false);
            });

            modelBuilder.Entity<ModelProduct>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK__ModelProduct__3214EC0773A64015");
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100).IsUnicode(false);
                entity.Property(e => e.PurchasePrice).HasColumnType("decimal(18, 2)").IsRequired();
                entity.Property(e => e.SalePrice).HasColumnType("decimal(18, 2)").IsRequired();
                entity.Property(e => e.Stock).IsRequired();
                entity.Property(e => e.LowStock);
                entity.Property(e => e.Codigo).HasMaxLength(25).IsUnicode(false);
                entity.Property(e => e.Url).HasMaxLength(100).IsUnicode(false);
                entity.Property(e => e.Estatus).IsRequired();

                entity.HasOne(d => d.IdCategoryNavigation).WithMany(p => p.Product)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Product_Category");

                entity.HasOne(d => d.IdProveedorNavigation).WithMany(p => p.Product)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Product_Proveedor");


                entity.HasOne(d => d.IdMarcanavigation).WithMany(p => p.Product)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Product_Marca");
            });

            modelBuilder.Entity<ModelHistorialProduct>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK__ModelHistorialProduct__3214EC0773A64126");
                entity.Property(e => e.NameUser).IsRequired().HasMaxLength(100).IsUnicode(false);
                entity.Property(e => e.Date).HasColumnType("date").IsRequired();
                entity.Property(e => e.Time).HasColumnType("time").IsRequired();
                entity.Property(e => e.DescriptionCambio).HasMaxLength(250).IsUnicode(false);
            });

            modelBuilder.Entity<ModelFactura>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK__ModelFactura__3214EC0773A64237");
                entity.Property(e => e.NameSucursal).IsRequired().HasMaxLength(75).IsUnicode(false);
                entity.Property(e => e.NameUser).IsRequired().HasMaxLength(100).IsUnicode(false);
                entity.Property(e => e.NameClient).HasMaxLength(75).IsUnicode(false);
                entity.Property(e => e.Date).HasColumnType("date").IsRequired();
                entity.Property(e => e.Time).HasColumnType("time").IsRequired();
            });

            modelBuilder.Entity<ModelDetalleFactura>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK__ModelDetalleFactura__3214EC0773A64348");
                entity.Property(e => e.SalePriceUnitario).HasColumnType("decimal(18, 2)").IsRequired();
                entity.Property(e => e.ValorDescuento).HasColumnType("decimal(18, 2)").HasDefaultValue(0);
                entity.Property(e => e.SalePriceDescuento).HasColumnType("decimal(18, 2)").IsRequired();
                entity.Property(e => e.PurchasePriceUnitario).HasColumnType("decimal(18, 2)").IsRequired();
                entity.Property(e => e.PriceTotal).HasColumnType("decimal(18, 2)").IsRequired();
                entity.HasOne(d => d.IdFacturaNavigation).WithMany(p => p.DetalleFactura)
                  .OnDelete(DeleteBehavior.ClientSetNull)
                  .HasConstraintName("FK1_Factura_DetalleFactura");

            });

            modelBuilder.Entity<ModelDevolucion>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK__ModelDevolucion__3214EC0773A64237");
                entity.Property(e => e.NameSucursal).IsRequired().HasMaxLength(75).IsUnicode(false);
                entity.Property(e => e.NameUser).IsRequired().HasMaxLength(100).IsUnicode(false);
                entity.Property(e => e.NameClient).HasMaxLength(75).IsUnicode(false);
                entity.Property(e => e.Date).HasColumnType("date").IsRequired();
                entity.Property(e => e.Time).HasColumnType("time").IsRequired();
                entity.HasMany(d => d.DetalleDevolucion)
                      .WithOne(p => p.IdDevolucionNavigation)
                      .OnDelete(DeleteBehavior.ClientSetNull)
                      .HasConstraintName("FK_Devolucion_DetalleDevolucion");
            });

            modelBuilder.Entity<ModelDetalleDevolucion>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK__ModelDetalleDevolucion__3214EC0773A64348");
                entity.Property(e => e.NameProduct).IsRequired().HasMaxLength(100).IsUnicode(false);
                entity.Property(e => e.CodigoProducto).HasMaxLength(25).IsUnicode(false);
                entity.Property(e => e.PurchasePrice).HasColumnType("decimal(18, 2)").IsRequired();
                entity.Property(e => e.SalePriceUnitario).HasColumnType("decimal(18, 2)").IsRequired();
                entity.Property(e => e.CantidadProduct).IsRequired();
                entity.Property(e => e.PriceReembolso).HasColumnType("decimal(18, 2)").IsRequired();
                entity.Property(e => e.PriceTotalReembolso).HasColumnType("decimal(18, 2)").IsRequired();
                entity.HasOne(d => d.IdDevolucionNavigation)
                      .WithMany(p => p.DetalleDevolucion)
                      .HasForeignKey(d => d.IdDevolucion)
                      .OnDelete(DeleteBehavior.ClientSetNull)
                      .HasConstraintName("FK_DetalleDevolucion_Devolucion");
            });

            modelBuilder.Entity<ModelCompra>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK__ModelCompra__3214EC0773A64237");
                entity.Property(e => e.NameSucursal).IsRequired().HasMaxLength(75).IsUnicode(false);
                entity.Property(e => e.NameUser).IsRequired().HasMaxLength(100).IsUnicode(false);
                entity.Property(e => e.Description).IsRequired().HasMaxLength(250).IsUnicode(false);
                entity.Property(e => e.NameProveedor).HasMaxLength(75).IsUnicode(false);
                entity.Property(e => e.CodigoFactura).HasMaxLength(75).IsUnicode(false);
                entity.Property(e => e.Date).HasColumnType("date").IsRequired();
                entity.Property(e => e.Time).HasColumnType("time").IsRequired();
                entity.HasMany(d => d.DetalleCompra)
                      .WithOne(p => p.IdCompraNavigation)
                      .OnDelete(DeleteBehavior.ClientSetNull)
                      .HasConstraintName("FK_Compra_DetalleCompra");
            });

            modelBuilder.Entity<ModelDetalleCompra>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK__ModelDetalleCompra__3214EC0773A64348");
                entity.Property(e => e.MarcaProducto).HasMaxLength(75).IsUnicode(false);
                entity.Property(e => e.NameProducto).IsRequired().HasMaxLength(100).IsUnicode(false);
                entity.Property(e => e.CodigoProducto).HasMaxLength(25).IsUnicode(false);
                entity.Property(e => e.CantidadProduct).IsRequired();
                entity.Property(e => e.PriceCompraUnitario).HasColumnType("decimal(18, 2)").IsRequired();
                entity.Property(e => e.PriceTotal).HasColumnType("decimal(18, 2)").IsRequired();
                entity.HasOne(d => d.IdCompraNavigation)
                      .WithMany(p => p.DetalleCompra)
                      .HasForeignKey(d => d.IdCompra)
                      .OnDelete(DeleteBehavior.ClientSetNull)
                      .HasConstraintName("FK_DetalleCompra_Compra");
            });


            // Configuración de ModelReport
            modelBuilder.Entity<ModelReport>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK_ModelReport");
                entity.Property(e => e.TypeReport).IsRequired().HasMaxLength(100).IsUnicode(false);
                entity.Property(e => e.Description).IsRequired().HasMaxLength(1000).IsUnicode(false);
                entity.Property(e => e.Estatus).IsRequired().HasMaxLength(30).IsUnicode(false);
                entity.Property(e => e.NameUser).HasMaxLength(100).IsUnicode(false);
                entity.Property(e => e.ComentaryUser).HasMaxLength(1000).IsUnicode(false);
                entity.Property(e => e.StarDate).IsRequired().HasColumnType("date");
                entity.Property(e => e.StarTime).IsRequired().HasColumnType("time");
                entity.Property(e => e.EndDate).HasColumnType("date");
                entity.Property(e => e.EndTime).HasColumnType("time");
                entity.Property(e => e.IdRelation).IsRequired(false);
            });



            // Ajustes restantes similares.
            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
        public DbSet<SysSoniaInventory.Models.ModelCompra> ModelCompra { get; set; } = default!;


    }
}
