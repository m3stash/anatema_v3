using IDbCommand = System.Data.IDbCommand;
using IDbConnection = System.Data.IDbConnection;
using Mono.Data.Sqlite;
using UnityEngine;
using System.Data;
using System;
using Database;

public class DatabaseManager {

    private TableManager tableManager;
    private readonly string dbName = "game_db";

    public DatabaseManager() {
        tableManager = new TableManager();
    }

    public TableManager GetTableManager() {
        return tableManager;
    }

    public IDbConnection GetConnection() {
        string filepath = Application.persistentDataPath + "/" + dbName + ".s3db";
        string connString = "URI=file:" + filepath;

        try {
            IDbConnection dbconn = new SqliteConnection(connString);
            dbconn.Open();
            return dbconn;
        }
        catch (Exception e) {
            Debug.LogError($"Error opening database connection: {e.Message}");
            throw;
        }
    }

    public void CloseDbConnection(IDbConnection dbconn) {
        if (dbconn != null && dbconn.State == ConnectionState.Open) {
            dbconn.Close();
            Debug.Log("Database connection closed.");
        }
    }

    public void AddParameter(IDbCommand command, string paramName, object paramValue) {
        IDbDataParameter parameter = command.CreateParameter();
        parameter.ParameterName = paramName;
        parameter.Value = paramValue;
        command.Parameters.Add(parameter);
    }

}
