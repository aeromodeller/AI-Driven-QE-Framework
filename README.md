# AI-Accelerated Quality Engineering Blueprint

🚀 A practical blueprint for modernizing quality engineering with browser automation, CI/CD, and generative AI.

## The value proposition

Legacy testing often relies on slow, brittle manual regression cycles that delay feedback and releases. This blueprint demonstrates how teams can shift repeatable checks into AI-accelerated, browser-based automation—helping shorten feedback loops, reduce time-to-market, and give engineers more time to focus on high-value quality risks.

The goal is not to replace engineering judgment with AI. It is to combine reliable automated execution with AI-assisted quality practices so teams can scale coverage while keeping people in control.

## Tech stack

| Technology | Role |
|---|---|
| C# and .NET | Test implementation and reusable framework code |
| Microsoft Playwright | Cross-browser web UI automation |
| NUnit | Test organization and execution |
| Azure DevOps | CI/CD pipeline orchestration |
| Allure | Test reporting and result visualization |
| Generative AI (GenAI) | Prompt-driven support for test design and quality workflows |

## Architecture highlights

- 🧱 **Page Object Model:** Encapsulates login-page selectors and actions in `src/pages/LoginPage.cs` to keep UI interactions reusable.
- 🔄 **CI/CD integration:** `pipelines/azure-pipelines.yml` defines build and test stages, browser installation, and test/report artifact publishing.
- 🧠 **AI-prompting strategy:** `ai-prompts/` is the home for reusable prompts that support consistent, reviewable GenAI-assisted quality workflows.
- 📊 **Reporting:** NUnit tests use Allure attributes to capture test descriptions and severity for reporting.
- ✅ **Test coverage example:** `src/tests/LoginTests.cs` demonstrates successful-login and locked-out-user scenarios using Arrange-Act-Assert.

## Project structure

```text
src/
  pages/       # Page Object Model classes
  tests/       # NUnit and Playwright tests
  utils/       # Shared test helpers
pipelines/     # Azure DevOps YAML pipelines
ai-prompts/    # Reusable GenAI prompts
```

## How to run

### Prerequisites

- .NET 8 SDK
- PowerShell (`pwsh`) to run Playwright's browser installer
- Node.js and npm if you want to generate the Allure HTML report locally
- Internet access to reach the public SauceDemo sample application

### Clone the repository

```bash
git clone https://github.com/aeromodeller/AI-Driven-QE-Framework.git
cd AI-Driven-QE-Framework
```

### Restore, install browsers, and run tests

The test project file is [AI.Driven.QE.Framework.Tests.csproj](./src/tests/AI.Driven.QE.Framework.Tests.csproj). It targets .NET 8 and references NUnit, Playwright, and Allure.NUnit.

```bash
dotnet restore src/tests/AI.Driven.QE.Framework.Tests.csproj
dotnet build src/tests/AI.Driven.QE.Framework.Tests.csproj --configuration Release
```

Install Chromium after building:

```bash
pwsh src/tests/bin/Release/net8.0/playwright.ps1 install chromium
```

Run the test suite:

```bash
dotnet test src/tests/AI.Driven.QE.Framework.Tests.csproj --configuration Release
```

The tests target `https://www.saucedemo.com/` using its public `standard_user` and `locked_out_user` demo accounts with password `secret_sauce`. The valid-login test verifies navigation to `/inventory.html`; the locked-out test checks SauceDemo's `[data-test="error"]` message.

To generate an Allure HTML report locally after the configured Allure NUnit adapter has produced results:

```bash
npm install --global allure-commandline
allure generate "$(find src/tests -type d -name allure-results -print -quit)" --clean --output allure-report
```

The pipeline builds the project, installs Chromium, runs tests, and publishes TRX and Allure report artifacts.
