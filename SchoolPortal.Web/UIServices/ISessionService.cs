using SchoolPortal.Core.DTOs;
using System;

namespace SchoolPortal.Web.UIServices
{
    public interface ISessionService
    {
        public string BaseUrl { get; }
        public string ControllerName { get; }
        public string ActionName { get; }

        public SessionObject UserSession { get; }

        public bool UserSessionExist { get; }
        public string Initial { get; }

        public string DisplayRoles { get; }
        public bool IsAdmin
        {
            get;
        }
        public bool IsHeadTeacher
        {
            get;
        }

        public bool IsTeacher
        {
            get;
        }

        public bool IsParent
        {
            get;
        }

        public bool IsStudent
        {
            get;
        }


        //public string AccessToken { get; }
        public string Culture { get; }

        public DateTimeOffset ConverDatetToClientTimeZone(DateTimeOffset date);
    }
}