using IDbCommand = System.Data.IDbCommand;
using IDbConnection = System.Data.IDbConnection;
using UnityEngine;
using System.Data;
using System;
using Database;

public class ItemTable {
    private IDbConnection dbconn;
    private readonly string tableName = "item_table";
    private TableManager tableManager;
    private DatabaseManager dbManager;

    public ItemTable(DatabaseManager dbManager) {
        dbconn = dbManager.GetConnection(tableName);
        tableManager = dbManager.GetTableManager();
    }

    public void CreateTable() {
       string sqlQuery = $"CREATE TABLE IF NOT EXISTS {tableName} (" +
            "[id] INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT," +
            "[ObjectID] INTEGER NOT NULL," +
            "[Dropables] BOOLEAN NOT NULL," +
            "[Consumable] BOOLEAN NOT NULL," +
            "[Craftable] BOOLEAN NOT NULL," +
            "[Limit] INTEGER NOT NULL," +
            "[Weight] REAL NOT NULL," +
            "[SizeX] INTEGER NOT NULL," +
            "[SizeY] INTEGER NOT NULL," +
            "[Icon] TEXT NOT NULL," +
            "[Category] TEXT NOT NULL," +
            "[Description] TEXT NOT NULL)";
        tableManager.CreateTable(tableName, sqlQuery, dbconn);
    }

    public void ReadTable() {
        try {
            using (IDbCommand dbcmd = dbconn.CreateCommand()) {
                dbcmd.CommandText = $"SELECT * FROM {tableName}";
                using (IDataReader dbreader = dbcmd.ExecuteReader()) {
                    while (dbreader.Read()) {
                        int id = dbreader.GetInt32(0);
                        int objectId = dbreader.GetInt32(1);
                        bool dropables = dbreader.GetBoolean(2);
                        bool consumable = dbreader.GetBoolean(3);
                        bool craftable = dbreader.GetBoolean(4);
                        int limit = dbreader.GetInt32(5);
                        float weight = dbreader.GetFloat(6);
                        int sizeX = dbreader.GetInt32(7);
                        int sizeY = dbreader.GetInt32(8);
                        string icon = dbreader.GetString(9);
                        string category = dbreader.GetString(10);
                        string description = dbreader.GetString(11);
                        Debug.Log($"ID: {id}, ObjectId: {objectId}, Dropables: {dropables}, Consumable: {consumable}, Craftable: {craftable}, Limit: {limit}, Weight: {weight}, SizeX: {sizeX}, SizeY: {sizeY}, Icon: {icon}, Category: {category}, Description: {description}");
                    }
                    Debug.Log("Table read successfully.");
                }
            }
        }
        catch (Exception e) {
            Debug.LogError($"Error reading table ${tableName}: {e.Message}");
        }
    }

    public void InsertItem(
        int objectId,
        bool dropables, 
        bool consumable, 
        bool craftable, 
        int limit, 
        float weight, 
        int sizeX, 
        int sizeY, 
        string icon, 
        string category, 
        string description) {

        try {
            dbconn.Open();
            IDbCommand dbcmd = dbconn.CreateCommand();
            dbcmd.CommandText = $"INSERT INTO {tableName} (ObjectID, Dropables, Consumable, Craftable, Limit, Weight, SizeX, SizeY, Icon, Category, Description) " +
                "VALUES (@ObjectID, @Dropables, @Consumable, @Craftable, @Limit, @Weight, @SizeX, @SizeY, @Icon, @Category, @Description)";
            dbManager.AddParameter(dbcmd, "@ObjectID", objectId);
            dbManager.AddParameter(dbcmd, "@Dropables", dropables);
            dbManager.AddParameter(dbcmd, "@Consumable", consumable);
            dbManager.AddParameter(dbcmd, "@Craftable", craftable);
            dbManager.AddParameter(dbcmd, "@Limit", limit);
            dbManager.AddParameter(dbcmd, "@Weight", weight);
            dbManager.AddParameter(dbcmd, "@SizeX", sizeX);
            dbManager.AddParameter(dbcmd, "@SizeY", sizeY);
            dbManager.AddParameter(dbcmd, "@Icon", icon);
            dbManager.AddParameter(dbcmd, "@Category", category);
            dbManager.AddParameter(dbcmd, "@Description", description);

            dbcmd.ExecuteNonQuery();
            Debug.Log($"{tableName} inserted successfully.");
        }
        catch (Exception e) {
            Debug.LogError($"Error inserting {tableName}: {e.Message}");
        }
        finally {
            if (dbconn.State == ConnectionState.Open) {
                dbconn.Close();
            }
        }
    }


}
