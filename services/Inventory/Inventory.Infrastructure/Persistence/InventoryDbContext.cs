using Microsoft.EntityFrameworkCore;
using Inventory.Domain.Entities;

namespace Inventory.Infrastructure.Persistence;

public class InventoryDbContext : DbContext
{
    public InventoryDbContext(DbContextOptions<InventoryDbContext> options)
        : base(options)
    {
    }

    public DbSet<Product> Products => Set<Product>();
    public DbSet<Warehouse> Warehouses => Set<Warehouse>();
    public DbSet<BinLocation> BinLocations => Set<BinLocation>();
    public DbSet<StockItem> StockItems => Set<StockItem>();
    public DbSet<Supplier> Suppliers => Set<Supplier>();
    public DbSet<PurchaseOrder> PurchaseOrders => Set<PurchaseOrder>();
    public DbSet<PurchaseOrderLine> PurchaseOrderLines => Set<PurchaseOrderLine>();
    public DbSet<StockAdjustment> StockAdjustments => Set<StockAdjustment>();
    public DbSet<InventoryTransaction> InventoryTransactions => Set<InventoryTransaction>();
    public DbSet<CycleCount> CycleCounts => Set<CycleCount>();
    public DbSet<StockTransfer> StockTransfers => Set<StockTransfer>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Product
        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.SKU).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Barcode).HasMaxLength(100);
            entity.Property(e => e.Cost).HasColumnType("decimal(18,2)");
            entity.Property(e => e.Price).HasColumnType("decimal(18,2)");
            entity.HasIndex(e => e.SKU).IsUnique();
            entity.HasIndex(e => e.Barcode);
        });

        // Warehouse
        modelBuilder.Entity<Warehouse>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Code).IsRequired().HasMaxLength(20);
            entity.HasIndex(e => e.Code).IsUnique();
        });

        // BinLocation
        modelBuilder.Entity<BinLocation>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Code).IsRequired().HasMaxLength(50);
            entity.HasOne(e => e.Warehouse)
                .WithMany(w => w.BinLocations)
                .HasForeignKey(e => e.WarehouseId)
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasIndex(e => new { e.WarehouseId, e.Code }).IsUnique();
        });

        // StockItem
        modelBuilder.Entity<StockItem>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.AverageCost).HasColumnType("decimal(18,2)");
            entity.HasOne(e => e.Product)
                .WithMany(p => p.StockItems)
                .HasForeignKey(e => e.ProductId)
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(e => e.Warehouse)
                .WithMany(w => w.StockItems)
                .HasForeignKey(e => e.WarehouseId)
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(e => e.BinLocation)
                .WithMany(b => b.StockItems)
                .HasForeignKey(e => e.BinLocationId)
                .OnDelete(DeleteBehavior.SetNull);
            entity.HasIndex(e => new { e.ProductId, e.WarehouseId, e.BinLocationId }).IsUnique();
        });

        // Supplier
        modelBuilder.Entity<Supplier>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Code).HasMaxLength(50);
            entity.Property(e => e.CreditLimit).HasColumnType("decimal(18,2)");
            entity.HasIndex(e => e.Code).IsUnique();
        });

        // PurchaseOrder
        modelBuilder.Entity<PurchaseOrder>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.OrderNumber).IsRequired().HasMaxLength(50);
            entity.Property(e => e.SubTotal).HasColumnType("decimal(18,2)");
            entity.Property(e => e.TaxAmount).HasColumnType("decimal(18,2)");
            entity.Property(e => e.ShippingCost).HasColumnType("decimal(18,2)");
            entity.Property(e => e.Total).HasColumnType("decimal(18,2)");
            entity.HasOne(e => e.Supplier)
                .WithMany(s => s.PurchaseOrders)
                .HasForeignKey(e => e.SupplierId)
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(e => e.Warehouse)
                .WithMany()
                .HasForeignKey(e => e.WarehouseId)
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasIndex(e => e.OrderNumber).IsUnique();
        });

        // PurchaseOrderLine
        modelBuilder.Entity<PurchaseOrderLine>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.UnitPrice).HasColumnType("decimal(18,2)");
            entity.Property(e => e.TaxRate).HasColumnType("decimal(5,4)");
            entity.HasOne(e => e.PurchaseOrder)
                .WithMany(po => po.Lines)
                .HasForeignKey(e => e.PurchaseOrderId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(e => e.Product)
                .WithMany()
                .HasForeignKey(e => e.ProductId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // StockAdjustment
        modelBuilder.Entity<StockAdjustment>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.AdjustmentNumber).IsRequired().HasMaxLength(50);
            entity.Property(e => e.UnitCost).HasColumnType("decimal(18,2)");
            entity.HasOne(e => e.Product)
                .WithMany()
                .HasForeignKey(e => e.ProductId)
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(e => e.Warehouse)
                .WithMany()
                .HasForeignKey(e => e.WarehouseId)
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(e => e.BinLocation)
                .WithMany()
                .HasForeignKey(e => e.BinLocationId)
                .OnDelete(DeleteBehavior.SetNull);
            entity.HasIndex(e => e.AdjustmentNumber).IsUnique();
        });

        // InventoryTransaction
        modelBuilder.Entity<InventoryTransaction>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.UnitCost).HasColumnType("decimal(18,2)");
            entity.HasOne(e => e.Product)
                .WithMany(p => p.Transactions)
                .HasForeignKey(e => e.ProductId)
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(e => e.Warehouse)
                .WithMany()
                .HasForeignKey(e => e.WarehouseId)
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(e => e.BinLocation)
                .WithMany()
                .HasForeignKey(e => e.BinLocationId)
                .OnDelete(DeleteBehavior.SetNull);
            entity.HasIndex(e => e.TransactionDate);
            entity.HasIndex(e => new { e.ProductId, e.WarehouseId });
        });

        // CycleCount
        modelBuilder.Entity<CycleCount>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.CountNumber).IsRequired().HasMaxLength(50);
            entity.HasOne(e => e.Product)
                .WithMany()
                .HasForeignKey(e => e.ProductId)
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(e => e.Warehouse)
                .WithMany()
                .HasForeignKey(e => e.WarehouseId)
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(e => e.BinLocation)
                .WithMany()
                .HasForeignKey(e => e.BinLocationId)
                .OnDelete(DeleteBehavior.SetNull);
            entity.HasIndex(e => e.CountNumber).IsUnique();
        });

        // StockTransfer
        modelBuilder.Entity<StockTransfer>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.TransferNumber).IsRequired().HasMaxLength(50);
            entity.HasOne(e => e.Product)
                .WithMany()
                .HasForeignKey(e => e.ProductId)
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(e => e.FromWarehouse)
                .WithMany()
                .HasForeignKey(e => e.FromWarehouseId)
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(e => e.ToWarehouse)
                .WithMany()
                .HasForeignKey(e => e.ToWarehouseId)
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasIndex(e => e.TransferNumber).IsUnique();
        });
    }
}
