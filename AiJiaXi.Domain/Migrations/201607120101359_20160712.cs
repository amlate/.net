namespace Project.Domain.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _20160712 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AiJiaXi__Order", "AppointmentTime", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.AiJiaXi__Order", "AppointmentTime");
        }
    }
}
