using System;
using System.Data.Entity;
using Project.Domain.Configs;
using Project.Domain.Entities;
using Project.Domain.Entities.Configs;
using Project.Domain.Entities.IdentityModel;
using Project.Domain.Entities.Location;
using Project.Domain.Entities.Logs;
using Project.Domain.Entities.News;
using Project.Domain.Entities.Orders;
using Project.Domain.Entities.UserProfile;
using Microsoft.AspNet.Identity.EntityFramework;
using ZhiYuan.IAR.Repository.EF;
using Project.Domain.Entities.PromoterManager;

namespace Project.Domain
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        static ApplicationDbContext()
        {
            System.Data.Entity.Database.SetInitializer<ApplicationDbContext>(null);
        }

        public static ApplicationDbContext Create()
        {
            // return new ApplicationDbContext();

            return ContextFactory.GetCurrentContext();
        }

        public DbSet<ApplicationRole> ApplicationRoles { get; set; }

        public DbSet<Navbar> Navbars { get; set; }

        public DbSet<Order> Orders { get; set; }

        public DbSet<OrderItemClass> OrderItemClasses { get; set; }

        public DbSet<OrderItem> OrderItems { get; set; }

        public DbSet<OrderImage> OrderImages { get; set; }

        public DbSet<CartItem> CartItems { get; set; }

        public DbSet<OrderRate> OrderRates { get; set; }

        public DbSet<OrderStep> OrderSteps { get; set; }

        public DbSet<Event> Events { get; set; }

        public DbSet<EventAward> EventAwards { get; set; }

        public DbSet<EventPrize> EventPrizes { get; set; }

        public DbSet<UserAccount> UserAccounts { get; set; }

        public DbSet<UserAddress> UserAddresses { get; set; }

        public DbSet<Employee> Employees { get; set; }

        public DbSet<Voucher> Vouchers { get; set; }

        public DbSet<AccountRecord> AccountRecords { get; set; }

        public DbSet<Agency> Agencies { get; set; }

        #region 系统日志相关
        public DbSet<LoginLog> LoginLogs { get; set; }

        public DbSet<OperationLog> OperationLogs { get; set; }
        #endregion  

        public DbSet<JoinApplication> JoinApplications { get; set; }

        public DbSet<Feedback> Feedbacks { get; set; }

        #region 省市县
        public DbSet<Province> Provinces { get; set; }

        public DbSet<City> Cities { get; set; }

        public DbSet<County> Counties { get; set; }
        #endregion

        public DbSet<SmsConfig> SmsConfigs { get; set; }

        public DbSet<EmailConfig> EmailConfigs { get; set; }

        public DbSet<BizPartner> BizPartners { get; set; }

        public DbSet<NewsClass> NewsClasses { get; set; }

        public DbSet<NewsMain> NewsMains { get; set; }

        public DbSet<PromoterInfo> PromoterInfos { get; set; }

        public DbSet<Withdrawals> Withdrawalss { get; set; }

        public DbSet<PreWithdrawals> PreWithdrawalss { get; set; }

        public DbSet<UserLocation> UserLocations { get; set; }
        

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            if (modelBuilder == null)
            {
                throw new ArgumentNullException();
            }
            base.OnModelCreating(modelBuilder);

            modelBuilder.Types().Configure(item => item.ToTable(string.Format("{0}_{1}", "AiJiaXi_", item.ClrType.Name)));

            modelBuilder.Configurations.Add(new NewsClassConfig());
            modelBuilder.Configurations.Add(new NewsMainConfig());

            modelBuilder.Entity<Navbar>()
                .HasMany(item => item.ApplicationRoles)
                .WithMany(r => r.Navbars)
                .Map(
                    m =>
                    {
                        m.MapLeftKey("NavbarId");
                        m.MapRightKey("ApplicationRoleId");
                        m.ToTable("ApplicationRoleNavbar");
                    });

            modelBuilder.Entity<Event>()
               .HasMany(item => item.Agencies)
               .WithMany(r => r.Events)
               .Map(
                   m =>
                   {
                       m.MapLeftKey("EventId");
                       m.MapRightKey("AgencyId");
                       m.ToTable("AgenciesEvents");
                   });
        }
    }
}