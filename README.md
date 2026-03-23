# Automation Framework – C# + Reqnroll + Playwright

## Overview
This project is a UI automation framework designed for Recruitment and Candidate workflows using:

- **C#**
- **Reqnroll** for BDD
- **Microsoft Playwright** for browser automation
- **NUnit** as the test runner
- **JSON-driven** configuration, test data, and locator management
- **Page Object Model** for maintainable test design

The framework is structured to support:
- login
- navigation to Recruitment → Candidates
- candidate creation
- resume upload
- vacancy selection
- success message validation
- candidate search
- candidate detail verification
- status updates
- logout and session validation
- tag-based execution

---

## Technology Stack

| Component | Choice |
|---|---|
| Language | C# |
| BDD Framework | Reqnroll |
| Automation Library | Playwright |
| Test Runner | NUnit |
| Assertions | FluentAssertions |
| JSON Handling | Newtonsoft.Json |
| Build Tool | .NET SDK / NuGet |

---

## Project Structure

```text
AutomationFramework/
├── Drivers/
│   ├── BrowserFactory.cs
│   └── PlaywrightDriver.cs
├── Features/
│   ├── CandidateManagement.feature
│   ├── CandidateSearch.feature
│   ├── CandidateStatusUpdate.feature
│   ├── Login.feature
│   └── Logout.feature
├── Hooks/
│   └── TestHooks.cs
├── Models/
│   ├── CandidateDataModel.cs
│   ├── ConfigModel.cs
│   └── RuntimeCandidateContext.cs
├── Pages/
│   ├── AddCandidatePage.cs
│   ├── BasePage.cs
│   ├── CandidateListPage.cs
│   ├── LoginPage.cs
│   └── RecruitmentPage.cs
├── StepDefinitions/
│   ├── CandidateCreationSteps.cs
│   ├── CandidateSearchSteps.cs
│   ├── CandidateStatusSteps.cs
│   ├── LoginSteps.cs
│   ├── LogoutSteps.cs
│   └── StepDefinitionBase.cs
├── Locator/
│   ├── AddCandidatePage.json
│   ├── CandidateListPage.json
│   ├── Common.json
│   ├── LoginPage.json
│   └── RecruitmentPage.json
├── TestData/
│   ├── candidateData.json
│   └── config.json
├── Utilities/
│   ├── CandidateDataReader.cs
│   ├── ConfigReader.cs
│   ├── FrameworkLogger.cs
│   ├── JsonFileReader.cs
│   ├── LocatorReader.cs
│   ├── PathHelper.cs
│   ├── RecruitmentWorkflow.cs
│   ├── ScreenshotHelper.cs
│   └── WaitHelper.cs
├── Resources/
│   └── resumes/
│       └── sample_resume.pdf
├── Reports/
├── Screenshots/
├── AutomationFramework.csproj
├── reqnroll.json
└── README.md
```

---

## Framework Design

### 1. BDD Layer
Feature files are written in Gherkin and describe user workflows in business-readable form.

### 2. Step Definition Layer
Step definitions are separated feature-wise for better maintainability. They:
- read data from JSON
- call reusable workflow methods and page object methods
- store runtime values such as candidate name and candidate ID
- perform assertions

Current step classes:
- `LoginSteps`
- `CandidateCreationSteps`
- `CandidateSearchSteps`
- `CandidateStatusSteps`
- `LogoutSteps`
- `StepDefinitionBase`

### 3. Page Object Model Layer
Each page class encapsulates UI interactions and reads selectors from page-wise JSON files in the `Locator/` folder.

Pages included:
- `LoginPage`
- `RecruitmentPage`
- `AddCandidatePage`
- `CandidateListPage`

### 4. Core Framework Layer
Reusable framework services include:
- Playwright browser startup and shutdown
- JSON data readers
- locator reader
- wait helper
- screenshot helper
- path helper
- `RecruitmentWorkflow` for reusable business flows
- `FrameworkLogger` for lightweight execution logging

### 5. Data Layer
All configurable and test-driven values are externalized into JSON files:
- `config.json`
- `candidateData.json`
- `Locator/LoginPage.json`
- `Locator/RecruitmentPage.json`
- `Locator/AddCandidatePage.json`
- `Locator/CandidateListPage.json`
- `Locator/Common.json`

---

## Configuration Files

## `TestData/config.json`
Stores framework and runtime settings.

Example fields:
- application URL
- browser
- headless mode
- timeouts
- screenshot options
- report and screenshot paths

## `TestData/candidateData.json`
Stores:
- login credentials
- candidate details
- vacancy
- keywords
- notes
- status transitions
- resume path

## `Locator/*.json`
Stores selectors in separate page/module files:
- `LoginPage.json`
- `RecruitmentPage.json`
- `AddCandidatePage.json`
- `CandidateListPage.json`
- `Common.json`

---

## Feature Coverage

### `Login.feature`
- successful login to application

### `CandidateManagement.feature`
- navigate to Recruitment → Candidates
- add a candidate
- upload resume
- select vacancy
- save candidate
- validate success
- verify candidate in list

### `CandidateSearch.feature`
- search candidate by name
- open candidate details
- validate candidate profile

### `CandidateStatusUpdate.feature`
- update status to shortlist
- update status to interview scheduled
- validate status changes

### `Logout.feature`
- logout
- validate session end

---

## Hooks and Lifecycle

The framework uses `TestHooks.cs` to handle:

### Before Scenario
- read config
- read test data
- read page-wise locators from the Locator folder
- initialize Playwright
- launch browser
- create page instance
- store objects in `ScenarioContext`

### After Scenario
- capture screenshot on failure
- close page/context/browser

---

## Browser Support

Configured from `config.json`.

Supported values:
- `chromium`
- `firefox`
- `webkit`

Current default:
```json
"browser": "chromium"
```

---

## Locator Strategy

Preferred locator order:
1. stable CSS/name/id
2. text-based Playwright selectors
3. XPath only where necessary

All locators are centralized in:
```text
Locator/LoginPage.json
Locator/RecruitmentPage.json
Locator/AddCandidatePage.json
Locator/CandidateListPage.json
Locator/Common.json
```

This keeps selectors outside page classes and makes maintenance easier, while also separating them page-wise for cleaner ownership.

---

## Wait Strategy

The framework uses:
- Playwright built-in waiting
- reusable explicit waits via `WaitHelper`

Avoided:
- `Thread.Sleep`
- hardcoded unnecessary delays

---

## File Upload Design

Resume upload is handled through Playwright file upload support.

Source path is externalized in:
```text
TestData/candidateData.json
```

The framework resolves the file path using `PathHelper` and uploads it through:
- `SetInputFilesAsync()`

---

## Assertions

Assertions are implemented using `FluentAssertions`.

Validations currently include:
- dashboard visible after login
- success message after candidate creation
- candidate profile available
- candidate appears in list
- status change reflected
- login page visible after logout

---

## Prerequisites

Install the following before execution:
- .NET SDK 8 or later
- Node.js is not required separately for Playwright package usage in .NET workflows
- Internet access for initial NuGet restore and Playwright browser installation

Recommended:
- VS Code or Visual Studio
- PowerShell or command prompt

---

## Setup Instructions

## 1. Restore NuGet packages
From the project folder:

```powershell
cd AutomationFramework
dotnet restore
```

## 2. Install Playwright browsers
Required once after package restore:

```powershell
pwsh bin/Debug/net10.0/playwright.ps1 install
```

If the project has not yet been built, run:

```powershell
dotnet build
pwsh bin/Debug/net10.0/playwright.ps1 install
```

---

## Execution Commands

## Build the project
```powershell
dotnet build
```

## Run all tests
```powershell
dotnet test
```

## Run smoke tests
```powershell
dotnet test --filter "TestCategory=smoke"
```

## Run login-related tests
```powershell
dotnet test --filter "TestCategory=login"
```

> Note: exact tag/category filtering behavior can vary depending on the Reqnroll + NUnit configuration generated during build.

---

## Tags Used

Feature files currently include:
- `@smoke`
- `@regression`
- `@login`
- `@candidate`
- `@search`
- `@status`
- `@logout`

These tags support selective suite execution and CI grouping.

---

## Runtime Data Handling

The framework stores runtime candidate values using `RuntimeCandidateContext`, such as:
- first name
- last name
- full name
- email
- candidate ID
- current status

This allows one step to create data and later steps to validate or update the same candidate.

The framework also generates unique runtime candidate values during execution to reduce duplicate-record issues across repeated test runs.

---

## Reporting

Current reporting support:
- NUnit result output
- Reqnroll scenario execution
- screenshots on failure in the `Screenshots/` folder
- console logging through `FrameworkLogger`

You can extend the framework later with:
- LivingDoc reporting
- Extent Reports
- CI pipeline publishing
- Allure integration

---

## Notes About Test Data

The supplied OrangeHRM demo application data can change over time, so some UI locators or business data values may need adjustment if the target application updates.

Typical fields likely to require updates:
- vacancy text
- candidate search selectors
- status button text
- login page header locator

---

## Recommended Improvements for Next Iteration

Possible enhancements:
1. Add environment-specific config files
2. Add API or database validation layer
3. Generate richer HTML reports
4. Add retry strategy for flaky environments
5. Add CI pipeline integration
6. Split steps into multiple step definition classes
7. Add base assertion and logging utilities
8. Add custom wrapper for dropdown/autocomplete handling

---

## Troubleshooting

### Reqnroll bindings not generated
Run:
```powershell
dotnet clean
dotnet build
```

### Playwright browser not found
Run:
```powershell
pwsh bin/Debug/net10.0/playwright.ps1 install
```

### File upload path not found
Verify:
- `Resources/resumes/sample_resume.pdf` exists
- the path in `candidateData.json` is correct

### Candidate not found after creation
Possible reasons:
- search locator needs adjustment
- application data grid changed
- candidate save did not complete before search

### Session/logout validation fails
Update the relevant page locator file in:
```text
Locator/
```

---

## Summary

This framework implements:
- C# + Reqnroll + Playwright
- Page Object Model
- JSON-based configuration and data externalization
- hooks-based setup/teardown
- locator centralization
- feature-based BDD scenarios
- reusable browser and utility layers
- candidate workflow automation foundation

It is ready to be extended for additional OrangeHRM modules or enterprise recruitment workflows.
