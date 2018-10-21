namespace Project.Domain.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddAgencyIsValid : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AiJiaXi__Agency", "IsValid", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.AiJiaXi__Agency", "IsValid");
        }
    }
}
