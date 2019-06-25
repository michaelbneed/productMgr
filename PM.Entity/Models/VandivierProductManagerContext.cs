using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace PM.Entity.Models
{
    public partial class VandivierProductManagerContext : DbContext
    {
        public VandivierProductManagerContext()
        {
        }

        public VandivierProductManagerContext(DbContextOptions<VandivierProductManagerContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Category> Category { get; set; }
        public virtual DbSet<Note> Note { get; set; }
        public virtual DbSet<Product> Product { get; set; }
        public virtual DbSet<ProductPackageType> ProductPackageType { get; set; }
        public virtual DbSet<ProductStoreSpecific> ProductStoreSpecific { get; set; }
        public virtual DbSet<Request> Request { get; set; }
        public virtual DbSet<RequestType> RequestType { get; set; }
        public virtual DbSet<StatusType> StatusType { get; set; }
        public virtual DbSet<Supplier> Supplier { get; set; }
        public virtual DbSet<User> User { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
				optionsBuilder.UseSqlServer("Server=user-pc;Database=VandivierProductManager;Trusted_Connection=True;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("ProductVersion", "2.2.4-servicing-10062");

            modelBuilder.Entity<Category>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.CategoryName)
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Note>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.CreatedOn).HasColumnType("datetime");

                entity.Property(e => e.NoteText).IsRequired();

                entity.Property(e => e.RequestId).HasColumnName("RequestID");

                entity.Property(e => e.UpdatedBy)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.UpdatedOn).HasColumnType("datetime");

                entity.HasOne(d => d.Request)
                    .WithMany(p => p.Note)
                    .HasForeignKey(d => d.RequestId)
                    .HasConstraintName("FK_Notes_Request");
            });

            modelBuilder.Entity<Product>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.CategoryId).HasColumnName("CategoryID");

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.CreatedOn).HasColumnType("datetime");

                entity.Property(e => e.OrderWeek)
                    .HasMaxLength(5)
                    .IsUnicode(false);

                entity.Property(e => e.PackageSize)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.PackageType)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.ProductCost).HasColumnType("decimal(18, 0)");

                entity.Property(e => e.ProductLocation)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.ProductName)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.ProductPrice).HasColumnType("decimal(18, 0)");

                entity.Property(e => e.Upccode)
                    .HasColumnName("UPCCode")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.UpdatedBy)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.UpdatedOn).HasColumnType("datetime");

                entity.HasOne(d => d.Category)
                    .WithMany(p => p.Product)
                    .HasForeignKey(d => d.CategoryId)
                    .HasConstraintName("FK_Product_Category");
            });

            modelBuilder.Entity<ProductPackageType>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.AlternateProductCost).HasColumnType("decimal(18, 0)");

                entity.Property(e => e.AlternateProductName)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.AlternateProductPrice).HasColumnType("decimal(18, 0)");

                entity.Property(e => e.AlternateProductUpccode)
                    .HasColumnName("AlternateProductUPCCode")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.CreatedOn).HasColumnType("datetime");

                entity.Property(e => e.ProductId).HasColumnName("ProductID");

                entity.Property(e => e.Quantity).HasColumnType("decimal(18, 0)");

                entity.Property(e => e.SupplierData)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.SupplierId).HasColumnName("SupplierID");

                entity.Property(e => e.Unit)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.UpdatedBy)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.UpdatedOn).HasColumnType("datetime");

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.ProductPackageType)
                    .HasForeignKey(d => d.ProductId)
                    .HasConstraintName("FK_ProductPackageType_Product");

                entity.HasOne(d => d.Supplier)
                    .WithMany(p => p.ProductPackageType)
                    .HasForeignKey(d => d.SupplierId)
                    .HasConstraintName("FK_ProductPackageType_Supplier");
            });

            modelBuilder.Entity<ProductStoreSpecific>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.CreatedOn).HasColumnType("datetime");

                entity.Property(e => e.PackageTypeId).HasColumnName("PackageTypeID");

                entity.Property(e => e.ProductId).HasColumnName("ProductID");

                entity.Property(e => e.StoreCost).HasColumnType("decimal(18, 0)");

                entity.Property(e => e.StoreName)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.StorePrice).HasColumnType("decimal(18, 0)");

				entity.Property(e => e.UpdatedBy)
					.HasMaxLength(100)
					.IsUnicode(false);

				entity.Property(e => e.UpdatedOn).HasColumnType("datetime");

                entity.HasOne(d => d.PackageType)
                    .WithMany(p => p.ProductStoreSpecific)
                    .HasForeignKey(d => d.PackageTypeId)
                    .HasConstraintName("FK_ProductStoreSpecific_ProductPackageType");

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.ProductStoreSpecific)
                    .HasForeignKey(d => d.ProductId)
                    .HasConstraintName("FK_ProductStoreSpecific_Product");
            });

            modelBuilder.Entity<Request>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.CreatedOn).HasColumnType("datetime");

                entity.Property(e => e.ProductId).HasColumnName("ProductID");

                entity.Property(e => e.RequestDescription).IsRequired();

                entity.Property(e => e.RequestTypeId).HasColumnName("RequestTypeID");

                entity.Property(e => e.StatusTypeId).HasColumnName("StatusTypeID");

                entity.Property(e => e.SupplierId).HasColumnName("SupplierID");

                entity.Property(e => e.UpdatedBy)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.UpdatedOn).HasColumnType("datetime");

                entity.Property(e => e.UserId).HasColumnName("UserID");

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.Request)
                    .HasForeignKey(d => d.ProductId)
                    .HasConstraintName("FK_Request_Product");

                entity.HasOne(d => d.RequestType)
                    .WithMany(p => p.Request)
                    .HasForeignKey(d => d.RequestTypeId)
                    .HasConstraintName("FK_Request_RequestTypes");

                entity.HasOne(d => d.StatusType)
                    .WithMany(p => p.Request)
                    .HasForeignKey(d => d.StatusTypeId)
                    .HasConstraintName("FK_Request_StatusTypes");

                entity.HasOne(d => d.Supplier)
                    .WithMany(p => p.Request)
                    .HasForeignKey(d => d.SupplierId)
                    .HasConstraintName("FK_Request_Supplier");
            });

            modelBuilder.Entity<RequestType>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.RequestTypeName).HasMaxLength(100);
            });

            modelBuilder.Entity<StatusType>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.StatusTypeName)
                    .HasMaxLength(100)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Supplier>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.CreatedOn).HasColumnType("datetime");

                entity.Property(e => e.SupplierName)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.UpdatedBy)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.UpdatedOn).HasColumnType("datetime");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.AuthId)
                    .HasColumnName("AuthID")
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.CreatedOn).HasColumnType("datetime");

                entity.Property(e => e.EmailAddress)
                    .IsRequired()
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.FirstName)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.LastName)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.SupplierId).HasColumnName("SupplierID");

                entity.Property(e => e.UpdatedBy)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.UpdatedOn).HasColumnType("datetime");

                entity.HasOne(d => d.Supplier)
                    .WithMany(p => p.User)
                    .HasForeignKey(d => d.SupplierId)
                    .HasConstraintName("FK_User_Supplier");
            });
        }
    }
}
