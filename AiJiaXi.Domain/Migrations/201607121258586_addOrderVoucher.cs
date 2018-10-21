namespace Project.Domain.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addOrderVoucher : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AiJiaXi__Order", "VoucherId", c => c.Guid());
            CreateIndex("dbo.AiJiaXi__Order", "VoucherId");
            AddForeignKey("dbo.AiJiaXi__Order", "VoucherId", "dbo.AiJiaXi__Voucher", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AiJiaXi__Order", "VoucherId", "dbo.AiJiaXi__Voucher");
            DropIndex("dbo.AiJiaXi__Order", new[] { "VoucherId" });
            DropColumn("dbo.AiJiaXi__Order", "VoucherId");
        }
    }
}
