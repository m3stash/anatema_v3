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
        this.dbManager = dbManager;
        dbconn = dbManager.GetConnection();
        tableManager = dbManager.GetTableManager();
    }

    public string GetTableName() {
        return tableName;
    }

    public void CreateTable(string elementTableName) {
        string sqlQuery = $@"CREATE TABLE IF NOT EXISTS {tableName} (
            [id] INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
            [ElementID] INTEGER NOT NULL,
            [Dropable] BOOLEAN NOT NULL,
            [Consumable] BOOLEAN NOT NULL,
            [Craftable] BOOLEAN NOT NULL,
            [Max] INTEGER NOT NULL,
            [Weight] REAL NOT NULL,
            [SubCategory] TEXT NOT NULL,
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
                int elementId = dbreader.GetInt32(1);
                bool dropables = dbreader.GetBoolean(2);
                bool consumable = dbreader.GetBoolean(3);
                bool craftable = dbreader.GetBoolean(4);
                int max = dbreader.GetInt32(5);
                float weight = dbreader.GetFloat(6);
                string subCategory = dbreader.GetString(7);
                Debug.Log($"ID: {id}, ElementId: {elementId}, Dropables: {dropables}, Consumable: {consumable}, Craftable: {craftable}, Max: {max}, Weight: {weight}, SubCategory: {subCategory}");
            }
            Debug.Log("Table read successfully.");

        }
        catch (Exception e) {
            Debug.LogError($"Error reading table ${tableName}: {e.Message}");
        }
    }

    public int Insert(
        int elementId,
        bool dropables, 
        bool consumable, 
        bool craftable, 
        int max, 
        float weight, 
        string subCategory) {

        int lastInsertedId = -1;

        try {
            using IDbCommand dbcmd = dbconn.CreateCommand();
            dbcmd.CommandText = $"INSERT INTO {tableName} (ElementID, Dropables, Consumable, Craftable, Max, Weight, SubCategory) " +
                "VALUES (@ElementID, @Dropables, @Consumable, @Craftable, @Max, @Weight, @SubCategory); " +
                "SELECT last_insert_rowid() AS new_id;";
            dbManager.AddParameter(dbcmd, "@ElementID", elementId);
            dbManager.AddParameter(dbcmd, "@Dropables", dropables);
            dbManager.AddParameter(dbcmd, "@Consumable", consumable);
            dbManager.AddParameter(dbcmd, "@Craftable", craftable);
            dbManager.AddParameter(dbcmd, "@Max", max);
            dbManager.AddParameter(dbcmd, "@Weight", weight);
            dbManager.AddParameter(dbcmd, "@SubCategory", subCategory);

            /*dbcmd.ExecuteNonQuery();
            Debug.Log($"{tableName} inserted successfully.");*/
            using IDataReader reader = dbcmd.ExecuteReader();
            if (reader.Read()) {
                lastInsertedId = Convert.ToInt32(reader["new_id"]);
                Debug.Log($"{tableName} inserted successfully. Last inserted ID: {lastInsertedId}");
            }
        }
        catch (Exception e) {
            Debug.LogError($"Error inserting {tableName}: {e.Message}");
            return lastInsertedId;
        }
        return lastInsertedId;
    }

    public Item GetItemById(int itemId) {
        try {
            using (IDbCommand dbcmd = dbconn.CreateCommand()) {
                dbcmd.CommandText = $"SELECT * FROM {tableName} WHERE id = @ItemId";
                dbManager.AddParameter(dbcmd, "@ItemId", itemId);

                using (IDataReader dbreader = dbcmd.ExecuteReader()) {
                    if (dbreader.Read()) {
                        int id = dbreader.GetInt32(0);
                        int elementId = dbreader.GetInt32(1);
                        bool dropables = dbreader.GetBoolean(2);
                        bool consumable = dbreader.GetBoolean(3);
                        bool craftable = dbreader.GetBoolean(4);
                        int max = dbreader.GetInt32(5);
                        float weight = dbreader.GetFloat(6);
                        string subCategory = dbreader.GetString(7);
                        // TODO TOUT REFAIRE !!!
                        return null;
                        //return new Item(id, elementId, dropables, consumable, craftable, max, weight, sizeX, sizeY, icon, category, description);
                    }
                }
            }
        }
        catch (Exception e) {
            Debug.LogError($"Error getting item by ID: {e.Message}");
        }

        return null;
    }


}
