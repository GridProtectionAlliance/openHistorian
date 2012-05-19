using System;
using System.Data.SqlServerCe;
using System.Text;

namespace openHistorian.V2.Service.Instance.Database
{
    public static class DatabaseCode
    {
        public static void CreateDatabase(string file)
        {
            SqlCeConnectionStringBuilder connectionString = new SqlCeConnectionStringBuilder();
            connectionString.DataSource = file;
            using (SqlCeEngine engine = new SqlCeEngine(connectionString.ToString()))
            {
                engine.CreateDatabase();
            }

            using (var connection = new SqlCeConnection(connectionString.ToString()))
            {
                using (var transaction = connection.BeginTransaction())
                {
                    ExecuteSql(connection, transaction, CreateTableApplication());
                    ExecuteSql(connection, transaction, CreateTableTables());
                    ExecuteSql(connection, transaction, CreateTableColumns());
                    ExecuteSql(connection, transaction, CreateTableRecords());
                    transaction.Commit(CommitMode.Immediate);
                }
            }
        }

        static void ExecuteSql(SqlCeConnection connection, SqlCeTransaction transaction, string sqlStatement)
        {
            using (SqlCeCommand command = connection.CreateCommand())
            {
                command.Transaction = transaction;
                command.CommandText = sqlStatement;
                command.ExecuteNonQuery();
            }
        }

        static String CreateTableApplication()
        {
            StringBuilder SB = new StringBuilder();
            SB.AppendLine("CREATE TABLE [Applications](");
            SB.AppendLine("	[ApplicationId] [int] IDENTITY(1,1) NOT NULL,");
            SB.AppendLine("	[ApplicationName] [nvarchar](255) NOT NULL,");
            SB.AppendLine("	CONSTRAINT Applications_PK PRIMARY KEY CLUSTERED ");
            SB.AppendLine("	(");
            SB.AppendLine("		[ApplicationId]");
            SB.AppendLine("	),");
            SB.AppendLine("	CONSTRAINT Applications_IDX UNIQUE");
            SB.AppendLine("	(");
            SB.AppendLine("		ApplicationName");
            SB.AppendLine("	)");
            SB.AppendLine(")");
            return SB.ToString();
        }

        static String CreateTableTables()
        {
            StringBuilder SB = new StringBuilder();

            SB.AppendLine("CREATE TABLE [Tables](");
            SB.AppendLine("	[TableId] [int] IDENTITY(1,1) NOT NULL,");
            SB.AppendLine("	[ApplicationId] [int] NOT NULL,");
            SB.AppendLine("	[TableName] [nvarchar](255) NOT NULL,");
            SB.AppendLine("	CONSTRAINT Tables_PK PRIMARY KEY CLUSTERED");
            SB.AppendLine("	(");
            SB.AppendLine("		[TableId] ASC");
            SB.AppendLine("	),");
            SB.AppendLine("	CONSTRAINT Tables_IDX UNIQUE");
            SB.AppendLine("	(");
            SB.AppendLine("		[ApplicationId],");
            SB.AppendLine("		[TableName]");
            SB.AppendLine("	),");
            SB.AppendLine("	CONSTRAINT Tables_FK FOREIGN KEY (ApplicationId)");
            SB.AppendLine("	REFERENCES Applications(ApplicationId)");
            SB.AppendLine("	ON UPDATE CASCADE");
            SB.AppendLine("	ON DELETE CASCADE");
            SB.AppendLine(")");

            return SB.ToString();

        }

        static String CreateTableColumns()
        {
            StringBuilder SB = new StringBuilder();

            SB.AppendLine("CREATE TABLE [dbo].[Columns](");
            SB.AppendLine("	[ColumnId] [int] IDENTITY(1,1) NOT NULL,");
            SB.AppendLine("	[TableId] [int] NOT NULL,");
            SB.AppendLine("	[ColumnName] [nvarchar](255) NOT NULL,");
            SB.AppendLine("	[DefaultValueIfNull] [nvarchar](255) NULL,");
            SB.AppendLine("	CONSTRAINT Columns_PK PRIMARY KEY CLUSTERED");
            SB.AppendLine("	(");
            SB.AppendLine("		[ColumnId] ASC");
            SB.AppendLine("	),");
            SB.AppendLine("	CONSTRAINT Columns_IDX UNIQUE");
            SB.AppendLine("	(");
            SB.AppendLine("		[TableId],");
            SB.AppendLine("		[ColumnName]");
            SB.AppendLine("	),");
            SB.AppendLine("	CONSTRAINT Columns_FK FOREIGN KEY (TableId)");
            SB.AppendLine("	REFERENCES Tables(TableId)");
            SB.AppendLine("	ON UPDATE CASCADE");
            SB.AppendLine("	ON DELETE CASCADE");
            SB.AppendLine(")");

            return SB.ToString();
  
        }

        static String CreateTableRecords()
        {
            StringBuilder SB = new StringBuilder();

            SB.AppendLine("CREATE TABLE [dbo].[Records](");
            SB.AppendLine("	[ColumnId] [int] NOT NULL,");
            SB.AppendLine("	[RowNumber] [int] NOT NULL,");
            SB.AppendLine("	[Field] [nvarchar](255) NULL,");
            SB.AppendLine("	CONSTRAINT Records_PK PRIMARY KEY CLUSTERED");
            SB.AppendLine("	(");
            SB.AppendLine("		[ColumnId] ASC,");
            SB.AppendLine("		RowNumber ASC");
            SB.AppendLine("	),");
            SB.AppendLine("	CONSTRAINT Records_FK FOREIGN KEY (ColumnId)");
            SB.AppendLine("	REFERENCES Columns(ColumnId)");
            SB.AppendLine("	ON UPDATE CASCADE");
            SB.AppendLine("	ON DELETE CASCADE");
            SB.AppendLine(")");

            return SB.ToString();
       
        }
    }
}
