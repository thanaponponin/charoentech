using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.WebPages.Html;

namespace Charoentech_Web.ModelViews
{
    public class AdminModelView
    {
    }

    public class ITEM_CATE_MODEL_VIEW
    {
        [Key]
        public int ITEM_CATE_ID { get; set; }
        [Display(Name = "ชื่อ"), Required]
        public string NAME { get; set; }
        [Display(Name = "รายละเอียด"), Required]
        public string DESCRIPTION { get; set; }
        [Display(Name = "สถานะ"), Required]
        public int STATUS { get; set; }
        [Display(Name = "แสดงในหน้าหลัก")]
        public Nullable<int> FLAG_HOME_PG { get; set; }
        [Display(Name = "ที่อยู่จัดเก็บ")]
        public string IMAGE { get; set; }
        public HttpPostedFileBase ImageFile { get; set; } 
    }

    public class ITEM_MODEL_VIEW
    {
        [Key]
        public int ITEM_ID { get; set; }
        [Display(Name = "ชื่อ"), Required]
        public string NAME { get; set; }
        [AllowHtml]
        [Display(Name = "รายละเอียด"), Required]
        public string DESCRIPTION { get; set; }
        [AllowHtml]
        [Display(Name = "รายละเอียด (Download Tab)")]
        public string DOWNLOAD_TAB { get; set; }
        [Display(Name = "ราคา"), Required]
        public Nullable<decimal> PRICE { get; set; }
        [Display(Name = "ราคาที่ลด")]
        public Nullable<decimal> DISCOUNT { get; set; }
        public Nullable<int> FLAG { get; set; }
        [Display(Name = "เรียงลำดับรายการ"), Required]
        public Nullable<int> ORDER_LIST { get; set; }
        [Display(Name = "รูปภาพ")]
        public string IMAGE { get; set; }
        public string BOSURE { get; set; }
        public string PO { get; set; }
        [Display(Name = "สถานะ"), Required]
        public Nullable<int> STATUS { get; set; }
        public byte[] CREATED_DT { get; set; }
        [Display(Name = "แสดงในหน้าหลัก"), Required]
        public Nullable<int> INDEX_HOME_PG { get; set; }
        [Display(Name = "ประเภทสินค้า")]
        public Nullable<int> ITEM_CATE_ID { get; set; }
        [Display(Name = "สินค้าขายดี"), Required]
        public Nullable<int> BEST_SALLER { get; set; }
        [Display(Name = "สินค้าใหม่"), Required]
        public Nullable<int> NEW { get; set; }
        [Display(Name = "ประเภทสินค้า")]
        public string ITEM_CATE_NAME { get; set; }
        public HttpPostedFileBase ImageFile { get; set; }
        public HttpPostedFileBase ImageFileBOSURE { get; set; }
        public HttpPostedFileBase ImageFilePO { get; set; } 
    }

    public class TEXT_MODEL_VIEW
    {
        public int TEXT_ID { get; set; }
        [Display(Name = "Task Code"), Required]
        public string TASK { get; set; }
        [Display(Name = "ชื่อ"), Required]
        public string NAME { get; set; }
        public string TOPIC { get; set; }
        [AllowHtml]
        [Display(Name = "รายละเอียด"), Required]
        public string DESCRIPTION { get; set; }
        [Display(Name = "รูป")]
        public string IMAGE { get; set; }
        [Display(Name = "สถานะ"), Required]
        public int STATUS { get; set; }
        public string ICON { get; set; }
        public HttpPostedFileBase ImageFile { get; set; } 
    }

    public class SERVICE_CATE_MODEL_VIEW
    {
        public int SERVICE_CATE_ID { get; set; }
        [Display(Name = "ชื่อ"), Required]
        public string NAME { get; set; }
        [Display(Name = "หัวเรื่อง"), Required]
        public string TOPIC { get; set; }
        [AllowHtml]
        [Display(Name = "รายละเอียด"), Required]
        public string DESCRIPTION { get; set; }
        [Display(Name = "รูปภาพ")]
        public string IMAGE { get; set; }
        public string ICON { get; set; }
        [Display(Name = "เรียงลำดับรายการ")]
        public Nullable<int> ORDER_LIST { get; set; }
        [Display(Name = "สถานะ"), Required]
        public int STATUS { get; set; }
        public HttpPostedFileBase ImageFile { get; set; } 
    }

    public class BLOG_MODEL_VIEW
    {
        public int BLOG_ID { get; set; }
        [Display(Name = "ชื่อ"), Required]
        public string NAME { get; set; }
        [Display(Name = "รายละเอียดแบบย่อ"), Required]
        public string TOPIC { get; set; }
        [AllowHtml]
        [Display(Name = "รายละเอียด"), Required]
        public string DESCRIPTION { get; set; }
        [Display(Name = "รูป")]
        public string IMAGE { get; set; }
        [Display(Name = "สถานะ"), Required]
        public int? STATUS { get; set; }
        public HttpPostedFileBase ImageFile { get; set; } 
    }

    public class CUSTOMER_MODEL_VIEW
    {
        public int CUST_ID { get; set; }
        [Display(Name = "ชื่อ"), Required]
        public string NAME { get; set; }
        [Display(Name = "รายละเอียดแบบย่อ"), Required]
        public string TOPIC { get; set; }
        [AllowHtml]
        [Display(Name = "รายละเอียด"), Required]
        public string DESCRIPTION { get; set; }
        [Display(Name = "รูป")]
        public string IMAGE { get; set; }
        [Display(Name = "สถานะ"), Required]
        public Nullable<int> STATUS { get; set; }
        public HttpPostedFileBase ImageFile { get; set; } 
    }

    public class EMAIL_MODEL_VIEW
    {
        public int ID { get; set; }
        [Required(ErrorMessage = "กรุณากรอกข้อมูล")]
        public string fromEmail { get; set; }
        [Required(ErrorMessage = "กรุณากรอกข้อมูล")]
        public string fromPhone { get; set; }
        [Required(ErrorMessage = "กรุณากรอกข้อมูล")]
        public string subject { get; set; }
        [Required(ErrorMessage = "กรุณากรอกข้อมูล")]
        public string description { get; set; }
        public string isActive { get; set; }
    }

    public class ItemForm
    {
        public int ID { get; set; }
        [Display(Name = "ชื่อสินค้า")]
        public string ItemName { get; set; }
        [Display(Name = "จำนวน"), Required]
        public string Qty { get; set; }
        [Display(Name = "ชื่อ-นามสกุล ผู้ติดต่อ"), Required]
        public string Name { get; set; }
        [Display(Name = "ชื่อองค์กร /หน่วยงาน"), Required]
        public string Company { get; set; }
        [Display(Name = "ที่อยู่"), Required]
        public string Address { get; set; }
        [Display(Name = "เบอร์ติดต่อ"), Required]
        public string Phone { get; set; }
        [Display(Name = "อีเมล์"), Required]
        public string Email { get; set; }
        [Display(Name = "ข้อมูลเพิ่มเติม")]
        public string Description { get; set; }
    }

    public class Ticket
    {
        public int ID { get; set; }
        [Display(Name = "ชื่อสินค้า"), Required]
        public string ItemName { get; set; }
        [Display(Name = "ชื่อ-นามสกุล ผู้ติดต่อ"), Required]
        public string Name { get; set; }
        [Display(Name = "ชื่อองค์กร /หน่วยงาน"), Required]
        public string Company { get; set; }
        [Display(Name = "เบอร์ติดต่อ"), Required]
        public string Phone { get; set; }
        [Display(Name = "อีเมล์"), Required]
        public string Email { get; set; }
        [Display(Name = "ข้อมูลเพิ่มเติม")]
        public string Description { get; set; }
    }

    public class Partner
    {
        public string Header { get; set; }
        public string Description { get; set; }
        [Display(Name = "ชื่อองค์กร /หน่วยงาน"), Required]
        public string Company { get; set; }
        [Display(Name = "ชื่อ-นามสกุล ผู้ติดต่อ"), Required]
        public string Name { get; set; }
        [Display(Name = "ตำแหน่ง")]
        public string Position { get; set; }
        [Display(Name = "เบอร์ติดต่อ"), Required]
        public string Phone { get; set; }
        [Display(Name = "อีเมล์"), Required]
        public string Email { get; set; }
        [Display(Name = "ที่อยู่"), Required]
        public string Address { get; set; }
        [Display(Name = "สินค้าที่สนใจ")]
        public string ItemName { get; set; }
    }
}