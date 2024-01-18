using IDbCommand = System.Data.IDbCommand;
using IDbConnection = System.Data.IDbConnection;
using Mono.Data.Sqlite;
using UnityEngine;
using System.Data;
using System;
public class DatabaseManager: MonoBehaviour {

    string conn;
    string sqlQuery;
    IDbConnection dbconn;
    IDbCommand dbcmd;
    IDataReader dbreader;
    string DATABASE_NAME = "/objects.s3db";
    void Start() {
        string filepath = Application.persistentDataPath + DATABASE_NAME;
        Debug.Log($"filepath={filepath}");
        conn = "URI=file:" + filepath;
        CreateATable("objects");
        ReadTable("objects");
    }

    private void CreateATable(string tableName) {
        using (dbconn = new SqliteConnection(conn)) {
            try {
                dbconn.Open();
                dbcmd = dbconn.CreateCommand();
                string sqlQuery = $"CREATE TABLE IF NOT EXISTS {tableName} (" +
                                "[id] INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT," +
                                "[name] VARCHAR(255) NOT NULL," +
                                "[age] INTEGER DEFAULT '18' NOT NULL)";
                dbcmd.CommandText = sqlQuery;
                dbcmd.ExecuteScalar();
                Debug.Log("Table created successfully.");
            } catch (Exception e) {
                Debug.LogError($"Error creating table: {e.Message}");
            } finally {
                if (dbconn.State == ConnectionState.Open) {
                    dbconn.Close();
                }
            }
        }
    }

    private void ReadTable(string tableName) {
        using (dbconn = new SqliteConnection(conn)) {
            try {
                dbconn.Open();
                Debug.Log("Database connection opened.");

                dbcmd = dbconn.CreateCommand();
                // Utilisez tableName directement dans la requête SQL
                dbcmd.CommandText = $"SELECT * FROM {tableName}";
                dbreader = dbcmd.ExecuteReader();

                while (dbreader.Read()) {
                    int id = dbreader.GetInt32(0);
                    string name = dbreader.GetString(1);
                    int age = dbreader.GetInt32(2);
                    Debug.Log($"ID: {id}, Name: {name}, Age: {age}");
                }
                Debug.Log("Table read successfully.");
            } catch (Exception e) {
                Debug.LogError($"Error reading table: {e.Message}");
            } finally {
                if (dbreader != null && !dbreader.IsClosed) {
                    dbreader.Close();
                }
                if (dbconn.State == ConnectionState.Open) {
                    dbconn.Close();
                    Debug.Log("Database connection closed.");
                }
            }
        }
    }

}