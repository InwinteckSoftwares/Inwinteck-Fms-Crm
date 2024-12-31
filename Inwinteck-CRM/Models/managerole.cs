using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Inwinteck_CRM.Models
{

    public class managerole
    {
    }

    public class RoleList
    {
        public string id { get; set; }
        public string name { get; set; }
    }

    [Table("business_component_master")]
    public class business_component_master
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int business_object_id { get; set; }
        public string business_object_name { get; set; }

        [Required]
        public int menu_id { get; set; }
        public string display_text { get; set; }
        public string target_url { get; set; }

        [Required]
        public int display_order_no { get; set; }
    }
    [Table("privilege_master")]
    public class privilege_master
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int privilege_id { get; set; }
        public string privilege_name { get; set; }

    }
    [Table("menu_master")]
    public class menu_master
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int menu_id { get; set; }
        public string display_name { get; set; }
        public int parent_menu_id { get; set; }
        public int menu_display_order { get; set; }
    }
    [Table("user_permission_map")]
    public class user_permission_map
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int unqi_id { get; set; }
        public string user_id { get; set; }
        public int business_object_id { get; set; }
        //public int Privilege_id { get; set; }
        public bool record_insert { get; set; }
        public bool record_update { get; set; }
        public bool record_delete { get; set; }
        public bool record_view { get; set; }
        public bool record_import { get; set; }
        public bool record_export { get; set; }

        public string user_role_id { get; set; }
    }
    public class user_manage
    {
        public string UserId { get; set; }
        //public string Fname { get; set; }
        //public string Lname { get; set; }
        //public string Email { get; set; }
        public string UserName { get; set; }
    }
    public class checkrl
    {
        public int unqi_id { get; set; }
        public string user_id { get; set; }
        public int business_object_id { get; set; }
        //public int Privilege_id { get; set; }
        public bool record_insert { get; set; }
        public bool record_update { get; set; }
        public bool record_delete { get; set; }
        public bool record_view { get; set; }
        public bool record_import { get; set; }
        public bool record_export { get; set; }

        public string user_role_id { get; set; }
    }
    public class security_management
    {
        public string RoleId { get; set; }
        public string UserId { get; set; }
        public List<RoleList> Role { get; set; }
        public List<user_manage> UserManage { get; set; }
        public List<user_permission_map> UserPermission { get; set; }
        public List<checkrl> UP { get; set; }
        public List<menu_master> MenuMaster { get; set; }
        public List<menu_master> Submenu { get; set; }
        public List<business_component_master> ComponentMaster { get; set; }
        public List<privilege_master> Previlege { get; set; }
    }

    public class security_management_deparment
    {
        public string RoleId { get; set; }
        public string UserId { get; set; }
        public List<RoleList> Role { get; set; }
        public List<user_manage> UserManage { get; set; }
        public List<user_permission_map> UserPermission { get; set; }
        public List<checkrl> UP { get; set; }
        public List<menu_master> MenuMaster { get; set; }
        public List<menu_master> Submenu { get; set; }
        public List<business_component_master> ComponentMaster { get; set; }
        public List<privilege_master> Previlege { get; set; }
    }
    public class managemenu
    {
        public List<user_permission_map> UserPermission { get; set; }
        public List<menu_master> MenuMaster { get; set; }
        public List<menu_master> Submenu { get; set; }
        public List<business_component_master> ComponentMaster { get; set; }
        public List<privilege_master> Previlege { get; set; }
    }
    public class Menu
    {
        public List<menu_master> mm { get; set; }
        public List<firstLevel> fl { get; set; }
        public List<secondlevel> sl { get; set; }

    }
    public class secondlevel
    {
        public int id { get; set; }
        public int menu_id { get; set; }
        public string display_text { get; set; }
        public string target_url { get; set; }
        public int display_order_no { get; set; }
        public bool record_insert { get; set; }
        public bool record_delete { get; set; }
        public bool record_update { get; set; }
        public bool record_export { get; set; }
        public bool record_import { get; set; }
        public bool record_view { get; set; }
    }
    public class firstLevel
    {
        public int menu_id { get; set; }
        public string display_name { get; set; }
        public int parent_menu_id { get; set; }
        public int menu_display_order { get; set; }
    }
}