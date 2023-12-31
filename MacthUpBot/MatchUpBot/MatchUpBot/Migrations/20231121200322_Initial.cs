﻿using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace MatchUpBot.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Interests",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Interests", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    TgId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Age = table.Column<int>(type: "integer", nullable: false),
                    City = table.Column<string>(type: "text", nullable: false),
                    Gender = table.Column<string>(type: "text", nullable: false),
                    TgUsername = table.Column<string>(type: "text", nullable: false),
                    Stage = table.Column<int>(type: "integer", nullable: false),
                    About = table.Column<string>(type: "text", nullable: false),
                    ZodiacSign = table.Column<string>(type: "text", nullable: false),
                    IsZodiacSignMatters = table.Column<bool>(type: "boolean", nullable: false),
                    GenderOfInterest = table.Column<string>(type: "text", nullable: false),
                    LastShowedBlankTgId = table.Column<long>(type: "bigint", nullable: false),
                    IsNotified = table.Column<bool>(type: "boolean", nullable: false),
                    IsVip = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.TgId);
                });

            migrationBuilder.CreateTable(
                name: "BlanksShowingHistory",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ReceivedUserTgId = table.Column<long>(type: "bigint", nullable: false),
                    ShownUserTgId = table.Column<long>(type: "bigint", nullable: false),
                    Date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BlanksShowingHistory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BlanksShowingHistory_Users_ReceivedUserTgId",
                        column: x => x.ReceivedUserTgId,
                        principalTable: "Users",
                        principalColumn: "TgId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BlanksShowingHistory_Users_ShownUserTgId",
                        column: x => x.ShownUserTgId,
                        principalTable: "Users",
                        principalColumn: "TgId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CreditCards",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    CardNumber = table.Column<string>(type: "text", nullable: false),
                    HolderName = table.Column<string>(type: "text", nullable: false),
                    ExpirationTime = table.Column<string>(type: "text", nullable: false),
                    CVV = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CreditCards", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CreditCards_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "TgId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "InterestWeightEntities",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    SportWeight = table.Column<byte>(type: "smallint", nullable: false),
                    ArtWeight = table.Column<byte>(type: "smallint", nullable: false),
                    NatureWeight = table.Column<byte>(type: "smallint", nullable: false),
                    MusicWeight = table.Column<byte>(type: "smallint", nullable: false),
                    TravelWeight = table.Column<byte>(type: "smallint", nullable: false),
                    PhotoWeight = table.Column<byte>(type: "smallint", nullable: false),
                    CookingWeight = table.Column<byte>(type: "smallint", nullable: false),
                    MovieWeight = table.Column<byte>(type: "smallint", nullable: false),
                    LiteratureWeight = table.Column<byte>(type: "smallint", nullable: false),
                    ScienceWeight = table.Column<byte>(type: "smallint", nullable: false),
                    TechnologiesWeight = table.Column<byte>(type: "smallint", nullable: false),
                    HistoryWeight = table.Column<byte>(type: "smallint", nullable: false),
                    PsychologyWeight = table.Column<byte>(type: "smallint", nullable: false),
                    ReligionWeight = table.Column<byte>(type: "smallint", nullable: false),
                    FashionWeight = table.Column<byte>(type: "smallint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InterestWeightEntities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InterestWeightEntities_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "TgId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Likes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    LikedByUserId = table.Column<long>(type: "bigint", nullable: false),
                    LikedUserId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Likes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Likes_Users_LikedByUserId",
                        column: x => x.LikedByUserId,
                        principalTable: "Users",
                        principalColumn: "TgId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Likes_Users_LikedUserId",
                        column: x => x.LikedUserId,
                        principalTable: "Users",
                        principalColumn: "TgId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UserInterestsEntities",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    InterestId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserInterestsEntities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserInterestsEntities_Interests_InterestId",
                        column: x => x.InterestId,
                        principalTable: "Interests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserInterestsEntities_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "TgId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BlanksShowingHistory_ReceivedUserTgId",
                table: "BlanksShowingHistory",
                column: "ReceivedUserTgId");

            migrationBuilder.CreateIndex(
                name: "IX_BlanksShowingHistory_ShownUserTgId",
                table: "BlanksShowingHistory",
                column: "ShownUserTgId");

            migrationBuilder.CreateIndex(
                name: "IX_CreditCards_UserId",
                table: "CreditCards",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_InterestWeightEntities_UserId",
                table: "InterestWeightEntities",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Likes_LikedByUserId",
                table: "Likes",
                column: "LikedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Likes_LikedUserId",
                table: "Likes",
                column: "LikedUserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserInterestsEntities_InterestId",
                table: "UserInterestsEntities",
                column: "InterestId");

            migrationBuilder.CreateIndex(
                name: "IX_UserInterestsEntities_UserId",
                table: "UserInterestsEntities",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BlanksShowingHistory");

            migrationBuilder.DropTable(
                name: "CreditCards");

            migrationBuilder.DropTable(
                name: "InterestWeightEntities");

            migrationBuilder.DropTable(
                name: "Likes");

            migrationBuilder.DropTable(
                name: "UserInterestsEntities");

            migrationBuilder.DropTable(
                name: "Interests");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
