using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NotebookShop.Classes
{
    public static class CookieLogic
    {
        public static void SetCookie(Controller controller, string key, string value, int? days)
        {
            if (days.HasValue)
                controller.Response.Cookies.Append(key, value, new CookieOptions { Expires = DateTime.Now.AddDays(days.Value) });
            else
                controller.Response.Cookies.Append(key, value, new CookieOptions { Expires = DateTime.Now.AddDays(1) });
        }

        public static string GetCookie(Controller controller, string key)
        {
            if (controller.Request == null || controller.Request.Cookies == null)
                return null;
            return controller.Request.Cookies[key];
        }

        public static void DeleteCookie(Controller controller, string key)
        {
            if (controller.Request != null || controller.Request.Cookies != null)
            {
                try
                {
                    controller.Response.Cookies.Delete(key);
                }
                catch { }
            }                
        }

        public static IDictionary<string, string> FromLegacyCookieString(this string legacyCookie) => legacyCookie.Split('&').Select(s => s.Split('=')).ToDictionary(kvp => kvp[0], kvp => kvp[1]);

        public static string ToLegacyCookieString(this IDictionary<string, string> dict) => string.Join("&", dict.Select(kvp => string.Join("=", kvp.Key, kvp.Value)));
    }
}