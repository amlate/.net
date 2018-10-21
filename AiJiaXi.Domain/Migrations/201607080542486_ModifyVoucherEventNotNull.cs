namespace Project.Domain.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ModifyVoucherEventNotNull : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.AiJiaXi__Voucher", "EventId", "dbo.AiJiaXi__Event");
            DropIndex("dbo.AiJiaXi__Voucher", new[] { "EventId" });
            AlterColumn("dbo.AiJiaXi__Voucher", "EventId", c => c.Long(nullable: false));
            CreateIndex("dbo.AiJiaXi__Voucher", "EventId");
            AddForeignKey("dbo.AiJiaXi__Voucher", "EventId", "dbo.AiJiaXi__Event", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AiJiaXi__Voucher", "EventId", "dbo.AiJiaXi__Event");
            DropIndex("dbo.AiJiaXi__Voucher", new[] { "EventId" });
            AlterColumn("dbo.AiJiaXi__Voucher", "EventId", c => c.Long());
            CreateIndex("dbo.AiJiaXi__Voucher", "EventId");
            AddForeignKey("dbo.AiJiaXi__Voucher", "EventId", "dbo.AiJiaXi__Event", "Id");
        }
    }
}
