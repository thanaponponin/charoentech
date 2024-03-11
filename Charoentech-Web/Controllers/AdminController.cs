using Charoentech_Web.ModelViews;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;
using System.IO;
using System.Configuration;
using System.Web.Hosting;
using Charoentech_Web.Classes;
using System.Net;
using System.Text;

namespace Charoentech_Web.Controllers
{
    [Charoentech_Web.Classes.Filter]
    public class AdminController : Controller
    {
        CharoenTechEntities db = new CharoenTechEntities();
        // GET: Admin
        public ActionResult Index()
        {
            return RedirectToAction("User");
        }

        public ActionResult ItemCate(int? page)
        {
            var itemCate = db.ITEM_CATE.ToList();

            int pageSize = 10;
            int pageNumber = (page ?? 1);

            return View(itemCate.ToPagedList(pageNumber, pageSize));
        }

        public ActionResult ItemCate_Create()
        {
            List<SelectListItem> model = new List<SelectListItem>(){
                new SelectListItem() { Selected = true, Text = "เปิด", Value = "1" },
                new SelectListItem() { Selected = true, Text = "ปิด", Value = "0" }
            };

            ViewBag.STATUS = model;

            return View();
        }

        [HttpPost]
        public ActionResult ItemCate_Create(ITEM_CATE_MODEL_VIEW model)
        {
            if (ModelState.IsValid)
            {
                ITEM_CATE m = new ITEM_CATE();
                m.NAME = model.NAME;
                m.DESCRIPTION = model.DESCRIPTION;
                m.FLAG_HOME_PG = model.FLAG_HOME_PG;
                m.STATUS = model.STATUS;
                m.IMAGE = model.IMAGE;

                db.ITEM_CATE.Add(m);
                db.SaveChanges();

                TempData["notice"] = "เพิ่มรายการเรียบร้อย";
                return RedirectToAction("ItemCate_Create");
            }

            List<SelectListItem> status = new List<SelectListItem>(){
                new SelectListItem() { Selected = true, Text = "เปิด", Value = "1" },
                new SelectListItem() { Selected = true, Text = "ปิด", Value = "0" }
            };

            ViewBag.STATUS = status;
            return View(model);
        }

        public ActionResult ItemCate_Edit(int id)
        {
            ITEM_CATE_MODEL_VIEW model = (from a in db.ITEM_CATE
                                          where a.ITEM_CATE_ID == id
                                            select new ITEM_CATE_MODEL_VIEW
                                               { 
                                                   ITEM_CATE_ID = a.ITEM_CATE_ID,
                                                   NAME = a.NAME,
                                                   DESCRIPTION = a.DESCRIPTION,
                                                   FLAG_HOME_PG = a.FLAG_HOME_PG,
                                                   IMAGE = a.IMAGE,
                                                   STATUS = a.STATUS
                                               }).FirstOrDefault();

            List<SelectListItem> status = new List<SelectListItem>(){
                new SelectListItem() { Selected = model.STATUS == 1 ? true : false, Text = "เปิด", Value = "1" },
                new SelectListItem() { Selected = model.STATUS == 0 ? true : false, Text = "ปิด", Value = "0" }
            };

            List<SelectListItem> status2 = new List<SelectListItem>(){
                new SelectListItem() { Selected = (model.FLAG_HOME_PG == 1)? true : false, Text = "เปิด", Value = "1" },
                new SelectListItem() { Selected = (model.FLAG_HOME_PG == 0)? true : false, Text = "ปิด", Value = "0" }
            };

            ViewBag.STATUS1 = status;
            ViewBag.STATUS2 = status2;

            return View(model);
        }

        [HttpPost]
        public ActionResult ItemCate_Edit(ITEM_CATE_MODEL_VIEW model)
        {
            if (ModelState.IsValid)
            {
                string FileName = null;
                ITEM_CATE item = db.ITEM_CATE.FirstOrDefault(x => x.ITEM_CATE_ID == model.ITEM_CATE_ID);

                if (model.ImageFile != null)
                {
                    //Get Upload path from Web.Config file AppSettings.  
                    string UploadPath = HostingEnvironment.ApplicationPhysicalPath;

                    string oldFilename = Path.Combine(UploadPath, (string.IsNullOrEmpty(item.IMAGE)) ? "" : item.IMAGE.TrimStart('/', '\\'));

                    if (System.IO.File.Exists(oldFilename))
                    {
                        System.IO.File.Delete(oldFilename);
                    }

                    FileName = Path.GetFileNameWithoutExtension(model.ImageFile.FileName);
                    //To Get File Extension  
                    string FileExtension = Path.GetExtension(model.ImageFile.FileName);

                    //Add Current Date To Attached File Name  
                    FileName = DateTime.Now.ToString("yyyyMMdd") + "-" + FileName.Trim() + FileExtension;

                    //Its Create complete path to store in server.  
                    model.IMAGE = UploadPath + "img\\product_cate\\" + FileName;

                    //To copy and save file into server.  
                    model.ImageFile.SaveAs(model.IMAGE);

                    item.IMAGE = "img/product_cate/" + FileName;
                }

                item.NAME = model.NAME;
                item.FLAG_HOME_PG = model.FLAG_HOME_PG;
                item.STATUS = model.STATUS;
                item.DESCRIPTION = model.DESCRIPTION;

                db.SaveChanges();

                TempData["notice"] = "แก้ไขรายการเรียบร้อย";
                return RedirectToAction("ItemCate_Edit", new { id = model.ITEM_CATE_ID });
            }

            List<SelectListItem> status = new List<SelectListItem>(){
                new SelectListItem() { Selected = (model.STATUS == 1)? true : false, Text = "เปิด", Value = "1" },
                new SelectListItem() { Selected = (model.STATUS == 0)? true : false, Text = "ปิด", Value = "0" }
            };

            List<SelectListItem> status2 = new List<SelectListItem>(){
                new SelectListItem() { Selected = (model.FLAG_HOME_PG == 1)? true : false, Text = "เปิด", Value = "1" },
                new SelectListItem() { Selected = (model.FLAG_HOME_PG == 0)? true : false, Text = "ปิด", Value = "0" }
            };

            ViewBag.STATUS1 = status;
            ViewBag.STATUS2 = status2;

            return View(model);
        }

        public ActionResult Item(int? page)
        {
            var item = (from i in db.ITEM
                       join item_cate in db.ITEM_CATE on i.ITEM_CATE_ID equals item_cate.ITEM_CATE_ID
                        select new ModelViews.ITEM_MODEL_VIEW
                       {
                           ITEM_ID = i.ITEM_ID,
                           ITEM_CATE_NAME = item_cate.NAME,
                           NAME = i.NAME,
                           NEW = i.NEW,
                           INDEX_HOME_PG = i.INDEX_HOME_PG,
                           STATUS = i.STATUS,
                           ORDER_LIST = i.ORDER_LIST
                       });

            int pageSize = 10;
            int pageNumber = (page ?? 1);

            return View(item.OrderBy(x => x.ITEM_ID).ToPagedList(pageNumber, pageSize));
        }

        public ActionResult Item_Create()
        {
            List<SelectListItem> status = new List<SelectListItem>(){
                new SelectListItem() { Selected = true, Text = "เปิด", Value = "1" },
                new SelectListItem() { Selected = false, Text = "ปิด", Value = "0" }
            };

            List<SelectListItem> model2 = new List<SelectListItem>(){
                new SelectListItem() { Selected = true, Text = "ใช่", Value = "1" },
                new SelectListItem() { Selected = false, Text = "ไม่ใช่", Value = "0" }
            };

            var listItemCate = (from item_cate in db.ITEM_CATE
                               select new SelectListItem
                               {
                                   Text = item_cate.NAME,
                                   Value = item_cate.ITEM_CATE_ID.ToString()
                               });

            ViewBag.STATUS = status;
            ViewBag.STATUS2 = model2;
            ViewBag.ITEM_CATE_LIST = listItemCate;

            return View();
        }

        public ActionResult Item_Edit(int id)
        {
            ITEM_MODEL_VIEW model = (from a in db.ITEM
                                          where a.ITEM_ID == id
                                          select new ITEM_MODEL_VIEW
                                          { 
                                              ITEM_ID = a.ITEM_ID,
                                              ITEM_CATE_ID = a.ITEM_CATE_ID,
                                              NAME = a.NAME,
                                              DESCRIPTION = a.DESCRIPTION, 
                                              DOWNLOAD_TAB = a.DOWNLOAD_TAB,
                                              BEST_SALLER = a.BEST_SALLER, 
                                              BOSURE = a.BOSURE, 
                                              PO = a.PO, 
                                              DISCOUNT = a.DISCOUNT,
                                              PRICE = a.PRICE, 
                                              FLAG = a.FLAG, 
                                              INDEX_HOME_PG = a.INDEX_HOME_PG, 
                                              NEW = a.NEW, 
                                              ORDER_LIST = a.ORDER_LIST,
                                              IMAGE = a.IMAGE,
                                              STATUS = a.STATUS
                                          }).FirstOrDefault();

            List<SelectListItem> status = new List<SelectListItem>(){
                new SelectListItem() { Selected = model.STATUS == 1 ? true : false, Text = "เปิด", Value = "1" },
                new SelectListItem() { Selected = model.STATUS == 0 ? true : false, Text = "ปิด", Value = "0" }
            };

            List<SelectListItem> status2 = new List<SelectListItem>(){
                new SelectListItem() { Selected = model.NEW == 1 ? true : false, Text = "ใช่", Value = "1" },
                new SelectListItem() { Selected = model.NEW == 0 ? true : false, Text = "ไม่ใช่", Value = "0" }
            };

            List<SelectListItem> status3 = new List<SelectListItem>(){
                new SelectListItem() { Selected = model.BEST_SALLER == 1 ? true : false, Text = "ใช่", Value = "1" },
                new SelectListItem() { Selected = model.BEST_SALLER == 0 ? true : false, Text = "ไม่ใช่", Value = "0" }
            };

            List<SelectListItem> status4 = new List<SelectListItem>(){
                new SelectListItem() { Selected = model.INDEX_HOME_PG == 1 ? true : false, Text = "ใช่", Value = "1" },
                new SelectListItem() { Selected = model.INDEX_HOME_PG == 0 ? true : false, Text = "ไม่ใช่", Value = "0" }
            };

            var listItemCate = (from item_cate in db.ITEM_CATE
                                select new SelectListItem
                                {
                                    Selected = model.ITEM_CATE_ID == item_cate.ITEM_CATE_ID ? true : false,
                                    Text = item_cate.NAME,
                                    Value = item_cate.ITEM_CATE_ID.ToString()
                                });

            ViewBag.STATUS1 = status;
            ViewBag.STATUS2 = status2;
            ViewBag.STATUS3 = status3;
            ViewBag.STATUS4 = status4;
            ViewBag.ITEM_CATE_LIST = listItemCate;

            ViewBag.STATUS1 = status;

            return View(model);
        }

        [HttpPost]
        public ActionResult Item_Edit(ITEM_MODEL_VIEW model)
        {
            if (ModelState.IsValid)
            {
                string FileName = null;
                string FileNameBOSURE = null;
                string FileNamePO = null;

                ITEM item = db.ITEM.FirstOrDefault(x => x.ITEM_ID == model.ITEM_ID);

                if (model.ImageFile != null)
                {
                    //Get Upload path from Web.Config file AppSettings.  
                    string UploadPath = HostingEnvironment.ApplicationPhysicalPath;

                    string oldFilename = Path.Combine(UploadPath, (string.IsNullOrEmpty(item.IMAGE)) ? "" : item.IMAGE.TrimStart('/', '\\'));

                    if (System.IO.File.Exists(oldFilename))
                    {
                        System.IO.File.Delete(oldFilename);
                    }

                    FileName = Path.GetFileNameWithoutExtension(model.ImageFile.FileName);
                    //To Get File Extension  
                    string FileExtension = Path.GetExtension(model.ImageFile.FileName);

                    //Add Current Date To Attached File Name  
                    FileName = DateTime.Now.ToString("yyyyMMdd") + "-" + FileName.Trim() + FileExtension;

                    //Its Create complete path to store in server.  
                    model.IMAGE = UploadPath + "img\\products\\" + FileName;

                    //To copy and save file into server.  
                    model.ImageFile.SaveAs(model.IMAGE);

                    item.IMAGE = "img/products/" + FileName;
                }

                if (model.ImageFileBOSURE != null)
                {
                    //Get Upload path from Web.Config file AppSettings.  
                    string UploadPath = HostingEnvironment.ApplicationPhysicalPath;
                    if (model.BOSURE != null)
                    {
                        string oldFilename = Path.Combine(UploadPath, model.BOSURE.TrimStart('/', '\\'));

                        if (System.IO.File.Exists(oldFilename))
                        {
                            System.IO.File.Delete(oldFilename);
                        }
                    }

                    FileNameBOSURE = Path.GetFileNameWithoutExtension(model.ImageFileBOSURE.FileName);
                    //To Get File Extension  
                    string FileExtension = Path.GetExtension(model.ImageFileBOSURE.FileName);
                    FileNameBOSURE = DateTime.Now.ToString("yyyyMMdd") + "-" + FileNameBOSURE.Trim() + FileExtension;

                    model.BOSURE = UploadPath + "upload\\bosure\\" + FileNameBOSURE;

                    model.ImageFileBOSURE.SaveAs(model.BOSURE);

                    item.BOSURE = "upload/bosure/" + FileNameBOSURE;
                }

                if (model.ImageFilePO != null)
                {
                    //Get Upload path from Web.Config file AppSettings.  
                    string UploadPath = HostingEnvironment.ApplicationPhysicalPath;

                    if (model.PO != null)
                    {
                        string oldFilename = Path.Combine(UploadPath, model.PO.TrimStart('/', '\\'));

                        if (System.IO.File.Exists(oldFilename))
                        {
                            System.IO.File.Delete(oldFilename);
                        }
                    }

                    FileNamePO= Path.GetFileNameWithoutExtension(model.ImageFilePO.FileName);
                    //To Get File Extension  
                    string FileExtension = Path.GetExtension(model.ImageFilePO.FileName);
                    FileNamePO = DateTime.Now.ToString("yyyyMMdd") + "-" + FileNamePO.Trim() + FileExtension;

                    model.PO = UploadPath + "upload\\po\\" + FileNamePO;

                    model.ImageFilePO.SaveAs(model.PO);

                    item.PO = "upload/po/" + FileNamePO;
                }

                item.ITEM_CATE_ID = model.ITEM_CATE_ID;
                item.NAME = model.NAME;
                item.DESCRIPTION = model.DESCRIPTION;
                item.DOWNLOAD_TAB = model.DOWNLOAD_TAB;
                item.BEST_SALLER = model.BEST_SALLER;
                item.DISCOUNT = model.DISCOUNT;
                item.PRICE = model.PRICE;
                item.FLAG = model.FLAG;
                item.INDEX_HOME_PG = model.INDEX_HOME_PG;
                item.NEW = model.NEW;
                item.ORDER_LIST = model.ORDER_LIST;
                item.STATUS = model.STATUS;

                db.SaveChanges();

                TempData["notice"] = "แก้ไขรายการเรียบร้อย";
                return RedirectToAction("Item_Edit", new { id = model.ITEM_ID });
            }

            List<SelectListItem> status = new List<SelectListItem>(){
                new SelectListItem() { Selected = model.STATUS == 1 ? true : false, Text = "เปิด", Value = "1" },
                new SelectListItem() { Selected = model.STATUS == 0 ? true : false, Text = "ปิด", Value = "0" }
            };

            List<SelectListItem> status2 = new List<SelectListItem>(){
                new SelectListItem() { Selected = model.NEW == 1 ? true : false, Text = "ใช่", Value = "1" },
                new SelectListItem() { Selected = model.NEW == 0 ? true : false, Text = "ไม่ใช่", Value = "0" }
            };

            List<SelectListItem> status3 = new List<SelectListItem>(){
                new SelectListItem() { Selected = model.BEST_SALLER == 1 ? true : false, Text = "ใช่", Value = "1" },
                new SelectListItem() { Selected = model.BEST_SALLER == 0 ? true : false, Text = "ไม่ใช่", Value = "0" }
            };

            List<SelectListItem> status4 = new List<SelectListItem>(){
                new SelectListItem() { Selected = model.INDEX_HOME_PG == 1 ? true : false, Text = "ใช่", Value = "1" },
                new SelectListItem() { Selected = model.INDEX_HOME_PG == 0 ? true : false, Text = "ไม่ใช่", Value = "0" }
            };

            var listItemCate = (from item_cate in db.ITEM_CATE
                                select new SelectListItem
                                {
                                    Selected = model.ITEM_CATE_ID == item_cate.ITEM_CATE_ID ? true : false,
                                    Text = item_cate.NAME,
                                    Value = item_cate.ITEM_CATE_ID.ToString()
                                });

            ViewBag.STATUS1 = status;
            ViewBag.STATUS2 = status2;
            ViewBag.STATUS3 = status3;
            ViewBag.STATUS4 = status4;
            ViewBag.ITEM_CATE_LIST = listItemCate;

            return View(model);
        }

        [HttpPost]
        public ActionResult Item_Create(ITEM_MODEL_VIEW model)
        {
            if (ModelState.IsValid)
            {
                //Get Upload path from Web.Config file AppSettings.  
                string UploadPath = HostingEnvironment.ApplicationPhysicalPath;

                string FileName = Path.GetFileNameWithoutExtension(model.ImageFile.FileName);
                
                //To Get File Extension  
                string FileExtension = Path.GetExtension(model.ImageFile.FileName);
                //string FileExtensionBOSURE = Path.GetExtension(model.ImageFileBOSURE.FileName);
                //string FileExtensionPO= Path.GetExtension(model.ImageFilePO.FileName);

                //Add Current Date To Attached File Name  
                FileName = DateTime.Now.ToString("yyyyMMdd") + "-" + FileName.Trim() + FileExtension;

                //Its Create complete path to store in server.  
                model.IMAGE = UploadPath + "img\\products\\" + FileName;

                string FileNameBOSURE = "";
                if (model.ImageFileBOSURE != null)
                {
                    FileNameBOSURE = Path.GetFileNameWithoutExtension(model.ImageFileBOSURE.FileName);
                    FileNameBOSURE = DateTime.Now.ToString("yyyyMMdd") + "-" + FileNameBOSURE.Trim() + FileExtension;
                    model.BOSURE = UploadPath + "upload\\bosure\\" + FileNameBOSURE;
                    model.ImageFile.SaveAs(model.BOSURE);
                }

                string FileNamePO = "";
                if (model.ImageFilePO != null)
                {
                    FileNamePO = Path.GetFileNameWithoutExtension(model.ImageFilePO.FileName);
                    FileNamePO = DateTime.Now.ToString("yyyyMMdd") + "-" + FileNamePO.Trim() + FileExtension;
                    model.PO = UploadPath + "upload\\po\\" + FileNamePO;
                    model.ImageFile.SaveAs(model.PO);
                }

                //To copy and save file into server.  
                model.ImageFile.SaveAs(model.IMAGE);

                ITEM item = new ITEM();
                item.NAME = model.NAME;
                item.NEW = model.NEW;
                item.ORDER_LIST = model.ORDER_LIST;
                item.PRICE = model.PRICE;
                item.DISCOUNT = model.DISCOUNT;
                item.ITEM_CATE_ID = model.ITEM_CATE_ID;
                item.IMAGE = "img/products/" + FileName;
                item.BOSURE = (model.BOSURE == null) ? "" : "upload/bosure/" + FileNameBOSURE;
                item.PO = (model.PO == null) ? "" : "upload/po/" + FileNamePO;
                item.STATUS = model.STATUS;
                item.BEST_SALLER = model.BEST_SALLER;
                item.FLAG = model.FLAG;
                item.DESCRIPTION = model.DESCRIPTION;
                item.DOWNLOAD_TAB = model.DOWNLOAD_TAB;
                item.INDEX_HOME_PG = model.INDEX_HOME_PG;

                db.ITEM.Add(item);
                db.SaveChanges();

                TempData["notice"] = "เพิ่มรายการเรียบร้อย";
                return RedirectToAction("Item_Create");
            }

            List<SelectListItem> status = new List<SelectListItem>(){
                new SelectListItem() { Selected = true, Text = "เปิด", Value = "1" },
                new SelectListItem() { Selected = false, Text = "ปิด", Value = "0" }
            };

            List<SelectListItem> model2 = new List<SelectListItem>(){
                new SelectListItem() { Selected = true, Text = "ใช่", Value = "1" },
                new SelectListItem() { Selected = false, Text = "ไม่ใช่", Value = "0" }
            };

            var listItemCate = (from item_cate in db.ITEM_CATE
                                select new SelectListItem
                                {
                                    Text = item_cate.NAME,
                                    Value = item_cate.ITEM_CATE_ID.ToString()
                                });

            ViewBag.STATUS = status;
            ViewBag.STATUS2 = model2;
            ViewBag.ITEM_CATE_LIST = listItemCate;

            return View(model);
        }

        public ActionResult Text(int? page)
        {
            var text = db.TEXT.ToList();

            int pageSize = 10;
            int pageNumber = (page ?? 1);

            return View(text.ToPagedList(pageNumber, pageSize));
        }

        public ActionResult Text_Edit(int id)
        {
            var text = (from a in db.TEXT
                       where a.TEXT_ID == id
                       select new TEXT_MODEL_VIEW
                       {
                           TEXT_ID = a.TEXT_ID,
                           IMAGE = a.IMAGE,
                           DESCRIPTION = a.DESCRIPTION,
                           ICON = a.ICON,
                           NAME = a.NAME,
                           STATUS = a.STATUS,
                           TASK = a.TASK,
                           TOPIC = a.TOPIC
                       }).FirstOrDefault();

            List<SelectListItem> status = new List<SelectListItem>(){
                new SelectListItem() { Selected = text.STATUS == 1 ? true : false, Text = "เปิด", Value = "1" },
                new SelectListItem() { Selected = text.STATUS == 0 ? true : false, Text = "ปิด", Value = "0" }
            };

            ViewBag.STATUS = status;

            return View(text);
        }

        [HttpPost]
        public ActionResult Text_Edit(TEXT_MODEL_VIEW model)
        {
            if (ModelState.IsValid)
            {
                string FileName = null;
                TEXT item = db.TEXT.FirstOrDefault(x => x.TEXT_ID == model.TEXT_ID);

                if (model.ImageFile != null)
                {
                    //Get Upload path from Web.Config file AppSettings.  
                    string UploadPath = HostingEnvironment.ApplicationPhysicalPath;

                    string oldFilename = Path.Combine(UploadPath, (string.IsNullOrEmpty(item.IMAGE)) ? "" : item.IMAGE.TrimStart('/', '\\'));

                    if (System.IO.File.Exists(oldFilename))
                    {
                        System.IO.File.Delete(oldFilename);
                    }

                    FileName = Path.GetFileNameWithoutExtension(model.ImageFile.FileName);
                    //To Get File Extension  
                    string FileExtension = Path.GetExtension(model.ImageFile.FileName);

                    //Add Current Date To Attached File Name  
                    FileName = DateTime.Now.ToString("yyyyMMdd") + "-" + FileName.Trim() + FileExtension;

                    //Its Create complete path to store in server.  
                    model.IMAGE = UploadPath + "img\\task\\" + FileName;

                    //To copy and save file into server.  
                    model.ImageFile.SaveAs(model.IMAGE);

                    item.IMAGE = "img/task/" + FileName;
                }

                item.NAME = model.NAME;
                item.STATUS = model.STATUS;
                item.DESCRIPTION = model.DESCRIPTION;

                db.SaveChanges();

                TempData["notice"] = "แก้ไขรายการเรียบร้อย";
                return RedirectToAction("Text_Edit", new { id = model.TEXT_ID });
            }

            List<SelectListItem> status = new List<SelectListItem>(){
                new SelectListItem() { Selected = (model.STATUS == 1)? true : false, Text = "เปิด", Value = "1" },
                new SelectListItem() { Selected = (model.STATUS == 0)? true : false, Text = "ปิด", Value = "0" }
            };

            ViewBag.STATUS1 = status;

            return View(model);
        }

        public ActionResult Service(int? page)
        {
            var service = db.SERVICE_CATE.ToList();

            int pageSize = 10;
            int pageNumber = (page ?? 1);

            return View(service.ToPagedList(pageNumber, pageSize));
        }

        public ActionResult Service_Edit(int id)
        {
            SERVICE_CATE_MODEL_VIEW service = (from a in db.SERVICE_CATE
                                               where a.SERVICE_CATE_ID == id
                                               select new SERVICE_CATE_MODEL_VIEW 
                                               { 
                                                   SERVICE_CATE_ID = a.SERVICE_CATE_ID,
                                                   NAME = a.NAME,
                                                   TOPIC = a.TOPIC, 
                                                   ORDER_LIST = a.ORDER_LIST, 
                                                   ICON = a.ICON, 
                                                   DESCRIPTION = a.DESCRIPTION, 
                                                   IMAGE = a.IMAGE, 
                                                   STATUS = a.STATUS
                                               }).FirstOrDefault();

            List<SelectListItem> status = new List<SelectListItem>(){
                new SelectListItem() { Selected = service.STATUS == 1 ? true : false, Text = "เปิด", Value = "1" },
                new SelectListItem() { Selected = service.STATUS == 0 ? true : false, Text = "ปิด", Value = "0" }
            };

            ViewBag.STATUS = status;

            return View(service);
        }

        [HttpPost]
        public ActionResult Service_Edit(SERVICE_CATE_MODEL_VIEW model)
        {
            if (ModelState.IsValid)
            {
                string FileName = null;
                SERVICE_CATE item = db.SERVICE_CATE.FirstOrDefault(x => x.SERVICE_CATE_ID == model.SERVICE_CATE_ID);

                if (model.ImageFile != null)
                {
                    //Get Upload path from Web.Config file AppSettings.  
                    string UploadPath = HostingEnvironment.ApplicationPhysicalPath;

                    string oldFilename = Path.Combine(UploadPath, (string.IsNullOrEmpty(item.IMAGE)) ? "" : item.IMAGE.TrimStart('/', '\\'));

                    if (System.IO.File.Exists(oldFilename))
                    {
                        System.IO.File.Delete(oldFilename);
                    }

                    FileName = Path.GetFileNameWithoutExtension(model.ImageFile.FileName);
                    //To Get File Extension  
                    string FileExtension = Path.GetExtension(model.ImageFile.FileName);

                    //Add Current Date To Attached File Name  
                    FileName = DateTime.Now.ToString("yyyyMMdd") + "-" + FileName.Trim() + FileExtension;

                    //Its Create complete path to store in server.  
                    model.IMAGE = UploadPath + "img\\service\\" + FileName;

                    //To copy and save file into server.  
                    model.ImageFile.SaveAs(model.IMAGE);

                    item.IMAGE = "img/service/" + FileName;
                }

                item.NAME = model.NAME;
                item.TOPIC = model.TOPIC;
                item.ICON = model.ICON;
                item.ORDER_LIST = model.ORDER_LIST;
                item.STATUS = model.STATUS;
                item.DESCRIPTION = model.DESCRIPTION;

                db.SaveChanges();

                TempData["notice"] = "แก้ไขรายการเรียบร้อย";
                return RedirectToAction("Service_Edit", new { id = model.SERVICE_CATE_ID });
            }

            List<SelectListItem> status = new List<SelectListItem>(){
                new SelectListItem() { Selected = (model.STATUS == 1)? true : false, Text = "เปิด", Value = "1" },
                new SelectListItem() { Selected = (model.STATUS == 0)? true : false, Text = "ปิด", Value = "0" }
            };

            ViewBag.STATUS = status;

            return View(model);
        }
        public ActionResult Blog(int? page)
        {
            var blog = db.BLOG.ToList();

            int pageSize = 10;
            int pageNumber = (page ?? 1);

            return View(blog.ToPagedList(pageNumber, pageSize));
        }
        public ActionResult Blog_Create()
        {
            List<SelectListItem> model = new List<SelectListItem>(){
                new SelectListItem() { Selected = true, Text = "เปิด", Value = "1" },
                new SelectListItem() { Selected = true, Text = "ปิด", Value = "0" }
            };

            ViewBag.STATUS = model;

            return View();
        }
        [HttpPost]
        public ActionResult Blog_Create(BLOG_MODEL_VIEW model)
        {
            if (ModelState.IsValid)
            {
                BLOG item = new BLOG();
                item.NAME = model.NAME;
                item.TOPIC = model.TOPIC;
                item.DESCRIPTION = model.DESCRIPTION;
                item.STATUS = model.STATUS;
                item.CREATED_DT = DateTime.Now;

                //Get Upload path from Web.Config file AppSettings.  
                string UploadPath = HostingEnvironment.ApplicationPhysicalPath;
                if (model.ImageFile != null)
                {
                    string FileName = Path.GetFileNameWithoutExtension(model.ImageFile.FileName);
                    //To Get File Extension  
                    string FileExtension = Path.GetExtension(model.ImageFile.FileName);

                    //Add Current Date To Attached File Name  
                    FileName = DateTime.Now.ToString("yyyyMMdd") + "-" + FileName.Trim() + FileExtension;

                    //Its Create complete path to store in server.  
                    model.IMAGE = UploadPath + "img\\blog\\" + FileName;

                    //To copy and save file into server.  
                    model.ImageFile.SaveAs(model.IMAGE);


                    item.IMAGE = "img/blog/" + FileName;
                }
                
                db.BLOG.Add(item);
                db.SaveChanges();

                TempData["notice"] = "เพิ่มรายการเรียบร้อย";
                return RedirectToAction("Blog_Create");
            }

            List<SelectListItem> status = new List<SelectListItem>(){
                new SelectListItem() { Selected = true, Text = "เปิด", Value = "1" },
                new SelectListItem() { Selected = false, Text = "ปิด", Value = "0" }
            };

            ViewBag.STATUS = status;

            return View(model);
        }
        public ActionResult Blog_Edit(int id)
        {
            BLOG_MODEL_VIEW service = (from a in db.BLOG
                                               where a.BLOG_ID == id
                                               select new BLOG_MODEL_VIEW
                                               {
                                                   BLOG_ID = a.BLOG_ID,
                                                   NAME = a.NAME,
                                                   TOPIC = a.TOPIC,
                                                   DESCRIPTION = a.DESCRIPTION,
                                                   IMAGE = a.IMAGE,
                                                   STATUS = a.STATUS
                                               }).FirstOrDefault();

            List<SelectListItem> status = new List<SelectListItem>(){
                new SelectListItem() { Selected = service.STATUS == 1 ? true : false, Text = "เปิด", Value = "1" },
                new SelectListItem() { Selected = service.STATUS == 0 ? true : false, Text = "ปิด", Value = "0" }
            };

            ViewBag.STATUS1 = status;

            return View(service);
        }
        [HttpPost]
        public ActionResult Blog_Edit(BLOG_MODEL_VIEW model)
        {
            if (ModelState.IsValid)
            {
                string FileName = null;
                BLOG item = db.BLOG.FirstOrDefault(x => x.BLOG_ID == model.BLOG_ID);

                if (model.ImageFile != null)
                {
                    //Get Upload path from Web.Config file AppSettings.  
                    string UploadPath = HostingEnvironment.ApplicationPhysicalPath;

                    string oldFilename = Path.Combine(UploadPath, (string.IsNullOrEmpty(item.IMAGE)) ? "" : item.IMAGE.TrimStart('/', '\\'));

                    if (System.IO.File.Exists(oldFilename))
                    {
                        System.IO.File.Delete(oldFilename);
                    }

                    FileName = Path.GetFileNameWithoutExtension(model.ImageFile.FileName);
                    //To Get File Extension  
                    string FileExtension = Path.GetExtension(model.ImageFile.FileName);

                    //Add Current Date To Attached File Name  
                    FileName = DateTime.Now.ToString("yyyyMMdd") + "-" + FileName.Trim() + FileExtension;

                    //Its Create complete path to store in server.  
                    model.IMAGE = UploadPath + "img\\blog\\" + FileName;

                    //To copy and save file into server.  
                    model.ImageFile.SaveAs(model.IMAGE);

                    item.IMAGE = "img/blog/" + FileName;
                }

                item.NAME = model.NAME;
                item.TOPIC = model.TOPIC;
                item.STATUS = model.STATUS;
                item.DESCRIPTION = model.DESCRIPTION;
                item.IMAGE = model.IMAGE;
                item.CREATED_DT = DateTime.Now;

                db.SaveChanges();

                TempData["notice"] = "แก้ไขรายการเรียบร้อย";
                return RedirectToAction("Blog_Edit", new { id = model.BLOG_ID });
            }

            List<SelectListItem> status = new List<SelectListItem>(){
                new SelectListItem() { Selected = (model.STATUS == 1)? true : false, Text = "เปิด", Value = "1" },
                new SelectListItem() { Selected = (model.STATUS == 0)? true : false, Text = "ปิด", Value = "0" }
            };

            ViewBag.STATUS1 = status;

            return View(model);
        }
        public ActionResult Customer(int? page)
        {
            var customer = db.CUSTOMER.ToList();

            int pageSize = 10;
            int pageNumber = (page ?? 1);

            return View(customer.ToPagedList(pageNumber, pageSize));
        }
        public ActionResult Customer_Create()
        {
            List<SelectListItem> model = new List<SelectListItem>(){
                new SelectListItem() { Selected = true, Text = "เปิด", Value = "1" },
                new SelectListItem() { Selected = true, Text = "ปิด", Value = "0" }
            };

            ViewBag.STATUS = model;

            return View();
        }
        [HttpPost]
        public ActionResult Customer_Create(CUSTOMER_MODEL_VIEW model)
        {
            if (ModelState.IsValid)
            {
                //Get Upload path from Web.Config file AppSettings.  
                string UploadPath = HostingEnvironment.ApplicationPhysicalPath;

                string FileName = Path.GetFileNameWithoutExtension(model.ImageFile.FileName);
                //To Get File Extension  
                string FileExtension = Path.GetExtension(model.ImageFile.FileName);

                //Add Current Date To Attached File Name  
                FileName = DateTime.Now.ToString("yyyyMMdd") + "-" + FileName.Trim() + FileExtension;

                //Its Create complete path to store in server.  
                model.IMAGE = UploadPath + "img\\customer\\" + FileName;

                //To copy and save file into server.  
                model.ImageFile.SaveAs(model.IMAGE);

                CUSTOMER item = new CUSTOMER();
                item.NAME = model.NAME;
                item.TOPIC = model.TOPIC;
                item.DESCRIPTION = model.DESCRIPTION;
                item.IMAGE = "img/customer/" + FileName;
                item.STATUS = model.STATUS;

                db.CUSTOMER.Add(item);
                db.SaveChanges();

                TempData["notice"] = "เพิ่มรายการเรียบร้อย";
                return RedirectToAction("Customer_Create");
            }

            List<SelectListItem> status = new List<SelectListItem>(){
                new SelectListItem() { Selected = true, Text = "เปิด", Value = "1" },
                new SelectListItem() { Selected = false, Text = "ปิด", Value = "0" }
            };

            ViewBag.STATUS = status;

            return View(model);
        }
        public ActionResult Customer_Edit(int id)
        {
            CUSTOMER_MODEL_VIEW service = (from a in db.CUSTOMER
                                       where a.CUST_ID == id
                                       select new CUSTOMER_MODEL_VIEW
                                       {
                                           CUST_ID = a.CUST_ID,
                                           NAME = a.NAME,
                                           TOPIC = a.TOPIC,
                                           DESCRIPTION = a.DESCRIPTION,
                                           IMAGE = a.IMAGE,
                                           STATUS = a.STATUS
                                       }).FirstOrDefault();

            List<SelectListItem> status = new List<SelectListItem>(){
                new SelectListItem() { Selected = service.STATUS == 1 ? true : false, Text = "เปิด", Value = "1" },
                new SelectListItem() { Selected = service.STATUS == 0 ? true : false, Text = "ปิด", Value = "0" }
            };

            ViewBag.STATUS1 = status;

            return View(service);
        }
        [HttpPost]
        public ActionResult Customer_Edit(CUSTOMER_MODEL_VIEW model)
        {
            if (ModelState.IsValid)
            {
                string FileName = null;
                CUSTOMER item = db.CUSTOMER.FirstOrDefault(x => x.CUST_ID == model.CUST_ID);

                if (model.ImageFile != null)
                {
                    //Get Upload path from Web.Config file AppSettings.  
                    string UploadPath = HostingEnvironment.ApplicationPhysicalPath;

                    string oldFilename = Path.Combine(UploadPath, (string.IsNullOrEmpty(item.IMAGE)) ? "" : item.IMAGE.TrimStart('/', '\\'));

                    if (System.IO.File.Exists(oldFilename))
                    {
                        System.IO.File.Delete(oldFilename);
                    }

                    FileName = Path.GetFileNameWithoutExtension(model.ImageFile.FileName);
                    //To Get File Extension  
                    string FileExtension = Path.GetExtension(model.ImageFile.FileName);

                    //Add Current Date To Attached File Name  
                    FileName = DateTime.Now.ToString("yyyyMMdd") + "-" + FileName.Trim() + FileExtension;

                    //Its Create complete path to store in server.  
                    model.IMAGE = UploadPath + "img\\customer\\" + FileName;

                    //To copy and save file into server.  
                    model.ImageFile.SaveAs(model.IMAGE);

                    item.IMAGE = "img/customer/" + FileName;
                }

                item.NAME = model.NAME;
                item.TOPIC = model.TOPIC;
                item.STATUS = model.STATUS;
                item.DESCRIPTION = model.DESCRIPTION;

                db.SaveChanges();

                TempData["notice"] = "แก้ไขรายการเรียบร้อย";
                return RedirectToAction("Customer_Edit", new { id = model.CUST_ID });
            }

            List<SelectListItem> status = new List<SelectListItem>(){
                new SelectListItem() { Selected = (model.STATUS == 1)? true : false, Text = "เปิด", Value = "1" },
                new SelectListItem() { Selected = (model.STATUS == 0)? true : false, Text = "ปิด", Value = "0" }
            };

            ViewBag.STATUS1 = status;

            return View(model);
        }

        public ActionResult User(int? page)
        {
            var user = db.USERS.ToList();

            int pageSize = 10;
            int pageNumber = (page ?? 1);

            return View(user.ToPagedList(pageNumber, pageSize));
        }

        public ActionResult User_Create()
        {
            List<SelectListItem> model = new List<SelectListItem>(){
                new SelectListItem() { Selected = true, Text = "เปิด", Value = "A" },
                new SelectListItem() { Selected = true, Text = "ปิด", Value = "I" }
            };

            ViewBag.STATUS = model;

            return View();
        }
        [HttpPost]
        public ActionResult User_Create(USERS model)
        {
            if (ModelState.IsValid)
            {
                USERS m = new USERS();
                m.USERNAME = model.USERNAME;
                m.PASSWORD = model.PASSWORD;
                m.ACTIVE = model.ACTIVE;

                db.USERS.Add(m);
                db.SaveChanges();

                TempData["notice"] = "เพิ่มรายการเรียบร้อย";
                return RedirectToAction("User_Create");
            }

            List<SelectListItem> status = new List<SelectListItem>(){
                new SelectListItem() { Selected = true, Text = "เปิด", Value = "A" },
                new SelectListItem() { Selected = true, Text = "ปิด", Value = "I" }
            };

            ViewBag.STATUS = status;
            return View(model);
        }
        public ActionResult User_Edit(int id)
        {
            var model = db.USERS.FirstOrDefault(x => x.USER_ID == id);
            model.PASSWORD = "";

            List<SelectListItem> status = new List<SelectListItem>(){
                new SelectListItem() { Selected = (model.ACTIVE == "A")? true : false, Text = "เปิด", Value = "A" },
                new SelectListItem() { Selected = (model.ACTIVE == "I")? true : false, Text = "ปิด", Value = "I" }
            };

            ViewBag.STATUS1 = status;

            return View(model);
        }
        [HttpPost]
        public ActionResult User_Edit(USERS model)
        {
            if (ModelState.IsValid)
            {
                USERS item = db.USERS.FirstOrDefault(x => x.USER_ID == model.USER_ID);

                item.PASSWORD = model.PASSWORD;
                item.ACTIVE = model.ACTIVE;

                db.SaveChanges();

                TempData["notice"] = "แก้ไขรายการเรียบร้อย";
                return RedirectToAction("User_Edit", new { id = model.USER_ID });
            }

            List<SelectListItem> status = new List<SelectListItem>(){
                new SelectListItem() { Selected = (model.ACTIVE == "A")? true : false, Text = "เปิด", Value = "A" },
                new SelectListItem() { Selected = (model.ACTIVE == "I")? true : false, Text = "ปิด", Value = "I" }
            };

            ViewBag.STATUS1 = status;

            return View(model);
        }

        public ActionResult Banner(int? page)
        {
            var banner = db.BANNER.ToList();
            int pageSize = 10;
            int pageNumber = (page ?? 1);

            return View(banner.ToPagedList(pageNumber, pageSize));
        }

        public ActionResult Banner_Create()
        {
            List<SelectListItem> status = new List<SelectListItem>(){
                new SelectListItem() { Selected = true, Text = "เปิด", Value = "1" },
                new SelectListItem() { Text = "ปิด", Value = "0" }
            };

            ViewBag.STATUS = status;

            return View();
        }

        [HttpPost]
        public ActionResult Banner_Create(BANNER model)
        {
            if (ModelState.IsValid)
            {
                string FileName = null;

                if (model.ImageFile != null)
                {
                    //Get Upload path from Web.Config file AppSettings.  
                    string UploadPath = HostingEnvironment.ApplicationPhysicalPath;

                    FileName = Path.GetFileNameWithoutExtension(model.ImageFile.FileName);
                    //To Get File Extension  
                    string FileExtension = Path.GetExtension(model.ImageFile.FileName);

                    //Add Current Date To Attached File Name  
                    FileName = DateTime.Now.ToString("yyyyMMdd") + "-" + FileName.Trim() + FileExtension;

                    //Its Create complete path to store in server.  
                    model.IMAGE = UploadPath + "img\\banner\\" + FileName;

                    //To copy and save file into server.  
                    model.ImageFile.SaveAs(model.IMAGE);
                }

                BANNER item = new BANNER();
                item.IMAGE = "img/banner/" + FileName;
                item.STATUS = model.STATUS;

                db.BANNER.Add(item);
                db.SaveChanges();

                TempData["notice"] = "เพิ่มรายการเรียบร้อย";
                return RedirectToAction("Banner_Create");
            }

            List<SelectListItem> status = new List<SelectListItem>(){
                new SelectListItem() { Selected = true, Text = "เปิด", Value = "1" },
                new SelectListItem() { Text = "ปิด", Value = "0" }
            };

            ViewBag.STATUS = status;

            return View();
        }

        public ActionResult Banner_Edit(int id)
        {
            var banner = db.BANNER.FirstOrDefault(x => x.ID == id);

            List<SelectListItem> status = new List<SelectListItem>(){
                new SelectListItem() { Selected = banner.STATUS == 1 ? true : false, Text = "เปิด", Value = "1" },
                new SelectListItem() { Selected = banner.STATUS == 0 ? true : false, Text = "ปิด", Value = "0" }
            };

            ViewBag.STATUS1 = status;

            return View(banner);
        }

        [HttpPost]
        public ActionResult Banner_Edit(BANNER model)
        {
            if (ModelState.IsValid)
            {
                string FileName = null;
                BANNER item = db.BANNER.FirstOrDefault(x => x.ID == model.ID);

                if (model.ImageFile != null)
                {
                    //Get Upload path from Web.Config file AppSettings.  
                    string UploadPath = HostingEnvironment.ApplicationPhysicalPath;

                    string oldFilename = Path.Combine(UploadPath, model.IMAGE.TrimStart('/', '\\'));

                    if (System.IO.File.Exists(oldFilename))
                    {
                        System.IO.File.Delete(oldFilename);
                    }

                    FileName = Path.GetFileNameWithoutExtension(model.ImageFile.FileName);
                    //To Get File Extension  
                    string FileExtension = Path.GetExtension(model.ImageFile.FileName);

                    //Add Current Date To Attached File Name  
                    FileName = DateTime.Now.ToString("yyyyMMdd") + "-" + FileName.Trim() + FileExtension;

                    //Its Create complete path to store in server.  
                    model.IMAGE = UploadPath + "img\\banner\\" + FileName;

                    //To copy and save file into server.  
                    model.ImageFile.SaveAs(model.IMAGE);

                    item.IMAGE = "img/banner/" + FileName;
                }

                item.STATUS = model.STATUS;

                db.SaveChanges();

                TempData["notice"] = "แก้ไขรายการเรียบร้อย";
                return RedirectToAction("Banner_Edit", new { id = model.ID });
            }

            List<SelectListItem> status = new List<SelectListItem>(){
                new SelectListItem() { Selected = (model.STATUS == 1)? true : false, Text = "เปิด", Value = "1" },
                new SelectListItem() { Selected = (model.STATUS == 0)? true : false, Text = "ปิด", Value = "0" }
            };

            ViewBag.STATUS1 = status;

            return View(model);
        }

        public ActionResult Download(int? page)
        {
            var download = db.DOWNLOAD.ToList();
            int pageSize = 10;
            int pageNumber = (page ?? 1);

            return View(download.ToPagedList(pageNumber, pageSize));
        }

        public ActionResult Download_Create()
        {
            List<SelectListItem> status = new List<SelectListItem>(){
                new SelectListItem() { Selected = true, Text = "เปิด", Value = "1" },
                new SelectListItem() { Text = "ปิด", Value = "0" }
            };

            ViewBag.STATUS = status;

            return View();
        }

        [HttpPost]
        public ActionResult Download_Create(DOWNLOAD model)
        {
            if (ModelState.IsValid)
            {
                DOWNLOAD item = new DOWNLOAD();
                item.NAME = model.NAME;
                item.PATH = "upload/setup/"+model.NAME;
                item.STATUS = model.STATUS;

                db.DOWNLOAD.Add(item);
                db.SaveChanges();

                TempData["notice"] = "เพิ่มรายการเรียบร้อย";
                return RedirectToAction("Download_Create");
            }

            List<SelectListItem> status = new List<SelectListItem>(){
                new SelectListItem() { Selected = true, Text = "เปิด", Value = "1" },
                new SelectListItem() { Text = "ปิด", Value = "0" }
            };

            ViewBag.STATUS = status;

            return View();
        }

        public ActionResult Download_Edit(int id)
        {
            var download = db.DOWNLOAD.FirstOrDefault(x => x.ID == id);

            List<SelectListItem> status = new List<SelectListItem>(){
                new SelectListItem() { Selected = download.STATUS == 1 ? true : false, Text = "เปิด", Value = "1" },
                new SelectListItem() { Selected = download.STATUS == 0 ? true : false, Text = "ปิด", Value = "0" }
            };

            ViewBag.STATUS1 = status;

            return View(download);
        }

        [HttpPost]
        public ActionResult Download_Edit(DOWNLOAD model)
        {
            if (ModelState.IsValid)
            {
                DOWNLOAD item = db.DOWNLOAD.FirstOrDefault(x => x.ID == model.ID);

                item.STATUS = model.STATUS;
                item.NAME = model.NAME;
                item.PATH = "upload/setup/"+model.NAME;
                db.SaveChanges();

                TempData["notice"] = "แก้ไขรายการเรียบร้อย";
                return RedirectToAction("Download_Edit", new { id = model.ID });
            }

            List<SelectListItem> status = new List<SelectListItem>(){
                new SelectListItem() { Selected = (model.STATUS == 1)? true : false, Text = "เปิด", Value = "1" },
                new SelectListItem() { Selected = (model.STATUS == 0)? true : false, Text = "ปิด", Value = "0" }
            };

            ViewBag.STATUS1 = status;

            return View(model);
        }
    }
}