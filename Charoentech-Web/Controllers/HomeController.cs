using Charoentech_Web.ModelViews;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;

namespace Charoentech_Web.Controllers
{
    public class HomeController : Controller
    {
        CharoenTechEntities db = new CharoenTechEntities();

        public HomeController()
        {
            var menuCate = db.ITEM_CATE.Where(x => x.STATUS == 1).ToList();
            ViewData["menuCate"] = menuCate;
        }

        public ActionResult Index()
        {
            var banner = db.BANNER.Where(x => x.STATUS == 1).ToList();
            ViewData["banner"] = banner;

            var itemCate = db.ITEM_CATE.Where(x => x.FLAG_HOME_PG == 1).ToList();
            ViewData["ItemCate"] = itemCate;

            var item = db.ITEM.Where(x => x.INDEX_HOME_PG == 1 && x.STATUS == 1).Take(16).OrderBy(x => x.ORDER_LIST).ToList();
            ViewData["Item"] = item;
            ViewData["BestSaller"] = item.Where(x => x.BEST_SALLER == 1).OrderBy(x => x.ORDER_LIST).ToList();
            ViewData["New"] = item.Where(x => x.NEW == 1).ToList();

            var text = db.TEXT.Where(x => x.TASK == "SERVICE_HOME" && x.STATUS == 1).Take(4).ToList();
            ViewData["text"] = text;

            return View();
        }

        public ActionResult About()
        {
            var about = db.TEXT.FirstOrDefault(x => x.TASK == "ABOUT" && x.STATUS == 1);
            ViewData["about"] = about;

            var purpose = db.TEXT.FirstOrDefault(x => x.TASK == "PURPOSE" && x.STATUS == 1);
            ViewData["purpose"] = purpose;

            return View();
        }

        public ActionResult Shopping(int id)
        {
            var oItemCate = db.ITEM_CATE.Where(x => x.ITEM_CATE_ID == id).FirstOrDefault();
            ViewBag.cateName = oItemCate.NAME;

            var olistItem = db.ITEM.Where(x => x.ITEM_CATE_ID == id).OrderBy(x => x.ORDER_LIST).ToList();
            ViewData["item"] = olistItem;

            return View();
        }

        public ActionResult ShoppingDetail(int id)
        {
            //var oItemCate = db.ITEM_CATE.Where(x => x.ITEM_CATE_ID == id).FirstOrDefault();
            //ViewBag.cateName = oItemCate.NAME;

            var oItem = db.ITEM.Where(x => x.ITEM_ID == id).FirstOrDefault();
            ViewData["item"] = oItem;

            var oItemCate = db.ITEM_CATE.Where(x => x.ITEM_CATE_ID == oItem.ITEM_CATE_ID).FirstOrDefault();
            ViewData["oItemCate"] = oItemCate.NAME;

            var model = new ItemForm();
            model.ID = id;
            model.ItemName = oItem.NAME;

            TempData["default_active"] = (TempData["contact_active"] == "active")? "" : "active";
            TempData["default_active_show"] = (TempData["contact_active"] == "active") ? "" : "show active";

            return View(model);
        }

        public FileResult Download(int id, string isBOSURE)
        {
            string UploadPath = HostingEnvironment.ApplicationPhysicalPath;
            var file = db.ITEM.FirstOrDefault(x => x.ITEM_ID == id);
            string path = Path.Combine(UploadPath, file.BOSURE.TrimStart('/', '\\'));
            string fileName = path.Split('/').Last();
            if(isBOSURE == "0")
            {
                path = Path.Combine(UploadPath, file.PO.TrimStart('/', '\\'));
                fileName = path.Split('/').Last();
            }
            byte[] fileBytes = System.IO.File.ReadAllBytes(path);
            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
        }

        [HttpPost]
        public ActionResult ShoppingDetail(ItemForm model)
        {
            string message = "";
            if (ModelState.IsValid)
            {
                try
                {

                    var senderEmail = new MailAddress("Support@thai-icontech.com", model.Email);
                    var password = "qzRb25*3";
                    var sub = model.Name;
                    var body = model.ItemName + "\r\n" + model.Name + "\r\n" + model.Qty + "\r\n" + model.Phone + "\r\n" + model.Company + "\r\n" + model.Address + "\r\n" + model.Email + "\r\n" + model.Description;
                    var smtp = new SmtpClient
                    {
                        Host = "mail.thai-icontech.com",
                        Port = 25,
                        Credentials = new NetworkCredential(senderEmail.Address, password)
                    };

                    MailMessage mailMessage = new MailMessage();
                    mailMessage.From = senderEmail;
                    mailMessage.To.Add("charoentech@gmail.com");
                    mailMessage.Subject = sub;
                    mailMessage.Body = body;

                    smtp.Send(mailMessage);

                    message = "ส่งเมล์สำเร็จ กรุณารอเจ้าหน้าที่ติดต่อกลับ";
                }
                catch (Exception)
                {
                    message = "ไม่สามารถส่งเมล์ได้ กรุณาลองใหม่อีกครั้ง";
                }

                TempData["notice"] = message;
                TempData["contact_active"] = "active";
                TempData["default_active"] = "";

                TempData["contact_active_show"] = "show active";
                TempData["default_active_show"] = "";

                return RedirectToAction("ShoppingDetail", new { id = model.ID });
            }

            var oItem = db.ITEM.Where(x => x.ITEM_ID == model.ID).FirstOrDefault();
            ViewData["item"] = oItem;

            var oItemCate = db.ITEM_CATE.Where(x => x.ITEM_CATE_ID == oItem.ITEM_CATE_ID).FirstOrDefault();
            ViewData["oItemCate"] = oItemCate.NAME;

            var model2 = new ItemForm();
            model2.ID = model.ID;

            TempData["contact_active"] = "active";
                TempData["default_active"] = "";

                TempData["contact_active_show"] = "show active";
                TempData["default_active_show"] = "";

            return View(model2);
        }

        public ActionResult BlogList()
        {
            //var oItemCate = db.ITEM_CATE.Where(x => x.ITEM_CATE_ID == id).FirstOrDefault();
            //ViewBag.cateName = oItemCate.NAME;

            var oListBlog = db.BLOG.Where(x => x.STATUS == 1).OrderByDescending(x => x.BLOG_ID).ToList();
            ViewData["oListBlog"] = oListBlog;

            return View();
        }

        public ActionResult BlogDetail(int id)
        {
            //var oItemCate = db.ITEM_CATE.Where(x => x.ITEM_CATE_ID == id).FirstOrDefault();
            //ViewBag.cateName = oItemCate.NAME;

            var oBlog = db.BLOG.Where(x => x.STATUS == 1 && x.BLOG_ID == id).FirstOrDefault();
            ViewData["oBlog"] = oBlog;

            return View();
        }

        public ActionResult ServiceList()
        {
            var oListService = db.SERVICE_CATE.Where(x => x.STATUS == 1);
            ViewData["oCountService"] = oListService.Count();
            ViewData["oListService"] = oListService.ToList();

            return View();
        }

        public ActionResult ServiceDetail(int id)
        {
            var oService = db.SERVICE_CATE.FirstOrDefault(x => x.STATUS == 1 && x.SERVICE_CATE_ID == id);
            ViewData["oService"] = oService;

            return View();
        }

        public ActionResult CustomerList()
        {
            var oListCust = db.CUSTOMER.Where(x => x.STATUS == 1).OrderByDescending(x => x.CUST_ID);
            ViewData["oListCust"] = oListCust.ToList();

            return View();
        }

        public ActionResult CustomerDetail(int id)
        {
            //var oItemCate = db.ITEM_CATE.Where(x => x.ITEM_CATE_ID == id).FirstOrDefault();
            //ViewBag.cateName = oItemCate.NAME;

            var oCust = db.CUSTOMER.Where(x => x.STATUS == 1 && x.CUST_ID == id).FirstOrDefault();
            ViewData["oCust"] = oCust;

            return View();
        }

        public ActionResult Partner()
        {
            var text = db.TEXT.FirstOrDefault(x => x.TASK == "PARTNER" && x.STATUS == 1);
            var model = new Partner();
            model.Header = text.NAME;
            model.Description = text.DESCRIPTION;

            return View(model);
        }

        [HttpPost]
        public ActionResult Partner(Partner model)
        {
            var text = db.TEXT.FirstOrDefault(x => x.TASK == "PARTNER" && x.STATUS == 1);
            var model2 = new Partner();
            model2.Header = text.NAME;
            model2.Description = text.DESCRIPTION;

            string message = "";
            if (ModelState.IsValid)
            {
                try
                {
                    var senderEmail = new MailAddress("Support@thai-icontech.com", model.Email);
                    var password = "qzRb25*3";
                    var sub = "PARTNER " + model.Name;
                    var body = model.Name + "\r\n" + model.Company + "\r\n" + model.Position + "\r\n" + model.Phone + "\r\n" + model.Email + "\r\n" + model.Address + "\r\n" + model.ItemName;
                    var smtp = new SmtpClient
                    {
                        Host = "mail.thai-icontech.com",
                        Port = 25,
                        Credentials = new NetworkCredential(senderEmail.Address, password)
                    };

                    MailMessage mailMessage = new MailMessage();
                    mailMessage.From = senderEmail;
                    mailMessage.To.Add("charoentech@gmail.com");
                    mailMessage.Subject = sub;
                    mailMessage.Body = body;

                    smtp.Send(mailMessage);

                    message = "ส่งเมล์สำเร็จ กรุณารอเจ้าหน้าที่ติดต่อกลับ";
                }
                catch (Exception)
                {
                    message = "ไม่สามารถส่งเมล์ได้ กรุณาลองใหม่อีกครั้ง";
                }

                TempData["notice"] = message;

                return RedirectToAction("Partner");
            }

            return View(model2);
        }

        public ActionResult Contact()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Contact(EMAIL_MODEL_VIEW model)
        {
            string message = "";
            if (ModelState.IsValid)
            {
                try
                {
                    var senderEmail = new MailAddress("Support@thai-icontech.com", model.fromEmail);
                    var password = "qzRb25*3";
                    var sub = model.subject;
                    var body = model.description + "\r\n" + model.fromPhone;
                    var smtp = new SmtpClient
                    {
                        Host = "mail.thai-icontech.com",
                        Port = 25,
                        Credentials = new NetworkCredential(senderEmail.Address, password)
                    };

                    MailMessage mailMessage = new MailMessage();
                    mailMessage.From = senderEmail;
                    mailMessage.To.Add("charoentech@gmail.com");
                    mailMessage.Subject = sub;
                    mailMessage.Body = body;

                    smtp.Send(mailMessage);

                    message = "ส่งเมล์สำเร็จ กรุณารอเจ้าหน้าที่ติดต่อกลับ";
                }
                catch (Exception)
                {
                    message = "ไม่สามารถส่งเมล์ได้ กรุณาลองใหม่อีกครั้ง";
                }

                TempData["notice"] = message;

                return RedirectToAction("Contact");
            }

            return View();
        }

        public ActionResult Ticket()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Ticket(Ticket model)
        {
            string message = "";
            if (ModelState.IsValid)
            {
                try
                {
                    var date = DateTime.Now.ToString("yyMM");
                    var ticket = "";
                    int ticketNo = 1;
                    var ticketObj = db.TICKET.OrderByDescending(x=>x.NUMBER).FirstOrDefault();
                    if (ticketObj == null)
                    {
                        ticket = date + ticketNo.ToString("D4");
                    }
                    else
                    {
                        ticketNo = Convert.ToInt32(ticketObj.NUMBER) + 1;
                        ticket = date + ticketNo.ToString("D4");
                    }

                    var senderEmail = new MailAddress("Support@thai-icontech.com", model.Email);
                    var password = "qzRb25*3";
                    var sub = "สอบถาม/แจ้งปัญหา จากคุณ"+ model.Name;
                    var body = "TICKET NO." + ticket + "\r\n" + model.ItemName + "\r\n" + model.Name + "\r\n" + model.Email + "\r\n" + model.Company + "\r\n" + model.Description + "\r\n" + model.Phone;
                    var smtp = new SmtpClient
                    {
                        Host = "mail.thai-icontech.com",
                        Port = 25,
                        Credentials = new NetworkCredential(senderEmail.Address, password)
                    };

                    MailMessage mailMessage = new MailMessage();
                    mailMessage.From = senderEmail;
                    mailMessage.To.Add("charoentech@gmail.com");
                    mailMessage.Subject = sub;
                    mailMessage.Body = body;

                    smtp.Send(mailMessage);

                    //Customer
                    var sub2 = "สอบถาม/แจ้งปัญหา จากคุณ" + model.Name;
                    var body2 = "TICKET NO." + ticket + "\r\n  ข้อมูลได้ถูกส่งไปยังเจ้าหน้าที่เรียบร้อยแล้ว กรุณารอติดต่อกลับ \r\n" + model.ItemName + "\r\n" + model.Name + "\r\n" + model.Email + "\r\n" + model.Company + "\r\n" + model.Description + "\r\n" + model.Phone;
                    MailMessage mailMessage2 = new MailMessage();
                    mailMessage2.From = senderEmail;
                    mailMessage2.To.Add(model.Email);
                    mailMessage2.Subject = sub2;
                    mailMessage2.Body = body2;

                    smtp.Send(mailMessage2);
                    TICKET t = new TICKET()
                    {
                        NUMBER = ticketNo
                    };
                    db.TICKET.Add(t);
                    db.SaveChanges();

                    message = "ส่งเมล์สำเร็จ กรุณารอเจ้าหน้าที่ติดต่อกลับ";
                }
                catch (Exception ex)
                {
                    message = "ไม่สามารถส่งเมล์ได้ กรุณาลองใหม่อีกครั้ง" + ex.InnerException;
                }

                TempData["notice"] = message;

                return RedirectToAction("Ticket");
            }

            return View();
        }
    }
}