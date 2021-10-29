using Microsoft.AspNetCore.Http;
using SchoolPortal.Core.Models;
using SchoolPortal.Data.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SchoolPortal.Services.Implementations
{
    public class ListService:IListService
    {
        private readonly IRepository<Term> termRepo;
        private readonly IRepository<TermSection> termSectionRepo;
        private readonly IRepository<ClassType> classTypeRepo;
        private readonly IRepository<Role> roleRepo;
        private readonly IHttpContextAccessor contextAccessor;

        public ListService(IRepository<Term> termRepo,
            IRepository<TermSection> termSectionRepo,
            IRepository<ClassType> classTypeRepo,
            IRepository<Role> roleRepo,
            IHttpContextAccessor contextAccessor)
        {
            this.termRepo = termRepo;
            this.termSectionRepo = termSectionRepo;
            this.classTypeRepo = classTypeRepo;
            this.roleRepo = roleRepo;
            this.contextAccessor = contextAccessor;
        }

        public IEnumerable<Term> GetTerms()
        {
            return termRepo.GetAll();
        }

        public IEnumerable<TermSection> GetTermSection()
        {
          return termSectionRepo.GetAll();
        }

        public IEnumerable<ClassType> GetClassTypes()
        {
            return classTypeRepo.GetAll();
        }

        public IEnumerable<Role> GetRoles()
        {
            var roles = roleRepo.GetAll();
            if (!contextAccessor.HttpContext.User.IsInRole(Core.Constants.ROLE_ADMIN))
            {
                roles = roles.Where(r => r.Name != Core.Constants.ROLE_ADMIN);
            }

            return roles;
        }
    }
}
