namespace AiJiaXi.Domain.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddUserAccountCardNumber : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AiJiaXi__UserAccount", "CardNumber", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.AiJiaXi__UserAccount", "CardNumber");
        }
    }
}
