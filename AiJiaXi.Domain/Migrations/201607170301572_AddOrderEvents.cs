namespace AiJiaXi.Domain.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddOrderEvents : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AiJiaXi__Order", "Events", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.AiJiaXi__Order", "Events");
        }
    }
}
