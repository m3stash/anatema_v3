using IDbCommand = System.Data.IDbCommand;
using IDbConnection = System.Data.IDbConnection;
using UnityEngine;
using System.Data;
using System;
using Database;
using System.Collections.Generic;

public class ItemTable {
    private IDbConnection dbconn;
    private readonly string tableName = "item_table";
    private TableManager tableManager;
    private DatabaseManager dbManager;
    private readonly string category = ElementCategoryType.ITEM.ToString();

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
        bool craftable
    ) {
        int lastInsertedId = -1;
        try {
            using IDbCommand dbcmd = dbconn.CreateCommand();
            dbcmd.CommandText = $"INSERT INTO {tableName} (ElementID, Dropable, Consumable, Craftable) " +
                "VALUES (@ElementID, @Dropable, @Consumable, @Craftable); " +
                "SELECT last_insert_rowid() AS new_id;";
            dbManager.AddParameter(dbcmd, "@ElementID", elementId);
            dbManager.AddParameter(dbcmd, "@Dropable", dropables);
            dbManager.AddParameter(dbcmd, "@Consumable", consumable);
            dbManager.AddParameter(dbcmd, "@Craftable", craftable);
            dbcmd.ExecuteNonQuery();
            Debug.Log($"{tableName} inserted successfully.");
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

    /*public List<Element> GetElementsByElementId(int idElement) {
        List<Element> elements = new List<Element>();

        using (IDbCommand dbcmd = dbconn.CreateCommand()) {
            dbcmd.CommandText = $@"
                SELECT *
                FROM {tableName}
                WHERE ElementID = @ElementID";
            dbManager.AddParameter(dbcmd, "@ElementID", idElement);

            try {
                using IDataReader dbreader = dbcmd.ExecuteReader();
                while (dbreader.Read()) {
                    int id = dbreader.GetInt32(0);
                    int elementID = dbreader.GetInt32(1);
                    bool dropable = dbreader.GetBoolean(9);
                    bool consumable = dbreader.GetBoolean(10);
                    bool craftable = dbreader.GetBoolean(11);
                    Element elt = new Element(
                        elementID,
                        id,
                        category
                    );
                    elements.Add(elt);
                }
            }
            catch (Exception e) {
                Debug.LogError($"Error reading table: {e.Message}");
            }
        }
        return elements;
    }*/

    /*public Item GetItemById(int idElement) {
        try {
            using (IDbCommand dbcmd = dbconn.CreateCommand()) {
                dbcmd.CommandText = $"SELECT * FROM {tableName} WHERE id = @ElementID";
                dbManager.AddParameter(dbcmd, "@ElementID", idElement);

                using (IDataReader dbreader = dbcmd.ExecuteReader()) {
                    if (dbreader.Read()) {
                        int id = dbreader.GetInt32(0);
                        int elementID = dbreader.GetInt32(1);
                        string displayName = dbreader.GetString(2);
                        string subCategory = dbreader.GetString(3);
                        string description = dbreader.GetString(4);
                        string spriteName = dbreader.GetString(5);
                        int sizeX = dbreader.GetInt32(6);
                        int sizeY = dbreader.GetInt32(7);
                        string biome = dbreader.GetString(8);
                        bool dropable = dbreader.GetBoolean(9);
                        bool consumable = dbreader.GetBoolean(10);
                        bool craftable = dbreader.GetBoolean(11);
                        string groupType = dbreader.GetString(12);
                        return new Item(
                            elementID,
                            id,
                            category,
                            displayName,
                            subCategory,
                            description,
                            spriteName,
                            sizeX,
                            sizeY,
                            biome,
                            groupType,
                            dropable,
                            consumable,
                            craftable
                        );
                    }
                }
            }
        }
        catch (Exception e) {
            Debug.LogError($"Error getting item by ID: {e.Message}");
        }
        return null;
    }*/

}
