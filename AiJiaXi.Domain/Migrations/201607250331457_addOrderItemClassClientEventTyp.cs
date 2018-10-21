namespace Project.Domain.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addOrderItemClassClientEventTyp : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AiJiaXi__OrderItem", "ClientEventType", c => c.Int(nullable: false));
            DropColumn("dbo.AiJiaXi__OrderItemClass", "ClientEventType");
        }
        
        public override void Down()
        {
            AddColumn("dbo.AiJiaXi__OrderItemClass", "ClientEventType", c => c.Int(nullable: false));
            DropColumn("dbo.AiJiaXi__OrderItem", "ClientEventType");
        }
    }
}
