using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace GarageLog.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "attachments",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    file_name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    mime_type = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    size_bytes = table.Column<long>(type: "bigint", nullable: false),
                    storage_key = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: true),
                    content_hash = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    activated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    deleted_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    deleted_reason = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_attachments", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "maintenance_records",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    vehicle_id = table.Column<Guid>(type: "uuid", nullable: false),
                    task_type_id = table.Column<Guid>(type: "uuid", nullable: false),
                    task_type_name = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    performed_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    odometer_km = table.Column<int>(type: "integer", nullable: false),
                    odometer_taken_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    odometer_source = table.Column<int>(type: "integer", nullable: false),
                    workshop_id = table.Column<Guid>(type: "uuid", nullable: true),
                    workshop_name = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    description = table.Column<string>(type: "text", nullable: true),
                    labor_cost_amount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    labor_cost_currency = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_maintenance_records", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "outbox_messages",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    type = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    payload = table.Column<string>(type: "text", nullable: false),
                    occurred_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    processed_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    attempts = table.Column<int>(type: "integer", nullable: false),
                    last_error = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_outbox_messages", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "parts",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    oem_number = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    brand = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    category = table.Column<int>(type: "integer", nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_parts", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "planned_maintenance",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    vehicle_id = table.Column<Guid>(type: "uuid", nullable: false),
                    task_type_id = table.Column<Guid>(type: "uuid", nullable: false),
                    task_type_name = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    trigger = table.Column<string>(type: "jsonb", nullable: false),
                    last_performed_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    last_odometer = table.Column<int>(type: "integer", nullable: true),
                    priority = table.Column<int>(type: "integer", nullable: false),
                    estimated_cost_amount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: true),
                    estimated_cost_currency = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: true),
                    status = table.Column<int>(type: "integer", nullable: false),
                    snoozed_until = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    linked_record_id = table.Column<Guid>(type: "uuid", nullable: true),
                    notes = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_planned_maintenance", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "planned_purchases",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    vehicle_id = table.Column<Guid>(type: "uuid", nullable: true),
                    part_id = table.Column<Guid>(type: "uuid", nullable: true),
                    name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    quantity = table.Column<decimal>(type: "numeric(18,3)", precision: 18, scale: 3, nullable: false),
                    unit = table.Column<int>(type: "integer", nullable: false),
                    estimated_price_amount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: true),
                    estimated_price_currency = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: true),
                    planned_for_date = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    priority = table.Column<int>(type: "integer", nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false),
                    linked_purchase_id = table.Column<Guid>(type: "uuid", nullable: true),
                    notes = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_planned_purchases", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "purchases",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    vehicle_id = table.Column<Guid>(type: "uuid", nullable: true),
                    purchased_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    vendor = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    total_amount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    total_currency = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false),
                    receipt_attachment_id = table.Column<Guid>(type: "uuid", nullable: true),
                    receipt_caption = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    notes = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    status = table.Column<int>(type: "integer", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_purchases", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "task_types",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    category = table.Column<int>(type: "integer", nullable: false),
                    default_interval_km = table.Column<int>(type: "integer", nullable: true),
                    default_interval_months = table.Column<int>(type: "integer", nullable: true),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_task_types", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "vehicles",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    owner_id = table.Column<Guid>(type: "uuid", nullable: false),
                    vin = table.Column<string>(type: "character varying(17)", maxLength: 17, nullable: false),
                    license_plate = table.Column<string>(type: "character varying(12)", maxLength: 12, nullable: true),
                    make = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    model = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    year = table.Column<int>(type: "integer", nullable: false),
                    engine = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    transmission = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    status = table.Column<int>(type: "integer", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    sold_at = table.Column<DateOnly>(type: "date", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_vehicles", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "workshops",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    address = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: true),
                    phone = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: true),
                    website = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_workshops", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "record_attachments",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    attachment_id = table.Column<Guid>(type: "uuid", nullable: false),
                    caption = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    record_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_record_attachments", x => x.id);
                    table.ForeignKey(
                        name: "fk_record_attachments_maintenance_records_record_id",
                        column: x => x.record_id,
                        principalTable: "maintenance_records",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "record_parts_used",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    part_id = table.Column<Guid>(type: "uuid", nullable: true),
                    name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    quantity = table.Column<decimal>(type: "numeric(18,3)", precision: 18, scale: 3, nullable: false),
                    unit = table.Column<int>(type: "integer", nullable: false),
                    record_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_record_parts_used", x => x.id);
                    table.ForeignKey(
                        name: "fk_record_parts_used_maintenance_records_record_id",
                        column: x => x.record_id,
                        principalTable: "maintenance_records",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "record_purchases",
                columns: table => new
                {
                    purchase_id = table.Column<Guid>(type: "uuid", nullable: false),
                    record_id = table.Column<Guid>(type: "uuid", nullable: false),
                    linked_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_record_purchases", x => new { x.record_id, x.purchase_id });
                    table.ForeignKey(
                        name: "fk_record_purchases_maintenance_records_record_id",
                        column: x => x.record_id,
                        principalTable: "maintenance_records",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "planned_maintenance_reminders",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    remind_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    channel = table.Column<int>(type: "integer", nullable: false),
                    sent = table.Column<bool>(type: "boolean", nullable: false),
                    sent_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    planned_id = table.Column<Guid>(type: "uuid", nullable: false),
                    reminder_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_planned_maintenance_reminders", x => x.id);
                    table.ForeignKey(
                        name: "fk_planned_maintenance_reminders_planned_maintenance_planned_id",
                        column: x => x.planned_id,
                        principalTable: "planned_maintenance",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "purchase_lines",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    part_id = table.Column<Guid>(type: "uuid", nullable: true),
                    name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    quantity = table.Column<decimal>(type: "numeric(18,3)", precision: 18, scale: 3, nullable: false),
                    unit = table.Column<int>(type: "integer", nullable: false),
                    unit_price_amount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    unit_price_currency = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false),
                    purchase_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_purchase_lines", x => x.id);
                    table.ForeignKey(
                        name: "fk_purchase_lines_purchases_purchase_id",
                        column: x => x.purchase_id,
                        principalTable: "purchases",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "odometer_readings",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    kilometers = table.Column<int>(type: "integer", nullable: false),
                    taken_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    source = table.Column<int>(type: "integer", nullable: false),
                    vehicle_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_odometer_readings", x => x.id);
                    table.ForeignKey(
                        name: "fk_odometer_readings_vehicles_vehicle_id",
                        column: x => x.vehicle_id,
                        principalTable: "vehicles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_attachments_content_hash",
                table: "attachments",
                column: "content_hash");

            migrationBuilder.CreateIndex(
                name: "ix_attachments_pending_created_at",
                table: "attachments",
                column: "created_at",
                filter: "status = 1");

            migrationBuilder.CreateIndex(
                name: "ix_attachments_status",
                table: "attachments",
                column: "status");

            migrationBuilder.CreateIndex(
                name: "ix_maintenance_records_vehicle_id",
                table: "maintenance_records",
                column: "vehicle_id");

            migrationBuilder.CreateIndex(
                name: "ix_maintenance_records_vehicle_id_performed_at",
                table: "maintenance_records",
                columns: new[] { "vehicle_id", "performed_at" });

            migrationBuilder.CreateIndex(
                name: "ix_odometer_readings_vehicle_id_taken_at",
                table: "odometer_readings",
                columns: new[] { "vehicle_id", "taken_at" });

            migrationBuilder.CreateIndex(
                name: "ix_outbox_pending",
                table: "outbox_messages",
                columns: new[] { "processed_at", "occurred_at" },
                filter: "processed_at IS NULL");

            migrationBuilder.CreateIndex(
                name: "ix_parts_name",
                table: "parts",
                column: "name");

            migrationBuilder.CreateIndex(
                name: "ix_parts_oem_number",
                table: "parts",
                column: "oem_number",
                filter: "oem_number IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "ix_planned_maintenance_status",
                table: "planned_maintenance",
                column: "status");

            migrationBuilder.CreateIndex(
                name: "ix_planned_maintenance_status_priority",
                table: "planned_maintenance",
                columns: new[] { "status", "priority" });

            migrationBuilder.CreateIndex(
                name: "ix_planned_maintenance_vehicle_id",
                table: "planned_maintenance",
                column: "vehicle_id");

            migrationBuilder.CreateIndex(
                name: "ix_planned_maintenance_reminders_planned_id",
                table: "planned_maintenance_reminders",
                column: "planned_id");

            migrationBuilder.CreateIndex(
                name: "ix_planned_purchases_priority",
                table: "planned_purchases",
                column: "priority");

            migrationBuilder.CreateIndex(
                name: "ix_planned_purchases_status",
                table: "planned_purchases",
                column: "status");

            migrationBuilder.CreateIndex(
                name: "ix_planned_purchases_status_planned_for_date",
                table: "planned_purchases",
                columns: new[] { "status", "planned_for_date" });

            migrationBuilder.CreateIndex(
                name: "ix_planned_purchases_vehicle_id",
                table: "planned_purchases",
                column: "vehicle_id");

            migrationBuilder.CreateIndex(
                name: "ix_purchase_lines_purchase_id",
                table: "purchase_lines",
                column: "purchase_id");

            migrationBuilder.CreateIndex(
                name: "ix_purchases_purchased_at",
                table: "purchases",
                column: "purchased_at");

            migrationBuilder.CreateIndex(
                name: "ix_purchases_status",
                table: "purchases",
                column: "status");

            migrationBuilder.CreateIndex(
                name: "ix_purchases_vehicle_id",
                table: "purchases",
                column: "vehicle_id");

            migrationBuilder.CreateIndex(
                name: "ix_record_attachments_record_id_attachment_id",
                table: "record_attachments",
                columns: new[] { "record_id", "attachment_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_record_parts_used_record_id",
                table: "record_parts_used",
                column: "record_id");

            migrationBuilder.CreateIndex(
                name: "ix_record_purchases_purchase_id",
                table: "record_purchases",
                column: "purchase_id");

            migrationBuilder.CreateIndex(
                name: "ux_task_types_name_active",
                table: "task_types",
                column: "name",
                unique: true,
                filter: "is_active = true");

            migrationBuilder.CreateIndex(
                name: "ix_vehicles_owner_id",
                table: "vehicles",
                column: "owner_id");

            migrationBuilder.CreateIndex(
                name: "ix_vehicles_owner_id_vin",
                table: "vehicles",
                columns: new[] { "owner_id", "vin" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ux_workshops_name_active",
                table: "workshops",
                column: "name",
                unique: true,
                filter: "is_active = true");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "attachments");

            migrationBuilder.DropTable(
                name: "odometer_readings");

            migrationBuilder.DropTable(
                name: "outbox_messages");

            migrationBuilder.DropTable(
                name: "parts");

            migrationBuilder.DropTable(
                name: "planned_maintenance_reminders");

            migrationBuilder.DropTable(
                name: "planned_purchases");

            migrationBuilder.DropTable(
                name: "purchase_lines");

            migrationBuilder.DropTable(
                name: "record_attachments");

            migrationBuilder.DropTable(
                name: "record_parts_used");

            migrationBuilder.DropTable(
                name: "record_purchases");

            migrationBuilder.DropTable(
                name: "task_types");

            migrationBuilder.DropTable(
                name: "workshops");

            migrationBuilder.DropTable(
                name: "vehicles");

            migrationBuilder.DropTable(
                name: "planned_maintenance");

            migrationBuilder.DropTable(
                name: "purchases");

            migrationBuilder.DropTable(
                name: "maintenance_records");
        }
    }
}
