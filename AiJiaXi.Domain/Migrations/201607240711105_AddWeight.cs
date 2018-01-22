namespace AiJiaXi.Domain.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddWeight : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AiJiaXi__EventPrize", "Weight", c => c.Long(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.AiJiaXi__EventPrize", "Weight");
        }
    }
}
