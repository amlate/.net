namespace Project.Domain.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class classUrl : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AiJiaXi__OrderItemClass", "Url", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.AiJiaXi__OrderItemClass", "Url");
        }
    }
}
