using System;
using System.Collections.Generic;
using System.Text;

namespace SchoolPortal.Core
{
    public enum AppRoles
    {
        ADMINISTRATOR=1,
        HEAD_TEACHER,
        TEACHER,
        PARENT,
        STUDENT
    }

    public enum Terms
    {
        FIRST=1,
        SECOND,
        THIRD
    }

    public enum TermSections
    {
        FIRST_HALF=1,
        SECOND_HALF
    }

    public class Constants
    {
        public const string SESSION_COOKIE_ID = ".CIS.Session";
        public const string SYSTEM_NAME = "SYSTEM";
        public const string CONTEXT_USER_KEY = "Identity";
        public const string DEFAULT_NEW_USER_PASSWORD = "C@lebP@ss123";
        public const string ROLE_ADMIN = "Administrator";
        public const string ROLE_HEAD_TEACHER = "Head Teacher";
        public const string ROLE_TEACHER = "Teacher";
        public const string ROLE_PARENT = "Parent";
        public const string ROLE_STUDENT = "Student";
    }
}
