# GitHub Portfolio API

An API application that displays your GitHub portfolio and allows repository search.

## Installation

1. Clone the repository:
   git clone https://github.com/YOUR_USERNAME/GitHubPortfolio.git
   cd GitHubPortfolio

2. Set up User Secrets (development environment only):
   cd Portfolio.API
   dotnet user-secrets init
   dotnet user-secrets set "GitHub:PersonalAccessToken" "YOUR_GITHUB_TOKEN"

3. Update your GitHub username in `appsettings.json`:
   "GitHub": {
     "Username": "YOUR_GITHUB_USERNAME"
   }

4. Build and run the project:
   dotnet build
   dotnet run --project Portfolio.API

5. Open Swagger UI:
   https://localhost:7048/swagger
   or  
   http://localhost:5132/swagger

## Features

- Displays your GitHub repositories
- Shows detailed info for each repository (languages, stars, pull requests, etc.)
- Supports searching public repositories by name, language, and username
- Implements caching for improved performance

## Technologies

- ASP.NET Core Web API  
- Octokit.NET  
- Swagger (OpenAPI)  
- In-memory caching
