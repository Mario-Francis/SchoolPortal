using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace SchoolPortal.Core.DTOs
{
    public class ObjectChangesCheckResult
    {
        public bool HasChanges { get; set; }
        public IEnumerable<KeyValuePair<string, string>> FromProperties { get; set; }
        public IEnumerable<KeyValuePair<string, string>> ToProperties { get; set; }
        [JsonIgnore]
        public string FormatFrom
        {
            get
            {
                var result = "";
                foreach (var p in FromProperties)
                {
                    result += $"{p.Key}:{p.Value}, ";
                }
                result = result.Length == 0 ? null : result.Substring(0, result.Length - 2);
                return result;
            }
        }
        [JsonIgnore]
        public string FormatTo
        {
            get
            {
                var result = "";
                foreach (var p in ToProperties)
                {
                    result += $"{p.Key}:{p.Value}, ";
                }
                result = result.Length == 0 ? null : result.Substring(0, result.Length - 2);
                return result;
            }
        }
    }
}
