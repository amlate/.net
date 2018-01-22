namespace AiJiaXi.Domain.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddVoucherNo : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AiJiaXi__Voucher", "VoucherNo", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.AiJiaXi__Voucher", "VoucherNo");
        }
    }
}
