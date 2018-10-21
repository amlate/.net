namespace Project.Domain.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddEventCounties : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.AiJiaXi__Event", "AgencyId", "dbo.AiJiaXi__Agency");
            DropIndex("dbo.AiJiaXi__Event", new[] { "AgencyId" });
            CreateTable(
                "dbo.AgenciesEvents",
                c => new
                    {
                        EventId = c.Long(nullable: false),
                        AgencyId = c.Long(nullable: false),
                    })
                .PrimaryKey(t => new { t.EventId, t.AgencyId })
                .ForeignKey("dbo.AiJiaXi__Event", t => t.EventId, cascadeDelete: true)
                .ForeignKey("dbo.AiJiaXi__Agency", t => t.AgencyId, cascadeDelete: true)
                .Index(t => t.EventId)
                .Index(t => t.AgencyId);
            
            AddColumn("dbo.AiJiaXi__OrderItemClass", "Counties", c => c.String());
            DropColumn("dbo.AiJiaXi__Event", "AgencyId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.AiJiaXi__Event", "AgencyId", c => c.Long(nullable: false));
            DropForeignKey("dbo.AgenciesEvents", "AgencyId", "dbo.AiJiaXi__Agency");
            DropForeignKey("dbo.AgenciesEvents", "EventId", "dbo.AiJiaXi__Event");
            DropIndex("dbo.AgenciesEvents", new[] { "AgencyId" });
            DropIndex("dbo.AgenciesEvents", new[] { "EventId" });
            DropColumn("dbo.AiJiaXi__OrderItemClass", "Counties");
            DropTable("dbo.AgenciesEvents");
            CreateIndex("dbo.AiJiaXi__Event", "AgencyId");
            AddForeignKey("dbo.AiJiaXi__Event", "AgencyId", "dbo.AiJiaXi__Agency", "Id", cascadeDelete: true);
        }
    }
}
