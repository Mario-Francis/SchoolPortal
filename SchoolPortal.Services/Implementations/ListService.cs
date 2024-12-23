﻿using Microsoft.AspNetCore.Http;
using SchoolPortal.Core;
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
        private readonly IRepository<Class> classRepo;
        private readonly IRepository<ClassRoom> classRoomRepo;
        private readonly IRepository<RoomCode> roomCodeRepo;
        private readonly IRepository<Relationship> relationshipRepo;
        private readonly IRepository<ExamType> examTypeRepo;
        private readonly IRepository<Exam> examRepo;
        private readonly IHttpContextAccessor contextAccessor;

        public ListService(IRepository<Term> termRepo,
            IRepository<TermSection> termSectionRepo,
            IRepository<ClassType> classTypeRepo,
            IRepository<Role> roleRepo,
            IRepository<Class> classRepo,
            IRepository<ClassRoom> classRoomRepo,
            IRepository<RoomCode> roomCodeRepo,
            IRepository<Relationship> relationshipRepo,
            IRepository<ExamType> examTypeRepo,
            IRepository<Exam> examRepo,
            IHttpContextAccessor contextAccessor)
        {
            this.termRepo = termRepo;
            this.termSectionRepo = termSectionRepo;
            this.classTypeRepo = classTypeRepo;
            this.roleRepo = roleRepo;
            this.classRepo = classRepo;
            this.classRoomRepo = classRoomRepo;
            this.roomCodeRepo = roomCodeRepo;
            this.relationshipRepo = relationshipRepo;
            this.examTypeRepo = examTypeRepo;
            this.examRepo = examRepo;
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
            var roles = roleRepo.GetWhere(r => r.Id != (long)AppRoles.STUDENT);
            if (!contextAccessor.HttpContext.User.IsInRole(Core.Constants.ROLE_ADMIN))
            {
                roles = roles.Where(r => r.Name != Core.Constants.ROLE_ADMIN);
            }

            return roles;
        }
        public IEnumerable<Class> GetClasses()
        {
            return classRepo.GetAll().OrderBy(c => c.ClassTypeId).ThenBy(c => c.ClassGrade);
        }

        public IEnumerable<ClassRoom> GetClassRooms()
        {
            return classRoomRepo.GetWhere(c=>c.IsActive).OrderBy(c => c.ClassId).ThenBy(c => c.RoomCode);
        }

        public IEnumerable<RoomCode> GetRoomCodes()
        {
            return roomCodeRepo.GetAll().OrderBy(c => c.Code);
        }
        public IEnumerable<Relationship> GetRelationships()
        {
            return relationshipRepo.GetAll().OrderBy(r => r.Id);
        }
        public IEnumerable<ExamType> GetExamTypes()
        {
            return examTypeRepo.GetAll().OrderBy(e => e.Id);
        }
        public IEnumerable<Exam> GetExams()
        {
            return examRepo.GetAll().OrderByDescending(e => e.StartDate);
        }
    }
}
