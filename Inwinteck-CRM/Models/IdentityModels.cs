using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using WebPush;
using Inwinteck_CRM.Helpers;

namespace Inwinteck_CRM.Models
{
    public class UserRole : IdentityUserRole<int>
    {
    }
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        public string Name { get; set; }
       
        public string CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }

        public string ModifiedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }

        public byte Status { get; set; }

        public byte ChangePassword { get; set; }
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }
    }
    public class Role : IdentityRole<int, UserRole>
    {
        public Role() { }
        public Role(string name) { Name = name; }
    }
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }


        public DbSet<GroupHeader> GroupHeader { get; set; }
        public DbSet<HeaderDescription> HeaderDescription { get; set; }
        public DbSet<StateMaster> StateMasters { get; set; }
        public DbSet<CountryMaster> Country { get; set; }
        public DbSet<PinCodeMaster> PinCode { get; set; }
        public DbSet<CityMaster> CityMasters { get; set; }

        public DbSet<FE_Master_Personal> FE_Master_Personal { get; set; }
        public DbSet<FE_Master_Financial> FE_Master_Financial { get; set; }
        public DbSet<FE_Master_Charges> FE_Master_Charges { get; set; }
        public DbSet<FE_Master_Skill> FE_Master_Skill { get; set; }
        public DbSet<EU_Master> EU_Master { get; set; }

        public DbSet<Country_Dialing_Code> Country_Dialing_Code { get; set; }
        public DbSet<Currency_Master> Currency_Master { get; set; }
        public DbSet<Ticket> Ticket { get; set; }
        public DbSet<Ticket_History> Ticket_History { get; set; }
        public DbSet<business_component_master> component_master { get; set; }

        public DbSet<privilege_master> privilege_master { get; set; }
        public DbSet<menu_master> menu_master { get; set; }
        public DbSet<user_permission_map> user_permission_map { get; set; }
        public DbSet<Header_Invoice_FE> Header_Invoice_FE { get; set; }
        public DbSet<Header_Invoice_Detail_FE> Header_Invoice_Detail_FE { get; set; }
        public DbSet<Header_Invoice_EU> Header_Invoice_EU { get; set; }
        public DbSet<Header_Invoice_Detail_EU> Header_Invoice_Detail_EU { get; set; }
        public DbSet<Certification_Master> Certification_Master { get; set; }

        public DbSet<Part_Ticket_Data> Part_Ticket_Data { get; set; }

        public DbSet<EU_Rate_Card> EU_Rate_Card { get; set; }

        public DbSet<FE_Master_Certification> FE_Master_Certification { get; set; }

        public DbSet<FE_Master_serviceArea> FE_Master_serviceArea { get; set; }

        // Added By Praveen

        public DbSet<FE_Master_Identification> FE_Master_Identification { get; set; }
        public DbSet<EU_Master_Branch> EU_Master_Branch { get; set; }
        public DbSet<EU_Master_Contacts> EU_Master_Contacts { get; set; }

        public DbSet<Ticket_Email> Ticket_Email { get; set; }

        public DbSet<Ticket_FE_Selection> Ticket_FE_Selection { get; set; }

        public DbSet<FE_Master_Skill_Data> FE_Master_Skill_Data { get; set; }

        public DbSet<CSAT> CSAT { get; set; }

        public DbSet<Vendor_Master> Vendor_Master { get; set; }

        public DbSet<Vendor_Master_Contacts> Vendor_Master_Contacts { get; set; }

        public DbSet<IT_Customer_Contacts> IT_Customer_Contacts { get; set; }

        public DbSet<IT_Customer_Master> IT_Customer_Master { get; set; }

        public DbSet<Enq_IT> Enq_IT { get; set; }

        public DbSet<Enq_History> Enq_History { get; set; }

        public DbSet<Enq_Vendor_Email> Enq_Vendor_Email { get; set; }

        public DbSet<Enq_Customer_Email> Enq_Customer_Email { get; set; }
        public DbSet<Enq_EU> Enq_EU { get; set; }

        public DbSet<Enq_EU_History> Enq_EU_History { get; set; }
        public DbSet<Ticket_System_Info> Ticket_System_Info { get; set; }
        public DbSet<EU_Master_Sales> EU_Master_Sales { get; set; }

        public DbSet<EU_Master_Branch_Sales> EU_Master_Branch_Sales { get; set; }
        public DbSet<EU_Master_Contacts_Sales> EU_Master_Contacts_Sales { get; set; }


        public DbSet<FE_Master_Other_Detail> FE_Master_Other_Detail { get; set; }

        public DbSet<FE_Master_Certification_Extra_Detail> FE_Master_Certification_Extra_Detail { get; set; }
        public DbSet<Ticket_EU_Detail> Ticket_EU_Detail { get; set; }
        public DbSet<blkFEMaster> blkFEMaster { get; set; }
        public DbSet<Employee_Detail> Employee_Detail { get; set; }
        public DbSet<HRMS_Users> HRMS_Users { get; set; }
        public DbSet<Timesheet_Log> Timesheet_Log { get; set; }
        public DbSet<Timesheet> Timesheet { get; set; }
        public DbSet<EmailLog> EmailLogs { get; set; }

        //added by rohit
        public DbSet<FE_BlackList> FE_Blacklist { get; set; }

        public DbSet<Ticket_Stagging> Ticket_Stagging { get; set; }

        public DbSet<Quality_Remark_Permission> Quality_Remark_Permission { get; set; }
       //added by Prashant
           public DbSet<Ticket_Eu_Selection> Ticket_Eu_Selection { get; set; }

           public DbSet<QualityMarksBraekdown> QualityMarksBraekdown { get; set; }
           public DbSet<QualityHotRequirement> QualityHotRequirement { get; set; }

           public DbSet<ChatMessage> ChatMessages { get; set; }
           public DbSet<Communication> Communication { get; set; }
        //public DbSet<Ticket_Fe_Charges> Ticket_Fe_Charges { get; set; }
        public DbSet<TimeZone> TimeZone { get; set; }
        public DbSet<ChatMessageHdToSource> ChatMessageHdToSource { get; set; }
        public DbSet<ChatMessageInwinteckInternal> ChatMessageInwinteckInternal { get; set; }

        public DbSet<Ticket_FE_Charges> TicketFeCharges { get; set; }

        public DbSet<UserConnection> UserConnections { get; set; }
        public DbSet<PushSubscription> PushSubscriptions { get; set; }


    }

    public class ApplicationDbContext1 : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext1()
            : base("DefaultConnection1", throwIfV1Schema: false)
        {
        }

        public static ApplicationDbContext1 Create()
        {
            return new ApplicationDbContext1();
        }


        public DbSet<Website_Carrer_Enq> WCE { get; set; }

    }
}