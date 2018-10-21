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
            // ��ӳ�ʼ������;

            #region ��̨�˵����ݳ�ʼ��
            var navbarRepository = new Repository<Navbar>();

            #region �����˵���ʼ��
            var navbars = new List<Navbar>()
            {
                new Navbar()
                {
                    NameOption = "ҵ������ͳ��",
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
                    NameOption = "��Ա����",
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
                    NameOption = "��������",
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
                    NameOption = "��Ӫ��Ŀ����",
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
                    NameOption = "�����̹���",
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
                    NameOption = "��Ϣ��������",
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
                    NameOption = "������Ϣ����",
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
                    NameOption = "�ҵ��ƹ���Ϣ",
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
                    NameOption = "�ƹ���Ϣ����",
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
                    NameOption = "����Ա����",
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
                    NameOption = "ϵͳ����",
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

            #region ҵ������ͳ�����Ӳ˵���ʼ��
            var bizSummary = navbarRepository.Find(item => item.NameOption == "ҵ������ͳ��");
            navbars = new List<Navbar>()
            {
                /*new Navbar()
                {
                    NameOption = "��ɶ�������ͳ��",
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
                    NameOption = "��Ӫҵ��ͳ��",
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
                    NameOption = "��Ʒ�µ�ͳ��",
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

            #region ��Ա�������Ӳ˵���ʼ��
            var usersM = navbarRepository.Find(item => item.NameOption == "��Ա����");
            navbars = new List<Navbar>()
            {
                new Navbar()
                {
                    NameOption = "����»�Ա",
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
                    NameOption = "��Ա�б�",
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
                    NameOption = "�����Ա�б�",
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

            #region �����������������Ӳ˵���ʼ��
            var orderManages = navbarRepository.Find(item => item.NameOption == "��������");
            navbars = new List<Navbar>()
            {
                new Navbar()
                {
                    NameOption = "ȫ������",
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
                    NameOption = "��֧������",
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
                    NameOption = "�����ж���",
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
                    NameOption = "����ɶ���",
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
                    NameOption = "��ȡ������",
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
                    NameOption = "�����˿�",
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

            #region ��Ӫ��Ŀ���Ӳ˵���ʼ��
            var product = navbarRepository.Find(item => item.NameOption == "��Ӫ��Ŀ����");
            navbars = new List<Navbar>()
            {
                new Navbar()
                {
                    NameOption = "��Ӳ�Ʒ���",
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
                    NameOption = "��Ʒ������",
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
                    NameOption = "�����»",
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
                    NameOption = "�����",
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
                    NameOption = "���ɴ���ȯ",
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
                    NameOption = "����վ",
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

            #region �����̹������Ӳ˵���ʼ��
            var joiners = navbarRepository.Find(item => item.NameOption == "�����̹���");
            navbars = new List<Navbar>()
            {
                new Navbar()
                {
                    NameOption = "��Ӽ�����",
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
                    NameOption = "�������б�",
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
                    NameOption = "����鿴",
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

            #region ��Ϣ�����������Ӳ˵�
           /* var news = navbarRepository.Find(item => item.NameOption == "��Ϣ��������");
            navbars = new List<Navbar>()
            {
                new Navbar()
                {
                    NameOption = "����������",
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
                    NameOption = "����������",
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
                    NameOption = "����վ",
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

            #region ������Ϣ�������Ӳ˵���ʼ��
            var joinApp = navbarRepository.Find(item => item.NameOption == "������Ϣ����");
            navbars = new List<Navbar>()
            {
                new Navbar()
                {
                    NameOption = "��������б�",
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
                    NameOption = "��Ӻ������",
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
                    NameOption = "��������б�",
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

            #region �ҵ��ƹ���Ϣ���Ӳ˵���ʼ��
            var MywidthdrawalsInfo = navbarRepository.Find(item => item.NameOption == "�ҵ��ƹ���Ϣ");
            navbars = new List<Navbar>()
            {
                new Navbar()
                {
                    NameOption = "�ҵ��ƹ�",
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
                    NameOption = "�ҵ����ּ�¼",
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

            #region �ܺ�̨�ƹ���Ϣ�������Ӳ˵���ʼ��
            var widthdrawalsManager = navbarRepository.Find(item => item.NameOption == "�ƹ���Ϣ����");
            navbars = new List<Navbar>()
            {
                new Navbar()
                {
                    NameOption = "�ƹ�Ա����",
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
                    NameOption = "�ƹ�Ա���ּ�¼",
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

            #region ����Ա�������Ӳ˵���ʼ��
            var admins = navbarRepository.Find(item => item.NameOption == "����Ա����");
            navbars = new List<Navbar>()
            {
                new Navbar()
                {
                    NameOption = "��ӹ���Ա",
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
                    NameOption = "����ƹ�Ա",
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
                    NameOption = "��Ӵ�����",
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
                    NameOption = "���ϴ�ӹ�˾",
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
                    NameOption = "����Ա�б�",
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
                    NameOption = "����Ա��ɫ",
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

            #region ϵͳ����˵����Ӳ˵���ʼ��
            var sys = navbarRepository.Find(item => item.NameOption == "ϵͳ����");
            navbars = new List<Navbar>()
            {
                new Navbar()
                {
                    NameOption = "�˵�����",
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
                    NameOption = "ϵͳ���ù���",
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
                    NameOption = "��Ϊ��־�鿴",
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
                    NameOption = "��¼��־�鿴",
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
                    NameOption = "��ʱ�������",
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
                    NameOption = "���ݿⱸ�ݻ�ԭ",
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

            #region ��������
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

            #region ��Ʒ��ʼ��
            // ��Ӳ�Ʒ��𼰲�Ʒ
            /*Repository<OrderItemClass> _orderItemClassRepository = new Repository<OrderItemClass>();
            var orderItemClasses = new List<OrderItemClass>()
            {
                new OrderItemClass()
                {
                    Name = "����",
                    IconUrl = "",
                    CityId = 210200,
                    OrderItems = new List<OrderItem>()
                    {
                        new OrderItem()
                        {
                            Name = "������",
                            Desc = "���/���/Χ��/ñ��/����",
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
                            Name = "С��",
                            Desc = "����/T��/����/ţ�п�",
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
                            Name = "�м�",
                            Desc = "��װ����/����ȹ",
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
                            Name = "���",
                            Desc = "������/������",
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
                    Name = "�ҷ�",
                    IconUrl = "",
                    CityId = 210200,
                    OrderItems = new List<OrderItem>()
                    {
                        new OrderItem()
                        {
                            Name = "С��",
                            Desc = "ë��/ԡ��",
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
                            Name = "�м�",
                            Desc = "˫�˱���/˫�˴���",
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
                            Name = "���",
                            Desc = "ɳ����/����",
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

            #region �����̳�ʼ��
            /*IRepository<Agency> _agencyRepository = new Repository<Agency>();
            var agencies = new List<Agency>()
            {
                new Agency()
                {
                    Name = "app-����",
                    CityId = 210200,
                    CountyId = 210204,
                    ProvinceId = 210000,
                    Note = "��������",
                    Contact = "����",
                    ContactMobile = "18878777877",
                    ContactEmail = "yangrui@baixime.com",
                    IsValid = false
                },
                new Agency()
                {
                    Name = "app-����",
                    CityId = 210200,
                    CountyId = 210202,
                    ProvinceId = 210000,
                    Note = "��������",
                    Contact = "����",
                    ContactMobile = "13388899899",
                    ContactEmail = "chao@baixime.com",
                    IsValid = false
                }
            };

            agencies.ForEach(item => _agencyRepository.AddOrUpdate(o => o.Name, item));*/
            #endregion

            #region ����������˳�ʼ��
            IRepository<JoinApplication> joinAppRepository = new Repository<JoinApplication>();
            var joinApps = new List<JoinApplication>()
            {
                new JoinApplication()
                {
                    Name = "����",
                    Mobile = "18878777877",
                    Email = "zhang3@baixime.com",
                    Area = "��ɽ��",
                    JoinType = "���˺���",
                    RiseTime = DateTime.Now,
                    FeedbackStatus = FeedbackStatus.UnDealed
                },
                new JoinApplication()
                {
                    Name = "����",
                    Mobile = "13388899899",
                    Email = "li4@baixime.com",
                    Area = "������",
                    JoinType = "��������",
                    RiseTime = DateTime.Now,
                    FeedbackStatus = FeedbackStatus.UnDealed
                }
            };

            joinApps.ForEach(item => joinAppRepository.AddOrUpdate(o => o.Name, item));
            #endregion
        }
    }
}
