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
        public virtual DbSet<ModelUser> modelUser{ get; set; }
        
        public virtual DbSet<ModelCategory> modelCategory { get; set; }
        public virtual DbSet<ModelProveedor> modelProveedor { get; set; }
        public virtual DbSet<ModelProduct> modelProduct { get; set; }
     

        public virtual DbSet<ModelFactura> modelFactura { get; set; }
        public virtual DbSet<ModelDetalleFactura> modelDetalleFactura { get; set; }

        public virtual DbSet<ModelDevolucion> modelDevolucion { get; set; }
        public virtual DbSet<ModelDetalleDevolucion> modelDetalleDevolucion { get; set; }

        public virtual DbSet<ModelHistorialProduct> modelHistorialProduct { get; set; }


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

                entity.Property(e => e.Password).IsFixedLength();

                entity.HasOne(d => d.IdRolNavigation).WithMany(p => p.User)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK1_Rol_Usuario");

                entity.HasOne(d => d.IdSucursalNavigation).WithMany(p => p.User)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK1_Sucursal_Usuario");
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
                entity.Property(e => e.Name).IsRequired().HasMaxLength(60).IsUnicode(false);
                entity.Property(e => e.Description).HasMaxLength(250).IsUnicode(false);
                entity.Property(e => e.Email).HasMaxLength(75).IsUnicode(false);
            });

            modelBuilder.Entity<ModelProduct>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK__ModelProduct__3214EC0773A64015");
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100).IsUnicode(false);
                entity.Property(e => e.PurchasePrice).HasColumnType("decimal(18, 2)").IsRequired();
                entity.Property(e => e.SalePrice).HasColumnType("decimal(18, 2)").IsRequired();

                entity.HasOne(d => d.IdCategoryNavigation).WithMany(p => p.Product)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK1_Category_Product");

                entity.HasOne(d => d.IdProveedorNavigation).WithMany(p => p.Product)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK1_Proveedor_Product");
            });

            modelBuilder.Entity<ModelHistorialProduct>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK__ModelHistorialProduct__3214EC0773A64126");
                entity.Property(e => e.NameUser).IsRequired().HasMaxLength(100).IsUnicode(false);
                entity.Property(e => e.Date).HasColumnType("date").IsRequired();
                entity.Property(e => e.Time).HasColumnType("time").IsRequired();

            });

            modelBuilder.Entity<ModelFactura>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK__ModelFactura__3214EC0773A64237");
                entity.Property(e => e.NameSucursal).IsRequired().HasMaxLength(75).IsUnicode(false);
                entity.Property(e => e.NameUser).IsRequired().HasMaxLength(75).IsUnicode(false);
                entity.Property(e => e.Date).HasColumnType("date").IsRequired();
                entity.Property(e => e.Time).HasColumnType("time").IsRequired();
            });

            modelBuilder.Entity<ModelDetalleFactura>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK__ModelDetalleFactura__3214EC0773A64348");
                entity.Property(e => e.SalePriceUnitario).HasColumnType("decimal(18, 2)").IsRequired();
                entity.Property(e => e.SalePriceDescuento).HasColumnType("decimal(18, 2)").IsRequired();
                entity.Property(e => e.PriceTotal).HasColumnType("decimal(18, 2)").IsRequired();

                entity.HasOne(d => d.IdFacturaNavigation).WithMany(p => p.DetalleFactura)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK1_Factura_DetalleFactura");

                
            });

            modelBuilder.Entity<ModelDevolucion>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK__ModelDevolucion__3214EC0773A64459");
                entity.Property(e => e.NameSucursal).IsRequired().HasMaxLength(50).IsUnicode(false);
                entity.Property(e => e.NameUser).IsRequired().HasMaxLength(50).IsUnicode(false);
                entity.Property(e => e.Date).HasColumnType("date").IsRequired();
                entity.Property(e => e.Time).HasColumnType("time").IsRequired();

                entity.HasOne(d => d.IdFacturaNavigation).WithMany()
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK1_Factura_Devolucion");
            });

            modelBuilder.Entity<ModelDetalleDevolucion>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK__ModelDetalleDevolucion__3214EC0773A6456A");
                entity.Property(e => e.NameProduct).IsRequired().HasMaxLength(100).IsUnicode(false);
                entity.Property(e => e.PurchasePrice).HasColumnType("decimal(18, 2)").IsRequired();
                entity.Property(e => e.SalePriceUnitario).HasColumnType("decimal(18, 2)").IsRequired();
                entity.Property(e => e.PriceReembolso).HasColumnType("decimal(18, 2)").IsRequired();
                entity.Property(e => e.PriceTotalReembolso).HasColumnType("decimal(18, 2)").IsRequired();

                entity.HasOne(d => d.IdDevolucionNavigation).WithMany(p => p.DetalleDevolucion)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK1_Devolucion_DetalleDevolucion");

             
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);


    }
}
