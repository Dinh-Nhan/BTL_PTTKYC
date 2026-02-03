using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backend.Migrations
{
    /// <inheritdoc />
    public partial class InitDatabase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "client",
                columns: table => new
                {
                    client_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    full_name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    phone_number = table.Column<string>(type: "varchar(10)", unicode: false, maxLength: 10, nullable: false),
                    email = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(sysdatetime())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__client__BF21A42480725818", x => x.client_id);
                });

            migrationBuilder.CreateTable(
                name: "room_type",
                columns: table => new
                {
                    room_type_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    type_name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    max_adult = table.Column<int>(type: "int", nullable: false),
                    max_children = table.Column<int>(type: "int", nullable: true, defaultValue: 0),
                    room_area = table.Column<decimal>(type: "decimal(5,2)", nullable: true),
                    allow_pet = table.Column<bool>(type: "bit", nullable: true, defaultValue: false),
                    base_price = table.Column<decimal>(type: "decimal(12,2)", nullable: false),
                    extra_adult_fee = table.Column<decimal>(type: "decimal(12,2)", nullable: true, defaultValue: 0m),
                    extra_child_fee = table.Column<decimal>(type: "decimal(12,2)", nullable: true, defaultValue: 0m),
                    amenities = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true, defaultValue: "Wifi, TV, Điều hòa"),
                    image_url = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: true),
                    is_active = table.Column<bool>(type: "bit", nullable: true, defaultValue: true),
                    created_at = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    updated_at = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__room_typ__42395E84E0522C1C", x => x.room_type_id);
                });

            migrationBuilder.CreateTable(
                name: "user",
                columns: table => new
                {
                    user_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    email = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: false),
                    password_hashing = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: false),
                    full_name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    phone_number = table.Column<string>(type: "varchar(10)", unicode: false, maxLength: 10, nullable: true),
                    gender = table.Column<bool>(type: "bit", nullable: true),
                    date_of_birth = table.Column<DateOnly>(type: "date", nullable: false),
                    role_id = table.Column<bool>(type: "bit", nullable: true, defaultValue: true),
                    is_active = table.Column<bool>(type: "bit", nullable: true, defaultValue: true),
                    created_at = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(sysdatetime())"),
                    updated_at = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__user__B9BE370F80268AA6", x => x.user_id);
                });

            migrationBuilder.CreateTable(
                name: "room",
                columns: table => new
                {
                    room_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    room_type_id = table.Column<int>(type: "int", nullable: false),
                    room_number = table.Column<string>(type: "varchar(10)", unicode: false, maxLength: 10, nullable: false),
                    floor = table.Column<int>(type: "int", nullable: false),
                    building = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    status = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: true, defaultValue: "AVAILABLE"),
                    note = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    is_active = table.Column<bool>(type: "bit", nullable: true, defaultValue: true),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: true, defaultValueSql: "(sysdatetime())"),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__room__19675A8A42D5D51F", x => x.room_id);
                    table.ForeignKey(
                        name: "FK__room__updated_at__5812160E",
                        column: x => x.room_type_id,
                        principalTable: "room_type",
                        principalColumn: "room_type_id");
                });

            migrationBuilder.CreateTable(
                name: "refresh_tokens",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    user_id = table.Column<int>(type: "int", nullable: false),
                    token = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    expires_at = table.Column<DateTime>(type: "datetime", nullable: false),
                    revoked = table.Column<bool>(type: "bit", nullable: true, defaultValue: false),
                    created_at = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__refresh___3213E83F0731EEB3", x => x.id);
                    table.ForeignKey(
                        name: "FK__refresh_t__user___09A971A2",
                        column: x => x.user_id,
                        principalTable: "user",
                        principalColumn: "user_id");
                });

            migrationBuilder.CreateTable(
                name: "booking",
                columns: table => new
                {
                    booking_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    client_id = table.Column<int>(type: "int", nullable: false),
                    room_id = table.Column<int>(type: "int", nullable: false),
                    check_in_datetime = table.Column<DateTime>(type: "datetime", nullable: false),
                    check_out_datetime = table.Column<DateTime>(type: "datetime", nullable: false),
                    number_day = table.Column<int>(type: "int", nullable: false, defaultValue: 2),
                    adult_count = table.Column<int>(type: "int", nullable: false),
                    child_count = table.Column<int>(type: "int", nullable: true, defaultValue: 0),
                    status = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: true, defaultValue: "PENDING"),
                    total_price = table.Column<decimal>(type: "decimal(12,2)", nullable: true),
                    deposit_amount = table.Column<decimal>(type: "decimal(12,2)", nullable: true),
                    payment_status = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: true, defaultValue: "UNPAID"),
                    note = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: true, defaultValueSql: "(sysdatetime())"),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__booking__5DE3A5B14F9DE5E9", x => x.booking_id);
                    table.ForeignKey(
                        name: "FK__booking__room_id__6754599E",
                        column: x => x.room_id,
                        principalTable: "room",
                        principalColumn: "room_id");
                    table.ForeignKey(
                        name: "FK__booking__updated__66603565",
                        column: x => x.client_id,
                        principalTable: "client",
                        principalColumn: "client_id");
                });

            migrationBuilder.CreateTable(
                name: "bill",
                columns: table => new
                {
                    bill_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    booking_id = table.Column<int>(type: "int", nullable: false),
                    user_id = table.Column<int>(type: "int", nullable: false),
                    total_amount = table.Column<decimal>(type: "decimal(12,2)", nullable: false),
                    discount = table.Column<decimal>(type: "decimal(12,2)", nullable: true, defaultValue: 0m),
                    final_amount = table.Column<decimal>(type: "decimal(12,2)", nullable: false),
                    payment_method = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: true, defaultValue: "CASH"),
                    payment_status = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: true, defaultValue: "PENDING"),
                    created_at = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__bill__D706DDB3D3637314", x => x.bill_id);
                    table.ForeignKey(
                        name: "FK__bill__booking_id__73BA3083",
                        column: x => x.booking_id,
                        principalTable: "booking",
                        principalColumn: "booking_id");
                    table.ForeignKey(
                        name: "FK__bill__user_id__74AE54BC",
                        column: x => x.user_id,
                        principalTable: "user",
                        principalColumn: "user_id");
                });

            migrationBuilder.CreateTable(
                name: "bill_detail",
                columns: table => new
                {
                    bill_detail_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    bill_id = table.Column<int>(type: "int", nullable: false),
                    item_name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    item_type = table.Column<string>(type: "varchar(30)", unicode: false, maxLength: 30, nullable: false, defaultValue: "ROOM"),
                    quantity = table.Column<int>(type: "int", nullable: true, defaultValue: 1),
                    unit_price = table.Column<decimal>(type: "decimal(12,2)", nullable: false),
                    total_price = table.Column<decimal>(type: "decimal(12,2)", nullable: false),
                    description = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__bill_det__3BCDAFBB0B4C33B4", x => x.bill_detail_id);
                    table.ForeignKey(
                        name: "fk_bill_detail_bill",
                        column: x => x.bill_id,
                        principalTable: "bill",
                        principalColumn: "bill_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "vnpay_transaction",
                columns: table => new
                {
                    vnpay_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    booking_id = table.Column<int>(type: "int", nullable: false),
                    bill_id = table.Column<int>(type: "int", nullable: true),
                    vnp_TxnRef = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    vnp_TransactionNo = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    vnp_PayDate = table.Column<string>(type: "varchar(14)", unicode: false, maxLength: 14, nullable: false),
                    vnp_Amount = table.Column<long>(type: "bigint", nullable: false),
                    vnp_CurrencyCode = table.Column<string>(type: "varchar(10)", unicode: false, maxLength: 10, nullable: true, defaultValue: "VND"),
                    vnp_ResponseCode = table.Column<string>(type: "varchar(10)", unicode: false, maxLength: 10, nullable: false),
                    vnp_TransactionStatus = table.Column<string>(type: "varchar(10)", unicode: false, maxLength: 10, nullable: false),
                    vnp_Message = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    vnp_BankCode = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: true),
                    vnp_CardType = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: true),
                    vnp_SecureHash = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: false),
                    is_valid_signature = table.Column<bool>(type: "bit", nullable: false),
                    payment_status = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: true, defaultValue: "PENDING"),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: true, defaultValueSql: "(sysdatetime())"),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__vnpay_tr__5AE561718D286A17", x => x.vnpay_id);
                    table.ForeignKey(
                        name: "FK__vnpay_tra__bill___1BC821DD",
                        column: x => x.bill_id,
                        principalTable: "bill",
                        principalColumn: "bill_id");
                    table.ForeignKey(
                        name: "FK__vnpay_tra__booki__1AD3FDA4",
                        column: x => x.booking_id,
                        principalTable: "booking",
                        principalColumn: "booking_id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_bill_booking_id",
                table: "bill",
                column: "booking_id");

            migrationBuilder.CreateIndex(
                name: "IX_bill_user_id",
                table: "bill",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_bill_detail_bill_id",
                table: "bill_detail",
                column: "bill_id");

            migrationBuilder.CreateIndex(
                name: "IX_booking_client_id",
                table: "booking",
                column: "client_id");

            migrationBuilder.CreateIndex(
                name: "IX_booking_room_id",
                table: "booking",
                column: "room_id");

            migrationBuilder.CreateIndex(
                name: "IX_refresh_tokens_user_id",
                table: "refresh_tokens",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_RefreshTokens_Token",
                table: "refresh_tokens",
                column: "token",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_room_room_type_id",
                table: "room",
                column: "room_type_id");

            migrationBuilder.CreateIndex(
                name: "UQ__room__079202B91B28D158",
                table: "room",
                columns: new[] { "room_number", "floor", "building" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UQ__user__A1936A6B9A5DE63A",
                table: "user",
                column: "phone_number",
                unique: true,
                filter: "[phone_number] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "UQ__user__AB6E6164D8D324F4",
                table: "user",
                column: "email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_vnpay_transaction_bill_id",
                table: "vnpay_transaction",
                column: "bill_id");

            migrationBuilder.CreateIndex(
                name: "IX_vnpay_transaction_booking_id",
                table: "vnpay_transaction",
                column: "booking_id");

            migrationBuilder.CreateIndex(
                name: "uq_vnp_txnref",
                table: "vnpay_transaction",
                column: "vnp_TxnRef",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "bill_detail");

            migrationBuilder.DropTable(
                name: "refresh_tokens");

            migrationBuilder.DropTable(
                name: "vnpay_transaction");

            migrationBuilder.DropTable(
                name: "bill");

            migrationBuilder.DropTable(
                name: "booking");

            migrationBuilder.DropTable(
                name: "user");

            migrationBuilder.DropTable(
                name: "room");

            migrationBuilder.DropTable(
                name: "client");

            migrationBuilder.DropTable(
                name: "room_type");
        }
    }
}
