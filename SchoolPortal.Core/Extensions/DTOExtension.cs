using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SchoolPortal.Core.Extensions
{
    public static class DTOExtension
    {
        public static IEnumerable<long> GetIdsNotInList(this IEnumerable<long> currentList, IEnumerable<long> targetList)
        {
            foreach (var id in currentList)
            {
                if (!targetList.Contains(id))
                    yield return id;
            }
        }
        public static IEnumerable<long> GetIdsInList(this IEnumerable<long> currentList, IEnumerable<long> targetList)
        {
            foreach (var id in currentList)
            {
                if (targetList.Contains(id))
                    yield return id;
            }
        }

        //public static User ToUser(this UserRequestObject req)
        //{
        //    return new User
        //    {
        //        FirstName = req.FirstName,
        //        LastName = req.LastName,
        //        Email = req.Email,
        //        PhoneNumber = req.PhoneNumber,
        //        RoleId = req.RoleId,
        //        Password = req.Password,
        //        IsActive = true,
        //        CreatedAt = DateTimeOffset.Now,
        //        UpdatedAt = DateTimeOffset.Now,
        //    };
        //}

        //public static UserObject ToUserObject(this User user)
        //{
        //    return new UserObject
        //    {
        //        Id = user.Id,
        //        FirstName = user.FirstName,
        //        LastName = user.LastName,
        //        Email = user.Email,
        //        PhoneNumber = user.PhoneNumber,
        //        Role = new RoleObject
        //        {
        //            Id = user.RoleId,
        //            Title = user.Role.Title,
        //            Privileges = user.Role?.RolePrivileges?.Select(rp => rp.Privilege)
        //        },
        //        IsActive = user.IsActive,
        //        IsEmailVerified = user.IsEmailVerified,
        //        IsPasswordChanged = user.IsPasswordChanged,
        //        CreatedAt = user.CreatedAt,
        //        CreatedBy = user.CreatedBy != null ? $"{user.CreatedBy.FirstName} {user.CreatedBy.LastName}" : "SYSTEM",
        //        UpdatedAt = user.UpdatedAt,
        //        UpdatedBy = user.UpdatedBy != null ? $"{user.UpdatedBy.FirstName} {user.UpdatedBy.LastName}" : "SYSTEM"
        //    };
        //}
    }
}
