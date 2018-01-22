namespace AiJiaXi.Domain.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class EventPrizes : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.AiJiaXi__EventAward", "EventId", "dbo.AiJiaXi__Event");
            DropIndex("dbo.AiJiaXi__EventAward", new[] { "EventId" });
            CreateTable(
                "dbo.AiJiaXi__EventPrize",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        EventId = c.Long(nullable: false),
                        Name = c.String(),
                        Desc = c.String(),
                        Images = c.String(),
                        Nums = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AiJiaXi__Event", t => t.EventId, cascadeDelete: true)
                .Index(t => t.EventId);
            
            AddColumn("dbo.AiJiaXi__EventAward", "EventPrizeId", c => c.Long(nullable: false));
            CreateIndex("dbo.AiJiaXi__EventAward", "EventPrizeId");
            AddForeignKey("dbo.AiJiaXi__EventAward", "EventPrizeId", "dbo.AiJiaXi__EventPrize", "Id", cascadeDelete: true);
            DropColumn("dbo.AiJiaXi__EventAward", "EventId");
            DropColumn("dbo.AiJiaXi__EventAward", "Award");
        }
        
        public override void Down()
        {
            AddColumn("dbo.AiJiaXi__EventAward", "Award", c => c.String());
            AddColumn("dbo.AiJiaXi__EventAward", "EventId", c => c.Long(nullable: false));
            DropForeignKey("dbo.AiJiaXi__EventAward", "EventPrizeId", "dbo.AiJiaXi__EventPrize");
            DropForeignKey("dbo.AiJiaXi__EventPrize", "EventId", "dbo.AiJiaXi__Event");
            DropIndex("dbo.AiJiaXi__EventAward", new[] { "EventPrizeId" });
            DropIndex("dbo.AiJiaXi__EventPrize", new[] { "EventId" });
            DropColumn("dbo.AiJiaXi__EventAward", "EventPrizeId");
            DropTable("dbo.AiJiaXi__EventPrize");
            CreateIndex("dbo.AiJiaXi__EventAward", "EventId");
            AddForeignKey("dbo.AiJiaXi__EventAward", "EventId", "dbo.AiJiaXi__Event", "Id", cascadeDelete: true);
        }
    }
}
