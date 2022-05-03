using SchoolPortal.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace SchoolPortal.Services.Implementations
{
    public class TokenService:ITokenService
    {
        Random rand;
        public TokenService()
        {
            rand = new Random();
        }
        public string GenerateTokenFromData(string data)
        {
            var _data = $"{rand.Next(100000, 999999999).ToString()}_{data}_{DateTimeOffset.Now.ToString()}_{rand.Next(100000, 999999999).ToString()}";
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(_data));
        }

        public string ExtractDataFromToken(string token)
        {
            try
            {
                var _data = Encoding.UTF8.GetString(Convert.FromBase64String(token));
                return _data.Split('_')[1];
            }
            catch
            {
                throw new AppException("Invalid token");
            }
        }

        public bool IsTokenExpired(string token, int ExpiryPeriodInHours)
        {
            try
            {
                var _data = Encoding.UTF8.GetString(Convert.FromBase64String(token));
                var generatedDate = DateTimeOffset.Parse(_data.Split('_')[2]);
                return DateTimeOffset.Now > generatedDate.AddHours(ExpiryPeriodInHours);
            }
            catch
            {
                throw new AppException("Invalid token");
            }
        }
    }
}
