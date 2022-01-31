using SchoolPortal.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace SchoolPortal.Services
{
    public interface IBehaviouralRatingService
    {
        IEnumerable<BehaviouralRating> GetBehaviouralRatings();
    }
}
