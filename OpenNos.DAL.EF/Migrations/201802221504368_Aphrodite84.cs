namespace OpenNos.DAL.EF.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class Aphrodite84 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.QuestProgress",
                c => new
                    {
                        QuestProgressId = c.Guid(nullable: false),
                        QuestId = c.Long(nullable: false),
                        QuestData = c.String(),
                        CharacterId = c.Long(nullable: false),
                        IsFinished = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.QuestProgressId)
                .ForeignKey("dbo.Quest", t => t.QuestId)
                .ForeignKey("dbo.Character", t => t.CharacterId)
                .Index(t => t.QuestId)
                .Index(t => t.CharacterId);
        }

        public override void Down()
        {
            DropForeignKey("dbo.QuestProgress", "CharacterId", "dbo.Character");
            DropForeignKey("dbo.QuestProgress", "QuestId", "dbo.Quest");
            DropIndex("dbo.QuestProgress", new[] { "CharacterId" });
            DropIndex("dbo.QuestProgress", new[] { "QuestId" });
            DropTable("dbo.QuestProgress");
        }
    }
}
