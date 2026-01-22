using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using backend.Models;

namespace backend.Data;

public partial class AppDbContext : DbContext
{
    public AppDbContext()
    {
    }

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Bill> Bills { get; set; }

    public virtual DbSet<BillDetail> BillDetails { get; set; }

    public virtual DbSet<Booking> Bookings { get; set; }

    public virtual DbSet<Client> Clients { get; set; }

    public virtual DbSet<RefreshToken> RefreshTokens { get; set; }

    public virtual DbSet<Room> Rooms { get; set; }

    public virtual DbSet<RoomType> RoomTypes { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<VnpayTransaction> VnpayTransactions { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Bill>(entity =>
        {
            entity.HasKey(e => e.BillId).HasName("PK__bill__D706DDB3D3637314");

            entity.ToTable("bill");

            entity.Property(e => e.BillId).HasColumnName("bill_id");
            entity.Property(e => e.BookingId).HasColumnName("booking_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.Discount)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(12, 2)")
                .HasColumnName("discount");
            entity.Property(e => e.FinalAmount)
                .HasColumnType("decimal(12, 2)")
                .HasColumnName("final_amount");
            entity.Property(e => e.PaymentMethod)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasDefaultValue("CASH")
                .HasColumnName("payment_method");
            entity.Property(e => e.PaymentStatus)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasDefaultValue("PENDING")
                .HasColumnName("payment_status");
            entity.Property(e => e.TotalAmount)
                .HasColumnType("decimal(12, 2)")
                .HasColumnName("total_amount");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Booking).WithMany(p => p.Bills)
                .HasForeignKey(d => d.BookingId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__bill__booking_id__73BA3083");

            entity.HasOne(d => d.User).WithMany(p => p.Bills)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__bill__user_id__74AE54BC");
        });

        modelBuilder.Entity<BillDetail>(entity =>
        {
            entity.HasKey(e => e.BillDetailId).HasName("PK__bill_det__3BCDAFBB0B4C33B4");

            entity.ToTable("bill_detail");

            entity.Property(e => e.BillDetailId).HasColumnName("bill_detail_id");
            entity.Property(e => e.BillId).HasColumnName("bill_id");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.ItemName)
                .HasMaxLength(100)
                .HasColumnName("item_name");
            entity.Property(e => e.ItemType)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasDefaultValue("ROOM")
                .HasColumnName("item_type");
            entity.Property(e => e.Quantity)
                .HasDefaultValue(1)
                .HasColumnName("quantity");
            entity.Property(e => e.TotalPrice)
                .HasColumnType("decimal(12, 2)")
                .HasColumnName("total_price");
            entity.Property(e => e.UnitPrice)
                .HasColumnType("decimal(12, 2)")
                .HasColumnName("unit_price");

            entity.HasOne(d => d.Bill).WithMany(p => p.BillDetails)
                .HasForeignKey(d => d.BillId)
                .HasConstraintName("fk_bill_detail_bill");
        });

        modelBuilder.Entity<Booking>(entity =>
        {
            entity.HasKey(e => e.BookingId).HasName("PK__booking__5DE3A5B14F9DE5E9");

            entity.ToTable("booking");

            entity.Property(e => e.BookingId).HasColumnName("booking_id");
            entity.Property(e => e.AdultCount).HasColumnName("adult_count");
            entity.Property(e => e.CheckInDatetime)
                .HasColumnType("datetime")
                .HasColumnName("check_in_datetime");
            entity.Property(e => e.CheckOutDatetime)
                .HasColumnType("datetime")
                .HasColumnName("check_out_datetime");
            entity.Property(e => e.ChildCount)
                .HasDefaultValue(0)
                .HasColumnName("child_count");
            entity.Property(e => e.ClientId).HasColumnName("client_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(sysdatetime())")
                .HasColumnName("created_at");
            entity.Property(e => e.DepositAmount)
                .HasColumnType("decimal(12, 2)")
                .HasColumnName("deposit_amount");
            entity.Property(e => e.Note).HasColumnName("note");
            entity.Property(e => e.NumberDay)
                .HasDefaultValue(2)
                .HasColumnName("number_day");
            entity.Property(e => e.PaymentStatus)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasDefaultValue("UNPAID")
                .HasColumnName("payment_status");
            entity.Property(e => e.RoomId).HasColumnName("room_id");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasDefaultValue("PENDING")
                .HasColumnName("status");
            entity.Property(e => e.TotalPrice)
                .HasColumnType("decimal(12, 2)")
                .HasColumnName("total_price");
            entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");

            entity.HasOne(d => d.Client).WithMany(p => p.Bookings)
                .HasForeignKey(d => d.ClientId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__booking__updated__66603565");

            entity.HasOne(d => d.Room).WithMany(p => p.Bookings)
                .HasForeignKey(d => d.RoomId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__booking__room_id__6754599E");
        });

        modelBuilder.Entity<Client>(entity =>
        {
            entity.HasKey(e => e.ClientId).HasName("PK__client__BF21A42480725818");

            entity.ToTable("client");

            entity.Property(e => e.ClientId).HasColumnName("client_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(sysdatetime())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.Email)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("email");
            entity.Property(e => e.FullName)
                .HasMaxLength(100)
                .HasColumnName("full_name");
            entity.Property(e => e.PhoneNumber)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("phone_number");
        });

        modelBuilder.Entity<RefreshToken>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__refresh___3213E83F0731EEB3");

            entity.ToTable("refresh_tokens");

            entity.HasIndex(e => e.Token, "IX_RefreshTokens_Token").IsUnique();

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.ExpiresAt)
                .HasColumnType("datetime")
                .HasColumnName("expires_at");
            entity.Property(e => e.Revoked)
                .HasDefaultValue(false)
                .HasColumnName("revoked");
            entity.Property(e => e.Token)
                .HasMaxLength(500)
                .HasColumnName("token");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.User).WithMany(p => p.RefreshTokens)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__refresh_t__user___09A971A2");
        });

        modelBuilder.Entity<Room>(entity =>
        {
            entity.HasKey(e => e.RoomId).HasName("PK__room__19675A8A42D5D51F");

            entity.ToTable("room");

            entity.HasIndex(e => new { e.RoomNumber, e.Floor, e.Building }, "UQ__room__079202B91B28D158").IsUnique();

            entity.Property(e => e.RoomId).HasColumnName("room_id");
            entity.Property(e => e.Building)
                .HasMaxLength(20)
                .HasColumnName("building");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(sysdatetime())")
                .HasColumnName("created_at");
            entity.Property(e => e.Floor).HasColumnName("floor");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("is_active");
            entity.Property(e => e.Note).HasColumnName("note");
            entity.Property(e => e.RoomNumber)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("room_number");
            entity.Property(e => e.RoomTypeId).HasColumnName("room_type_id");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasDefaultValue("AVAILABLE")
                .HasColumnName("status");
            entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");

            entity.HasOne(d => d.RoomType).WithMany(p => p.Rooms)
                .HasForeignKey(d => d.RoomTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__room__updated_at__5812160E");
        });

        modelBuilder.Entity<RoomType>(entity =>
        {
            entity.HasKey(e => e.RoomTypeId).HasName("PK__room_typ__42395E84E0522C1C");

            entity.ToTable("room_type");

            entity.Property(e => e.RoomTypeId).HasColumnName("room_type_id");
            entity.Property(e => e.AllowPet)
                .HasDefaultValue(false)
                .HasColumnName("allow_pet");
            entity.Property(e => e.Amenities)
                .HasMaxLength(500)
                .HasDefaultValue("Wifi, TV, Điều hòa")
                .HasColumnName("amenities");
            entity.Property(e => e.BasePrice)
                .HasColumnType("decimal(12, 2)")
                .HasColumnName("base_price");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.ExtraAdultFee)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(12, 2)")
                .HasColumnName("extra_adult_fee");
            entity.Property(e => e.ExtraChildFee)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(12, 2)")
                .HasColumnName("extra_child_fee");
            entity.Property(e => e.ImageUrl)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("image_url");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("is_active");
            entity.Property(e => e.MaxAdult).HasColumnName("max_adult");
            entity.Property(e => e.MaxChildren)
                .HasDefaultValue(0)
                .HasColumnName("max_children");
            entity.Property(e => e.RoomArea)
                .HasColumnType("decimal(5, 2)")
                .HasColumnName("room_area");
            entity.Property(e => e.TypeName)
                .HasMaxLength(100)
                .HasColumnName("type_name");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("datetime")
                .HasColumnName("updated_at");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__user__B9BE370F80268AA6");

            entity.ToTable("user");

            entity.HasIndex(e => e.PhoneNumber, "UQ__user__A1936A6B9A5DE63A").IsUnique();

            entity.HasIndex(e => e.Email, "UQ__user__AB6E6164D8D324F4").IsUnique();

            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(sysdatetime())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.DateOfBirth).HasColumnName("date_of_birth");
            entity.Property(e => e.Email)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("email");
            entity.Property(e => e.FullName)
                .HasMaxLength(255)
                .HasColumnName("full_name");
            entity.Property(e => e.Gender).HasColumnName("gender");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("is_active");
            entity.Property(e => e.PasswordHashing)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("password_hashing");
            entity.Property(e => e.PhoneNumber)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("phone_number");
            entity.Property(e => e.RoleId)
                .HasDefaultValue(true)
                .HasColumnName("role_id");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("datetime")
                .HasColumnName("updated_at");
        });

        modelBuilder.Entity<VnpayTransaction>(entity =>
        {
            entity.HasKey(e => e.VnpayId).HasName("PK__vnpay_tr__5AE561718D286A17");

            entity.ToTable("vnpay_transaction");

            entity.HasIndex(e => e.VnpTxnRef, "uq_vnp_txnref").IsUnique();

            entity.Property(e => e.VnpayId).HasColumnName("vnpay_id");
            entity.Property(e => e.BillId).HasColumnName("bill_id");
            entity.Property(e => e.BookingId).HasColumnName("booking_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(sysdatetime())")
                .HasColumnName("created_at");
            entity.Property(e => e.IsValidSignature).HasColumnName("is_valid_signature");
            entity.Property(e => e.PaymentStatus)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasDefaultValue("PENDING")
                .HasColumnName("payment_status");
            entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");
            entity.Property(e => e.VnpAmount).HasColumnName("vnp_Amount");
            entity.Property(e => e.VnpBankCode)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("vnp_BankCode");
            entity.Property(e => e.VnpCardType)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("vnp_CardType");
            entity.Property(e => e.VnpCurrencyCode)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasDefaultValue("VND")
                .HasColumnName("vnp_CurrencyCode");
            entity.Property(e => e.VnpMessage)
                .HasMaxLength(255)
                .HasColumnName("vnp_Message");
            entity.Property(e => e.VnpPayDate)
                .HasMaxLength(14)
                .IsUnicode(false)
                .HasColumnName("vnp_PayDate");
            entity.Property(e => e.VnpResponseCode)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("vnp_ResponseCode");
            entity.Property(e => e.VnpSecureHash)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("vnp_SecureHash");
            entity.Property(e => e.VnpTransactionNo)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("vnp_TransactionNo");
            entity.Property(e => e.VnpTransactionStatus)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("vnp_TransactionStatus");
            entity.Property(e => e.VnpTxnRef)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("vnp_TxnRef");

            entity.HasOne(d => d.Bill).WithMany(p => p.VnpayTransactions)
                .HasForeignKey(d => d.BillId)
                .HasConstraintName("FK__vnpay_tra__bill___1BC821DD");

            entity.HasOne(d => d.Booking).WithMany(p => p.VnpayTransactions)
                .HasForeignKey(d => d.BookingId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__vnpay_tra__booki__1AD3FDA4");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
