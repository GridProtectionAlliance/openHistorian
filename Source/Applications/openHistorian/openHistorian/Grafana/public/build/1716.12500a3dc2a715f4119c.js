(self.webpackChunkgrafana=self.webpackChunkgrafana||[]).push([[1716],{21716:()=>{ace.define("ace/snippets/sqlserver",["require","exports","module"],function(n,e,t){"use strict";e.snippetText=`# ISNULL
snippet isnull
	ISNULL(\${1:check_expression}, \${2:replacement_value})
# FORMAT
snippet format
	FORMAT(\${1:value}, \${2:format})
# CAST
snippet cast
	CAST(\${1:expression} AS \${2:data_type})
# CONVERT
snippet convert
	CONVERT(\${1:data_type}, \${2:expression})
# DATEPART
snippet datepart
	DATEPART(\${1:datepart}, \${2:date})
# DATEDIFF
snippet datediff
	DATEDIFF(\${1:datepart}, \${2:startdate}, \${3:enddate})
# DATEADD
snippet dateadd
	DATEADD(\${1:datepart}, \${2:number}, \${3:date})
# DATEFROMPARTS 
snippet datefromparts
	DATEFROMPARTS(\${1:year}, \${2:month}, \${3:day})
# OBJECT_DEFINITION
snippet objectdef
	SELECT OBJECT_DEFINITION(OBJECT_ID('\${1:sys.server_permissions /*object name*/}'))
# STUFF XML
snippet stuffxml
	STUFF((SELECT ', ' + \${1:ColumnName}
		FROM \${2:TableName}
		WHERE \${3:WhereClause}
		FOR XML PATH('')), 1, 1, '') AS \${4:Alias}
	\${5:/*https://msdn.microsoft.com/en-us/library/ms188043.aspx*/}
# Create Procedure
snippet createproc
	-- =============================================
	-- Author:		\${1:Author}
	-- Create date: \${2:Date}
	-- Description:	\${3:Description}
	-- =============================================
	CREATE PROCEDURE \${4:Procedure_Name}
		\${5:/*Add the parameters for the stored procedure here*/}
	AS
	BEGIN
		-- SET NOCOUNT ON added to prevent extra result sets from interfering with SELECT statements.
		SET NOCOUNT ON;
		
		\${6:/*Add the T-SQL statements to compute the return value here*/}
		
	END
	GO
# Create Scalar Function
snippet createfn
	-- =============================================
	-- Author:		\${1:Author}
	-- Create date: \${2:Date}
	-- Description:	\${3:Description}
	-- =============================================
	CREATE FUNCTION \${4:Scalar_Function_Name}
		-- Add the parameters for the function here
	RETURNS \${5:Function_Data_Type}
	AS
	BEGIN
		DECLARE @Result \${5:Function_Data_Type}
		
		\${6:/*Add the T-SQL statements to compute the return value here*/}
		
	END
	GO`,e.scope="sqlserver"})}}]);

//# sourceMappingURL=1716.12500a3dc2a715f4119c.js.map