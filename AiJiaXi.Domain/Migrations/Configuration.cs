using System.Collections.Generic;
using Project.Domain.Entities;
using Project.Domain.Entities.Configs;
using Project.Domain.Entities.IdentityModel;
using Project.Domain.Entities.Orders;
using Project.Domain.Enums;
using Project.Domain.Repositories.Impl;
using Project.Domain.Repositories.Interface;

namespace Project.Domain.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<Project.Domain.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(Project.Domain.ApplicationDbContext context)
        {
            return;
            // 添加初始化数据;

            #region 后台菜单数据初始化
            var navbarRepository = new Repository<Navbar>();

            #region 顶级菜单初始化
            var navbars = new List<Navbar>()
            {
                new Navbar()
                {
                    NameOption = "业务数据统计",
                    Controller = string.Empty,
                    Action = string.Empty,
                    Area = string.Empty,
                    ImageClass = string.Empty,
                    Status = true,
                    Order = 0,
                    IsParent = true
                },
                new Navbar()
                {
                    NameOption = "会员管理",
                    Controller = string.Empty,
                    Action = string.Empty,
                    Area = string.Empty,
                    ImageClass = string.Empty,
                    Status = true,
                    Order = 1,
                    IsParent = true
                },
                new Navbar()
                {
                    NameOption = "订单管理",
                    Controller = string.Empty,
                    Action = string.Empty,
                    Area = string.Empty,
                    ImageClass = string.Empty,
                    Status = true,
                    Order = 2,
                    IsParent = true
                },
                new Navbar()
                {
                    NameOption = "经营项目管理",
                    Controller = string.Empty,
                    Action = string.Empty,
                    Area = string.Empty,
                    ImageClass = string.Empty,
                    Status = true,
                    Order = 3,
                    IsParent = true
                },
                new Navbar()
                {
                    NameOption = "加盟商管理",
                    Controller = string.Empty,
                    Action = string.Empty,
                    Area = string.Empty,
                    ImageClass = string.Empty,
                    Status = true,
                    Order = 4,
                    IsParent = true
                },
                /*new Navbar()
                {
                    NameOption = "信息发布管理",
                    Controller = string.Empty,
                    Action = string.Empty,
                    Area = string.Empty,
                    ImageClass = string.Empty,
                    Status = true,
                    Order = 5,
                    IsParent = true
                },*/
                new Navbar()
                {
                    NameOption = "招商信息管理",
                    Controller = string.Empty,
                    Action = string.Empty,
                    Area = string.Empty,
                    ImageClass = string.Empty,
                    Status = true,
                    Order = 6,
                    IsParent = true
                },                
                 new Navbar()
                {
                    NameOption = "我的推广信息",
                    Controller = string.Empty,
                    Action = string.Empty,
                    Area = string.Empty,
                    ImageClass = string.Empty,
                    Status = true,
                    Order = 7,
                    IsParent = true
                },
                new Navbar()
                {
                    NameOption = "推广信息管理",
                    Controller = string.Empty,
                    Action = string.Empty,
                    Area = string.Empty,
                    ImageClass = string.Empty,
                    Status = true,
                    Order = 8,
                    IsParent = true
                },
                 new Navbar()
                {
                    NameOption = "管理员管理",
                    Controller = string.Empty,
                    Action = string.Empty,
                    Area = string.Empty,
                    ImageClass = string.Empty,
                    Status = true,
                    Order = 9,
                    IsParent = true
                },
                new Navbar()
                {
                    NameOption = "系统管理",
                    Controller = string.Empty,
                    Action = string.Empty,
                    Area = string.Empty,
                    ImageClass = string.Empty,
                    Status = true,
                    Order = 10,
                    IsParent = true
                }
            };

            navbars.ForEach(item => navbarRepository.AddOrUpdate(o => o.NameOption, item)); 
            #endregion

            #region 业务数据统计下子菜单初始化
            var bizSummary = navbarRepository.Find(item => item.NameOption == "业务数据统计");
            navbars = new List<Navbar>()
            {
                /*new Navbar()
                {
                    NameOption = "完成订单数量统计",
                    Controller = "Summary",
                    Action = "Orders",
                    Area = "Admin",
                    ImageClass = string.Empty,
                    Status = true,
                    Order = 1,
                    IsParent = false,
                    ParentId = bizSummary.Id
                },*/
                new Navbar()
                {
                    NameOption = "月营业额统计",
                    Controller = "Summary",
                    Action = "Turnover",
                    Area = "Admin",
                    ImageClass = string.Empty,
                    Status = true,
                    Order = 2,
                    IsParent = false,
                    ParentId = bizSummary.Id
                },
               /* new Navbar()
                {
                    NameOption = "产品下单统计",
                    Controller = "Summary",
                    Action = "Products",
                    Area = "Admin",
                    ImageClass = string.Empty,
                    Status = true,
                    Order = 3,
                    IsParent = false,
                    ParentId = bizSummary.Id
                }*/
            };
            navbars.ForEach(item => navbarRepository.AddOrUpdate(o => o.NameOption, item)); 
            #endregion

            #region 会员管理下子菜单初始化
            var usersM = navbarRepository.Find(item => item.NameOption == "会员管理");
            navbars = new List<Navbar>()
            {
                new Navbar()
                {
                    NameOption = "添加新会员",
                    Controller = "Users",
                    Action = "Manage",
                    Area = "Admin",
                    ImageClass = string.Empty,
                    Status = true,
                    Order = 1,
                    IsParent = false,
                    ParentId = usersM.Id
                },
                new Navbar()
                {
                    NameOption = "会员列表",
                    Controller = "Users",
                    Action = "Index",
                    Area = "Admin",
                    ImageClass = string.Empty,
                    Status = true,
                    Order = 2,
                    IsParent = false,
                    ParentId = usersM.Id
                },
                new Navbar()
                {
                    NameOption = "冻结会员列表",
                    Controller = "Users",
                    Action = "Frozen",
                    Area = "Admin",
                    ImageClass = string.Empty,
                    Status = true,
                    Order = 3,
                    IsParent = false,
                    ParentId = usersM.Id
                }
            };
            navbars.ForEach(item => navbarRepository.AddOrUpdate(o => o.NameOption, item)); 
            #endregion

            #region 订单管理订单管理下子菜单初始化
            var orderManages = navbarRepository.Find(item => item.NameOption == "订单管理");
            navbars = new List<Navbar>()
            {
                new Navbar()
                {
                    NameOption = "全部订单",
                    Controller = "Orders",
                    Action = "Index",
                    Area = "Admin",
                    ImageClass = string.Empty,
                    Status = true,
                    Order = 0,
                    IsParent = false,
                    ParentId = orderManages.Id
                },
                new Navbar()
                {
                    NameOption = "待支付订单",
                    Controller = "Orders",
                    Action = "Topay",
                    Area = "Admin",
                    ImageClass = string.Empty,
                    Status = true,
                    Order = 1,
                    IsParent = false,
                    ParentId = orderManages.Id
                },
                new Navbar()
                {
                    NameOption = "进行中订单",
                    Controller = "Orders",
                    Action = "Going",
                    Area = "Admin",
                    ImageClass = string.Empty,
                    Status = true,
                    Order = 2,
                    IsParent = false,
                    ParentId = orderManages.Id
                },
                new Navbar()
                {
                    NameOption = "已完成订单",
                    Controller = "Orders",
                    Action = "Complete",
                    Area = "Admin",
                    ImageClass = string.Empty,
                    Status = true,
                    Order = 3,
                    IsParent = false,
                    ParentId = orderManages.Id
                },
                new Navbar()
                {
                    NameOption = "已取消订单",
                    Controller = "Orders",
                    Action = "Cancel",
                    Area = "Admin",
                    ImageClass = string.Empty,
                    Status = true,
                    Order = 4,
                    IsParent = false,
                    ParentId = orderManages.Id
                },
                new Navbar()
                {
                    NameOption = "订单退款",
                    Controller = "Orders",
                    Action = "Refund",
                    Area = "Admin",
                    ImageClass = string.Empty,
                    Status = true,
                    Order = 5,
                    IsParent = false,
                    ParentId = orderManages.Id
                }
            };
            navbars.ForEach(item => navbarRepository.AddOrUpdate(o => o.NameOption, item)); 
            #endregion

            #region 经营项目下子菜单初始化
            var product = navbarRepository.Find(item => item.NameOption == "经营项目管理");
            navbars = new List<Navbar>()
            {
                new Navbar()
                {
                    NameOption = "添加产品类别",
                    Controller = "ProductClasses",
                    Action = "Manage",
                    Area = "Admin",
                    ImageClass = string.Empty,
                    Status = true,
                    Order = 1,
                    IsParent = false,
                    ParentId = product.Id
                },
                new Navbar()
                {
                    NameOption = "产品类别管理",
                    Controller = "ProductClasses",
                    Action = "Index",
                    Area = "Admin",
                    ImageClass = string.Empty,
                    Status = true,
                    Order = 2,
                    IsParent = false,
                    ParentId = product.Id
                },
                new Navbar()
                {
                    NameOption = "发布新活动",
                    Controller = "Events",
                    Action = "Manage",
                    Area = "Admin",
                    ImageClass = string.Empty,
                    Status = true,
                    Order = 3,
                    IsParent = false,
                    ParentId = product.Id
                },
                new Navbar()
                {
                    NameOption = "活动管理",
                    Controller = "Events",
                    Action = "Index",
                    Area = "Admin",
                    ImageClass = string.Empty,
                    Status = true,
                    Order = 4,
                    IsParent = false,
                    ParentId = product.Id
                },
                new Navbar()
                {
                    NameOption = "生成代金券",
                    Controller = "Voucher",
                    Action = "Generate",
                    Area = "Admin",
                    ImageClass = string.Empty,
                    Status = true,
                    Order = 5,
                    IsParent = false,
                    ParentId = product.Id
                },
                new Navbar()
                {
                    NameOption = "回收站",
                    Controller = "RecycleBin",
                    Action = "Products",
                    Area = "Admin",
                    ImageClass = string.Empty,
                    Status = true,
                    Order = 6,
                    IsParent = false,
                    ParentId = product.Id
                }
            };
            navbars.ForEach(item => navbarRepository.AddOrUpdate(o => o.NameOption, item));
            #endregion

            #region 加盟商管理下子菜单初始化
            var joiners = navbarRepository.Find(item => item.NameOption == "加盟商管理");
            navbars = new List<Navbar>()
            {
                new Navbar()
                {
                    NameOption = "添加加盟商",
                    Controller = "Agency",
                    Action = "Manage",
                    Area = "Admin",
                    ImageClass = string.Empty,
                    Status = true,
                    Order = 1,
                    IsParent = false,
                    ParentId = joiners.Id
                },
                new Navbar()
                {
                    NameOption = "加盟商列表",
                    Controller = "Agency",
                    Action = "Index",
                    Area = "Admin",
                    ImageClass = string.Empty,
                    Status = true,
                    Order = 2,
                    IsParent = false,
                    ParentId = joiners.Id
                },
                new Navbar()
                {
                    NameOption = "区域查看",
                    Controller = "Location",
                    Action = "Index",
                    Area = "Admin",
                    ImageClass = string.Empty,
                    Status = true,
                    Order = 2,
                    IsParent = false,
                    ParentId = joiners.Id
                }
            };
            navbars.ForEach(item => navbarRepository.AddOrUpdate(o => o.NameOption, item)); 
            #endregion

            #region 信息发布管理下子菜单
           /* var news = navbarRepository.Find(item => item.NameOption == "信息发布管理");
            navbars = new List<Navbar>()
            {
                new Navbar()
                {
                    NameOption = "添加新闻类别",
                    Controller = "NewsClass",
                    Action = "Manage",
                    Area = "Admin",
                    ImageClass = string.Empty,
                    Status = true,
                    Order = 1,
                    IsParent = false,
                    ParentId = news.Id
                },
                new Navbar()
                {
                    NameOption = "新闻类别管理",
                    Controller = "NewsClass",
                    Action = "Index",
                    Area = "Admin",
                    ImageClass = string.Empty,
                    Status = true,
                    Order = 2,
                    IsParent = false,
                    ParentId = news.Id
                },
                new Navbar()
                {
                    NameOption = "回收站",
                    Controller = "RecycleBin",
                    Action = "News",
                    Area = "Admin",
                    ImageClass = string.Empty,
                    Status = true,
                    Order = 3,
                    IsParent = false,
                    ParentId = news.Id
                }
            };
            navbars.ForEach(item => navbarRepository.AddOrUpdate(o => o.NameOption, item)); */
            #endregion

            #region 招商信息管理下子菜单初始化
            var joinApp = navbarRepository.Find(item => item.NameOption == "招商信息管理");
            navbars = new List<Navbar>()
            {
                new Navbar()
                {
                    NameOption = "申请加盟列表",
                    Controller = "JoinApplications",
                    Action = "Index",
                    Area = "Admin",
                    ImageClass = string.Empty,
                    Status = true,
                    Order = 1,
                    IsParent = false,
                    ParentId = joinApp.Id
                },
                new Navbar()
                {
                    NameOption = "添加合作伙伴",
                    Controller = "BizPartner",
                    Action = "Manage",
                    Area = "Admin",
                    ImageClass = string.Empty,
                    Status = true,
                    Order = 2,
                    IsParent = false,
                    ParentId = joinApp.Id
                },
                new Navbar()
                {
                    NameOption = "合作伙伴列表",
                    Controller = "BizPartner",
                    Action = "Index",
                    Area = "Admin",
                    ImageClass = string.Empty,
                    Status = true,
                    Order = 3,
                    IsParent = false,
                    ParentId = joinApp.Id
                }
            };
            navbars.ForEach(item => navbarRepository.AddOrUpdate(o => o.NameOption, item));
            #endregion

            #region 我的推广信息下子菜单初始化
            var MywidthdrawalsInfo = navbarRepository.Find(item => item.NameOption == "我的推广信息");
            navbars = new List<Navbar>()
            {
                new Navbar()
                {
                    NameOption = "我的推广",
                    Controller = "PromoterInfo",
                    Action = "Index",
                    Area = "Admin",
                    ImageClass = string.Empty,
                    Status = true,
                    Order = 1,
                    IsParent = false,
                    ParentId = MywidthdrawalsInfo.Id
                },
                new Navbar()
                {
                    NameOption = "我的提现记录",
                    Controller = "Withdrawals",
                    Action = "Index",
                    Area = "Admin",
                    ImageClass = string.Empty,
                    Status = true,
                    Order = 2,
                    IsParent = false,
                    ParentId = MywidthdrawalsInfo.Id
                }
               
            };
            navbars.ForEach(item => navbarRepository.AddOrUpdate(o => o.NameOption, item));
            #endregion

            #region 总后台推广信息管理下子菜单初始化
            var widthdrawalsManager = navbarRepository.Find(item => item.NameOption == "推广信息管理");
            navbars = new List<Navbar>()
            {
                new Navbar()
                {
                    NameOption = "推广员管理",
                    Controller = "PromotionManager",
                    Action = "Index",
                    Area = "Admin",
                    ImageClass = string.Empty,
                    Status = true,
                    Order = 1,
                    IsParent = false,
                    ParentId = widthdrawalsManager.Id
                },
                new Navbar()
                {
                    NameOption = "推广员提现记录",
                    Controller = "PromoterWithdrawals",
                    Action = "Index",
                    Area = "Admin",
                    ImageClass = string.Empty,
                    Status = true,
                    Order = 2,
                    IsParent = false,
                    ParentId = widthdrawalsManager.Id
                }

            };
            navbars.ForEach(item => navbarRepository.AddOrUpdate(o => o.NameOption, item));
            #endregion

            #region 管理员管理下子菜单初始化
            var admins = navbarRepository.Find(item => item.NameOption == "管理员管理");
            navbars = new List<Navbar>()
            {
                new Navbar()
                {
                    NameOption = "添加管理员",
                    Controller = "Admin",
                    Action = "Admin",
                    Area = "Admin",
                    ImageClass = string.Empty,
                    Status = true,
                    Order = 1,
                    IsParent = false,
                    ParentId = admins.Id
                },
                new Navbar()
                {
                    NameOption = "添加推广员",
                    Controller = "Admin",
                    Action = "Withdrawals",
                    Area = "Admin",
                    ImageClass = string.Empty,
                    Status = true,
                    Order = 2,
                    IsParent = false,
                    ParentId = admins.Id
                },
                new Navbar()
                {
                    NameOption = "添加代理商",
                    Controller = "Admin",
                    Action = $"Agency",
                    Area = "Admin",
                    ImageClass = string.Empty,
                    Status = true,
                    Order = 3,
                    IsParent = false,
                    ParentId = admins.Id
                },
                new Navbar()
                {
                    NameOption = "添加洗涤公司",
                    Controller = "Admin",
                    Action = $"Washers",
                    Area = "Admin",
                    ImageClass = string.Empty,
                    Status = true,
                    Order = 5,
                    IsParent = false,
                    ParentId = admins.Id
                },
                new Navbar()
                {
                    NameOption = "管理员列表",
                    Controller = "Admin",
                    Action = "Index",
                    Area = "Admin",
                    ImageClass = string.Empty,
                    Status = true,
                    Order = 6,
                    IsParent = false,
                    ParentId = admins.Id
                },
                new Navbar()
                {
                    NameOption = "管理员角色",
                    Controller = "Roles",
                    Action = "Index",
                    Area = "Admin",
                    ImageClass = string.Empty,
                    Status = true,
                    Order = 7,
                    IsParent = false,
                    ParentId = admins.Id
                }
            };
            navbars.ForEach(item => navbarRepository.AddOrUpdate(o => o.NameOption, item));
            #endregion

            #region 系统管理菜单下子菜单初始化
            var sys = navbarRepository.Find(item => item.NameOption == "系统管理");
            navbars = new List<Navbar>()
            {
                new Navbar()
                {
                    NameOption = "菜单管理",
                    Controller = "Navbars",
                    Action = "Index",
                    Area = "Admin",
                    ImageClass = string.Empty,
                    Status = true,
                    Order = 1,
                    IsParent = false,
                    ParentId = sys.Id
                },
                new Navbar()
                {
                    NameOption = "系统配置管理",
                    Controller = "Config",
                    Action = "Index",
                    Area = "Admin",
                    ImageClass = string.Empty,
                    Status = true,
                    Order = 1,
                    IsParent = false,
                    ParentId = sys.Id
                },
                new Navbar()
                {
                    NameOption = "行为日志查看",
                    Controller = "OperationLog",
                    Action = "Index",
                    Area = "Admin",
                    ImageClass = string.Empty,
                    Status = true,
                    Order = 2,
                    IsParent = false,
                    ParentId = sys.Id
                },
                new Navbar()
                {
                    NameOption = "登录日志查看",
                    Controller = "LoginLog",
                    Action = "Index",
                    Area = "Admin",
                    ImageClass = string.Empty,
                    Status = true,
                    Order = 2,
                    IsParent = false,
                    ParentId = sys.Id
                },
                /*new Navbar()
                {
                    NameOption = "定时任务管理",
                    Controller = "Task",
                    Action = "Index",
                    Area = "Admin",
                    ImageClass = string.Empty,
                    Status = true,
                    Order = 3,
                    IsParent = false,
                    ParentId = sys.Id
                },
                new Navbar()
                {
                    NameOption = "数据库备份还原",
                    Controller = "Database",
                    Action = "Index",
                    Area = "Admin",
                    ImageClass = string.Empty,
                    Status = true,
                    Order = 3,
                    IsParent = false,
                    ParentId = sys.Id
                }*/
            };
            navbars.ForEach(item => navbarRepository.AddOrUpdate(o => o.NameOption, item));
            #endregion
            #endregion

            #region 短信配置
            Repository<SmsConfig> _smsConfigRepository = new Repository<SmsConfig>();
            var config = new SmsConfig()
            {
                EntId = string.Empty,
                Account = "AA00250",
                Password = "AA0025058",
                ExtNo = String.Empty,
                SmsSendUrl = "http://dx.ipyy.net/sms.aspx",
            };

            _smsConfigRepository.AddOrUpdate(o => o.Account, config);
            #endregion

            #region 产品初始化
            // 添加产品类别及产品
            /*Repository<OrderItemClass> _orderItemClassRepository = new Repository<OrderItemClass>();
            var orderItemClasses = new List<OrderItemClass>()
            {
                new OrderItemClass()
                {
                    Name = "衣物",
                    IconUrl = "",
                    CityId = 210200,
                    OrderItems = new List<OrderItem>()
                    {
                        new OrderItem()
                        {
                            Name = "配饰类",
                            Desc = "领带/领结/围巾/帽子/手套",
                            ImageUrl = "",
                            Price = 8,
                            Nums = 1,
                            Days = 3,
                            AddTime = DateTime.Now,
                            ModifyTime = DateTime.Now,
                            IsValid = true
                        },
                        new OrderItem()
                        {
                            Name = "小件",
                            Desc = "衬衣/T恤/西裤/牛仔裤",
                            ImageUrl = "",
                            Price = 12,
                            Nums = 1,
                            Days = 3,
                            AddTime = DateTime.Now,
                            ModifyTime = DateTime.Now,
                            IsValid = true
                        },
                        new OrderItem()
                        {
                            Name = "中件",
                            Desc = "西装上衣/连衣裙",
                            ImageUrl = "",
                            Price = 18,
                            Nums = 1,
                            Days = 5,
                            AddTime = DateTime.Now,
                            ModifyTime = DateTime.Now,
                            IsValid = true
                        },
                        new OrderItem()
                        {
                            Name = "大件",
                            Desc = "风衣类/大衣类",
                            ImageUrl = "",
                            Price = 28,
                            Nums = 1,
                            Days = 5,
                            AddTime = DateTime.Now,
                            ModifyTime = DateTime.Now,
                            IsValid = true
                        }
                    },
                    AddTime = DateTime.Now,
                    ModifyTime = DateTime.Now,
                    IsValid = true
                },
                new OrderItemClass()
                {
                    Name = "家纺",
                    IconUrl = "",
                    CityId = 210200,
                    OrderItems = new List<OrderItem>()
                    {
                        new OrderItem()
                        {
                            Name = "小件",
                            Desc = "毛巾/浴巾",
                            ImageUrl = "",
                            Price = 18,
                            Nums = 1,
                            Days = 3,
                            AddTime = DateTime.Now,
                            ModifyTime = DateTime.Now,
                            IsValid = true
                        },
                        new OrderItem()
                        {
                            Name = "中件",
                            Desc = "双人被套/双人床单",
                            ImageUrl = "",
                            Price = 28,
                            Nums = 1,
                            Days = 5,
                            AddTime = DateTime.Now,
                            ModifyTime = DateTime.Now,
                            IsValid = true
                        },
                        new OrderItem()
                        {
                            Name = "大件",
                            Desc = "沙发套/窗帘",
                            ImageUrl = "",
                            Price = 38,
                            Nums = 1,
                            Days = 5,
                            AddTime = DateTime.Now,
                            ModifyTime = DateTime.Now,
                            IsValid = true
                        }
                    },
                    AddTime = DateTime.Now,
                    ModifyTime = DateTime.Now,
                    IsValid = true
                }
            }; ;

            orderItemClasses.ForEach(item => _orderItemClassRepository.AddOrUpdate(o => o.Name, item));*/
            #endregion

            #region 代理商初始化
            /*IRepository<Agency> _agencyRepository = new Repository<Agency>();
            var agencies = new List<Agency>()
            {
                new Agency()
                {
                    Name = "app-杨瑞",
                    CityId = 210200,
                    CountyId = 210204,
                    ProvinceId = 210000,
                    Note = "测试数据",
                    Contact = "杨瑞",
                    ContactMobile = "18878777877",
                    ContactEmail = "yangrui@baixime.com",
                    IsValid = false
                },
                new Agency()
                {
                    Name = "app-梁超",
                    CityId = 210200,
                    CountyId = 210202,
                    ProvinceId = 210000,
                    Note = "测试数据",
                    Contact = "梁超",
                    ContactMobile = "13388899899",
                    ContactEmail = "chao@baixime.com",
                    IsValid = false
                }
            };

            agencies.ForEach(item => _agencyRepository.AddOrUpdate(o => o.Name, item));*/
            #endregion

            #region 代理申请加盟初始化
            IRepository<JoinApplication> joinAppRepository = new Repository<JoinApplication>();
            var joinApps = new List<JoinApplication>()
            {
                new JoinApplication()
                {
                    Name = "张三",
                    Mobile = "18878777877",
                    Email = "zhang3@baixime.com",
                    Area = "中山区",
                    JoinType = "加盟合作",
                    RiseTime = DateTime.Now,
                    FeedbackStatus = FeedbackStatus.UnDealed
                },
                new JoinApplication()
                {
                    Name = "李四",
                    Mobile = "13388899899",
                    Email = "li4@baixime.com",
                    Area = "西岗区",
                    JoinType = "合作加盟",
                    RiseTime = DateTime.Now,
                    FeedbackStatus = FeedbackStatus.UnDealed
                }
            };

            joinApps.ForEach(item => joinAppRepository.AddOrUpdate(o => o.Name, item));
            #endregion
        }
    }
}
