namespace Project.Domain.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addUserLocation : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AiJiaXi__UserLocation", "LocationProvinceName", c => c.String());
            AddColumn("dbo.AiJiaXi__UserLocation", "LocationCityName", c => c.String());
            AddColumn("dbo.AiJiaXi__UserLocation", "LocationCountyName", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.AiJiaXi__UserLocation", "LocationCountyName");
            DropColumn("dbo.AiJiaXi__UserLocation", "LocationCityName");
            DropColumn("dbo.AiJiaXi__UserLocation", "LocationProvinceName");
        }
    }
}
