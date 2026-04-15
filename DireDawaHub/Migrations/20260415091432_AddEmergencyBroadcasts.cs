using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DireDawaHub.Migrations
{
    /// <inheritdoc />
    public partial class AddEmergencyBroadcasts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "WaterSchedules",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                table: "WaterSchedules",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Zone",
                table: "WaterSchedules",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AdminComment",
                table: "JobPostings",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ContributorId",
                table: "JobPostings",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsApproved",
                table: "JobPostings",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "JobType",
                table: "JobPostings",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Salary",
                table: "JobPostings",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ScheduledPublishAt",
                table: "JobPostings",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ContactPhone",
                table: "ClinicRecords",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Location",
                table: "ClinicRecords",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Services",
                table: "ClinicRecords",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                table: "ClinicRecords",
                type: "TEXT",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ContentVersions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    EntityType = table.Column<string>(type: "TEXT", nullable: false),
                    EntityId = table.Column<int>(type: "INTEGER", nullable: false),
                    Title = table.Column<string>(type: "TEXT", nullable: true),
                    Content = table.Column<string>(type: "TEXT", nullable: true),
                    SerializedData = table.Column<string>(type: "TEXT", nullable: true),
                    Action = table.Column<string>(type: "TEXT", nullable: false),
                    ChangedBy = table.Column<string>(type: "TEXT", nullable: true),
                    ChangedByName = table.Column<string>(type: "TEXT", nullable: true),
                    ChangedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ChangeSummary = table.Column<string>(type: "TEXT", nullable: true),
                    PreviousVersionId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContentVersions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EmergencyBroadcasts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Message = table.Column<string>(type: "TEXT", nullable: false),
                    SentAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    SentBy = table.Column<string>(type: "TEXT", nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    Severity = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmergencyBroadcasts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "IdDocumentImages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UserId = table.Column<string>(type: "TEXT", nullable: false),
                    FileName = table.Column<string>(type: "TEXT", nullable: false),
                    FilePath = table.Column<string>(type: "TEXT", nullable: false),
                    FileSize = table.Column<long>(type: "INTEGER", nullable: false),
                    ContentType = table.Column<string>(type: "TEXT", nullable: true),
                    UploadedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    IsVisuallyVerified = table.Column<bool>(type: "INTEGER", nullable: false),
                    VisualVerificationNotes = table.Column<string>(type: "TEXT", nullable: true),
                    VerifiedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    VerifiedBy = table.Column<string>(type: "TEXT", nullable: true),
                    SelfieFilePath = table.Column<string>(type: "TEXT", nullable: true),
                    FaceMatchCompleted = table.Column<bool>(type: "INTEGER", nullable: false),
                    FaceMatchConfidence = table.Column<double>(type: "REAL", nullable: true),
                    ExtractedIdNumber = table.Column<string>(type: "TEXT", nullable: true),
                    ExtractedFullName = table.Column<string>(type: "TEXT", nullable: true),
                    ExtractedDepartment = table.Column<string>(type: "TEXT", nullable: true),
                    DocumentType = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IdDocumentImages", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SecurityAuditLogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Action = table.Column<string>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: true),
                    PerformedBy = table.Column<string>(type: "TEXT", nullable: false),
                    TargetUserId = table.Column<string>(type: "TEXT", nullable: true),
                    TargetEntityType = table.Column<string>(type: "TEXT", nullable: true),
                    TargetEntityId = table.Column<int>(type: "INTEGER", nullable: true),
                    Timestamp = table.Column<DateTime>(type: "TEXT", nullable: false),
                    IpAddress = table.Column<string>(type: "TEXT", nullable: true),
                    AdditionalData = table.Column<string>(type: "TEXT", nullable: true),
                    Severity = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SecurityAuditLogs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "WorkIdVerifications",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UserId = table.Column<string>(type: "TEXT", nullable: false),
                    WorkId = table.Column<string>(type: "TEXT", nullable: false),
                    IsVerified = table.Column<bool>(type: "INTEGER", nullable: false),
                    EmployeeName = table.Column<string>(type: "TEXT", nullable: true),
                    Department = table.Column<string>(type: "TEXT", nullable: true),
                    BackgroundCheckCompleted = table.Column<bool>(type: "INTEGER", nullable: false),
                    VerifiedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    VerifiedBy = table.Column<string>(type: "TEXT", nullable: true),
                    Notes = table.Column<string>(type: "TEXT", nullable: true),
                    MunicipalityDbMatch = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkIdVerifications", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserBroadcastAcknowledgments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    BroadcastId = table.Column<int>(type: "INTEGER", nullable: false),
                    UserId = table.Column<string>(type: "TEXT", nullable: false),
                    AcknowledgedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserBroadcastAcknowledgments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserBroadcastAcknowledgments_EmergencyBroadcasts_BroadcastId",
                        column: x => x.BroadcastId,
                        principalTable: "EmergencyBroadcasts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserBroadcastAcknowledgments_BroadcastId",
                table: "UserBroadcastAcknowledgments",
                column: "BroadcastId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ContentVersions");

            migrationBuilder.DropTable(
                name: "IdDocumentImages");

            migrationBuilder.DropTable(
                name: "SecurityAuditLogs");

            migrationBuilder.DropTable(
                name: "UserBroadcastAcknowledgments");

            migrationBuilder.DropTable(
                name: "WorkIdVerifications");

            migrationBuilder.DropTable(
                name: "EmergencyBroadcasts");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "WaterSchedules");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "WaterSchedules");

            migrationBuilder.DropColumn(
                name: "Zone",
                table: "WaterSchedules");

            migrationBuilder.DropColumn(
                name: "AdminComment",
                table: "JobPostings");

            migrationBuilder.DropColumn(
                name: "ContributorId",
                table: "JobPostings");

            migrationBuilder.DropColumn(
                name: "IsApproved",
                table: "JobPostings");

            migrationBuilder.DropColumn(
                name: "JobType",
                table: "JobPostings");

            migrationBuilder.DropColumn(
                name: "Salary",
                table: "JobPostings");

            migrationBuilder.DropColumn(
                name: "ScheduledPublishAt",
                table: "JobPostings");

            migrationBuilder.DropColumn(
                name: "ContactPhone",
                table: "ClinicRecords");

            migrationBuilder.DropColumn(
                name: "Location",
                table: "ClinicRecords");

            migrationBuilder.DropColumn(
                name: "Services",
                table: "ClinicRecords");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "ClinicRecords");
        }
    }
}
