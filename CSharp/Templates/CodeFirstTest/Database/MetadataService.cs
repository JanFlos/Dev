using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Oracle.DataAccess.Client;
using System.Data.SqlServerCe;
using System.Data;
using System.Diagnostics;
using System.Data.Common;

namespace MetadataService
{
    class DataLoader
    {


        Nullable<DateTime> _lastSync;
        bool _lastSyncRead = false;

        private Nullable<DateTime> LastSync
        { 
            get 
            {
                if (!_lastSyncRead)
                {
                    var localCommand = new SqlCeCommand("SELECT LastSync FROM Properties", LocalConnection);
                    var executeReader = localCommand.ExecuteReader();
                    bool parameterExists = false;
                    DateTime _lastSynclastSynchronization = DateTime.Now;

                    while (executeReader.Read())
                    {
                        parameterExists = true;
                        _lastSync = executeReader.GetDateTime(0);
                    }

                    _lastSyncRead = true;
                    
                }

                return _lastSync; 
            } 
        }

        private readonly string _serverConnectionString;
        private readonly string _localConnectionString;

        private OracleConnection _serverConnection;
        private SqlCeConnection _localConnection;

        public DataLoader(String serverConnectionString, String localConnectionString)
        {
            Debug.Assert(serverConnectionString != null, "serverConnectionString == null");
            Debug.Assert(localConnectionString != null, "localConnectionString == null");

            _serverConnectionString = serverConnectionString;
            _localConnectionString = localConnectionString;
        }

        public OracleConnection ServerConnection
        {
            get
            {
                if (_serverConnection == null || _serverConnection.State != ConnectionState.Open)
                {
                    _serverConnection = new OracleConnection(_serverConnectionString);
                    _serverConnection.Open();
                }
                return _serverConnection;
            }
        }

        public SqlCeConnection LocalConnection
        {
            get
            {

                if (_localConnection == null || _localConnection.State != ConnectionState.Open)
                {
                    _localConnection = new SqlCeConnection(_localConnectionString);
                    _localConnection.Open();
                }
                return _localConnection;
            }
        }


        public void LoadPackages()
        {
            // Build command for local database
            var localSQL = "INSERT INTO packages([Id], [Name], [Schema]) " +
                           "VALUES (@Id, @Name, @Schema)";

            var localCommand = new SqlCeCommand(localSQL, LocalConnection);

            var id = new SqlCeParameter("Id", SqlDbType.Int);
            var name = new SqlCeParameter("Name", SqlDbType.NVarChar);
            var schema = new SqlCeParameter("Schema", SqlDbType.NVarChar);

            var localParams = localCommand.Parameters;

            localParams.Add(id);
            localParams.Add(name);
            localParams.Add(schema);

            // Build command for server database
            var serverSQL = "SELECT distinct object_id id, lower(object_name) name, lower(owner) owner " +
                            "  FROM dba_objects where object_type = 'PACKAGE' order by 2";

            var oracleCmd = new OracleCommand(serverSQL, ServerConnection);
            var executeReader = oracleCmd.ExecuteReader();

            // Transfer the Data
            try
            {

                while (executeReader.Read())
                {
                    id.Value = executeReader.GetInt32(0);
                    name.Value = executeReader.GetString(1);
                    schema.Value = executeReader.GetString(2);

                    localCommand.ExecuteNonQuery();
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.ToString());
            }

        }

        public void LoadMethods()
        {
            // Build command for local database
            var localSQL = "INSERT INTO methods([SubprogramId], [Name], [Overload], [ObjectId]) " +
                           "VALUES (@SubprogramId, @Name, @Overload, @PackageId)";


            var localCommand = new SqlCeCommand(localSQL, LocalConnection);

            var subprogramId = new SqlCeParameter("SubprogramId", SqlDbType.Int);
            var name = new SqlCeParameter("Name", SqlDbType.NVarChar);
            var overload = new SqlCeParameter("Overload", SqlDbType.NVarChar);
            var packageId = new SqlCeParameter("PackageId", SqlDbType.Int);

            var localParams = localCommand.Parameters;

            localParams.Add(subprogramId);
            localParams.Add(name);
            localParams.Add(overload);
            localParams.Add(packageId);

            // Build command for server database
            var serverSQL = "SELECT subprogram_id, " +
                            "       lower(procedure_name) procedure_name, " +
                            "       nvl(overload, '0') overload, " +
                            "       object_id " +
                            "  FROM dba_procedures p " +
                            " WHERE object_type = 'PACKAGE' " +
                            "   AND subprogram_id > 0 ";

            var oracleCmd = new OracleCommand(serverSQL, ServerConnection);
            var executeReader = oracleCmd.ExecuteReader();

            // Transfer the Data
            try
            {

                while (executeReader.Read())
                {
                    subprogramId.Value = executeReader.GetValue(0);
                    name.Value = executeReader.GetValue(1);
                    overload.Value = executeReader.GetValue(2);
                    packageId.Value = executeReader.GetValue(3);

                    localCommand.ExecuteNonQuery();
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.ToString());
            }

        }

        public void LoadArguments()
        {
            // Build command for local database
            var localSQL = "INSERT INTO arguments([Name], [Datatype], [Position], [SubprogramId], [Overload], [Defaulted], [ObjectId], [InOut]) " +
                           "VALUES (@Name, @Datatype, @Position, @MethodId, @Overload, @Defaulted, @ObjectId, @InOut)";


            var localCommand = new SqlCeCommand(localSQL, LocalConnection);

            var name = new SqlCeParameter("Name", SqlDbType.NVarChar);
            var datatype = new SqlCeParameter("Datatype", SqlDbType.NVarChar);
            var position = new SqlCeParameter("Position", SqlDbType.Int);
            var methodId = new SqlCeParameter("MethodId", SqlDbType.Int);
            var overload = new SqlCeParameter("Overload", SqlDbType.NVarChar);
            var defaulted = new SqlCeParameter("Defaulted", SqlDbType.NVarChar);
            var objectId = new SqlCeParameter("ObjectId", SqlDbType.Int);
            var inOut = new SqlCeParameter("InOut", SqlDbType.NVarChar);

            var localParams = localCommand.Parameters;

            localParams.Add(name);
            localParams.Add(datatype);
            localParams.Add(position);
            localParams.Add(methodId);
            localParams.Add(overload);
            localParams.Add(defaulted);
            localParams.Add(objectId);
            localParams.Add(inOut);

            // Build command for server database
            var serverSQL = "SELECT lower(a.argument_name), " +
                            "       CASE data_type " +
                            "         WHEN 'PL/SQL RECORD'  THEN lower(a.type_owner || '.' || a.type_name || '.' || a.type_subname) " +
                            "         WHEN 'PL/SQL TABLE'   THEN lower(a.type_owner || '.' || a.type_name || '.' || a.type_subname) " +
                            "         WHEN 'PL/SQL BOOLEAN' THEN 'BOOLEAN' " +
                            "         WHEN 'BINARY_INTEGER' THEN 'PLS_INTEGER' " +
                            "         WHEN 'CHAR'           THEN  'VARCHAR2' " +
                            "         ELSE  a.data_type " +
                            "       END data_type, " +
                            "       a.position, " +
                            "       a.subprogram_id method_id, " +
                            "       nvl(a.overload, '0') overload, " +
                            "       a.defaulted, " +
                            "       a.object_id, " +
                            "       a.in_out " +
                            "  FROM dba_arguments a, dba_objects o " +
                            " WHERE data_level = 0 " +
                            "   AND subprogram_id > 0 " +
                            "   AND a.object_id = o.object_id " +
                            "   AND o.object_type = 'PACKAGE' ";

            var oracleCmd = new OracleCommand(serverSQL, ServerConnection);
            var executeReader = oracleCmd.ExecuteReader();

            // Transfer the Data
            try
            {

                while (executeReader.Read())
                {
                    name.Value = executeReader.GetValue(0);
                    datatype.Value = executeReader.GetValue(1);
                    position.Value = executeReader.GetValue(2);
                    methodId.Value = executeReader.GetValue(3);
                    overload.Value = executeReader.GetValue(4);
                    defaulted.Value = executeReader.GetValue(5);
                    objectId.Value = executeReader.GetValue(6);
                    inOut.Value = executeReader.GetValue(7);

                    localCommand.ExecuteNonQuery();
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.ToString());
            }

        }
        public void load()
        {

            LoadPackages();
            /*
            LoadMethods();
            LoadArguments();
            CompleteData();
            */
            Debug.WriteLine(LastSync);

            UpdateLastSync();

        }

        private void UpdateLastSync()
        {
            SqlCeCommand localCommand;

            if (LastSync == null)
            {
                localCommand = new SqlCeCommand("INSERT INTO Properties ([LastSync]) VALUES (@LastSync)", LocalConnection);
            }
            else
            {
                localCommand = new SqlCeCommand("UPDATE Properties SET [LastSync] = @LastSync", LocalConnection);
            }

            var lastSyncPrameter = new SqlCeParameter("LastSync", DateTime.Now);
            localCommand.Parameters.Add(lastSyncPrameter);
            localCommand.ExecuteNonQuery();
            _lastSync = (DateTime)lastSyncPrameter.Value;

        }

        private void CompleteData()
        {
            var localSQL = "SELECT a.ObjectId, a.SubprogramId, a.Overload, a.Datatype " +
                           "   FROM Arguments AS a " +
                           "  INNER JOIN Methods AS m " +
                           "     ON a.ObjectId = m.ObjectId " +
                           "    AND a.SubprogramId = m.SubprogramId " +
                           "    AND a.Overload = m.Overload " +
                           "  WHERE (a.Position = 0)";

            var queryCommand = new SqlCeCommand(localSQL, LocalConnection);
            var updateCommand = new SqlCeCommand("UPDATE Methods SET Datatype = @Datatype " +
                                                 "  WHERE ObjectId     = @ObjectId " +
                                                 "    AND SubprogramId = @SubprogramId " +
                                                 "    AND Overload   = @Overload"
                                                 , LocalConnection);


            var datatype = new SqlCeParameter("Datatype", SqlDbType.NVarChar);
            var objectId = new SqlCeParameter("ObjectId", SqlDbType.Int);
            var subprogramId = new SqlCeParameter("SubprogramId", SqlDbType.Int);
            var overload = new SqlCeParameter("Overload", SqlDbType.NVarChar);

            var localParams = updateCommand.Parameters;

            localParams.Add(datatype);
            localParams.Add(objectId);
            localParams.Add(subprogramId);
            localParams.Add(overload);



            // Transfer the Data
            var executeReader = queryCommand.ExecuteReader();
            try
            {

                while (executeReader.Read())
                {
                    objectId.Value = executeReader.GetValue(0);
                    subprogramId.Value = executeReader.GetValue(1);
                    overload.Value = executeReader.GetValue(2);
                    datatype.Value = executeReader.GetValue(3);

                    updateCommand.ExecuteNonQuery();
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.ToString());
            }

            queryCommand.ExecuteNonQuery();
        }


        ~DataLoader()
        {
            if (_serverConnection != null && _serverConnection.State == ConnectionState.Open) _serverConnection.Close();
            if (_serverConnection != null && _serverConnection.State == ConnectionState.Open) _serverConnection.Close();

        }
    }

}
