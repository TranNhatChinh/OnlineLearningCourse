using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class IdentityInit : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "user_profiles",
                columns: table => new
                {
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    full_name = table.Column<string>(type: "text", nullable: true),
                    avatar_url = table.Column<string>(type: "text", nullable: true),
                    phone = table.Column<string>(type: "text", nullable: true),
                    status = table.Column<string>(type: "text", nullable: false, defaultValue: "Active"),
                    user_role = table.Column<string>(type: "text", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_profiles", x => x.user_id);
                });

            migrationBuilder.CreateTable(
                name: "admins",
                columns: table => new
                {
                    admin_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_admins", x => x.admin_id);
                    table.ForeignKey(
                        name: "FK_admins_user_profiles_admin_id",
                        column: x => x.admin_id,
                        principalTable: "user_profiles",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "instructors",
                columns: table => new
                {
                    instructor_id = table.Column<Guid>(type: "uuid", nullable: false),
                    bio = table.Column<string>(type: "text", nullable: true),
                    expertise = table.Column<string>(type: "text", nullable: true),
                    verification_status = table.Column<string>(type: "text", nullable: false, defaultValue: "Pending"),
                    rating_avg = table.Column<decimal>(type: "numeric(3,2)", precision: 3, scale: 2, nullable: false, defaultValue: 0m),
                    rating_count = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_instructors", x => x.instructor_id);
                    table.ForeignKey(
                        name: "FK_instructors_user_profiles_instructor_id",
                        column: x => x.instructor_id,
                        principalTable: "user_profiles",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "learners",
                columns: table => new
                {
                    learner_id = table.Column<Guid>(type: "uuid", nullable: false),
                    parent_id = table.Column<Guid>(type: "uuid", nullable: true),
                    grade_level = table.Column<string>(type: "text", nullable: true),
                    birth_year = table.Column<int>(type: "integer", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_learners", x => x.learner_id);
                    table.ForeignKey(
                        name: "FK_learners_user_profiles_learner_id",
                        column: x => x.learner_id,
                        principalTable: "user_profiles",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "parents",
                columns: table => new
                {
                    parent_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_parents", x => x.parent_id);
                    table.ForeignKey(
                        name: "FK_parents_user_profiles_parent_id",
                        column: x => x.parent_id,
                        principalTable: "user_profiles",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "staff",
                columns: table => new
                {
                    staff_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_staff", x => x.staff_id);
                    table.ForeignKey(
                        name: "FK_staff_user_profiles_staff_id",
                        column: x => x.staff_id,
                        principalTable: "user_profiles",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "idx_instructors_rating",
                table: "instructors",
                column: "rating_avg");

            migrationBuilder.CreateIndex(
                name: "idx_instructors_verification",
                table: "instructors",
                column: "verification_status");

            migrationBuilder.CreateIndex(
                name: "idx_learners_parent",
                table: "learners",
                column: "parent_id");

            migrationBuilder.CreateIndex(
                name: "idx_user_profiles_role",
                table: "user_profiles",
                column: "user_role");

            migrationBuilder.CreateIndex(
                name: "idx_user_profiles_status",
                table: "user_profiles",
                column: "status");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "admins");

            migrationBuilder.DropTable(
                name: "instructors");

            migrationBuilder.DropTable(
                name: "learners");

            migrationBuilder.DropTable(
                name: "parents");

            migrationBuilder.DropTable(
                name: "staff");

            migrationBuilder.DropTable(
                name: "user_profiles");
        }
    }
}
