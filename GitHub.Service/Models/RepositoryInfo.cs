using System;
using System.Collections.Generic;

namespace GitHub.Service.Models
{
    public class RepositoryInfo
    {
        public long Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public IEnumerable<string> Languages { get; set; } = new List<string>();
        public DateTimeOffset LastCommitDate { get; set; }
        public int Stars { get; set; }
        public int PullRequests { get; set; }
        public string? WebsiteUrl { get; set; }
        public string HtmlUrl { get; set; } = string.Empty;
    }
}