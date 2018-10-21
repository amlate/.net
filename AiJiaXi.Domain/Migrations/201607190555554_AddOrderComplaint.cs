namespace Project.Domain.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddOrderComplaint : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AiJiaXi__Order", "ComplaintType", c => c.Int(nullable: false));
            AddColumn("dbo.AiJiaXi__Order", "ComplaintNote", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.AiJiaXi__Order", "ComplaintNote");
            DropColumn("dbo.AiJiaXi__Order", "ComplaintType");
        }
    }
}
