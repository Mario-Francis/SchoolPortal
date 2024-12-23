﻿using Microsoft.AspNetCore.Mvc.Rendering;
using SchoolPortal.Core;
using SchoolPortal.Core.Extensions;
using SchoolPortal.Services;
using System.Collections.Generic;
using System.Linq;

namespace SchoolPortal.Web.UIServices
{
    public class DropdownService:IDropdownService
    {
        private readonly IListService listService;
        private readonly IStudentResultService studentResultService;

        public DropdownService(IListService listService, IStudentResultService studentResultService)
        {
            this.listService = listService;
            this.studentResultService = studentResultService;
        }

        public IEnumerable<SelectListItem> GetTerms(string value=null)
        {
            List<SelectListItem> terms =  listService.GetTerms()
                .Select(c => new SelectListItem { Text = c.Name, Value = c.Id.ToString(), Selected=c.Id.ToString() == value }).ToList();
            terms.Insert(0, new SelectListItem { Text = "- Select term -", Value = "" });
            return terms;
        }

        public IEnumerable<SelectListItem> GetTermSections(string value = null)
        {
            List<SelectListItem> termSections = listService.GetTermSection()
                .Select(t => new SelectListItem { Text = t.Name, Value = t.Id.ToString(), Selected = t.Id.ToString() == value }).ToList();

            termSections.Insert(0, new SelectListItem { Text = "- Select term section -", Value = "" });
            return termSections;
        }

        public IEnumerable<SelectListItem> GetClassTypes(string value = null)
        {
            List<SelectListItem> classTypes = listService.GetClassTypes()
                .Select(c => new SelectListItem { Text = c.Name, Value = c.Id.ToString(), Selected = c.Id.ToString() == value }).ToList();

            classTypes.Insert(0, new SelectListItem { Text = "- Select class types -", Value = "" });
            return classTypes;
        }

        public IEnumerable<SelectListItem> GetRoles(string value = null)
        {
            List<SelectListItem> roles = listService.GetRoles()
                .Select(l => new SelectListItem { Text = l.Name, Value = l.Id.ToString(), Selected = l.Id.ToString() == value }).ToList();

            roles.Insert(0, new SelectListItem { Text = "- Select role -", Value = "" });
            return roles;
        }

        public IEnumerable<SelectListItem> GetClasses(string value = null)
        {
            List<SelectListItem> classes = listService.GetClasses()
                .Select(c => new SelectListItem { Text = $"{c.ClassType.Name.Capitalize()} {c.ClassGrade}", Value = c.Id.ToString(), Selected = c.Id.ToString() == value }).ToList();

            classes.Insert(0, new SelectListItem { Text = "- Select class -", Value = "" });
            return classes;
        }

        public IEnumerable<SelectListItem> GetClassRooms(string value = null)
        {
            List<SelectListItem> classRooms = listService.GetClassRooms()
                .Select(c => new SelectListItem { Text = $"{c.Class.ClassType.Name.Capitalize()} {c.Class.ClassGrade} {c.RoomCode.Capitalize()}", Value = c.Id.ToString(), Selected = c.Id.ToString() == value }).ToList();

            classRooms.Insert(0, new SelectListItem { Text = "- Select classroom -", Value = "" });
            return classRooms;
        }

        public IEnumerable<SelectListItem> GetRoomCodes(string value = null)
        {
            List<SelectListItem> codes = listService.GetRoomCodes()
                .Select(c => new SelectListItem { Text = $"{c.Code}", Value = c.Code, Selected = c.Code.ToString() == value }).ToList();

            codes.Insert(0, new SelectListItem { Text = "- Select room code -", Value = "" });
            return codes;
        }
        public IEnumerable<SelectListItem> GetRelationships(string value = null)
        {
            List<SelectListItem> relationships = listService.GetRelationships()
                .Select(c => new SelectListItem { Text = $"{c.Name}", Value = c.Id.ToString(), Selected = c.Id.ToString() == value }).ToList();

            relationships.Insert(0, new SelectListItem { Text = "- Select relationship -", Value = "" });
            return relationships;
        }

        public IEnumerable<SelectListItem> GetExamTypes(string value = null)
        {
            List<SelectListItem> examTypes = listService.GetExamTypes()
                .Select(c => new SelectListItem { Text = $"{c.Name}", Value = c.Id.ToString(), Selected = c.Id.ToString() == value }).ToList();

            examTypes.Insert(0, new SelectListItem { Text = "- Select exam type -", Value = "" });
            return examTypes;
        }

        public IEnumerable<SelectListItem> GetExams(ExamTypes examType = ExamTypes.NONE, string value = null)
        {
            var _exams = listService.GetExams();
            if(examType!= ExamTypes.NONE)
            {
                _exams = _exams.Where(e => e.ExamType.Id == (int)examType).OrderByDescending(e=>e.Session).ThenByDescending(e=>e.TermId);
            }
            List<SelectListItem> exams = _exams
                .Select(e => new SelectListItem { Text = $"{e.Session} {e.Term.Name} Term | {e.ExamType.Name} Exam | ({e.StartDate.ToString("MMM d, yyyy")} - {e.EndDate.ToString("MMM d, yyyy")})", Value = e.Id.ToString(), Selected = e.Id.ToString() == value }).ToList();

            exams.Insert(0, new SelectListItem { Text = "- Select exam -", Value = "" });
            return exams;
        }

        public IEnumerable<SelectListItem> GetStudentSessions(long studentId, string value = null)
        {
            List<SelectListItem> sessions = studentResultService.GetResultSessions(studentId)
                .Select(c => new SelectListItem { Text = c, Value = c, Selected = c == value }).ToList();

            sessions.Insert(0, new SelectListItem { Text = "- Select session -", Value = "" });
            return sessions;
        }

    }
}
