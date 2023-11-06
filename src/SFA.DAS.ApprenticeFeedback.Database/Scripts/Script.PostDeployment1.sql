/*
Post-Deployment Script Template							
--------------------------------------------------------------------------------------
 This file contains SQL statements that will be appended to the build script.		
 Use SQLCMD syntax to include a file in the post-deployment script.			
 Example:      :r .\myfile.sql								
 Use SQLCMD syntax to reference a variable in the post-deployment script.		
 Example:      :setvar TableName MyTable							
               SELECT * FROM [$(TableName)]					
--------------------------------------------------------------------------------------
*/

:r .\AddFeedbackTargetStatus.sql
:r .\AddFeedbackEligibilityStatus.sql
:r .\AddDefaultExitSurveyAttributes.sql
:r .\SetPrimaryReasonForExistingExitSurveys.sql
:r .\AddProviderExclusions.sql
:r .\AddEngagementEmails.sql

IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ApprenticeExitSurvey_20221118091741]') AND type in (N'U'))
    DROP TABLE [dbo].[ApprenticeExitSurvey_20221118091741];

IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ApprenticeExitSurvey_20221118111008]') AND type in (N'U'))
    DROP TABLE [dbo].[ApprenticeExitSurvey_20221118111008];

IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ApprenticeFeedback]') AND type in (N'U'))
    DROP TABLE [dbo].[ApprenticeFeedback];

IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ApprenticeFeedbackTargets]') AND type in (N'U'))
    DROP TABLE [dbo].[ApprenticeFeedbackTargets];

IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ClientOutboxData]') AND type in (N'U'))
    DROP TABLE [dbo].[ClientOutboxData];

IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[dbo.ApprenticeExitSurvey]') AND type in (N'U'))
    DROP TABLE [dbo].[dbo.ApprenticeExitSurvey];

IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Exclusions]') AND type in (N'U'))
    DROP TABLE [dbo].[Exclusions];

IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[FeedbackEmailHistory]') AND type in (N'U'))
    DROP TABLE [dbo].[FeedbackEmailHistory];

IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[FeedbackEmailTransaction]') AND type in (N'U'))
    DROP TABLE [dbo].[FeedbackEmailTransaction];

IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[FeedbackEmailTransactions]') AND type in (N'U'))
    DROP TABLE [dbo].[FeedbackEmailTransactions];

IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[FeedbackTarget]') AND type in (N'U'))
    DROP TABLE [dbo].[FeedbackTarget];

IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[FeedbackTargets]') AND type in (N'U'))
    DROP TABLE [dbo].[FeedbackTargets];

IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[OutboxData]') AND type in (N'U'))
    DROP TABLE [dbo].[OutboxData];

IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ProviderAttributes]') AND type in (N'U'))
    DROP TABLE [dbo].[ProviderAttributes];

IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Providers]') AND type in (N'U'))
    DROP TABLE [dbo].[Providers];

IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Standards]') AND type in (N'U'))
    DROP TABLE [dbo].[Standards];
