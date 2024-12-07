using Microsoft.AspNetCore.Http;
using SchoolPortal.Core.DTOs;

namespace SchoolPortal.Core.Extensions
{
    public static class HttpContextExtension
    {
        public static string GetBaseUrl(this HttpContext context)
        {
            return $"{context.Request.Scheme}://{context.Request.Host}{context.Request.PathBase}/";
        }
        public static string GetIPAddress(this HttpContext context)
        {
            return context?.Connection?.RemoteIpAddress?.ToString();
        }
        public static void SetUserSession(this HttpContext context, SessionObject userSession)
        {
            context.Items[Constants.CONTEXT_USER_KEY] = userSession;
        }



        public static SessionObject GetUserSession(this HttpContext context)
        {
            return context.Items[Constants.CONTEXT_USER_KEY] as SessionObject;
        }

        public static bool IsUserSessionExist(this HttpContext context)
        {
            return context.Items[Constants.CONTEXT_USER_KEY] != null;
        }

        public static void AddSessionItem(this HttpContext context, string key, string value)
        {
            context.Session.SetString(key, value);
        }

        public static void RemoveSessionItem(this HttpContext context, string key)
        {
            context.Session.Remove(key);
        }

        public static string GetSessionItem(this HttpContext context, string key)
        {
            return context.Session.GetString(key);
        }
        public static void ClearUserSession(this HttpContext context)
        {
            context.Items.Remove(Constants.CONTEXT_USER_KEY);
            context.Session.Clear();
        }


    }
}
