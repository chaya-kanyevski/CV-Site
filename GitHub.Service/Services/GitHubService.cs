using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GitHub.Service.Interfaces;
using GitHub.Service.Models;
using Microsoft.Extensions.Options;
using Octokit;

namespace GitHub.Service.Services
{
    public class GitHubService : IGitHubService
    {
        private readonly GitHubClient _client;
        private readonly string _username;

        public GitHubService(IOptions<GitHubOptions> options)
        {
            _username = options.Value.Username;

            _client = new GitHubClient(new ProductHeaderValue("GitHubPortfolio"));

            if (!string.IsNullOrEmpty(options.Value.PersonalAccessToken))
            {
                _client.Credentials = new Credentials(options.Value.PersonalAccessToken);
            }
        }

        public async Task<IEnumerable<RepositoryInfo>> GetPortfolioAsync()
        {
            var repositories = await _client.Repository.GetAllForUser(_username);
            var result = new List<RepositoryInfo>();

            foreach (var repo in repositories)
            {
                try
                {
                    var languagesDict = await _client.Repository.GetAllLanguages(repo.Owner.Login, repo.Name);
                    var pullRequests = await _client.PullRequest.GetAllForRepository(repo.Owner.Login, repo.Name);

                    DateTimeOffset lastCommitDate = repo.UpdatedAt;
                    try
                    {
                        var commits = await _client.Repository.Commit.GetAll(repo.Owner.Login, repo.Name);
                        if (commits.Any())
                        {
                            lastCommitDate = commits.OrderByDescending(c => c.Commit.Author.Date).First().Commit.Author.Date;
                        }
                    }
                    catch (ApiException ex) when (ex.Message.Contains("Git Repository is empty"))
                    {
                        Console.WriteLine($"Repository {repo.Name} is empty. Using repository updated date instead.");
                    }

                    var languages = languagesDict.Select(l => l.Name).ToList();

                    result.Add(new RepositoryInfo
                    {
                        Id = repo.Id,
                        Name = repo.Name,
                        Description = repo.Description,
                        Languages = languages,
                        LastCommitDate = lastCommitDate,
                        Stars = repo.StargazersCount,
                        PullRequests = pullRequests.Count,
                        WebsiteUrl = repo.Homepage,
                        HtmlUrl = repo.HtmlUrl
                    });
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error processing repository {repo.Name}: {ex.Message}");
                }
            }

            return result;
        }

        public async Task<IEnumerable<RepositoryInfo>> SearchRepositoriesAsync(string? name = null, string? language = null, string? username = null)
        {
            var searchQuery = new List<string>();

            if (!string.IsNullOrEmpty(name))
                searchQuery.Add(name);

            if (!string.IsNullOrEmpty(username))
                searchQuery.Add($"user:{username}");

            var searchQueryString = string.Join(" ", searchQuery);
            var request = new SearchRepositoriesRequest(searchQueryString);

            if (!string.IsNullOrEmpty(language))
            {
                try
                {
                    var languageEnum = (Language)Enum.Parse(typeof(Language), language, true);
                    request.Language = languageEnum;
                }
                catch (ArgumentException)
                {
                    Console.WriteLine($"Warning: Language '{language}' is not recognized by Octokit.");
                }
            }

            var searchResult = await _client.Search.SearchRepo(request);
            var result = new List<RepositoryInfo>();

            foreach (var repo in searchResult.Items.Take(10)) 
            {
                try
                {
                    var languagesDict = await _client.Repository.GetAllLanguages(repo.Owner.Login, repo.Name);
                    var pullRequests = await _client.PullRequest.GetAllForRepository(repo.Owner.Login, repo.Name);

                    DateTimeOffset lastCommitDate = repo.UpdatedAt;
                    try
                    {
                        var commits = await _client.Repository.Commit.GetAll(repo.Owner.Login, repo.Name);
                        if (commits.Any())
                        {
                            lastCommitDate = commits.OrderByDescending(c => c.Commit.Author.Date).First().Commit.Author.Date;
                        }
                    }
                    catch (ApiException ex) when (ex.Message.Contains("Git Repository is empty"))
                    {
                        Console.WriteLine($"Repository {repo.Name} is empty. Using repository updated date instead.");
                    }

                    var languages = languagesDict.Select(l => l.Name).ToList();

                    result.Add(new RepositoryInfo
                    {
                        Id = repo.Id,
                        Name = repo.Name,
                        Description = repo.Description,
                        Languages = languages,
                        LastCommitDate = lastCommitDate,
                        Stars = repo.StargazersCount,
                        PullRequests = pullRequests.Count,
                        WebsiteUrl = repo.Homepage,
                        HtmlUrl = repo.HtmlUrl
                    });
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error processing repository {repo.Name}: {ex.Message}");
                }
            }

            return result;
        }

        public async Task<DateTimeOffset> GetLatestActivityDateAsync()
        {
            var events = await _client.Activity.Events.GetAllUserPerformed(_username);
            return events.Any() ? events.Max(e => e.CreatedAt) : DateTimeOffset.MinValue;
        }
    }
}
