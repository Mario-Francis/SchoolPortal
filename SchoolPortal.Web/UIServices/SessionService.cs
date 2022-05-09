using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Options;
using SchoolPortal.Core;
using SchoolPortal.Core.DTOs;
using SchoolPortal.Core.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SchoolPortal.Web.UIServices
{
    public class SessionService:ISessionService
    {
        private readonly IHttpContextAccessor accessor;
        private readonly IOptionsSnapshot<AppSettings> appSettingsDelegete;

        public SessionService(IHttpContextAccessor accessor, IOptionsSnapshot<AppSettings> appSettingsDelegete)
        {
            this.accessor = accessor;
            this.appSettingsDelegete = appSettingsDelegete;
        }

        public string BaseUrl
        {
            get
            {
                return $"{accessor.HttpContext.Request.Scheme}://{accessor.HttpContext.Request.Host}{accessor.HttpContext.Request.PathBase}/";
            }
        }

        public AppSettings AppSettings
        {
            get
            {
                return this.appSettingsDelegete.Value;
            }
        }
        public string ControllerName
        {
            get
            {
                var path = accessor.HttpContext.Request.Path.ToString();
                if (path.Length > 1)
                {
                    path = path.Substring(1);
                    return path.Split(new char[] { '/' })[0];
                }
                else
                {
                    return "";
                }
            }
        }
        public string ActionName
        {
            get
            {
                var path = accessor.HttpContext.Request.Path.ToString();
                if (path.Length > 1)
                {
                    path = path.Substring(1);
                    var arr = path.Split(new char[] { '/' });
                    if (arr.Length > 1)
                    {
                        return arr[1];
                    }
                    else
                    {
                        return "";
                    }
                }
                else
                {
                    return "";
                }
            }
        }

        public SessionObject UserSession
        {
            get
            {
                return accessor.HttpContext.GetUserSession();
            }
        }

        public bool UserSessionExist
        {
            get
            {
                return accessor.HttpContext.IsUserSessionExist();
            }
        }
        public string Initial
        {
            get
            {
                var session = UserSession;
                return session.FirstName.Substring(0, 1) + session.Surname.Substring(0, 1);
            }
        }

        public string DisplayRoles
        {
            get
            {
                var session = UserSession;
                if (session?.Roles?.Any(r => r?.Name == Constants.ROLE_ADMIN) ?? false)
                {
                    return Constants.ROLE_ADMIN;
                }
                else
                {
                    var roles = new List<string>();
                    if (session?.Roles?.Any(r => r?.Name == Constants.ROLE_HEAD_TEACHER) ?? false)
                    {
                        roles.Add(Constants.ROLE_HEAD_TEACHER);
                    }

                    if (session?.Roles?.Any(r => r?.Name == Constants.ROLE_TEACHER) ?? false)
                    {
                        roles.Add(Constants.ROLE_TEACHER);
                    }

                    if (session?.Roles?.Any(r => r?.Name == Constants.ROLE_PARENT) ?? false)
                    {
                        roles.Add(Constants.ROLE_PARENT);
                    }

                    if (session?.Roles?.Any(r => r?.Name == Constants.ROLE_STUDENT) ?? false)
                    {
                        roles.Add(Constants.ROLE_STUDENT);
                    }

                    return string.Join(" | ",  roles);
                }
            }
        }

        public bool IsAdmin
        {
            get
            {
                return accessor.HttpContext.User.IsInRole(Constants.ROLE_ADMIN);
            }
        }
        public bool IsHeadTeacher
        {
            get
            {
                return accessor.HttpContext.User.IsInRole(Constants.ROLE_HEAD_TEACHER);
            }
        }

        public bool IsTeacher
        {
            get
            {
                return accessor.HttpContext.User.IsInRole(Constants.ROLE_TEACHER);
            }
        }

        public bool IsParent
        {
            get
            {
                return accessor.HttpContext.User.IsInRole(Constants.ROLE_PARENT);
            }
        }

        public bool IsStudent
        {
            get
            {
                return accessor.HttpContext.GetUserSession().UserType == Constants.USER_TYPE_STUDENT;
            }
        }

        //public string AccessToken
        //{
        //    get
        //    {
        //        return accessor.HttpContext.GetSessionItem(Constants.ACCESS_TOKEN_KEY);
        //    }
        //}

        public string Culture
        {
            get
            {
                var rcf= accessor.HttpContext.Request.HttpContext.Features.Get<IRequestCultureFeature>();
                return rcf.RequestCulture.Culture.Name;
            }
        }

        public DateTimeOffset ConverDatetToClientTimeZone(DateTimeOffset date)
        {
            int defaultOffset = appSettingsDelegete.Value.DefaultTimeZoneOffset;
            var val = accessor.HttpContext.Request.Cookies["clientTimeZoneOffset"];
            int offsetInMminutes = string.IsNullOrEmpty(val) ? defaultOffset : Convert.ToInt32(val);
            return date.ToOffset(new TimeSpan(0, offsetInMminutes, 0));
        }

    }
}
