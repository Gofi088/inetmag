using System.Linq;
using System;
using Microsoft.AspNetCore.Http;
using NotebookShop.Classes;
using NotebookShop.Models.Database;

namespace NotebookShop
{
    public class GlobalValues
    {
        public GlobalValues(IHttpContextAccessor httpContextaccessor) => _httpContextaccessor = httpContextaccessor;
        public static IHttpContextAccessor _httpContextaccessor { get; set; }
        public static string RandomString(int length) => new string(Enumerable.Repeat("ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789", length).Select(s => s[new Random().Next(s.Length)]).ToArray());             
        public static bool SendMessage { get; set; }
        public static string GetAboutProject() => Connector.Get<aboutproject>().FirstOrDefault().Text ?? "";
    }
}