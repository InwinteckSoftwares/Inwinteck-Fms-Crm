using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static Inwinteck_CRM.Controllers.InwinfmsAPIController;
using Newtonsoft.Json;

namespace Inwinteck_CRM.Models
{
    [Table("StateMaster")]
    public class StateMaster
    {
        public int Id { get; set; }

        public string State { get; set; }

        [StringLength(50)]
        public string Zone { get; set; }

        [StringLength(50)]
        public string Country { get; set; }

        public byte? status { get; set; }

        public DateTime? CreateDate { get; set; }
        public string CreatedBy { get; set; }

        public string ModifiedBy { get; set; }

        public DateTime? ModifiedOn { get; set; }
    }
    [Table("CountryMaster")]
    public class CountryMaster
    {
        public CountryMaster()
        {
            CreateDate = DateTime.Now.Date;
        }
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string CountryName { get; set; }
        public string CountryCode { get; set; }
        public DateTime CreateDate { get; set; }
        public byte Status { get; set; }
        public string CreatedBy { get; set; }

        public string ModifiedBy { get; set; }

        public DateTime? ModifiedOn { get; set; }


    }
    [Table("PinCodeMaster")]
    public class PinCodeMaster
    {

        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }

        [Required]
        [Display(Name = "Pin Code")]
        public string pincode { get; set; }
        [Required]
        [Display(Name = "City")]
        public string city { get; set; }

        [Required]
        [Display(Name = "state")]
        public string state { get; set; }

        [Required]
        [Display(Name = "Country")]
        public string Country { get; set; }

        [Display(Name = "Zone")]
        public string Zone { get; set; }
        public byte? status { get; set; }

        public DateTime? CreateDate { get; set; }

        public string CreatedBy { get; set; }

        public string ModifiedBy { get; set; }

        public DateTime? ModifiedOn { get; set; }


    }
    [Table("CityMaster")]
    public partial class CityMaster
    {
        public int Id { get; set; }

        public string City { get; set; }

        public string State { get; set; }

        public string Zone { get; set; }

        public string Country { get; set; }

        public byte? Status { get; set; }

        public DateTime? CreateDate { get; set; }
        public string CreatedBy { get; set; }

        public string ModifiedBy { get; set; }

        public DateTime? ModifiedOn { get; set; }
    }

    [Table("GroupHeader")]
    public class GroupHeader
    {
        public int id { get; set; }

        [Required]
        [StringLength(20)]
        public string value { get; set; }
    }


    [Table("HeaderDescription")]
    public class HeaderDescription
    {
        public int id { get; set; }

        public int header_id { get; set; }

        [Required]
        public string header_description { get; set; }

        public byte Status { get; set; }

        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }

        public string ModifiedBy { get; set; }

        public DateTime? ModifiedOn { get; set; }

    }
    [Table("FE_Master_Personal")]
    public class FE_Master_Personal
    {
        public int Id { get; set; }

        public string Photo { get; set; }

        public string First_Name { get; set; }

        public string Last_Name { get; set; }

        public string Email { get; set; }

        public string Alt_Email { get; set; }

        public string Phone_Number { get; set; }
        public string Phone_Number_Code { get; set; }
        public string Chat_Phone_Number { get; set; }
        public string Chat_Phone_Number_Code { get; set; }
        public string Chat_mode { get; set; }

        public string Alt_Phone_Number { get; set; }
        public string Manager_Name { get; set; }

        public string Manager_Phone_Number { get; set; }

        public string Manager_Email { get; set; }

        public string Language_Spoken { get; set; }

        public string Language_Spoken_1 { get; set; }

        public string Language_Spoken_2 { get; set; }

        public string Citizenship { get; set; }
        public string Permanent_Resident { get; set; }

        public string Work_Permit { get; set; }

        public string House_Name_Number { get; set; }

        public string Street_Address { get; set; }

        public string City { get; set; }

        public string State { get; set; }

        public string Country { get; set; }

        public string ZipCode_Pincode { get; set; }

        public int Identification { get; set; }

        public string Identification_No { get; set; }

        public int Identification_1 { get; set; }
        public string Identification_No_1 { get; set; }

        public int Identification_2 { get; set; }

        public string Identification_No_2 { get; set; }

        public byte? Status { get; set; }

        public DateTime CreatedOn { get; set; }
        public string CreatedBy { get; set; }

        public string ModifiedBy { get; set; }

        public DateTime? ModifiedOn { get; set; }

        public int FE_Type { get; set; }

        public string latitude { get; set; }

        public string longitude { get; set; }
        public string FE_Nick_name { get; set; }
        public string Company_Name { get; set; }
        public string Company_Email { get; set; }
        public string Company_Phone_code { get; set; }
        public string Company_Phone { get; set; }
        public string Company_Website { get; set; }
        public string Company_Address { get; set; }
        public string Company_City { get; set; }
        public string Company_State { get; set; }
        public string Company_Country { get; set; }
        public string Company_Zipcode_Pincode { get; set; }
        public string Freelance_Website { get; set; }
        public string Alt_Phone_Number_Code { get; set; }
        public string Alt_Phone_Number_1 { get; set; }
        public string Alt_Phone_Number_Code_1 { get; set; }
        public string Alt_Phone_Number_2 { get; set; }
        public string Alt_Phone_Number_Code_2 { get; set; }
        public string Other_detail { get; set; }
        public string Identification_Upload_1 { get; set; }
        public DateTime? NDA_Acceptance_Date { get; set; }

        public string Signature { get; set; }

        public string Alt_Chat_Mode { get; set; }

        public string Alt_Chat_Mode_1 { get; set; }

        public string Alt_Chat_Mode_2 { get; set; }

        public int? NDA_Accept { get; set; }

        public string InwinFEID { get; set; }

        public int? Blacklist { get; set; }
        public int? FeInterest { get; set; }


    }

    [Table("FE_Master_Financial")]
    public partial class FE_Master_Financial
    {
        public int Id { get; set; }

        public int FE_ID { get; set; }

        public string Account_No { get; set; }

        public string Account_Name { get; set; }

        public string Swift_Code { get; set; }

        public string Bank_Address { get; set; }

        public string VIC_Code { get; set; }

        public string Bank_Name { get; set; }

        public string VAT_Number { get; set; }
        public string GST_Number { get; set; }

        public string Tax_ID { get; set; }

        public string Other_detail { get; set; }

        public DateTime? CreatedOn { get; set; }
        public string CreatedBy { get; set; }

        public string ModifiedBy { get; set; }

        public DateTime? ModifiedOn { get; set; }
    }

    [Table("FE_Master_Charges")]
    public class FE_Master_Charges
    {
        public int Id { get; set; }

        public int FE_ID { get; set; }

        public int Charges_Business_Hour { get; set; }

        public int Charges_Non_Business_Hour { get; set; }

        public int Charge_Job { get; set; }

        public int Charge_Day { get; set; }

        public int Travel_Charge { get; set; }

        public DateTime? CreatedOn { get; set; }
        public string CreatedBy { get; set; }

        public string ModifiedBy { get; set; }

        public DateTime? ModifiedOn { get; set; }

        public int? Minimum_Hrs { get; set; }

        public int? Charge_Month { get; set; }

        public string Other_detail { get; set; }
        public string Currency { get; set; }
    }

    [Table("FE_Master_Skill")]
    public partial class FE_Master_Skill
    {
        public int Id { get; set; }

        public int FE_ID { get; set; }

        public string Server { get; set; }

        public string Networking { get; set; }

        public string Storage { get; set; }

        public string Bio_Data { get; set; }

        public string Others { get; set; }

        public string Desktop { get; set; }

        public string Laptop { get; set; }

        public DateTime? CreatedOn { get; set; }
        public string CreatedBy { get; set; }

        public string ModifiedBy { get; set; }

        public DateTime? ModifiedOn { get; set; }

        public string Certification { get; set; }

        public int? Hardware_Repair { get; set; }
        public int? Service_performed { get; set; }
        public int? Service_Hospitals { get; set; }
        public int? Service_Government { get; set; }
        public int? Work_Stations { get; set; }
        public int? Desktop_Servers { get; set; }
        public int? Rack_Mount { get; set; }
        public int? Blade_Servers { get; set; }
        public int? Multi_Node_Server { get; set; }
        public int? SAN_Storage { get; set; }
        public int? Tape_Libraries { get; set; }
        public int? Security_Camera { get; set; }
        public int? Systems_Installation { get; set; }
        public int? CLI { get; set; }
        public int? Console_Connectivity { get; set; }
        public int? Cat_5 { get; set; }
        public int? Cabling_Fibre { get; set; }
        public int? Switch_Configuration { get; set; }
        public int? RAID_Configuration { get; set; }
        public int? Out_of_Band_Interface { get; set; }
        public int? Telephony { get; set; }
        public int? Digital_Display { get; set; }
        public int? VM_Ware { get; set; }
        public string Ext_1 { get; set; }
        public int? Ext_1_Sel { get; set; }
        public string Ext_2 { get; set; }
        public int? Ext_2_Sel { get; set; }
        public string Ext_3 { get; set; }
        public int? Ext_3_Sel { get; set; }
    }

    [Table("FE_Master_Certification")]
    public partial class FE_Master_Certification
    {
        public int Id { get; set; }

        public int FE_ID { get; set; }

        public int Certification_Name { get; set; }

        public string Certification_Upload { get; set; }

        public DateTime? CreatedOn { get; set; }
        public string CreatedBy { get; set; }

        public string ModifiedBy { get; set; }

        public DateTime? ModifiedOn { get; set; }
    }

   


        [Table("FE_Master_Skill_Data")]
    public partial class FE_Master_Skill_Data
    {
        public int Id { get; set; }

        public int FE_ID { get; set; }

        public string Skill { get; set; }

        public string Exp { get; set; }

        public DateTime? CreatedOn { get; set; }
        public string CreatedBy { get; set; }

        public string ModifiedBy { get; set; }

        public DateTime? ModifiedOn { get; set; }
    }

    [Table("FE_Master_serviceArea")]
    public partial class FE_Master_serviceArea
    {
        public int Id { get; set; }

        public int FE_ID { get; set; }

        public string Country { get; set; }

        public string ZipCode_pincode { get; set; }

        public string TravelCharge { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string CreatedBy { get; set; }

        public string ModifiedBy { get; set; }

        public DateTime? ModifiedOn { get; set; }
    }

    [Table("FE_Master_Other_Detail")]
    public partial class FE_Master_Other_Detail
    {
        public int Id { get; set; }

        public int FE_ID { get; set; }

        public string Other_Details { get; set; }

        public DateTime? CreatedOn { get; set; }
        public string CreatedBy { get; set; }
    }


    [Table("FE_Master_Certification_Extra_Detail")]
    public partial class FE_Master_Certification_Extra_Detail
    {
        public int Id { get; set; }

        public int FE_ID { get; set; }

        public string Other_Details { get; set; }

        public DateTime? CreatedOn { get; set; }
        public string CreatedBy { get; set; }
    }

    [Table("EU_Master")]
    public partial class EU_Master
    {
        public int Id { get; set; }

        public string Customer_Name { get; set; }
        public string Customer_Contact { get; set; }
        public string Customer_Email { get; set; }
        public string Customer_Address { get; set; }

        public string Country { get; set; }

        public string State { get; set; }

        public string City { get; set; }

        public string Pincode_Zipcode { get; set; }

        public byte Status { get; set; }

        public DateTime CreatedOn { get; set; }
        public string CreatedBy { get; set; }

        public string ModifiedBy { get; set; }

        public DateTime? ModifiedOn { get; set; }

        public int? Travel_Charge { get; set; }

        public int? Charges_Business_Hour { get; set; }

        public int? Charges_Non_Business_Hour { get; set; }
        public int? Charges_Job { get; set; }

        public int? Minimum_Hrs { get; set; }

        public string Ticket_Abv { get; set; }
    }

    public class grouplist
    {
        public int? id { get; set; }
        public string value { get; set; }
        public string Description { get; set; }
        public Byte? Status { get; set; }
    }

    [Table("Country_Dialing_Code")]
    public class Country_Dialing_Code
    {

        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Code { get; set; }
        public string Country { get; set; }
        public byte Status { get; set; }

        public string CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string ModifiedBy { get; set; }

        public DateTime? ModifiedOn { get; set; }


    }

    [Table("Currency_Master")]
    public class Currency_Master
    {

        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Currency { get; set; }
        public string Country { get; set; }
        public byte Status { get; set; }

        public string CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string ModifiedBy { get; set; }

        public DateTime? ModifiedOn { get; set; }

    }

    [Table("Ticket")]
    public class Ticket
    {

        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int EU_ID { get; set; }
        public string Case_No { get; set; }
        public int OEM { get; set; }
        public string Site_Name { get; set; }
        public string Country { get; set; }

        public string TSE_Name { get; set; }
        public string TSE_Name2 { get; set; }
        public string TSE_Name3 { get; set; }

        public string WhatsappChat { get; set; }

        public string State { get; set; }

        public string City { get; set; }

        public string Zip_Pin_Code { get; set; }

        public string Street_Address { get; set; }
        public DateTime? Dispatch_Date { get; set; }
        public int FE_ID { get; set; }
        public int? FE_ID_2 { get; set; }
        public bool IsSecondFEEnabled { get; set; } = false;
        public string Pregame { get; set; }
        public DateTime? In_Time { get; set; }
        public DateTime? Out_Time { get; set; }
        public DateTime? In_Time2 { get; set; }
        public DateTime? Out_Time2 { get; set; }
        public string Total_Hours { get; set; }
        public string Total_Hours2 { get; set; }
        public int Status { get; set; }
        public int Part_Management { get; set; }
        public int Part_Handover { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public string ModifiedBy { get; set; }

        public DateTime? ModifiedOn { get; set; }

        public string Job_Description { get; set; }
        public string Scope_Off_Work { get; set; }

        public int? Ticket_Mode { get; set; }

        public int? Ticket_Type { get; set; }

        public int? SLA { get; set; }

        public DateTime? Ticket_Date { get; set; }

        public string Certification_Name { get; set; }

        public string EU_Name { get; set; }

        public string EU_Contact { get; set; }

        public string EU_Email { get; set; }

        public int? Certification_Need { get; set; }

        public byte Is_Reschedule { get; set; }
        public DateTime? Reschedule_DT { get; set; }

        public string Reschedule_Reason { get; set; }
        public byte Is_Decline { get; set; }
        public string Decline_Reason { get; set; }
        public int? Cancel_Reason { get; set; }

        public string ph_Name { get; set; }
        public string ph_contact { get; set; }

        public byte Return_Label { get; set; }

        public string Tracking_number { get; set; }
        public string Rl_Reason { get; set; }

        public byte Customer_Email { get; set; }
        public string Customer_Email_Name { get; set; }

        public byte FE_Email { get; set; }
        public string FE_Email_Name { get; set; }

        public string latitude { get; set; }

        public string longitude { get; set; }

        public string pregame_detail { get; set; }

        public string pregame_upload { get; set; }

        public string Ticket_No { get; set; }

        public int EU_Office { get; set; }

        public int? Old_Ticket { get; set; }
     
        public string EmailMessageId { get; set; }

        // Navigation property table TimeZone
        public int? TimeZoneId { get; set; }
        public TimeZone TimeZone { get; set; }
        public virtual ICollection<Ticket_FE_Charges> TicketFeCharges { get; set; } //One To Many RelationShip with TicketFECharge table
        public bool feChanged { get; set; }

        public string Vender_Remark { get; set; }
        public string Quality_Remark { get; set; }
    }

    [Table("Ticket_History")]
    public class Ticket_History
    {

        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int Ticket_no { get; set; }
        public string Comments { get; set; }

       // public string WhatsappChat { get; set; }
       // //public string Quality_Remark { get; set; }
       //// public string QualityRemarkDropdown { get; set; }
        public int? FE_ID { get; set; }

        public int? status { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        //public string FE_Available_Time { get; set; }

        //public decimal? Travel_Amount { get; set; }

        public string SLA { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }

    }


    public class TicketDetails
    {
        public int Id { get; set; }
        public string Ticket_No { get; set; }
        public string Ticket_Created { get; set; }
        public string EU_Name { get; set; }
        public string Site_Name { get; set; }
        public string City { get; set; }
        public string Dispatch { get; set; }
        public string FE_Name { get; set; }
        public string Status { get; set; }
        public int Status_ID { get; set; }

        public string EU_Contact { get; set; }
        public string EU_Email { get; set; }
        public string FE_Contact { get; set; }
        public string FE_Email { get; set; }

        public string Country { get; set; }

        public string Case_Date { get; set; }
        public string Case_No { get; set; }
        public string Username { get; set; }

        public int? CSAT { get; set; }
        public string checked_In { get; set; }

        public string Handler1Name { get; set; }
        public int? Handler1Score { get; set; }

        public string Handler2Name { get; set; }
        public int? Handler2Score { get; set; }

        public string Handler3Name { get; set; }
        public int? Handler3Score { get; set; }

        public string Handler4Name { get; set; }
        public int? Handler4Score { get; set; }

        public string Handlers { get; set; }
        public string Ticket_Score { get; set; }
    }


    public class TicketHist
    {
        public int Ticket_No { get; set; }

        public string Remark { get; set; }

        public string Createdon { get; set; }

        public string CreatedBy { get; set; }

        public string SLA { get; set; }

        public string Status { get; set; }
    }


    public class FEDetails
    {

        public string Type { get; set; }
        public string FE_Name { get; set; }

        public string latitude { get; set; }

        public string longitude { get; set; }

        public int FE_ID { get; set; }
        public string FE_Origin { get; set; }

        public string Phone { get; set; }

        public string Email { get; set; }

        public string Certifications { get; set; }
        public int? Charges_Business_Hour { get; set; }
        public int? Charges_Non_Business_Hour { get; set; }
        public int? Minimum_Hrs { get; set; }
        public int? Charge_Job { get; set; }

        public string Remark { get; set; }
        public string Status { get; set; }
        public string Manager_Name { get; set; }
        public string Manager_Phone_Number { get; set; }
    }

    public class FEDetailsmap
    {

        public string FE_Name { get; set; }

        public string latitude { get; set; }

        public string longitude { get; set; }


    }

    public class userlist
    {
        public string id { get; set; }
        public string Name { get; set; }
        public string email { get; set; }
        public string role { get; set; }
        public byte status { get; set; }

    }

    public class checkpermission
    {
        public bool? record_insert { get; set; }
        public bool? record_delete { get; set; }
        public bool? record_update { get; set; }
        public bool? record_export { get; set; }
        public bool? record_import { get; set; }
        public bool? record_view { get; set; }
    }

    [Table("Header_Invoice_FE")]
    public class Header_Invoice_FE
    {

        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public DateTime Invoice_Date { get; set; }
        public int FE_ID { get; set; }
        public decimal Total_Amt { get; set; }
        public DateTime? from_date { get; set; }
        public DateTime? to_date { get; set; }

        public DateTime? FE_Pay_Date { get; set; }

        public string FE_Reference_ID { get; set; }

        public string FE_Payment_Status { get; set; }

        public int FE_PaymentMode { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string ModifiedBy { get; set; }

        public DateTime? ModifiedOn { get; set; }

    }

    [Table("Header_Invoice_Detail_FE")]
    public class Header_Invoice_Detail_FE
    {

        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int Inv_no { get; set; }
        public int Ticket_no { get; set; }
        public string Business_hour { get; set; }

        public decimal Business_hour_amt { get; set; }

        public string Non_Business_hour { get; set; }

        public decimal Non_Business_hour_amt { get; set; }

        public decimal Travel_Charge { get; set; }
        public decimal Part_Handling_Charge { get; set; }

        public decimal Fixed_amt { get; set; }
        public int FE_ID { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string ModifiedBy { get; set; }

        public DateTime? ModifiedOn { get; set; }

    }

    public class ManageHeader_FE
    {
        public int Ticket { get; set; }
        public string EU { get; set; }

        public string site_name { get; set; }

        public string ClosedOn { get; set; }

        public string Amount { get; set; }
    }

    public class Invoice_FE
    {
        public int Ticket { get; set; }
        public string EU { get; set; }
        public string Site_Name { get; set; }
        public string OEM { get; set; }
        public string Zip_Pin_Code { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Street_Address { get; set; }
        public string FE_Payment_Mode { get; set; }

        public DateTime In_Time { get; set; }
        public DateTime Out_Time { get; set; }
        public int Charges_Business_Hour { get; set; }

        public decimal FE_Buss_Hrs { get; set; }
        public int Charges_Non_Business_Hour { get; set; }
        public decimal FE_Non_Buss_Hrs { get; set; }
        public int Travel_Charge { get; set; }
        public decimal FE_Allowance { get; set; }

        public decimal? FE_Minimum_Hrs { get; set; }
        public decimal FE_Fixed { get; set; }

        public decimal FE_Part_Storage_Amt { get; set; }
        public string Currency { get; set; }
        public int FE_ID { get; set; }

    }

    public class Invoice_FE_Print
    {
        public string FE_Name { get; set; }
        public string FE_Address { get; set; }
        public string FE_Phone { get; set; }
        public string FE_Email { get; set; }
        public int Ticket_No { get; set; }
        public DateTime In_Time { get; set; }
        public DateTime Out_Time { get; set; }
        public int Inv_No { get; set; }
        public DateTime Invoice_Date { get; set; }
        public decimal Total_Amt { get; set; }
        public string Business_hour { get; set; }
        public decimal Charges_Business_Hour { get; set; }
        public decimal Business_hour_amt { get; set; }
        public string Non_Business_hour { get; set; }
        public decimal Charges_Non_Business_Hour { get; set; }
        public decimal Non_Business_hour_amt { get; set; }
        public decimal FE_Travel { get; set; }
        public decimal Inv_Travel { get; set; }
        public decimal Fixed_amt { get; set; }
        public decimal Part_Handling_Charge { get; set; }
        public string Currency { get; set; }

        public string FE_Payment_Status { get; set; }

        public DateTime? FE_Pay_Date { get; set; }

        public int FE_PaymentMode { get; set; }
        public string FE_Reference_ID { get; set; }

    }




    public class FEL
    {
        public int id { get; set; }
        public string name { get; set; }
    }

    [Table("Header_Invoice_EU")]
    public class Header_Invoice_EU
    {

        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public DateTime Invoice_Date { get; set; }
        public int EU_ID { get; set; }
        public decimal Total_Amt { get; set; }
        public DateTime? from_date { get; set; }
        public DateTime? to_date { get; set; }

        public string CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string ModifiedBy { get; set; }

        public DateTime? ModifiedOn { get; set; }

    }

    [Table("Header_Invoice_Detail_EU")]
    public class Header_Invoice_Detail_EU
    {

        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int Inv_no { get; set; }
        public int Ticket_no { get; set; }
        public string Business_hour { get; set; }

        public decimal Business_hour_amt { get; set; }

        public string Non_Business_hour { get; set; }

        public decimal Non_Business_hour_amt { get; set; }

        public decimal Travel_Charge { get; set; }
        public decimal Part_Handling_Charge { get; set; }
        public decimal Cancel_Charge { get; set; }
        public decimal Reschedule_Charge { get; set; }
        public decimal Fixed_amt { get; set; }
        public int EU_ID { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string ModifiedBy { get; set; }

        public DateTime? ModifiedOn { get; set; }

    }

    public class Invoice_EU
    {
        public int Ticket { get; set; }
        public string EU { get; set; }
        public string Site_Name { get; set; }
        public string OEM { get; set; }
        public string Zip_Pin_Code { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Street_Address { get; set; }
        public string CT_Payment_Mode { get; set; }
        public DateTime In_Time { get; set; }
        public DateTime Out_Time { get; set; }
        public decimal Charges_Business_Hour { get; set; }
        public decimal CT_Buss_Hrs { get; set; }
        public decimal Charges_Non_Business_Hour { get; set; }
        public decimal CT_Non_Buss_Hrs { get; set; }
        public decimal Travel_Charge { get; set; }
        public decimal CT_Fixed { get; set; }
        public decimal CT_Allowance { get; set; }
        public int Cancel_Charge { get; set; }
        public int Reschedule_Charge { get; set; }
        public string Currency { get; set; }
        public int EU_ID { get; set; }

    }

    [Table("Certification_Master")]
    public class Certification_Master
    {

        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int OEM { get; set; }
        public string Certification_Name { get; set; }
        public byte Status { get; set; }

        public string CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string ModifiedBy { get; set; }

        public DateTime? ModifiedOn { get; set; }

    }

    public class certification_detail
    {
        public int Id { get; set; }
        public string OEM { get; set; }
        public string Certification_Name { get; set; }
        public byte Status { get; set; }
    }

    [Table("Part_Ticket_Data")]
    public class Part_Ticket_Data
    {

        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int Ticket_No { get; set; }
        public string Part_type { get; set; }
        public string Serial_No { get; set; }
        public string Make_Model { get; set; }
        public string Part_Description { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string ModifiedBy { get; set; }

        public DateTime? ModifiedOn { get; set; }

    }


    public class Invoice_EU_Print
    {
        public string EU_Name { get; set; }
        public string EU_Address { get; set; }
        public string EU_Phone { get; set; }
        public string EU_Email { get; set; }
        public int Ticket_No { get; set; }
        public DateTime In_Time { get; set; }
        public DateTime Out_Time { get; set; }
        public int Inv_No { get; set; }
        public DateTime Invoice_Date { get; set; }
        public decimal Total_Amt { get; set; }
        public string Business_hour { get; set; }
        public decimal Charges_Business_Hour { get; set; }
        public decimal Business_hour_amt { get; set; }
        public string Non_Business_hour { get; set; }
        public decimal Charges_Non_Business_Hour { get; set; }
        public decimal Non_Business_hour_amt { get; set; }
        public decimal EU_Travel { get; set; }
        public decimal Inv_Travel { get; set; }
        public decimal Part_Handling_Charge { get; set; }

        public decimal Fixed_amt { get; set; }

        public decimal Cancel_Charge { get; set; }

        public decimal Reschedule_Charge { get; set; }
        public string Currency { get; set; }

        public string FE_Payment_Status { get; set; }

        public DateTime? FE_Pay_Date { get; set; }

        public int FE_PaymentMode { get; set; }
        public string FE_Reference_ID { get; set; }

    }


    [Table("EU_Rate_Card")]
    public class EU_Rate_Card
    {

        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int EU_ID { get; set; }
        public string Country { get; set; }
        public decimal SLA_Per_Hr_4 { get; set; }
        public decimal SLA_Per_Hr_4_add { get; set; }
        public decimal SLA_Per_Hr_NBD { get; set; }
        public decimal SLA_Per_Hr_NBD_add { get; set; }
        public decimal SLA_Per_Hr_2 { get; set; }
        public decimal SLA_Per_Hr_2_add { get; set; }
        public decimal Business_Hr { get; set; }
        public decimal Non_Business_Hr { get; set; }
        public decimal Per_Job { get; set; }
        public decimal Travel_Charges { get; set; }

        public int Minimum_Hrs { get; set; }

        public decimal Daily { get; set; }
        public decimal Weekly { get; set; }
        public decimal Monthly { get; set; }
        public decimal FTE_Rates_Backfil { get; set; }
        public decimal FTE_Rates_Backfil_wo { get; set; }
        public int Business_Hr_Uplift { get; set; }
        public int Weekends_Holiday_Uplift { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string ModifiedBy { get; set; }

        public DateTime? ModifiedOn { get; set; }

    }

    public class EditEU_Ratecard
    {
        public List<EU_Rate_Card> ERC { get; set; }

        public EU_Rate_Card EERC { get; set; }
    }


    public class TicketListMont
    {
        public int? Ticket_Count { get; set; }

        public string Date { get; set; }
    }

    public class FEtListMonth
    {
        public string Count { get; set; }

        public string Date { get; set; }
    }

    public class EUListMonth
    {
        public int? Count { get; set; }

        public string Date { get; set; }
    }

    public class CountryList
    {
        public string Country { get; set; }

        public int Number { get; set; }
        public string City { get; set; }
    }


    // Added By Praveen
    [Table("EU_Master_Branch")]

    public class EU_Master_Branch

    {



        [Key]

        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]

        public int Id { get; set; }

        public int EU_ID { get; set; }

        public string Office { get; set; }

        public string City { get; set; }
        public string Pincode_Zipcode { get; set; }
        public string State { get; set; }


        public string Country { get; set; }

        public string Address { get; set; }

        public string Number { get; set; }

        public string Email { get; set; }

        public string Website { get; set; }

        public string CreatedBy { get; set; }

        public DateTime? CreatedOn { get; set; }

        public string ModifiedBy { get; set; }



        public DateTime? ModifiedOn { get; set; }



    }



    [Table("EU_Master_Contacts")]

    public class EU_Master_Contacts

    {



        [Key]

        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]

        public int Id { get; set; }

        public int Office_ID { get; set; }

        public string Person { get; set; }

        public string Designation { get; set; }

        public string Department { get; set; }

        public string Number { get; set; }

        public string Email { get; set; }

        public string CreatedBy { get; set; }

        public DateTime? CreatedOn { get; set; }

        public string ModifiedBy { get; set; }



        public DateTime? ModifiedOn { get; set; }



    }
    [Table("FE_Master_Identification")]

    public class FE_Master_Identification
    {
        public int Id { get; set; }
        public int FE_ID { get; set; }
        public int ID_Type { get; set; }
        public string ID_Number { get; set; }
        public string ID_Upload { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
    }

  

    public class IdentificationInsertView
    {
        public int FE_ID { get; set; }
        public int ID_Type { get; set; }
        public string ID_Number { get; set; }
        public string Upload { get; set; }
        public HttpPostedFileBase ID_Upload { get; set; }
    }

    public class IdentificationInsertFill
    {
        public int FE_ID { get; set; }
        public string ID_Type { get; set; }
        public int IdType { get; set; }
        public string ID_Number { get; set; }
        public string Upload { get; set; }

    }

    public class ManageFePersonal
    {
        public FE_Master_Personal FE { get; set; }
        public List<IdentificationInsertView> IV { get; set; }
        public List<IdentificationInsertFill> IF { get; set; }

        public FE_Master_Charges FEC { get; set; }
        public List<InsertServiceArea> IVS { get; set; }
        public List<InsertServiceArea> ICS { get; set; }

        public FE_Master_Financial FEF { get; set; }
        public FE_Master_Skill FES { get; set; }
        public List<InsertCertificate> IVCE { get; set; }
        public List<InsertCertificateView> ICCE { get; set; }
        public List<InsertSkillsView> ISV { get; set; }
        public List<InsertSkills> IVSk { get; set; }
         
        public FE_BlackList bl { get; set; }

        public List<FE_BlackList> selectedCertificates { get; set; }
        public string BlackListStatus { get; set; }
        

    }

    public class InsertCertificate
    {
        public int FE_ID { get; set; }

        public int Certification_Name { get; set; }

        public HttpPostedFileBase Certification_Upload { get; set; }
    }
    public class InsertCertificateView
    {
        public int Id { get; set; }
        public int FE_ID { get; set; }

        public string Certification_Name { get; set; }

        public string Certification_Upload { get; set; }
    }
    public class ManageSkill
    {
        public int? Id { get; set; }
        public FE_Master_Skill FE { get; set; }
        public List<InsertCertificate> IV { get; set; }
        public List<InsertCertificateView> IC { get; set; }

    }

    public class InsertServiceArea
    {
        public int FE_ID { get; set; }
        public string Country { get; set; }
        public string ZipCode_pincode { get; set; }

        public string TravelCharge { get; set; }
        

    }

    public class ManageFeCharges
    {
        public int? Id { get; set; }
        public FE_Master_Charges FE { get; set; }
        public List<InsertServiceArea> IV { get; set; }
        public List<InsertServiceArea> IC { get; set; }

    }

    public class InsertSkills
    {
        public int FE_ID { get; set; }

        public string Skill { get; set; }

        public string Exp { get; set; }
    }
    public class InsertSkillsView
    {
        public int Id { get; set; }
        public int FE_ID { get; set; }

        public string Skill_Name { get; set; }

        public string Exp_Upload { get; set; }
    }

    [Table("Ticket_Email")]

    public class Ticket_Email
    {
        public int Id { get; set; }
        public int Ticket_no { get; set; }
        public string Email { get; set; }
        public string Email_Subject { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string CreatedBy { get; set; }
    }

    public class Ticket_FE_Selection
    {
        public int Id { get; set; }
        public int Ticket_no { get; set; }
        public int FE_ID { get; set; }
        public string Status { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string CreatedBy { get; set; }

        public DateTime? ModifiedOn { get; set; }

        public string Remark { get; set; }

    }

    
    public class FETicket
    {

        public int FE_ID { get; set; }
        public string Name { get; set; }
        public string Status { get; set; }
        public string Sent { get; set; }
        public string Remark { get; set; }
        public string Request { get; set; }

    }

    [Table("CSAT")]
    public class CSAT
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int TicketNo { get; set; }
        public int Q1 { get; set; }
        public int Q2 { get; set; }
        public int Q3 { get; set; }
        public int Q4 { get; set; }
        public int Q5 { get; set; }
        public string Remarks { get; set; }
        public DateTime CreatedOn { get; set; }
    }


    [Table("Vendor_Master")]
    public class Vendor_Master
    {
        public int Id { get; set; }

        public string Customer_Name { get; set; }
        public string Customer_Contact { get; set; }
        public string Customer_Email { get; set; }
        public string Customer_Address { get; set; }

        public string Country { get; set; }

        public string State { get; set; }

        public string City { get; set; }

        public string Pincode_Zipcode { get; set; }

        public byte Status { get; set; }

        public DateTime CreatedOn { get; set; }
        public string CreatedBy { get; set; }

        public string ModifiedBy { get; set; }

        public DateTime? ModifiedOn { get; set; }
    }

    [Table("Vendor_Master_Contacts")]

    public class Vendor_Master_Contacts
    {

        [Key]

        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]

        public int Id { get; set; }

        public int Vendor_ID { get; set; }

        public string Person { get; set; }

        public string Designation { get; set; }

        public string Department { get; set; }

        public string Number { get; set; }

        public string Email { get; set; }

        public string CreatedBy { get; set; }

        public DateTime? CreatedOn { get; set; }

        public string ModifiedBy { get; set; }

        public DateTime? ModifiedOn { get; set; }

    }

    [Table("IT_Customer_Master")]
    public class IT_Customer_Master
    {
        public int Id { get; set; }

        public string Customer_Name { get; set; }
        public string Customer_Contact { get; set; }
        public string Customer_Email { get; set; }
        public string Customer_Website { get; set; }
        public string Customer_Address { get; set; }

        public string Enq_Abbreviation { get; set; }
        public string Country { get; set; }

        public string State { get; set; }

        public string City { get; set; }

        public string Pincode_Zipcode { get; set; }

        public byte Status { get; set; }

        public DateTime CreatedOn { get; set; }
        public string CreatedBy { get; set; }

        public string ModifiedBy { get; set; }

        public DateTime? ModifiedOn { get; set; }
    }

    [Table("IT_Customer_Contacts")]

    public class IT_Customer_Contacts
    {

        [Key]

        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]

        public int Id { get; set; }

        public int IT_Customer_ID { get; set; }

        public string Person { get; set; }

        public string Designation { get; set; }

        public string Department { get; set; }

        public string Number { get; set; }

        public string Email { get; set; }

        public string CreatedBy { get; set; }

        public DateTime? CreatedOn { get; set; }

        public string ModifiedBy { get; set; }

        public DateTime? ModifiedOn { get; set; }

    }

    [Table("Enq_IT")]
    public class Enq_IT
    {
        public int Id { get; set; }

        public int Customer_Id { get; set; }
        public DateTime? Requirement_Date { get; set; }
        public string Reff_No { get; set; }

        public string Job_Title { get; set; }

        public string Job_Location { get; set; }

        public string Job_Duration { get; set; }

        public string Job_Description { get; set; }

        public string Job_Exp { get; set; }

        public string Candidate_Rate { get; set; }

        public DateTime? Closing_Date { get; set; }

        public int Status { get; set; }

        public string Enq_No { get; set; }

        public int? Candidate { get; set; }

        public DateTime CreatedOn { get; set; }
        public string CreatedBy { get; set; }

        public string ModifiedBy { get; set; }

        public DateTime? ModifiedOn { get; set; }
    }

    [Table("Enq_History")]
    public class Enq_History
    {

        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int Enq_Id { get; set; }
        public string Comments { get; set; }
        
        public int? Status { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }

    }


    public class EnqDetails
    {
        public int Id { get; set; }
        public string Enq_No { get; set; }
        public string Job_Title { get; set; }
        public string Enq_Created { get; set; }
        public string Requirement_Date { get; set; }
        public string Reff_No { get; set; }
        public string Cust_Name { get; set; }
        public int Status { get; set; }

    }


    public class EnqHist
    {
        public string Remark { get; set; }

        public string Createdon { get; set; }

        public string CreatedBy { get; set; }

    }


    [Table("Enq_Vendor_Email")]
    public class Enq_Vendor_Email
    {

        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int Enq_Id { get; set; }
        public string Vendor_Id { get; set; }
        public string Email_Vendor { get; set; }
        public string Email_Subject { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }

    }

    public class EnqVenEmail
    {
        public string Vendor { get; set; }

        public string Email { get; set; }

        public string Subject { get; set; }

    }

    [Table("Enq_Customer_Email")]
    public class Enq_Customer_Email
    {

        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int Enq_Id { get; set; }
        public int Vendor_Id { get; set; }
        public string Customer_Email { get; set; }
        public string Subject { get; set; }
        public DateTime? Response_date { get; set; }
        public string Candidate_Name { get; set; }
        public string Resume { get; set; }
        public string Proposed_Rate { get; set; }
        public int Status { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }

    }

    public class EnqCustEmail
    {
        public string Candidate { get; set; }

        public string Resume { get; set; }

        public string Proposed { get; set; }

        public string Vendor { get; set; }
        public string Response_Date { get; set; }

        public string Customer_email { get; set; }
        public string Email_on { get; set; }

    }

    [Table("Enq_EU")]
    public class Enq_EU
    {

        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int EU_ID { get; set; }
        public int EU_Office { get; set; }
        public DateTime Enq_Date { get; set; }
        public string Enq_Reference { get; set; }
        public int Project_Type { get; set; }
        public string Project_Details { get; set; }
        public string Our_Action { get; set; }
        public DateTime? Offer_Date { get; set; }
        public int Status { get; set; }
        public string EU_Feedback { get; set; }
        public string Attach { get; set; }

        public string CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }

        public string ModifiedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }

    }

    [Table("Enq_EU_History")]
    public class Enq_EU_History
    {

        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int Enq_Id { get; set; }
        public string Comments { get; set; }

        public int? Status { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }

    }

    public class EnqDetailsEU
    {
        public int Id { get; set; }
        public string EU_Name { get; set; }
        public string Enq_Created { get; set; }
        public string Enq_Date { get; set; }
        public int Status { get; set; }

    }

    [Table("Ticket_System_Info")]

    public class Ticket_System_Info
    {
        public int Id { get; set; }
        public int Ticket_no { get; set; }
        public string System_Information { get; set; }
        public string Make_Model { get; set; }
        public string Serial_Number { get; set; }
        public string Required_Tool { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public string ModifiedBy { get; set; }
    }

    [Table("Ticket_EU_Detail")]

    public class Ticket_EU_Detail
    {
        public int Id { get; set; }
        public int Ticket_no { get; set; }
        public string EU_Name { get; set; }
        public string EU_Contact { get; set; }
        public string EU_Email { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public string ModifiedBy { get; set; }
    }

    [Table("EU_Master_Sales")]
    public partial class EU_Master_Sales
    {
        public int Id { get; set; }

        public string Customer_Name { get; set; }
        public string Customer_Contact { get; set; }
        public string Customer_Email { get; set; }
        public string Customer_Address { get; set; }

        public string Country { get; set; }

        public string State { get; set; }

        public string City { get; set; }

        public string Pincode_Zipcode { get; set; }

        public byte Status { get; set; }

        public DateTime CreatedOn { get; set; }
        public string CreatedBy { get; set; }

        public string ModifiedBy { get; set; }

        public DateTime? ModifiedOn { get; set; }
    }


    [Table("EU_Master_Branch_Sales")]

    public class EU_Master_Branch_Sales
    {


        [Key]

        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]

        public int Id { get; set; }

        public int EU_ID { get; set; }

        public string Office { get; set; }

        public string City { get; set; }
        public string Pincode_Zipcode { get; set; }
        public string State { get; set; }


        public string Country { get; set; }

        public string Address { get; set; }

        public string Number { get; set; }

        public string Email { get; set; }

        public string Website { get; set; }

        public string CreatedBy { get; set; }

        public DateTime? CreatedOn { get; set; }

        public string ModifiedBy { get; set; }



        public DateTime? ModifiedOn { get; set; }



    }



    [Table("EU_Master_Contacts_Sales")]

    public class EU_Master_Contacts_Sales

    {



        [Key]

        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]

        public int Id { get; set; }

        public int Office_ID { get; set; }

        public string Person { get; set; }

        public string Designation { get; set; }

        public string Department { get; set; }

        public string Number { get; set; }

        public string Email { get; set; }

        public string CreatedBy { get; set; }

        public DateTime? CreatedOn { get; set; }

        public string ModifiedBy { get; set; }



        public DateTime? ModifiedOn { get; set; }



    }

    public partial class FE_Master_Other_Detail_History
    {
   
        public string Other_Details { get; set; }

        public string CreatedOn { get; set; }
    }

    public partial class FE_Master_Other_Extra_Detail_History
    {

        public string Other_Details { get; set; }

        public string CreatedOn { get; set; }
    }

    public class FE_Master_Personal_Old
    {
        public int Id { get; set; }

        public string Photo { get; set; }

        public string First_Name { get; set; }

        public string Last_Name { get; set; }

        public string Email { get; set; }

        public string Alt_Email { get; set; }

        public string Phone_Number { get; set; }
        public string Phone_Number_Code { get; set; }
        public string Chat_Phone_Number { get; set; }
        public string Chat_Phone_Number_Code { get; set; }
        public string Chat_mode { get; set; }

        public string Alt_Phone_Number { get; set; }
        public string Manager_Name { get; set; }

        public string Manager_Phone_Number { get; set; }

        public string Manager_Email { get; set; }

        public string Language_Spoken { get; set; }

        public string Language_Spoken_1 { get; set; }

        public string Language_Spoken_2 { get; set; }

        public string Citizenship { get; set; }
        public string Permanent_Resident { get; set; }

        public string Work_Permit { get; set; }

        public string House_Name_Number { get; set; }

        public string Street_Address { get; set; }

        public string City { get; set; }

        public string State { get; set; }

        public string Country { get; set; }

        public string ZipCode_Pincode { get; set; }

        public int Identification { get; set; }

        public string Identification_No { get; set; }

        public int Identification_1 { get; set; }
        public string Identification_No_1 { get; set; }

        public int Identification_2 { get; set; }

        public string Identification_No_2 { get; set; }

        public byte? Status { get; set; }

        public DateTime CreatedOn { get; set; }
        public string CreatedBy { get; set; }

        public string ModifiedBy { get; set; }

        public DateTime? ModifiedOn { get; set; }

        public int FE_Type { get; set; }

        public string latitude { get; set; }

        public string longitude { get; set; }
        public string FE_Nick_name { get; set; }
        public string Company_Name { get; set; }
        public string Company_Email { get; set; }
        public string Company_Phone_code { get; set; }
        public string Company_Phone { get; set; }
        public string Company_Website { get; set; }
        public string Company_Address { get; set; }
        public string Company_City { get; set; }
        public string Company_State { get; set; }
        public string Company_Country { get; set; }
        public string Company_Zipcode_Pincode { get; set; }
        public string Freelance_Website { get; set; }
        public string Alt_Phone_Number_Code { get; set; }
        public string Alt_Phone_Number_1 { get; set; }
        public string Alt_Phone_Number_Code_1 { get; set; }
        public string Alt_Phone_Number_2 { get; set; }
        public string Alt_Phone_Number_Code_2 { get; set; }
        public string Other_detail { get; set; }
        public string Identification_Upload_1 { get; set; }
        public DateTime? NDA_Acceptance_Date { get; set; }

        public string Signature { get; set; }

        public string Alt_Chat_Mode { get; set; }

        public string Alt_Chat_Mode_1 { get; set; }

        public string Alt_Chat_Mode_2 { get; set; }

        public int? NDA_Accept { get; set; }

        public string InwinFEID { get; set; }


    }


    public class FE_Master_Charges_Old
    {
        public int Id { get; set; }

        public int FE_ID { get; set; }

        public int Charges_Business_Hour { get; set; }

        public int Charges_Non_Business_Hour { get; set; }

        public int Charge_Job { get; set; }

        public int Charge_Day { get; set; }

        public int Travel_Charge { get; set; }

        public DateTime? CreatedOn { get; set; }
        public string CreatedBy { get; set; }

        public string ModifiedBy { get; set; }

        public DateTime? ModifiedOn { get; set; }

        public int? Minimum_Hrs { get; set; }

        public int? Charge_Month { get; set; }

        public string Other_detail { get; set; }
        public string Currency { get; set; }
    }

    [Table("blkFEMaster")]
    public class blkFEMaster
    {
        [Key]

        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]

        public int Id { get; set; }
        public DateTime Date { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
        public string Company { get; set; }
        public string Address { get; set; }
        public string Contact { get; set; }
        public string EmailId { get; set; }
    }

    public class TicketDetailsDash
    {
        public int Id { get; set; }

        public string Ticket_No { get; set; }
        public string Site_Name { get; set; }
        public DateTime? Ticket_date { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public DateTime? Dispatch { get; set; }
        public string FE_Name { get; set; }
        public string Status { get; set; }
        public string Activity_Status { get; set; }
    }


    [Table("Employee_Detail")]

    public class Employee_Detail
    {
        public int Id { get; set; }
        public string First_Name { get; set; }
        public string Last_Name { get; set; }
        public DateTime? Date_of_Birth { get; set; }
        public string Address { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Designation { get; set; }
        public DateTime? DOJ { get; set; }
        public string Emp_Code { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public string ModifiedBy { get; set; }
    }

    [Table("HRMS_Users")]

    public class HRMS_Users
    {
        public int Id { get; set; }
        public int Emp_Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public string ModifiedBy { get; set; }
        public byte Status { get; set; }
    }

    [Table("Timesheet_Log")]

    public class Timesheet_Log
    {
        public int Id { get; set; }
        public string Activity { get; set; }
        public DateTime log { get; set; }
        public int Emp_Id { get; set; }
    }

    [Table("Timesheet")]

    public class Timesheet
    {
        public int Id { get; set; }
        public int Emp_Id { get; set; }
        public DateTime Date { get; set; }
        public DateTime? Punch_In { get; set; }
        public DateTime? Punch_Out { get; set; }
        public decimal Working_Hrs { get; set; }
        public DateTime? Last_Break_In { get; set; }
        public DateTime? Last_Break_Out { get; set; }
        public decimal Total_Break { get; set; }
    }


    public class Activity_log
    {
        public string Activity { get; set; }
        public DateTime Timesheet { get; set; }
    }

    public class FECSAT
    {
        public decimal CSAT { get; set; }
        public int Count { get; set; }
    }

    [Table("EmailLog")]

    public class EmailLog
    {
        public int Id { get; set; }
        public string Emailid { get; set; }
        public string Subject { get; set; }
        public DateTime Sent_On { get; set; }
        public string Status { get; set; }
    }


    public class FE_Master_Personal_list
    {
        public int Id { get; set; }
        public string Photo { get; set; }
        public string First_Name { get; set; }
        public string Last_Name { get; set; }
        public string Email { get; set; }
        public string Alt_Email { get; set; }
        public string Phone_Number { get; set; }
        public string Phone_Number_Code { get; set; }
        public string Chat_Phone_Number { get; set; }
        public string Chat_Phone_Number_Code { get; set; }
        public string Chat_mode { get; set; }
        public string Alt_Phone_Number { get; set; }
        public string Manager_Name { get; set; }
        public string Manager_Phone_Number { get; set; }
        public string Manager_Email { get; set; }
        public string Language_Spoken { get; set; }
        public string Language_Spoken_1 { get; set; }
        public string Language_Spoken_2 { get; set; }
        public string Citizenship { get; set; }
        public string Permanent_Resident { get; set; }
        public string Work_Permit { get; set; }
        public string House_Name_Number { get; set; }
        public string Street_Address { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        public string ZipCode_Pincode { get; set; }
        public int Identification { get; set; }
        public string Identification_No { get; set; }
        public int Identification_1 { get; set; }
        public string Identification_No_1 { get; set; }
        public int Identification_2 { get; set; }
        public string Identification_No_2 { get; set; }
        public byte? Status { get; set; }
        public DateTime CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public int FE_Type { get; set; }
        public string latitude { get; set; }
        public string longitude { get; set; }
        public string FE_Nick_name { get; set; }
        public string Company_Name { get; set; }
        public string Company_Email { get; set; }
        public string Company_Phone_code { get; set; }
        public string Company_Phone { get; set; }
        public string Company_Website { get; set; }
        public string Company_Address { get; set; }
        public string Company_City { get; set; }
        public string Company_State { get; set; }
        public string Company_Country { get; set; }
        public string Company_Zipcode_Pincode { get; set; }
        public string Freelance_Website { get; set; }
        public string Alt_Phone_Number_Code { get; set; }
        public string Alt_Phone_Number_1 { get; set; }
        public string Alt_Phone_Number_Code_1 { get; set; }
        public string Alt_Phone_Number_2 { get; set; }
        public string Alt_Phone_Number_Code_2 { get; set; }
        public string Other_detail { get; set; }
        public string Identification_Upload_1 { get; set; }
        public DateTime? NDA_Acceptance_Date { get; set; }
        public string Signature { get; set; }
        public string Alt_Chat_Mode { get; set; }
        public string Alt_Chat_Mode_1 { get; set; }
        public string Alt_Chat_Mode_2 { get; set; }
        public int? NDA_Accept { get; set; }
        public string InwinFEID { get; set; }
        public int Certification { get; set; }
        public int CertificationCount { get; set; }  
        public string CertificationNames { get; set; }
        public int Name { get; set; }
        public int? FeInterest { get; set; } 
    }
    public class FEMasterCounts
    {
        public int Activated { get; set; }
        public int DeActivated { get; set; }
        public int BlackListed { get; set; }
        public int CertiAct { get; set; }
        public int NotInterested { get; set; }
        public string CertificationNames { get; set; }
    }
    [Table("FE_BlackList")]
    public partial class FE_BlackList
    {
        public int ID { get; set; }

        public int FE_ID { get; set; }

        public int Certificate_Id { get; set; }
    }

    [Table("Ticket_Stagging")]
    public partial class Ticket_Stagging
    {
        public int ID { get; set; }

        public string Case_No { get; set; }

        public string OEM { get; set; }

        public string Email_Subject { get; set; }
        public string Site_Name { get; set; }

        public string Zip_Pin_Code { get; set; }

        public string Country { get; set; }

        public string City { get; set; }

        public string State { get; set; }

        public string Street_Address { get; set; }

        public DateTime? Dispatch_Date { get; set; }

        public string Job_Description { get; set; }

        public string EU_Name { get; set; }

        public string EU_Contact { get; set; }
        public string EU_Email { get; set; }

        public DateTime CreatedOn { get; set; }
        public string Ticket_No { get; set; }

        public string Status { get; set; }

        public string Error_Msg { get; set; }
        public string Type { get; set; }

        public string Certification_Name { get; set; }

        public string TSE_Name { get; set; }
    }
}
