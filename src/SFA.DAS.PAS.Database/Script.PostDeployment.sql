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

IF OBJECT_ID(N'dbo.ClientOutboxData', N'U') IS NOT NULL  
   DROP TABLE [dbo].[ClientOutboxData];

GO

IF OBJECT_ID(N'dbo.OutboxData', N'U') IS NOT NULL  
   DROP TABLE [dbo].[OutboxData];
