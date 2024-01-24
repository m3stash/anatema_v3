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
            [ObjectID] INTEGER NOT NULL,
            [Dropables] BOOLEAN NOT NULL,
            [Consumable] BOOLEAN NOT NULL,
            [Craftable] BOOLEAN NOT NULL,
            [Max] INTEGER NOT NULL,
            [Weight] REAL NOT NULL,
            [SizeX] INTEGER NOT NULL,
            [SizeY] INTEGER NOT NULL,
            [Icon] TEXT NOT NULL,
            [Category] TEXT NOT NULL,
            [Description] TEXT NOT NULL,
            FOREIGN KEY (ObjectID) REFERENCES {elementTableName}(id)
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
                int sizeX = dbreader.GetInt32(7);
                int sizeY = dbreader.GetInt32(8);
                string icon = dbreader.GetString(9);
                string category = dbreader.GetString(10);
                string description = dbreader.GetString(11);
                Debug.Log($"ID: {id}, ObjectId: {elementId}, Dropables: {dropables}, Consumable: {consumable}, Craftable: {craftable}, Max: {max}, Weight: {weight}, SizeX: {sizeX}, SizeY: {sizeY}, Icon: {icon}, Category: {category}, Description: {description}");
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
        int sizeX, 
        int sizeY, 
        string icon, 
        string category, 
        string description) {

        int lastInsertedId = -1;

        try {
            using IDbCommand dbcmd = dbconn.CreateCommand();
            dbcmd.CommandText = $"INSERT INTO {tableName} (ObjectID, Dropables, Consumable, Craftable, Max, Weight, SizeX, SizeY, Icon, Category, Description) " +
                "VALUES (@ObjectID, @Dropables, @Consumable, @Craftable, @Max, @Weight, @SizeX, @SizeY, @Icon, @Category, @Description); " +
                "SELECT last_insert_rowid() AS new_id;";
            dbManager.AddParameter(dbcmd, "@ObjectID", elementId);
            dbManager.AddParameter(dbcmd, "@Dropables", dropables);
            dbManager.AddParameter(dbcmd, "@Consumable", consumable);
            dbManager.AddParameter(dbcmd, "@Craftable", craftable);
            dbManager.AddParameter(dbcmd, "@Max", max);
            dbManager.AddParameter(dbcmd, "@Weight", weight);
            dbManager.AddParameter(dbcmd, "@SizeX", sizeX);
            dbManager.AddParameter(dbcmd, "@SizeY", sizeY);
            dbManager.AddParameter(dbcmd, "@Icon", icon);
            dbManager.AddParameter(dbcmd, "@Category", category);
            dbManager.AddParameter(dbcmd, "@Description", description);

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
                        int sizeX = dbreader.GetInt32(7);
                        int sizeY = dbreader.GetInt32(8);
                        string icon = dbreader.GetString(9);
                        string category = dbreader.GetString(10);
                        string description = dbreader.GetString(11);
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
