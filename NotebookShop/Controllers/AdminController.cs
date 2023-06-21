using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using NotebookShop.Classes;
using NotebookShop.Models.Database;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using X.PagedList;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using System.IO;

namespace NotebookShop.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly IHostingEnvironment _hostingEnvironment;
        int PageIndex = 1;
        int OldId = 1;

        public AdminController(IHostingEnvironment hostingEnvironment)
        {
            //Config for SQLite
            _hostingEnvironment = hostingEnvironment;
            Connector.SetHostingEnvironment(_hostingEnvironment);
        }

        public IActionResult Index()
        {
            if (Connector.SQLQuery("select version();") == false)
            {
                ViewBag.ErrorMessage = "Есть проблемы, возможно сервер выключен или временно недоступен!";
                return View("~/Views/Error/Error.cshtml");
            }
            return View();
        }

        [HttpPost]
        public IActionResult AddEditShow(string table, string method, int? id, int? page)
        {
            object Model = null;

            if (string.IsNullOrEmpty(table))
                return null;

            PageIndex = page.HasValue ? Convert.ToInt32(page) : 1;

            if (method == "Update")
                Model = Connector.Get(table.ToLower(), "WHERE Id = " + id).FirstOrDefault();
            else
                Model = Activator.CreateInstance(Type.GetType("NotebookShop.Models.Database." + table.ToLower()));

            if (Model == null)
            {
                ViewBag.Method = "Загрузка";
                ViewBag.ErrorMessage = "Ошибка";
                return PartialView("~/Views/Error/ErrorAdmin.cshtml");
            }

            ViewBag.ValidationList = GetValidationList(Model);
            ViewBag.OldId = OldId;
            ViewBag.Model = Model;
            ViewBag.Table = table;
            ViewBag.Page = PageIndex;
            ViewBag.Columns = ModelAttributes.GetFieldsName(Model);
            ViewBag.Properties = Model.GetType().GetProperties();

            if (method == "Update")
                ViewBag.MethodText = "Редактирование";
            else
                ViewBag.MethodText = "Добавление";

            ViewBag.MethodValue = method;

            return PartialView("~/Views/Admin/PartialViews/AddEdit.cshtml");
        }

        [HttpPost]
        public IActionResult ListView(string table, string sortingField, int? page, bool orderBy)
        {
            object ListModels = null;

            PageIndex = page.HasValue ? Convert.ToInt32(page) : 1;

            string CurrentTable = "";

            try
            {
                CurrentTable = Request.HttpContext.Session.GetString("CurrentTable");
            }
            catch { }

            if (string.IsNullOrEmpty(CurrentTable) || !CurrentTable.Equals(table))
            {
                CurrentTable = table;
                Response.HttpContext.Session.SetString("CurrentTable", table);
            }

            string SearchValue = string.IsNullOrEmpty(Request.HttpContext.Session.GetString("SearchValue")) ? null : Request.HttpContext.Session.GetString("SearchValue");

            int? PageSize = Request.HttpContext.Session.GetInt32("PageSize").HasValue ? Request.HttpContext.Session.GetInt32("PageSize") : 5;

            if (string.IsNullOrEmpty(sortingField) && string.IsNullOrEmpty(SearchValue))
            {
                try
                {
                    ListModels = Connector.Get(CurrentTable.ToLower(), "ORDER BY Id ASC").ToPagedList(PageIndex, (int)PageSize);
                }
                catch (Exception exc)
                {
                    ViewBag.Method = "Загрузка";
                    ViewBag.ErrorMessage = exc.Message;
                    return PartialView("~/Views/Error/ErrorAdmin.cshtml");
                }
            }
            else
            {
                string Condition = "";

                if (!string.IsNullOrEmpty(SearchValue))
                    Condition += GenerateLikeCondition(CurrentTable);

                if (!string.IsNullOrEmpty(sortingField))
                {
                    if (orderBy == true)
                    {
                        Condition += string.Format("ORDER BY {0} {1}", sortingField, "ASC");
                        orderBy = false;
                    }
                    else
                    {
                        Condition += string.Format("ORDER BY {0} {1}", sortingField, "DESC");
                        orderBy = true;
                    }
                }

                ListModels = Connector.Get(CurrentTable, Condition).ToPagedList(PageIndex, (int)PageSize);
            }

            if (ListModels == null)
            {
                ViewBag.Method = "Загрузка";
                ViewBag.ErrorMessage = "Ошибка";
                return PartialView("~/Views/Error/ErrorAdmin.cshtml");
            }

            object Model = Activator.CreateInstance(Type.GetType("NotebookShop.Models.Database." + CurrentTable));
            ViewBag.Page = PageIndex;
            ViewBag.Table = CurrentTable;
            ViewBag.Model = ListModels;
            ViewBag.OrderBy = orderBy;
            ViewBag.Properties = Model.GetType().GetProperties();
            ViewBag.Columns = ModelAttributes.GetFieldsName(Model);
            ViewBag.TableCount = Connector.GetValue("SELECT COUNT(*) FROM " + CurrentTable);

            if (string.IsNullOrEmpty(ViewBag.TableCount))
                ViewBag.TableCount = "0";

            return PartialView("~/Views/Admin/PartialViews/List.cshtml");
        }

        [HttpPost]
        public IActionResult Delete(string table, int page, int? id)
        {
            if (!id.HasValue)
                return Json(new { message = "Ошибка" });

            var result = Connector.SQLQuery("DELETE FROM " + table + " WHERE Id = " + id);

            if (result == false)
                return Json(new { message = "Ошибка" });

            return Json(new { page, url = "/Admin/ListView" });
        }
        
        [HttpPost]
        public async Task<IActionResult> Apply(string table, string method, int? page, int oldId, IFormFile fileImg, string status,
            aboutproject aboutproject, admins admins, basket basket, emaillist emaillist, memories memories, motherboards motherboards,
            orders orders, processors processors, screens screens, videocards videocards, winchesters winchesters)
        {
            object obj = null;
            string fileFolder = "";

            switch (table.ToLower())
            {
                case "aboutproject":
                    obj = aboutproject;
                    break;
                case "admins":

                    admins.Password = Encryption.Encrypt(admins.Password);

                    obj = admins;
                    break;
                case "basket":
                    obj = basket;
                    break;
                case "emaillist":
                    obj = emaillist;
                    break;                
                case "memories":

                    if (status == "Add")
                    {
                        if (fileImg != null)
                        {
                            fileFolder = await ApplyImgFile(fileImg, status, table, memories.Id);
                            memories.Photo = "~/images/user/" + table.ToLower() + "/" + fileFolder + "/" + fileImg.FileName;
                        }
                        else
                            memories.Photo = memories.Photo ?? "~/images/user/Default.png";
                    }
                    else
                    {
                        fileFolder = await ApplyImgFile(fileImg, status, table, memories.Id);
                        memories.Photo = "~/images/user/Default.png";
                    };

                    obj = memories;
                    break;
                case "motherboards":

                    if (status == "Add")
                    {
                        if (fileImg != null)
                        {
                            fileFolder = await ApplyImgFile(fileImg, status, table, motherboards.Id);
                            motherboards.Photo = "~/images/user/" + table.ToLower() + "/" + fileFolder + "/" + fileImg.FileName;
                        }
                        else
                            motherboards.Photo = motherboards.Photo ?? "~/images/user/Default.png";
                    }
                    else
                    {
                        fileFolder = await ApplyImgFile(fileImg, status, table, motherboards.Id);
                        motherboards.Photo = "~/images/user/Default.png";
                    };

                    obj = motherboards;
                    break;
                case "orders":
                    obj = orders;
                    break;
                case "processors":

                    if (status == "Add")
                    {
                        if (fileImg != null)
                        {
                            fileFolder = await ApplyImgFile(fileImg, status, table, processors.Id);
                            processors.Photo = "~/images/user/" + table.ToLower() + "/" + fileFolder + "/" + fileImg.FileName;
                        }
                        else
                            processors.Photo = processors.Photo ?? "~/images/user/Default.png";
                    }
                    else
                    {
                        fileFolder = await ApplyImgFile(fileImg, status, table, processors.Id);
                        processors.Photo = "~/images/user/Default.png";
                    };

                    obj = processors;
                    break;
                case "screens":

                    if (status == "Add")
                    {
                        if (fileImg != null)
                        {
                            fileFolder = await ApplyImgFile(fileImg, status, table, screens.Id);
                            screens.Photo = "~/images/user/" + table.ToLower() + "/" + fileFolder + "/" + fileImg.FileName;
                        }
                        else
                            screens.Photo = screens.Photo ?? "~/images/user/Default.png";
                    }
                    else
                    {
                        fileFolder = await ApplyImgFile(fileImg, status, table, screens.Id);
                        screens.Photo = "~/images/user/Default.png";
                    };

                    obj = screens;
                    break;
                case "videocards":

                    if (status == "Add")
                    {
                        if (fileImg != null)
                        {
                            fileFolder = await ApplyImgFile(fileImg, status, table, videocards.Id);
                            videocards.Photo = "~/images/user/" + table.ToLower() + "/" + fileFolder + "/" + fileImg.FileName;
                        }
                        else
                            videocards.Photo = videocards.Photo ?? "~/images/user/Default.png";
                    }
                    else
                    {
                        fileFolder = await ApplyImgFile(fileImg, status, table, videocards.Id);
                        videocards.Photo = "~/images/user/Default.png";
                    };

                    obj = videocards;
                    break;
                case "winchesters":

                    if (status == "Add")
                    {
                        if (fileImg != null)
                        {
                            fileFolder = await ApplyImgFile(fileImg, status, table, winchesters.Id);
                            winchesters.Photo = "~/images/user/" + table.ToLower() + "/" + fileFolder + "/" + fileImg.FileName;
                        }
                        else
                            winchesters.Photo = winchesters.Photo ?? "~/images/user/Default.png";
                    }
                    else
                    {
                        fileFolder = await ApplyImgFile(fileImg, status, table, winchesters.Id);
                        winchesters.Photo = "~/images/user/Default.png";
                    };

                    obj = winchesters;
                    break;
                default:
                    obj = null;
                    break;
            }

            PageIndex = page.HasValue ? Convert.ToInt32(page) : 1;

            if (method == "Add")
            {
                if (Connector.Insert(obj) == false)
                    return Json(new { message = "Ошибка", url = "/Admin/ErrorAdmin" });
            }
            else
            {
                if (Connector.Update(oldId, obj) == false)
                    return Json(new { message = "Ошибка", url = "/Admin/ErrorAdmin" });
            }

            return Json(new { page = PageIndex, url = "/Admin/ListView" });
        }

        [HttpPost]
        public IActionResult ErrorAdmin(string message, string method)
        {
            ViewBag.ErrorMessage = message;
            ViewBag.Method = method;
            return PartialView("~/Views/Error/ErrorAdmin.cshtml");
        }

        public IActionResult SetPageSize(int pageSize)
        {
            try
            {
                Response.HttpContext.Session.SetInt32("PageSize", pageSize);
            }
            catch { }

            try
            {
                Response.HttpContext.Session.Set("OrderBy", false);
            }
            catch { }

            return Json(pageSize);
        }

        public IActionResult SetSearchValue(string searchValue)
        {
            try
            {
                if (string.IsNullOrEmpty(searchValue))
                    Response.HttpContext.Session.Remove("SearchValue");
                else
                    Response.HttpContext.Session.SetString("SearchValue", searchValue);
            }
            catch { }

            return Json(searchValue);
        }

        [HttpPost]
        public async Task<string> ApplyImgFile(IFormFile fileImg, string status, string table, int? id)
        {
            string fileFolder = "";
            string fileName = "";
            string serverPath = _hostingEnvironment.WebRootPath;

            if (status.Equals("Delete"))
            {
                string imgPath = "";

                imgPath = Connector.GetValue("SELECT Photo FROM " + table + " WHERE Id = " + id);

                if (imgPath.Equals("~/images/user/Default.png"))
                    return fileName;

                imgPath = imgPath.Replace("~", "");
                imgPath = imgPath.Replace("/", "\\");

                serverPath += imgPath;

                FileInfo file = new FileInfo(serverPath);

                if (file.Exists)
                {
                    file.Delete();
                }

                serverPath = serverPath.Substring(0, serverPath.LastIndexOf("\\"));

                Directory.Delete(serverPath);

                fileName = "";
            }
            // Add new file
            else
            {
                // Also show list of uploaded files!
                //var filesObj = Request.Form.Files;

                if (fileImg != null)
                {
                    fileName = fileImg.FileName;
                    fileFolder = GlobalValues.RandomString(10) + "_" + DateTime.Now.ToShortDateString();

                    string pathToFolder = "\\images\\user\\" + table.ToLower() + "\\";

                    Directory.CreateDirectory(serverPath + pathToFolder + fileFolder);

                    pathToFolder += fileFolder + "\\" + fileImg.FileName;

                    serverPath += pathToFolder;

                    using (var fileStream = new FileStream(serverPath, FileMode.Create))
                    {
                        await fileImg.CopyToAsync(fileStream);
                    }
                }
            }

            return fileFolder;
        }

        /*Other methods*/

        public async Task UploadFileFromServerAsync(string link, IFormFile theFile)
        {
            // Copy contents to memory stream.
            Stream stream;
            stream = new MemoryStream();
            theFile.CopyTo(stream);
            stream.Position = 0;
            string serverPath = link;

            // Save the file
            using (FileStream writerFileStream = System.IO.File.Create(serverPath))
            {
                await stream.CopyToAsync(writerFileStream);
                writerFileStream.Dispose();
            }
        }

        public List<string> GetValidationList(object model)
        {
            List<string> validationList = new List<string>();

            foreach (var item in model.GetType().GetProperties())
            {
                PropertyInfo info = model.GetType().GetProperty(item.Name);

                if ((bool)ModelAttributes.GetFieldAttribute(item.Name, model, 2) == true)
                {
                    if (info.PropertyType == typeof(int))
                    {
                        if (item.Name.ToLower().IndexOf("fk") != -1)
                            validationList.Add("data-validation=\"number\" data-validation-allowing=\"range[1;2147483647]\"");
                        else
                            validationList.Add("data-validation=\"number\"");
                    }
                    else if (info.PropertyType == typeof(string))
                    {
                        var buffer = ModelAttributes.GetFieldAttribute(item.Name, model, 3);

                        if (buffer != null)
                        {
                            if ((DataType)buffer == DataType.EmailAddress)
                                validationList.Add("data-validation=\"email\"");
                        }
                        else
                            validationList.Add("data-validation=\"length\" data-validation-length=\"1-" + ModelAttributes.GetFieldAttribute(item.Name, model, 0).ToString() + "\"");

                    }
                }
                else
                {
                    if (item.Name.ToLower().Equals("id"))
                        OldId = (int)item.GetValue(model, null);

                    validationList.Add("");
                }
            }

            return validationList;
        }

        public string GenerateLikeCondition(string table)
        {
            string LikeCondition = "WHERE ";
            string SearchValue = Request.HttpContext.Session.GetString("SearchValue");

            object Model = Activator.CreateInstance(Type.GetType("NotebookShop.Models.Database." + table));

            foreach (var item in Model.GetType().GetProperties())
            {
                if (item.PropertyType == typeof(int))
                    LikeCondition += string.Format("CAST({0} AS TEXT) ILIKE '%{1}%' OR ", item.Name, SearchValue);
                else if (item.PropertyType == typeof(string))
                    LikeCondition += string.Format("{0} ILIKE '%{1}%' OR ", item.Name, SearchValue);
            }

            LikeCondition = LikeCondition.Substring(0, LikeCondition.LastIndexOf(" OR"));

            return LikeCondition;
        }
    }
}