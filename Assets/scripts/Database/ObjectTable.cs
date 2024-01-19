using IDbCommand = System.Data.IDbCommand;
using IDbConnection = System.Data.IDbConnection;
using UnityEngine;
using System.Data;
using System;
using Database;

public class ObjectTable {
    private IDbConnection dbconn;
    private readonly string tableName = "object_table";
    private TableManager tableManager;    

    private DatabaseManager dbManager;

    public ObjectTable(DatabaseManager dbManager) {
        this.dbManager = dbManager;
        dbconn = dbManager.GetConnection(tableName);
        tableManager = dbManager.GetTableManager();
    }

    public void CreateTable() {
        string sqlQuery = $"CREATE TABLE IF NOT EXISTS {tableName} (" +
            "[id] INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT," +
            "[Type] TEXT NOT NULL)";
        tableManager.CreateTable(tableName, sqlQuery, dbconn);
    }

    public void Read() {
        try {
            using IDbCommand dbcmd = dbconn.CreateCommand();
            dbcmd.CommandText = $"SELECT * FROM {tableName}";

            using IDataReader dbreader = dbcmd.ExecuteReader();
            while (dbreader.Read())
            {
                int id = dbreader.GetInt32(0);
                string type = dbreader.GetString(1);
                Debug.Log($"ID: {id}, Type: {type}");
            }
            Debug.Log("Table read successfully.");
        }
        catch (Exception e) {
            Debug.LogError($"Error reading table: {e.Message}");
        }
    }

    public void Insert(string type) {
        try {
            dbconn.Open();
            using IDbCommand dbcmd = dbconn.CreateCommand();
            dbcmd.CommandText = $"INSERT INTO {tableName} (Type) VALUES (@Type)";
            dbManager.AddParameter(dbcmd, "@Type", type);

            dbcmd.ExecuteNonQuery();
            Debug.Log("Item inserted successfully.");
        }
        catch (Exception e) {
            Debug.LogError($"Error inserting item: {e.Message}");
        }
        finally {
            if (dbconn.State == ConnectionState.Open) {
                dbconn.Close();
            }
        }
    }

    public int GetIdByType(string type) {
        try {
            dbconn.Open();
            IDbCommand dbcmd = dbconn.CreateCommand();
            dbcmd.CommandText = $"SELECT id FROM {tableName} WHERE Type = @Type";
            dbManager.AddParameter(dbcmd, "@Type", type);

            object result = dbcmd.ExecuteScalar();
            if (result != null) {
                return Convert.ToInt32(result);
            } else {
                return -1; // Retourne une valeur spécifique pour indiquer que le type n'a pas été trouvé
            }
        } catch (Exception e) {
            Debug.LogError($"Error getting ID by type: {e.Message}");
            return -1;
        } finally {
            if (dbconn.State == ConnectionState.Open) {
                dbconn.Close();
            }
        }
    }


}