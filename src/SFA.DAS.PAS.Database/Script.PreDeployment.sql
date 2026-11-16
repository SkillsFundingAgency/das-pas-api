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

WITH CTE AS (
    SELECT 
        userref,
        ROW_NUMBER() OVER (PARTITION BY userref ORDER BY (SELECT NULL)) AS row_num
    FROM UserSettings
)
DELETE FROM CTE
WHERE row_num > 1;

/* end remove script after first deployment */
