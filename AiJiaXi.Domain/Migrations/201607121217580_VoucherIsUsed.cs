namespace AiJiaXi.Domain.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class VoucherIsUsed : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AiJiaXi__Voucher", "IsUsed", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.AiJiaXi__Voucher", "IsUsed");
        }
    }
}
