/*
 Pre-Deployment Script Template              
--------------------------------------------------------------------------------------
 This file contains SQL statements that will be executed before the build script.  
 Use SQLCMD syntax to include a file in the pre-deployment script.      
 Example:      :r .\myfile.sql                
 Use SQLCMD syntax to reference a variable in the pre-deployment script.    
 Example:      :setvar TableName MyTable              
               SELECT * FROM [$(TableName)]          
--------------------------------------------------------------------------------------
*/

/* Begin Delete any duplicates in UserSettings table to allow the constraints to be applied */

 IF OBJECT_ID(N'dbo.UserSettings', N'U') IS NOT NULL
 BEGIN
     ;WITH CTE AS (
         SELECT
             userref,
             ROW_NUMBER() OVER (PARTITION BY userref ORDER BY (SELECT NULL)) AS row_num
         FROM [dbo].[UserSettings]
     )
     DELETE FROM CTE
     WHERE row_num > 1;
 END;

/* end remove script after first deployment */
