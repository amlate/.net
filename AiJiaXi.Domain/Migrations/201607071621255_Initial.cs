namespace Project.Domain.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.AiJiaXi__AccountRecord",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        UserAccountId = c.String(maxLength: 128),
                        TradeType = c.Int(nullable: false),
                        RiseTime = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        ResultType = c.Int(nullable: false),
                        TradeMoney = c.Decimal(nullable: false, precision: 18, scale: 2),
                        AccountBallance = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TradeScore = c.Int(nullable: false),
                        ScoreBalance = c.Int(nullable: false),
                        TradeId = c.String(),
                        Note = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AiJiaXi__UserAccount", t => t.UserAccountId)
                .Index(t => t.UserAccountId);
            
            CreateTable(
                "dbo.AiJiaXi__UserAccount",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Balance = c.Decimal(nullable: false, precision: 18, scale: 2),
                        FrozenMoney = c.Decimal(nullable: false, precision: 18, scale: 2),
                        CommissionMoney = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Score = c.Int(nullable: false),
                        OpenId = c.String(),
                        FrozenScore = c.Int(nullable: false),
                        CommissionUrl = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.Id)
                .Index(t => t.Id);
            
            CreateTable(
                "dbo.AspNetUsers",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        RealName = c.String(),
                        Dept = c.String(),
                        Position = c.String(),
                        AddTime = c.DateTime(precision: 7, storeType: "datetime2"),
                        HeadAppearUrl = c.String(),
                        UserType = c.Int(nullable: false),
                        AgencyId = c.Long(),
                        Remark = c.String(),
                        OpenId = c.String(),
                        IsFrozen = c.Boolean(nullable: false),
                        Email = c.String(maxLength: 256),
                        EmailConfirmed = c.Boolean(nullable: false),
                        PasswordHash = c.String(),
                        SecurityStamp = c.String(),
                        PhoneNumber = c.String(),
                        PhoneNumberConfirmed = c.Boolean(nullable: false),
                        TwoFactorEnabled = c.Boolean(nullable: false),
                        LockoutEndDateUtc = c.DateTime(),
                        LockoutEnabled = c.Boolean(nullable: false),
                        AccessFailedCount = c.Int(nullable: false),
                        UserName = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AiJiaXi__Agency", t => t.AgencyId)
                .Index(t => t.AgencyId)
                .Index(t => t.UserName, unique: true, name: "UserNameIndex");
            
            CreateTable(
                "dbo.AiJiaXi__Agency",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Name = c.String(),
                        CountyId = c.Int(nullable: false),
                        CityId = c.Int(nullable: false),
                        ProvinceId = c.Int(nullable: false),
                        Note = c.String(),
                        Contact = c.String(),
                        ContactMobile = c.String(),
                        ContactEmail = c.String(),
                        Title = c.String(),
                        RangeMap = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AiJiaXi__County", t => t.CountyId, cascadeDelete: true)
                .ForeignKey("dbo.AiJiaXi__Province", t => t.ProvinceId, cascadeDelete: true)
                .Index(t => t.CountyId)
                .Index(t => t.ProvinceId);
            
            CreateTable(
                "dbo.AiJiaXi__County",
                c => new
                    {
                        Id = c.Int(nullable: false),
                        Name = c.String(),
                        Note = c.String(),
                        Validity = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.AiJiaXi__Employee",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        AgencyId = c.Long(nullable: false),
                        EmployeeNo = c.String(),
                        Name = c.String(),
                        Mobile = c.String(),
                        Phone = c.String(),
                        IdNum = c.String(),
                        InServiceState = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AiJiaXi__Agency", t => t.AgencyId, cascadeDelete: true)
                .Index(t => t.AgencyId);
            
            CreateTable(
                "dbo.AiJiaXi__OrderStep",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        OrderStatus = c.Int(nullable: false),
                        RiseTime = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        EmployeeId = c.Long(),
                        OperationUser = c.String(),
                        UserType = c.Int(nullable: false),
                        Note = c.String(),
                        OrderId = c.Long(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AiJiaXi__Employee", t => t.EmployeeId)
                .ForeignKey("dbo.AiJiaXi__Order", t => t.OrderId, cascadeDelete: true)
                .Index(t => t.EmployeeId)
                .Index(t => t.OrderId);
            
            CreateTable(
                "dbo.AiJiaXi__Order",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        OrderNo = c.String(),
                        UserAddressId = c.Long(),
                        ApplicationUserId = c.String(maxLength: 128),
                        Appointment = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        TotalPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Fact = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Freight = c.Decimal(nullable: false, precision: 18, scale: 2),
                        RiseTime = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        CompleteTime = c.DateTime(precision: 7, storeType: "datetime2"),
                        OrderStatus = c.Int(nullable: false),
                        AgencyId = c.Long(nullable: false),
                        OrderRateId = c.Long(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AiJiaXi__Agency", t => t.AgencyId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.ApplicationUserId)
                .ForeignKey("dbo.AiJiaXi__UserAddress", t => t.UserAddressId)
                .Index(t => t.UserAddressId)
                .Index(t => t.ApplicationUserId)
                .Index(t => t.AgencyId);
            
            CreateTable(
                "dbo.AiJiaXi__CartItem",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        OrderId = c.Long(nullable: false),
                        OrderItemId = c.Long(nullable: false),
                        Nums = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AiJiaXi__Order", t => t.OrderId, cascadeDelete: true)
                .ForeignKey("dbo.AiJiaXi__OrderItem", t => t.OrderItemId, cascadeDelete: true)
                .Index(t => t.OrderId)
                .Index(t => t.OrderItemId);
            
            CreateTable(
                "dbo.AiJiaXi__OrderImage",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Url = c.String(),
                        Name = c.String(),
                        Note = c.String(),
                        CartItemId = c.Long(nullable: false),
                        OrderImageType = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AiJiaXi__CartItem", t => t.CartItemId, cascadeDelete: true)
                .Index(t => t.CartItemId);
            
            CreateTable(
                "dbo.AiJiaXi__OrderItem",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Name = c.String(),
                        Desc = c.String(),
                        ImageUrl = c.String(),
                        HoverImageUrl = c.String(),
                        Price = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Nums = c.Int(nullable: false),
                        ItemClassId = c.Long(nullable: false),
                        Days = c.Int(nullable: false),
                        AddTime = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        AddUser = c.String(),
                        ModifyTime = c.DateTime(precision: 7, storeType: "datetime2"),
                        ModifyUser = c.String(),
                        IsValid = c.Boolean(nullable: false),
                        Order = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AiJiaXi__OrderItemClass", t => t.ItemClassId, cascadeDelete: true)
                .Index(t => t.ItemClassId);
            
            CreateTable(
                "dbo.AiJiaXi__OrderItemClass",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Name = c.String(),
                        IconUrl = c.String(),
                        HoverIconUrl = c.String(),
                        CityId = c.Int(nullable: false),
                        AddTime = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        AddUser = c.String(),
                        ModifyTime = c.DateTime(precision: 7, storeType: "datetime2"),
                        ModifyUser = c.String(),
                        IsValid = c.Boolean(nullable: false),
                        Order = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AiJiaXi__City", t => t.CityId, cascadeDelete: true)
                .Index(t => t.CityId);
            
            CreateTable(
                "dbo.AiJiaXi__City",
                c => new
                    {
                        Id = c.Int(nullable: false),
                        Name = c.String(),
                        Note = c.String(),
                        Validity = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.AiJiaXi__OrderRate",
                c => new
                    {
                        Id = c.Long(nullable: false),
                        Stars = c.Single(nullable: false),
                        PatchStars = c.Single(nullable: false),
                        DispatchStars = c.Single(nullable: false),
                        OrderComment = c.String(),
                        PatchComment = c.String(),
                        DispatchComment = c.String(),
                        ShareOrderImgUrls = c.String(),
                        RiseTime = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        IsApproval = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AiJiaXi__Order", t => t.Id)
                .Index(t => t.Id);
            
            CreateTable(
                "dbo.AiJiaXi__UserAddress",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Contact = c.String(),
                        Gender = c.Int(nullable: false),
                        ContactPhoneNum = c.String(),
                        Addr = c.String(),
                        Note = c.String(),
                        ApplicationUserId = c.String(maxLength: 128),
                        IsDefault = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.ApplicationUserId)
                .Index(t => t.ApplicationUserId);
            
            CreateTable(
                "dbo.AiJiaXi__Province",
                c => new
                    {
                        Id = c.Int(nullable: false),
                        Name = c.String(),
                        Note = c.String(),
                        Validity = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.AspNetUserClaims",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(nullable: false, maxLength: 128),
                        ClaimType = c.String(),
                        ClaimValue = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.AiJiaXi__EventAward",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        UserId = c.String(maxLength: 128),
                        AddressId = c.Long(),
                        EventId = c.Long(nullable: false),
                        Award = c.String(),
                        Note = c.String(),
                        Flag = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AiJiaXi__UserAddress", t => t.AddressId)
                .ForeignKey("dbo.AiJiaXi__Event", t => t.EventId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
                .Index(t => t.UserId)
                .Index(t => t.AddressId)
                .Index(t => t.EventId);
            
            CreateTable(
                "dbo.AiJiaXi__Event",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Name = c.String(),
                        Desc = c.String(),
                        EventType = c.Int(nullable: false),
                        EventUrl = c.String(),
                        Discount = c.Single(nullable: false),
                        PriceTo = c.Decimal(nullable: false, precision: 18, scale: 2),
                        BenefitPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                        StartTime = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        EndTime = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        AgencyId = c.Long(nullable: false),
                        ApplyStatus = c.Int(nullable: false),
                        Flag = c.Boolean(nullable: false),
                        UseMaxVoucherNum = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AiJiaXi__Agency", t => t.AgencyId, cascadeDelete: true)
                .Index(t => t.AgencyId);
            
            CreateTable(
                "dbo.AspNetUserLogins",
                c => new
                    {
                        LoginProvider = c.String(nullable: false, maxLength: 128),
                        ProviderKey = c.String(nullable: false, maxLength: 128),
                        UserId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.LoginProvider, t.ProviderKey, t.UserId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.AspNetUserRoles",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        RoleId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.UserId, t.RoleId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetRoles", t => t.RoleId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.RoleId);
            
            CreateTable(
                "dbo.AiJiaXi__Voucher",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        Name = c.String(),
                        Desc = c.String(),
                        Amount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        PriceToUse = c.Decimal(nullable: false, precision: 18, scale: 2),
                        StartTime = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        EndTime = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        UserAccountId = c.String(maxLength: 128),
                        IsOccu = c.Boolean(nullable: false),
                        EventId = c.Long(),
                        AgencyId = c.Long(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AiJiaXi__Agency", t => t.AgencyId)
                .ForeignKey("dbo.AiJiaXi__Event", t => t.EventId)
                .ForeignKey("dbo.AiJiaXi__UserAccount", t => t.UserAccountId)
                .Index(t => t.UserAccountId)
                .Index(t => t.EventId)
                .Index(t => t.AgencyId);
            
            CreateTable(
                "dbo.AspNetRoles",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true, name: "RoleNameIndex");
            
            CreateTable(
                "dbo.AiJiaXi__Navbar",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        NameOption = c.String(),
                        Controller = c.String(),
                        Action = c.String(),
                        Area = c.String(),
                        ImageClass = c.String(),
                        Status = c.Boolean(nullable: false),
                        ParentId = c.Int(nullable: false),
                        IsParent = c.Boolean(nullable: false),
                        Order = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.AiJiaXi__BizPartner",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Name = c.String(),
                        ThumbNail = c.String(),
                        ImageEntityId = c.Long(nullable: false),
                        Desc = c.String(),
                        IsShow = c.Boolean(nullable: false),
                        Order = c.Int(nullable: false),
                        AddTime = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AiJiaXi__ImageEntity", t => t.ImageEntityId, cascadeDelete: true)
                .Index(t => t.ImageEntityId);
            
            CreateTable(
                "dbo.AiJiaXi__ImageEntity",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        ImgUrl = c.String(),
                        Height = c.Int(nullable: false),
                        Width = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.AiJiaXi__EmailConfig",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        ServerUrl = c.String(),
                        Port = c.Int(nullable: false),
                        UserAccount = c.String(),
                        Password = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.AiJiaXi__Feedback",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Content = c.String(),
                        UserId = c.String(),
                        UserName = c.String(),
                        RiseTime = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        DealStatus = c.Int(nullable: false),
                        DealTime = c.DateTime(precision: 7, storeType: "datetime2"),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.AiJiaXi__JoinApplication",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Name = c.String(),
                        Mobile = c.String(),
                        Email = c.String(),
                        JoinType = c.String(),
                        Area = c.String(),
                        RiseTime = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        FeedbackStatus = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.AiJiaXi__LoginLog",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        LoginUserName = c.String(),
                        LoginIp = c.String(),
                        LoginDesc = c.Int(nullable: false),
                        RiseTime = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.AiJiaXi__NewsClass",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Name = c.String(),
                        ImageEntityId = c.Long(),
                        ThumbNailId = c.Long(),
                        Desc = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AiJiaXi__ImageEntity", t => t.ImageEntityId)
                .ForeignKey("dbo.AiJiaXi__ImageEntity", t => t.ThumbNailId)
                .Index(t => t.ImageEntityId)
                .Index(t => t.ThumbNailId);
            
            CreateTable(
                "dbo.AiJiaXi__NewsMain",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Title = c.String(),
                        ThumbNailId = c.Long(),
                        ImageEntityId = c.Long(),
                        Content = c.String(),
                        AddTime = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        AddUser = c.String(),
                        ModifyTime = c.DateTime(precision: 7, storeType: "datetime2"),
                        ModifyUser = c.String(),
                        StartTime = c.DateTime(precision: 7, storeType: "datetime2"),
                        EndTime = c.DateTime(precision: 7, storeType: "datetime2"),
                        IsShow = c.String(),
                        IsDelete = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AiJiaXi__ImageEntity", t => t.ImageEntityId)
                .ForeignKey("dbo.AiJiaXi__ImageEntity", t => t.ThumbNailId)
                .Index(t => t.ThumbNailId)
                .Index(t => t.ImageEntityId);
            
            CreateTable(
                "dbo.AiJiaXi__OperationLog",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Module = c.String(),
                        Action = c.String(),
                        UserName = c.String(),
                        UserIP = c.String(),
                        RiseTime = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.AiJiaXi__PreWithdrawals",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        ApplyDate = c.String(),
                        Phone = c.String(),
                        Amount = c.String(),
                        State = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.AiJiaXi__PromoterInfo",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        FriendsWeiXinId = c.String(),
                        FriendsPhone = c.String(),
                        FollowDate = c.String(),
                        MyPhone = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.AiJiaXi__SmsConfig",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        EntId = c.String(),
                        Account = c.String(),
                        Password = c.String(),
                        ExtNo = c.String(),
                        SmsSendUrl = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.AiJiaXi__UserLocation",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        ToUserName = c.String(),
                        FromUserName = c.String(),
                        CreateTime = c.DateTime(nullable: false),
                        Latitude = c.Double(nullable: false),
                        Longitude = c.Double(nullable: false),
                        Precision = c.Double(nullable: false),
                        ProvinceName = c.String(),
                        CityName = c.String(),
                        CountyName = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.AiJiaXi__Withdrawals",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Phone = c.String(),
                        PreApplyDate = c.String(),
                        ApplyDate = c.String(),
                        Bank = c.String(),
                        Name = c.String(),
                        Accounts = c.String(),
                        Amount = c.String(),
                        HandleDate = c.String(),
                        State = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.ApplicationRoleNavbar",
                c => new
                    {
                        NavbarId = c.Int(nullable: false),
                        ApplicationRoleId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.NavbarId, t.ApplicationRoleId })
                .ForeignKey("dbo.AiJiaXi__Navbar", t => t.NavbarId, cascadeDelete: true)
                .ForeignKey("dbo.AiJiaXi__ApplicationRole", t => t.ApplicationRoleId, cascadeDelete: true)
                .Index(t => t.NavbarId)
                .Index(t => t.ApplicationRoleId);
            
            CreateTable(
                "dbo.AiJiaXi__ApplicationRole",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Description = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetRoles", t => t.Id)
                .Index(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AiJiaXi__ApplicationRole", "Id", "dbo.AspNetRoles");
            DropForeignKey("dbo.AspNetUserRoles", "RoleId", "dbo.AspNetRoles");
            DropForeignKey("dbo.AiJiaXi__NewsMain", "ThumbNailId", "dbo.AiJiaXi__ImageEntity");
            DropForeignKey("dbo.AiJiaXi__NewsMain", "ImageEntityId", "dbo.AiJiaXi__ImageEntity");
            DropForeignKey("dbo.AiJiaXi__NewsClass", "ThumbNailId", "dbo.AiJiaXi__ImageEntity");
            DropForeignKey("dbo.AiJiaXi__NewsClass", "ImageEntityId", "dbo.AiJiaXi__ImageEntity");
            DropForeignKey("dbo.AiJiaXi__BizPartner", "ImageEntityId", "dbo.AiJiaXi__ImageEntity");
            DropForeignKey("dbo.ApplicationRoleNavbar", "ApplicationRoleId", "dbo.AiJiaXi__ApplicationRole");
            DropForeignKey("dbo.ApplicationRoleNavbar", "NavbarId", "dbo.AiJiaXi__Navbar");
            DropForeignKey("dbo.AiJiaXi__Voucher", "UserAccountId", "dbo.AiJiaXi__UserAccount");
            DropForeignKey("dbo.AiJiaXi__Voucher", "EventId", "dbo.AiJiaXi__Event");
            DropForeignKey("dbo.AiJiaXi__Voucher", "AgencyId", "dbo.AiJiaXi__Agency");
            DropForeignKey("dbo.AiJiaXi__UserAccount", "Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserRoles", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserLogins", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AiJiaXi__EventAward", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AiJiaXi__EventAward", "EventId", "dbo.AiJiaXi__Event");
            DropForeignKey("dbo.AiJiaXi__Event", "AgencyId", "dbo.AiJiaXi__Agency");
            DropForeignKey("dbo.AiJiaXi__EventAward", "AddressId", "dbo.AiJiaXi__UserAddress");
            DropForeignKey("dbo.AspNetUserClaims", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AiJiaXi__Agency", "ProvinceId", "dbo.AiJiaXi__Province");
            DropForeignKey("dbo.AiJiaXi__Order", "UserAddressId", "dbo.AiJiaXi__UserAddress");
            DropForeignKey("dbo.AiJiaXi__UserAddress", "ApplicationUserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AiJiaXi__OrderStep", "OrderId", "dbo.AiJiaXi__Order");
            DropForeignKey("dbo.AiJiaXi__OrderRate", "Id", "dbo.AiJiaXi__Order");
            DropForeignKey("dbo.AiJiaXi__OrderItem", "ItemClassId", "dbo.AiJiaXi__OrderItemClass");
            DropForeignKey("dbo.AiJiaXi__OrderItemClass", "CityId", "dbo.AiJiaXi__City");
            DropForeignKey("dbo.AiJiaXi__CartItem", "OrderItemId", "dbo.AiJiaXi__OrderItem");
            DropForeignKey("dbo.AiJiaXi__OrderImage", "CartItemId", "dbo.AiJiaXi__CartItem");
            DropForeignKey("dbo.AiJiaXi__CartItem", "OrderId", "dbo.AiJiaXi__Order");
            DropForeignKey("dbo.AiJiaXi__Order", "ApplicationUserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AiJiaXi__Order", "AgencyId", "dbo.AiJiaXi__Agency");
            DropForeignKey("dbo.AiJiaXi__OrderStep", "EmployeeId", "dbo.AiJiaXi__Employee");
            DropForeignKey("dbo.AiJiaXi__Employee", "AgencyId", "dbo.AiJiaXi__Agency");
            DropForeignKey("dbo.AiJiaXi__Agency", "CountyId", "dbo.AiJiaXi__County");
            DropForeignKey("dbo.AspNetUsers", "AgencyId", "dbo.AiJiaXi__Agency");
            DropForeignKey("dbo.AiJiaXi__AccountRecord", "UserAccountId", "dbo.AiJiaXi__UserAccount");
            DropIndex("dbo.AiJiaXi__ApplicationRole", new[] { "Id" });
            DropIndex("dbo.ApplicationRoleNavbar", new[] { "ApplicationRoleId" });
            DropIndex("dbo.ApplicationRoleNavbar", new[] { "NavbarId" });
            DropIndex("dbo.AiJiaXi__NewsMain", new[] { "ImageEntityId" });
            DropIndex("dbo.AiJiaXi__NewsMain", new[] { "ThumbNailId" });
            DropIndex("dbo.AiJiaXi__NewsClass", new[] { "ThumbNailId" });
            DropIndex("dbo.AiJiaXi__NewsClass", new[] { "ImageEntityId" });
            DropIndex("dbo.AiJiaXi__BizPartner", new[] { "ImageEntityId" });
            DropIndex("dbo.AspNetRoles", "RoleNameIndex");
            DropIndex("dbo.AiJiaXi__Voucher", new[] { "AgencyId" });
            DropIndex("dbo.AiJiaXi__Voucher", new[] { "EventId" });
            DropIndex("dbo.AiJiaXi__Voucher", new[] { "UserAccountId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "RoleId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "UserId" });
            DropIndex("dbo.AspNetUserLogins", new[] { "UserId" });
            DropIndex("dbo.AiJiaXi__Event", new[] { "AgencyId" });
            DropIndex("dbo.AiJiaXi__EventAward", new[] { "EventId" });
            DropIndex("dbo.AiJiaXi__EventAward", new[] { "AddressId" });
            DropIndex("dbo.AiJiaXi__EventAward", new[] { "UserId" });
            DropIndex("dbo.AspNetUserClaims", new[] { "UserId" });
            DropIndex("dbo.AiJiaXi__UserAddress", new[] { "ApplicationUserId" });
            DropIndex("dbo.AiJiaXi__OrderRate", new[] { "Id" });
            DropIndex("dbo.AiJiaXi__OrderItemClass", new[] { "CityId" });
            DropIndex("dbo.AiJiaXi__OrderItem", new[] { "ItemClassId" });
            DropIndex("dbo.AiJiaXi__OrderImage", new[] { "CartItemId" });
            DropIndex("dbo.AiJiaXi__CartItem", new[] { "OrderItemId" });
            DropIndex("dbo.AiJiaXi__CartItem", new[] { "OrderId" });
            DropIndex("dbo.AiJiaXi__Order", new[] { "AgencyId" });
            DropIndex("dbo.AiJiaXi__Order", new[] { "ApplicationUserId" });
            DropIndex("dbo.AiJiaXi__Order", new[] { "UserAddressId" });
            DropIndex("dbo.AiJiaXi__OrderStep", new[] { "OrderId" });
            DropIndex("dbo.AiJiaXi__OrderStep", new[] { "EmployeeId" });
            DropIndex("dbo.AiJiaXi__Employee", new[] { "AgencyId" });
            DropIndex("dbo.AiJiaXi__Agency", new[] { "ProvinceId" });
            DropIndex("dbo.AiJiaXi__Agency", new[] { "CountyId" });
            DropIndex("dbo.AspNetUsers", "UserNameIndex");
            DropIndex("dbo.AspNetUsers", new[] { "AgencyId" });
            DropIndex("dbo.AiJiaXi__UserAccount", new[] { "Id" });
            DropIndex("dbo.AiJiaXi__AccountRecord", new[] { "UserAccountId" });
            DropTable("dbo.AiJiaXi__ApplicationRole");
            DropTable("dbo.ApplicationRoleNavbar");
            DropTable("dbo.AiJiaXi__Withdrawals");
            DropTable("dbo.AiJiaXi__UserLocation");
            DropTable("dbo.AiJiaXi__SmsConfig");
            DropTable("dbo.AiJiaXi__PromoterInfo");
            DropTable("dbo.AiJiaXi__PreWithdrawals");
            DropTable("dbo.AiJiaXi__OperationLog");
            DropTable("dbo.AiJiaXi__NewsMain");
            DropTable("dbo.AiJiaXi__NewsClass");
            DropTable("dbo.AiJiaXi__LoginLog");
            DropTable("dbo.AiJiaXi__JoinApplication");
            DropTable("dbo.AiJiaXi__Feedback");
            DropTable("dbo.AiJiaXi__EmailConfig");
            DropTable("dbo.AiJiaXi__ImageEntity");
            DropTable("dbo.AiJiaXi__BizPartner");
            DropTable("dbo.AiJiaXi__Navbar");
            DropTable("dbo.AspNetRoles");
            DropTable("dbo.AiJiaXi__Voucher");
            DropTable("dbo.AspNetUserRoles");
            DropTable("dbo.AspNetUserLogins");
            DropTable("dbo.AiJiaXi__Event");
            DropTable("dbo.AiJiaXi__EventAward");
            DropTable("dbo.AspNetUserClaims");
            DropTable("dbo.AiJiaXi__Province");
            DropTable("dbo.AiJiaXi__UserAddress");
            DropTable("dbo.AiJiaXi__OrderRate");
            DropTable("dbo.AiJiaXi__City");
            DropTable("dbo.AiJiaXi__OrderItemClass");
            DropTable("dbo.AiJiaXi__OrderItem");
            DropTable("dbo.AiJiaXi__OrderImage");
            DropTable("dbo.AiJiaXi__CartItem");
            DropTable("dbo.AiJiaXi__Order");
            DropTable("dbo.AiJiaXi__OrderStep");
            DropTable("dbo.AiJiaXi__Employee");
            DropTable("dbo.AiJiaXi__County");
            DropTable("dbo.AiJiaXi__Agency");
            DropTable("dbo.AspNetUsers");
            DropTable("dbo.AiJiaXi__UserAccount");
            DropTable("dbo.AiJiaXi__AccountRecord");
        }
    }
}
