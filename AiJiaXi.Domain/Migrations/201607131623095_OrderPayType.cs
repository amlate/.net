namespace AiJiaXi.Domain.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class OrderPayType : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AiJiaXi__Order", "PayType", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.AiJiaXi__Order", "PayType");
        }
    }
}
