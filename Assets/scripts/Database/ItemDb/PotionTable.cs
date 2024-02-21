using IDbCommand = System.Data.IDbCommand;
using IDbConnection = System.Data.IDbConnection;
using UnityEngine;
using System.Data;
using System;
using Database;

public class PotionTable {
    private IDbConnection dbconn;
    private readonly string tableName = "potion_table";
    private TableManager tableManager;
    private DatabaseManager dbManager;

    public PotionTable(DatabaseManager dbManager) {
        this.dbManager = dbManager;
        dbconn = dbManager.GetConnection();
        tableManager = dbManager.GetTableManager();
    }

    public void CreateTable(string elementTableName) {
        string sqlQuery = $@"CREATE TABLE IF NOT EXISTS {tableName} (
            [id] INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
            [ElementID] INTEGER NOT NULL,
            FOREIGN KEY (ElementID) REFERENCES {elementTableName}(id)
        )";
        tableManager.CreateTable(tableName, sqlQuery, dbconn);
    }

    public void Read() {
        try {
            using IDbCommand dbcmd = dbconn.CreateCommand();
            dbcmd.CommandText = $"SELECT * FROM {tableName}";
            using IDataReader dbreader = dbcmd.ExecuteReader();
            while (dbreader.Read()) {
                int id = dbreader.GetInt32(0);
                int itemId = dbreader.GetInt32(1);
                Debug.Log($"ID: {id}, ItemID: {itemId}");
            }
            Debug.Log("Table read successfully.");
        }
        catch (Exception e) {
            Debug.LogError($"Error reading table: {e.Message}");
        }
    }

    public void Insert(int itemId) {
        try {
            using IDbCommand dbcmd = dbconn.CreateCommand();
            dbcmd.CommandText = $"INSERT INTO {tableName} (ItemID) " +
                "VALUES (@ItemID)";
            dbManager.AddParameter(dbcmd, "@ItemID", itemId);
            dbcmd.ExecuteNonQuery();
            Debug.Log($"{tableName} inserted successfully.");
        }
        catch (Exception e) {
            Debug.LogError($"Error inserting {tableName}: {e.Message}");
        }
    }


}