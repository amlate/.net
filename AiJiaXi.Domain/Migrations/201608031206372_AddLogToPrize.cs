namespace AiJiaXi.Domain.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddLogToPrize : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AiJiaXi__EventPrize", "IsLog", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.AiJiaXi__EventPrize", "IsLog");
        }
    }
}
