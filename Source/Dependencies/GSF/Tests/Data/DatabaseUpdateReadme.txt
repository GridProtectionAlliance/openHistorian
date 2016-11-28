Database Updates
---------------------------------------------------------------------------------------------

WARNING! In general, script modifications should not be made at the time-series host
application level (e.g., SIEGate or openPDC) - these scripts will be overwritten during
the automated Grid Solutions Framework dependency roll-down process. It is expected that
changes to the primary schema will be made in GSF then rolled-down to all time-series
implementations. Custom script modifications should go in an independent local script
to be run after the initial schema setup.

---------------------------------------------------------------------------------------------

Updating the Grid Solutions Framework databases requires a number of steps, enumerated below.
There are five database types supported: MySQL, Oracle, SQL Server, PostgreSQL and SQLite.

 1) Open GridSolutionsFramework.sln
 2) In the 'Data' folder in the GSF.TimeSeries project and select the sub-folder for the
      database type you wish to edit - GSFSchema.sql contains the primary schema
 3) Edit the schema as needed - repeat for other databases

To initialize the SQLite databases:

 4) Close the solution to avoid any conflicts
 5) Navigate to the ...\Data\SQLite folder
 6) Run db-update.bat
 7) The update script will ask if you want to check in the database updates, select
      no unless all your schema changes have already been checked-in (not likely)

To update the SerializedSchema.bin used by the Data Migration Utility:

 8) Load a new blank schema (i.e., GSFSchema.sql) into SQL Server
 9) Run the Data Migration Utility
10) Select SQLServer from the Data Type list for the "From Connect String"
11) Answer "Yes" to verify connection string replacement question
12) Answer "No" to verify connection string type question for OLE DB
13) Update the from connection string to connect to your new GSF Schema, for example:
      Provider=SQLOLEDB; Data Source=localhost\SQLEXPRESS; Initial Catalog=GSFSchema;
      Integrated Security=SSPI
14) Click the from connection string "Test" link, if this succeeds a "Serialize"
      button will be visible on the application
15) Click the "Serialize" button to create a new "SerializedSchema.bin", the updated
      file will be located in the DataMigrationUtility.exe path
16) Check out and replace the "...\GSF.TimeSeries\Data\SerializedSchema.bin" file
      with the new updated file
17) Check in the replaced "...\GSF.TimeSeries\Data\SerializedSchema.bin" file

Your database changes are now complete!

You will need to run roll-down scripts for specific projects to make sure schema changes
are applied to specific time-series implementations.