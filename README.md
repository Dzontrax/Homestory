# Homestory

A task for the Homestory company

# Tools

Machine: MacBook Air M2 16GB
The client - VS code

# Prerequisites

.NET SDK ≥ 8.0
Chromium browsers - installed via Playwright

Find all of the dependencies in the HomeStoryTest.csproj

# HTTPS

git clone https://github.com/your‑org/homestory-playwright-tests.git
cd homestory-playwright-tests

# restore NuGet packages

dotnet restore

# The project

Nothing more complicated than this:

Core/BaseTest - handles Playwright life-cycle,
Pages/SearchPage expresses user flows,
Core/Utils - keeps nitty-gritty utilities out of the way.
Validators/Assertions - little class to keep the assertions in one place.
