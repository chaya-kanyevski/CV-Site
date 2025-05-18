namespace GitHub.Service.Models
{
    public class GitHubOptions
    {
        public const string GitHub = "GitHub";

        public string Username { get; set; } = string.Empty;
        public string PersonalAccessToken { get; set; } = string.Empty;
    }
}