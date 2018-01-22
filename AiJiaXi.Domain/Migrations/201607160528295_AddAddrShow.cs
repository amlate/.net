namespace AiJiaXi.Domain.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddAddrShow : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AiJiaXi__UserAddress", "IsShow", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.AiJiaXi__UserAddress", "IsShow");
        }
    }
}
