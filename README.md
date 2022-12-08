## ⛔Never push sensitive information such as client id's, secrets or keys into repositories including in the README file⛔

# Apprentice Feedback API 
<img src="https://avatars.githubusercontent.com/u/9841374?s=200&v=4" align="right" alt="UK Government logo">

[![Build Status](https://sfa-gov-uk.visualstudio.com/Digital%20Apprenticeship%20Service/_apis/build/status/das-apprentice-feedback-api?repoName=SkillsFundingAgency%2Fdas-apprentice-feedback-api&branchName=master)](https://sfa-gov-uk.visualstudio.com/Digital%20Apprenticeship%20Service/_build/latest?definitionId=2539&repoName=SkillsFundingAgency%2Fdas-apprentice-feedback-api&branchName=master)
[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=SkillsFundingAgency_das-apprentice-feedback-api&metric=alert_status)](https://sonarcloud.io/project/overview?id=SkillsFundingAgency_das-apprentice-feedback-api)
[![Jira Project](https://img.shields.io/badge/Jira-Project-blue)](https://skillsfundingagency.atlassian.net/browse/QF-72)
[![Confluence Project](https://img.shields.io/badge/Confluence-Project-blue)](https://skillsfundingagency.atlassian.net/wiki/spaces/NDL/pages/3776446580/Apprentice+Feedback+-+QF)
[![License](https://img.shields.io/badge/license-MIT-lightgrey.svg?longCache=true&style=flat-square)](https://en.wikipedia.org/wiki/MIT_License)

This repository represents the Apprentice Feedback API code base. Apprentice Feedback is a service that allows apprentices to provide feedback on their training providers. The apprentice is able to submit feedback via the ad hoc journey, or an emailing journey. Either way, the UI code base is the das-apprentice-feedback-web repository, this repository is the inner API, and the outer API code base is in the das-apim-endpoints repository within the ApprenticeFeedback project.

## Developer Setup
### Requirements

In order to run this solution locally you will need the following:

* [.NET Core SDK >= 3.1](https://www.microsoft.com/net/download/)
* (VS Code Only) [C# Extension](https://marketplace.visualstudio.com/items?itemName=ms-vscode.csharp)
* [SQL Server Express LocalDB](https://docs.microsoft.com/en-us/sql/database-engine/configure-windows/sql-server-express-localdb)
* [Azurite](https://docs.microsoft.com/en-us/azure/storage/common/storage-use-azurite) (previosuly known as Azure Storage Emulator)

### Environment setup

* Publish the local database from the `SFA.DAS.ApprenticeFeedback.Database` project. 
* Add the following to an `appsettings.development.json` file.
    * Add your connection strings for CosmosDB and ASB to the relevant sections of the file
* The CosmosDB will be created automatically if it does not already exist and the credentials you are connected with have the appropriate rights within the Azure tenant otherwise it will need to be created manually using the details in the config below under `CosmosDbSettings`.

appsettings.development.json file:

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft": "Warning",
      "Microsoft.Hosting.Lifetime": "Information"
    }
  },
  "AllowedHosts": "*",
  "ConfigurationStorageConnectionString": "UseDevelopmentStorage=true",
  "ConfigNames": "SFA.DAS.ApprenticeFeedback.Api",
  "EnvironmentName": "LOCAL",
  "Version": "1.0"
}  
```

Azure Table Storage Config:

Row Key: SFA.DAS.ApprenticeFeedback.Api_1.0

Partition Key: LOCAL

Data:

```
{
  "ApplicationSettings": {
    "DbConnectionString": "Data Source=HOSTNAME;Initial Catalog=SFA.DAS.ApprenticeFeedback.Database;Integrated Security=True;Pooling=False;Connect Timeout=30",
    "NServiceBusConnectionString": "UseLearningEndpoint=true",
    "NServiceBusLicense": "",
    "InitialDenyPeriodDays": 92,
    "FinalAllowedPeriodDays": 61,
    "RecentDenyPeriodDays": 14,
    "MinimumActiveApprenticeshipCount": 10,
    "ReportingFeedbackCutoffMonths": 12,
    "ReportingMinNumberOfResponses": 1
  },
  "AzureAd": {
    "tenant": "TENANT.onmicrosoft.com",
    "identifier": "https://IDENTIFIER.onmicrosoft.com/ENVIRONMENT"
  }
}
```

### Running

* Start Azurite e.g. using a command `C:\Program Files (x86)\Microsoft SDKs\Azure\Storage Emulator>AzureStorageEmulator.exe start`
* Run the solution
* NB: You may need other applications running in conjuntion with this, such as the backend API `das-apim-endpoints/ApprenticeFeedback` project and also the UI `das-apprentice-feedback-web` codebase for the UI journey.

### Tests

This codebase includes unit tests and integration tests. These are all in seperate projects aptly named and kept in the 'Tests' folder. 

#### Unit Tests

There are several unit test projects in the solution built using C#, NUnit, Moq, FluentAssertions, .NET and AutoFixture.

#### Integration Tests

There is one integration test project and it tests the APIs.

