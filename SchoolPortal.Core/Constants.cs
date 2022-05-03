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

    public enum ExamTypes
    {
        NONE=0,
        MID_TERM=1,
        END_TERM
    }

    public enum BehaviouralRatingCategory
    {
        Affective,
        Psychomotor
    }

    public enum ActivityActionType
    {
        OTHER,
        LOGIN,
        LOGIN_ATTEMPT,

        ADDED_END_TERM_RESULT,
        BATCH_ADDED_END_TERM_RESULT,
        UPDATED_END_TERM_RESULT,
        DELETED_END_TERM_RESULT,

        CREATED_REMARK,
        UPDATED_REMARK,
        DELETED_REMARK,
        BATCH_ADDED_REMARK,

        CREATED_HEALTH_RECORD,
        UPDATED_HEALTH_RECORD,
        DELETED_HEALTH_RECORD,
        BATCH_ADDED_HEALTH_RECORD,

        CREATED_BEHAVIOURAL_RESULT,
        UPDATED_BEHAVIOURAL_RESULT,
        DELETED_BEHAVIOURAL_RESULT,
        BATCH_ADDED_BEHAVIOURAL_RESULT,

        UPDATED_USER,
        UPDATED_USER_PHOTO,
        UPDATED_USER_PASSWORD_RECOVERY_TOKEN,
        RESET_USER_PASSWORD,

        UPDATED_STUDENT,
        UPDATED_STUDENT_PHOTO,
        UPDATED_STUDENT_PASSWORD_RECOVERY_TOKEN,
        RESET_STUDENT_PASSWORD,

        ADDED_COURSE_WORK,
        UPDATED_COURSE_WORK,
        DELETED_COURSE_WORK,

        ADDED_GRADE,
        UPDATED_GRADE,
        DELETED_GRADE

    }

    public class Constants
    {
        public const string SESSION_COOKIE_ID = ".CIS.Session";
        public const string AUTH_COOKIE_ID = ".CIS.Auth";
        public const string SYSTEM_NAME = "SYSTEM";
        public const string CONTEXT_USER_KEY = "Identity";
        public const string DEFAULT_NEW_USER_PASSWORD = "C@lebP@ss123";

        public const string ROLE_ADMIN = "Administrator";
        public const string ROLE_HEAD_TEACHER = "Head Teacher";
        public const string ROLE_TEACHER = "Teacher";
        public const string ROLE_PARENT = "Parent";
        public const string ROLE_STUDENT = "Student";

        public const string USER_TYPE_USER = "User";
        public const string USER_TYPE_STUDENT = "Student";

        public const string CLIENT_TIMEOFFSET_COOKIE_ID = "clientTimeZoneOffset";
        public static string[] IGNORED_COLUMNS = new string[] { "Id", "CreatedBy", "CreatedDate" };
    }
}
