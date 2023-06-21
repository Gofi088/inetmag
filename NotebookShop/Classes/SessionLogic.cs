using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace NotebookShop.Classes
{
    public static class SessionLogic
    {
        public static void Set<T>(this ISession session, string key, T value) => session.SetString(key, JsonConvert.SerializeObject(value));

        public static T Get<T>(this ISession session, string key)
        {
            var value = session.GetString(key);
            return value == null ? default(T) : JsonConvert.DeserializeObject<T>(value);
        }
    }
}