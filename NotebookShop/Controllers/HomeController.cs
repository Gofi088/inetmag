using CookieManager;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NotebookShop.Classes;
using NotebookShop.Models.Common;
using NotebookShop.Models.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using X.PagedList;

namespace NotebookShop.Controllers
{
    public class HomeController : Controller
    {
        public struct DocumentData
        {
            public string Key { get; set; }

            public string Value { get; set; }
        }

        private readonly IHostingEnvironment _hostingEnvironment;
        private object obj;
        int PageIndex = 1;
        readonly int PageSize = 10;

        public HomeController(IHostingEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
        }

        public IActionResult Index()
        {
            var userKey = Request.Cookies["userKey"];

            if (userKey == null)
                Response.Cookies.Append("userKey", GlobalValues.RandomString(5), new CookieOptions { Expires = DateTime.Now.AddDays(1) });
            else userKey = Request.Cookies["userKey"];

            List<IndexModel> indexModels = new List<IndexModel>()
            {
                new IndexModel()
                {
                    Name = "Экраны",
                    Table = "screens",
                    Description = "Экраны для ноутбуков / компьютеров",
                    Photo = "~/images/user/screensPreview.jpg"
                },
                new IndexModel()
                {
                    Name = "Процессоры",
                    Table = "processors",
                    Description = "Процессоры для ноутбуков / компьютеров",
                    Photo = "~/images/user/processorsPreview.jpg"
                },
                new IndexModel()
                {
                    Name = "Видеокарты",
                    Table = "videocards",
                    Description = "Видеокарты для ноутбуков / компьютеров",
                    Photo = "~/images/user/videoCardsPreview.jpg"
                },
                new IndexModel()
                {
                    Name = "Оперативная память",
                    Table = "memories",
                    Description = "Оперативная память для ноутбуков / компьютеров",
                    Photo = "~/images/user/memoriesPreview.jpg"
                },
                new IndexModel()
                {
                    Name = "Материнские платы",
                    Table = "motherboards",
                    Description = "Материнские платы для ноутбуков / компьютеров",
                    Photo = "~/images/user/motherboardsPreview.jpg"
                },
                new IndexModel()
                {
                    Name = "Винчестера",
                    Table = "winchesters",
                    Description = "Винчестера для ноутбуков / компьютеров",
                    Photo = "~/images/user/winchestersPreview.jpg"
                }
            };

            return View(indexModels);
        }

        public IActionResult SinglePost(string table, int? id)
        {
            string Header = "";
            string CurrentTable = "";
            object Model = null;

            if (string.IsNullOrEmpty(table))
            {
                try
                {
                    CurrentTable = Request.HttpContext.Session.GetString("CurrentTable");
                }
                catch
                {
                    return TryCatchNotFoundPosts();
                }
            }
            else
            {
                CurrentTable = table;
                Response.HttpContext.Session.SetString("CurrentTable", CurrentTable);
            }

            //Activator.CreateInstance(Type.GetType("NotebookShop.Models.Database." + table.ToLower()));
            Model = Connector.Get(table, "WHERE Id = " + id).FirstOrDefault();
            Header = Connector.GetValue("SELECT Model FROM " + table);

            if (Model == null)
            {
                return TryCatchNotFoundPosts();
            }

            ViewBag.Model = Model;
            ViewBag.Table = CurrentTable;
            ViewBag.Header = Header;
            ViewBag.Columns = ModelAttributes.GetFieldsName(Model);
            ViewBag.Properties = Model.GetType().GetProperties();

            return View("~/Views/Home/SinglePost.cshtml");
        }

        public IActionResult Accessories(string table, int? page, string nextPrev)
        {
            string searchValue = string.IsNullOrEmpty(Request.HttpContext.Session.GetString("SearchValue")) ? null : Request.HttpContext.Session.GetString("SearchValue");

            Connector.CheckAndDeleteFolders(_hostingEnvironment, table, "photo");

            string CurrentTable = "";

            if (string.IsNullOrEmpty(table))
            {
                try
                {
                    CurrentTable = Request.HttpContext.Session.GetString("CurrentTable");
                }
                catch
                {
                    ViewBag.RedirectMessage = "Ошибка";
                    return View("~/Views/Home/RedirectPage.cshtml");
                }
            }
            else
            {
                CurrentTable = table;
                Response.HttpContext.Session.SetString("CurrentTable", CurrentTable);
            }

            PageIndex = page.HasValue ? Convert.ToInt32(page) : 1;

            if (!string.IsNullOrEmpty(nextPrev))
            {
                if (nextPrev.Equals("next"))
                    PageIndex++;
                else if (nextPrev.Equals("prev"))
                    PageIndex--;
            }

            if (PageIndex <= 0)
                PageIndex = 1;

            try
            {
                if (string.IsNullOrEmpty(searchValue))
                    obj = Connector.Get(table, "ORDER BY Id").ToPagedList(PageIndex, PageSize);
                else
                    obj = Connector.Get(table, "WHERE " + searchValue.Trim() + " ORDER BY Id").ToPagedList(PageIndex, PageSize);
            }
            catch
            {
                return TryCatchNotFoundPosts();
            }

            if (obj == null)
            {
                return TryCatchNotFoundPosts();
            }

            string htmlCode = "";
            object model = Activator.CreateInstance(Type.GetType("NotebookShop.Models.Database." + CurrentTable));
            var prop = model.GetType().GetProperties();
            var columns = ModelAttributes.GetFieldsName(model);

            for (int i = 0; i < prop.Length; i++)
            {
                if (prop[i].Name == "Id" || prop[i].Name == "Photo")
                    continue;

                htmlCode += "<div><label>" + columns[i] + "</label>" +
                    "<input class='full-width' type='text' name='" + prop[i].Name + "' placeholder='" + columns[i] + "'/>" +
                    "</div>";
            }

            ViewBag.FilterFields = htmlCode;
            ViewBag.Page = PageIndex;
            ViewBag.List = obj;
            ViewBag.Table = CurrentTable;

            return View("~/Views/Home/Accessories.cshtml");
        }

        [HttpPost]
        public IActionResult SetSearchValue([FromBody]dynamic postData)
        {
            string searchValue = "";

            List<DocumentData> list = new List<DocumentData>();
            foreach (var item in JsonConvert.DeserializeObject(postData))
            {
                string buffer = item.Value;

                if (!string.IsNullOrEmpty(buffer))
                    searchValue += string.Format(" (LOWER({0}) LIKE LOWER('%{1}%') OR LOWER({0}) = '') AND", item.Key, item.Value);
            }

            searchValue = searchValue.Substring(0, searchValue.LastIndexOf("AND"));

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

        public IActionResult TryCatchNotFoundPosts()
        {
            ViewBag.RedirectMessage = "Не найдено";
            return View("~/Views/Home/RedirectPage.cshtml");
        }

        [HttpGet]
        public async Task<IActionResult> LogOut()
        {
            await HttpContext.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        public IActionResult AdminAuth() => View();

        [HttpPost]
        public async Task<IActionResult> SignIn(string email, string password)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
                return Json(new { divId = "signInInfo", message = "Все поля обязательны для заполнения!", btnId = "signInBtn", time = 3000 });

            admins admin = Connector.Get<admins>().Where(m => (m.Email.Equals(email))).FirstOrDefault();

            if (admin != null)
            {
                if (Encryption.Decrypt(admin.Password) == password)
                {
                    await HttpContext.SignInAsync(new ClaimsPrincipal(new ClaimsIdentity(new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, admin.Email),
                        new Claim(ClaimTypes.Role, "Admin")
                    }, "login")));

                    return Json(new { divId = "signInSuccess", message = "Успех. Перенаправление произойдёт через 3 секунды автоматически!", btnId = "signInBtn", home = true, time = 3000 });
                }
                else
                    return Json(new { divId = "signInError", message = "Ошибка. Такого адреса почты не существует!", btnId = "signInBtn", time = 3000 });
            }
            else
                return Json(new { divId = "signInError", message = "Ошибка. Такого адреса почты не существует!", btnId = "signInBtn", time = 3000 });
        }

        public IActionResult About() => View(Connector.Get<aboutproject>().FirstOrDefault());

        [HttpPost]
        public IActionResult Subscribe(string email)
        {
            if (string.IsNullOrEmpty(email))
                return Json(new { message = "Все поля обязательны для заполнения!" });

            EmailLogic emailLogic = new EmailLogic();

            if (emailLogic.IsEmailValid(email) == false)
                return Json(new { message = "Не верный формат Email!" });

            emaillist CheckEmail = Connector.Get<emaillist>().Where(e => e.Email.Equals(email)).FirstOrDefault();

            if (CheckEmail == null)
            {
                Connector.Insert(new emaillist()
                {
                    CreateDate = DateTime.Now.ToShortDateString(),
                    CreateTime = DateTime.Now.ToShortTimeString(),
                    Email = email
                });
                obj = new { message = "Успех. Ваш адрес добавлен в список рассылки!" };
            }
            else
            {
                obj = new { message = "Вы уже подписались на список рассылки!" };
            }

            return Json(obj);
        }

        public IActionResult Basket()
        {
            var basket = Connector.Get<basket>().Where(b => b.UserKey.Equals(Request.Cookies["userKey"]));

            if (basket.Count() <= 0)
            {
                ViewBag.RedirectMessage = "Корзина пуста";
                return View("~/Views/Home/RedirectPage.cshtml");
            }

            int summ = 0;
            List<BasketModel> order = new List<BasketModel>();

            foreach (var item in basket)
            {
                order.Add(new BasketModel()
                {
                    Table = item.TableName,
                    ModelId = item.ModelId,
                    Model = Connector.GetValue(string.Format("SELECT Model FROM {0} WHERE Id = {1}", item.TableName, item.ModelId)),
                    Description = Connector.GetValue(string.Format("SELECT Description FROM {0} WHERE Id = {1}", item.TableName, item.ModelId)),
                    Cost = Connector.GetValue(string.Format("SELECT Cost FROM {0} WHERE Id = {1}", item.TableName, item.ModelId)),
                    Photo = Connector.GetValue(string.Format("SELECT Photo FROM {0} WHERE Id = {1}", item.TableName, item.ModelId))
                });

                try
                {
                    summ += int.Parse(Connector.GetValue(string.Format("SELECT Cost FROM {0} WHERE Id = {1}", item.TableName, item.ModelId)));
                }
                catch { }
            }

            ViewBag.Summ = summ;

            return View(order);
        }

        [HttpPost]
        public IActionResult FinishOrder(string initials, string address)
        {
            if (string.IsNullOrEmpty(initials) || string.IsNullOrEmpty(address))
                return Json(new { divId = "orderError", message = "Все поля обязательны для заполнения!", btnId = "btnOrder", time = 3000 });

            var basket = Connector.Get<basket>().Where(b => b.UserKey.Equals(Request.Cookies["userKey"]));

            int summ = 0;
            string description = "Заказываем товар(ы): ";

            foreach (var item in basket)
            {
                description += "Артикул: " + item.ModelId + ", модель: " + Connector.GetValue(string.Format("SELECT Model FROM {0} WHERE Id = {1}", item.TableName, item.ModelId)) + ". ";
                try
                {
                    summ += int.Parse(Connector.GetValue(string.Format("SELECT Cost FROM {0} WHERE Id = {1}", item.TableName, item.ModelId)));
                }
                catch { }
                Connector.Delete(item);
            }

            Connector.Insert(new orders()
            {
                Address = address,
                Cost = summ.ToString(),
                DateOrder = DateTime.Now.ToString(),
                Description = description,
                Initials = initials
            });

            return Json(new
            {
                divId = "orderSuccess",
                message = "Ваш заказ был принят, в скором времени с ваши свяжются наши менеджеры!",
                btnId = "btnOrder",
                time = 5000,
                userHome = true
            });
        }

        [HttpPost]
        public IActionResult DeleteFromBasket(string table, int modelId, int minusCost)
        {
            int summ = 0;

            var basket = Connector.Get<basket>().Where(b => b.UserKey.Equals(Request.Cookies["userKey"]));
            foreach (var item in basket)
            {
                try
                {
                    summ += int.Parse(Connector.GetValue(string.Format("SELECT Cost FROM {0} WHERE Id = {1}", item.TableName, item.ModelId)));
                }
                catch { }
            }

            summ -= minusCost;

            var deleteGood = Connector.Get<basket>().Where(b => b.TableName.Equals(table) && b.ModelId == modelId).FirstOrDefault();

            if (deleteGood != null)
                Connector.Delete(deleteGood);
            else
                summ += minusCost;

            return Json(new { divClass = "." + table + "_" + modelId, summ });
        }

        public IActionResult AddToBasket(string table, int id)
        {
            var Product = Connector.Get(table, "WHERE Id = " + id).FirstOrDefault();

            if (Product == null)
                return TryCatchNotFoundPosts();

            var userKey = Request.Cookies["userKey"];

            basket Goood = null;
            try
            {
                Goood = Connector.Get<basket>().Where(b => b.UserKey.Equals(userKey) && b.TableName.Equals(table) && b.ModelId == id).FirstOrDefault();
            }
            catch (Exception)
            {
                Goood = null;
            }

            if (Goood == null)
            {
                Connector.Insert(new basket()
                {
                    ModelId = id,
                    TableName = table,
                    UserKey = Request.Cookies["userKey"]
                });
            }
            else
                return Json(new { divId = "success" + id, message = "Товар уже добавлен в корзину!", btnId = "btnBusket", time = 3000 });

            return Json(new { divId = "success" + id, message = "Товар добавлен в корзину!", btnId = "btnBusket", time = 3000 });
        }
    }
}