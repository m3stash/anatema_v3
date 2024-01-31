using IDbCommand = System.Data.IDbCommand;
using IDbConnection = System.Data.IDbConnection;
using UnityEngine;
using System.Data;
using System;
using Database;
public class ElementTable {
    private IDbConnection dbconn;
    private readonly string tableName = "element_table";
    private TableManager tableManager;    
    private DatabaseManager dbManager;

    public ElementTable(DatabaseManager dbManager) {
        this.dbManager = dbManager;
        dbconn = dbManager.GetConnection();
        tableManager = dbManager.GetTableManager();
        CreateTable();
    }

    public void CreateTable() {
        string sqlQuery = $@"CREATE TABLE IF NOT EXISTS {tableName} (
            [id] INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
            [Category] TEXT NOT NULL
        )";
        tableManager.CreateTable(tableName, sqlQuery, dbconn);
    }

    public string GetTableName(){
        return tableName;
    }

    public void Read() {
        try {
            using IDbCommand dbcmd = dbconn.CreateCommand();
            dbcmd.CommandText = $"SELECT * FROM {tableName}";

            using IDataReader dbreader = dbcmd.ExecuteReader();
            while (dbreader.Read()) {
                int id = dbreader.GetInt32(0);
                string category = dbreader.GetString(1);
            }
            Debug.Log("Table read successfully.");
        }
        catch (Exception e) {
            Debug.LogError($"Error reading table: {e.Message}");
        }
    }

    public void Insert(
            string category
        ) {
        try {
            using IDbCommand dbcmd = dbconn.CreateCommand();
            dbcmd.CommandText = $"INSERT INTO {tableName} (Category) "+ 
                "VALUES (@Category)";
            dbManager.AddParameter(dbcmd, "@Category", category);
            dbcmd.ExecuteNonQuery();
            Debug.Log("Item inserted successfully.");
        } catch (Exception e) {
            Debug.LogError($"Error inserting item: {e.Message}");
        }
    }

    public int GetIdByType(string category) {
        try {
            using IDbCommand dbcmd = dbconn.CreateCommand();
            dbcmd.CommandText = $"SELECT id FROM {tableName} WHERE Category = @Category";
            dbManager.AddParameter(dbcmd, "@Category", category);

            using IDataReader dbreader = dbcmd.ExecuteReader();
            if (dbreader.Read()) {
                return dbreader.GetInt32(0);
            } else {
                return -1;
            }
        } catch (Exception e) {
            Debug.LogError($"Error getting ID by type: {e.Message}");
            return -1;
        }
    }

}