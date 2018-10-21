namespace Project.Domain.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addOrderItemClassClientEventType : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AiJiaXi__OrderItemClass", "ClientEventType", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.AiJiaXi__OrderItemClass", "ClientEventType");
        }
    }
}
