using SchoolPortal.Core.Models;
using SchoolPortal.Data.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SchoolPortal.Services.Implementations
{
    public class BehaviouralRatingService: IBehaviouralRatingService
    {
        private readonly IRepository<BehaviouralRating> behaviouralRatingRepo;
        private readonly IRepository<BehaviouralResult> behaviouralResultRepo;
        private readonly ILoggerService<StudentResultService> logger;

        public BehaviouralRatingService(
            IRepository<BehaviouralRating> behaviouralRatingRepo,
             IRepository<BehaviouralResult> behaviouralResultRepo,
             ILoggerService<StudentResultService> logger)
        {
            this.behaviouralRatingRepo = behaviouralRatingRepo;
            this.behaviouralResultRepo = behaviouralResultRepo;
            this.logger = logger;
        }

        // get ratings
        public IEnumerable<BehaviouralRating> GetBehaviouralRatings()
        {
            return behaviouralRatingRepo.GetAll().OrderBy(r => r.Category).ThenBy(r => r.Id);
        }
    }
}
