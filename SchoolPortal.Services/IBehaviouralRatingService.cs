﻿using Microsoft.AspNetCore.Http;
using SchoolPortal.Core.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;

namespace SchoolPortal.Services
{
    public interface IBehaviouralRatingService
    {
        IEnumerable<BehaviouralRating> GetBehaviouralRatings();
        Task<BehaviouralRating> GetBehaviouralRatingByName(string name);
        Task CreateBehaviouralResult(BehaviouralResult behaviouralResult);
        Task CreateBehaviouralResults(string session, long termId, long studentId, IEnumerable<(long rateId, string score)> scores);
        Task DeleteBehaviouralResult(long behaviouralResultId);
        Task DeleteBehaviouralResults(string session, long termId, long studentId);
        Task UpdateBehaviouralResult(BehaviouralResult behaviouralResult);
        Task UpdateBehaviouralResults(string session, long termId, long studentId, IEnumerable<(long id, long rateId, string score)> scores);
        IEnumerable<BehaviouralResult> GetBehaviouralResults();
        Task<BehaviouralResult> GetBehaviouralResult(long id);
        IEnumerable<BehaviouralResult> GetBehaviouralResults(string session, long termId, long studentId);

        byte[] GenerateBatchUploadTemaplate();
        Task<IEnumerable<IEnumerable<BehaviouralResult>>> ExtractData(IFormFile file);
        Task BatchCreateBehaviouralResults(IEnumerable<IEnumerable<BehaviouralResult>> results, string session, long termId);
    }
}
