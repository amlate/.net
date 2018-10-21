namespace Project.Domain.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class EventNums : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AiJiaXi__Event", "Nums", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.AiJiaXi__Event", "Nums");
        }
    }
}
