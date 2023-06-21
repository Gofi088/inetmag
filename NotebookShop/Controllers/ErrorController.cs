using Microsoft.AspNetCore.Mvc;
using NotebookShop.Models.Common;
using System.Diagnostics;

namespace NotebookShop.Controllers
{
    public class ErrorController : Controller
    {
        ErrorModel errorModel;

        public IActionResult ErrorStatus(int? statusCode = null)
        {
            if (statusCode.HasValue)
            {
                switch (statusCode)
                {
                    case 204:
                        errorModel = new ErrorModel
                        {
                            Code = "204",
                            MessageToUser = "Нет контента"
                        };
                        break;
                    case 400:
                        errorModel = new ErrorModel
                        {
                            Code = "400",
                            MessageToUser = "Ошибка запроса"
                        };
                        break;
                    case 404:
                        errorModel = new ErrorModel
                        {
                            Code = "404",
                            MessageToUser = "Не найдено"
                        };
                        break;
                    case 500:
                        errorModel = new ErrorModel
                        {
                            Code = "500",
                            MessageToUser = "Ошибка сервера"
                        };
                        break;
                    case 501:
                        errorModel = new ErrorModel
                        {
                            Code = "501",
                            MessageToUser = "Не реализовано"
                        };
                        break;

                    default:
                        errorModel = new ErrorModel
                        {
                            Code = "500",
                            MessageToUser = "Ошибка сервера"
                        };
                        break;
                }
            }
            else
            {
                errorModel = new ErrorModel
                {
                    Code = Activity.Current?.Id ?? HttpContext.TraceIdentifier,
                    MessageToUser = "Ошибка, что за она, увы, мы не знаем :C Если знаете сообщите нам"
                };
            }

            return View("~/Views/Error/Error.cshtml", errorModel);
        }
    }
}