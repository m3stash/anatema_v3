using IDbCommand = System.Data.IDbCommand;
using IDbConnection = System.Data.IDbConnection;
using UnityEngine;
using System.Data;
using System;
using Database;
using System.Collections.Generic;

public class BlockTable {
    private IDbConnection dbconn;
    private readonly string tableName = "block_table";
    private readonly string category = ElementCategoryType.BLOCK.ToString();
    private TableManager tableManager;
    private DatabaseManager dbManager;

    public BlockTable(DatabaseManager dbManager) {
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
            [DisplayName] TEXT NOT NULL,
            [SubCategory] TEXT NOT NULL,
            [Description] TEXT NOT NULL,
            [SpriteName] TEXT NOT NULL,
            [SizeX] INTEGER NOT NULL,
            [SizeY] INTEGER NOT NULL,
            [Biome] TEXT NOT NULL,
            [GroupType] TEXT NOT NULL,
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
                string displayName = dbreader.GetString(2);
                string subCategory = dbreader.GetString(3);
                string description = dbreader.GetString(4);
                string spriteName = dbreader.GetString(5);
                int sizeX = dbreader.GetInt32(6);
                int sizeY = dbreader.GetInt32(7);
                string biome = dbreader.GetString(8);
                string groupType = dbreader.GetString(9);
            }
            Debug.Log("Table read successfully.");
        }
        catch (Exception e) {
            Debug.LogError($"Error reading table ${tableName}: {e.Message}");
        }
    }

    public void Insert(
        int elementId,
        string displayName,
        string subCategory,
        string description,
        string spriteName,
        int sizeX,
        int sizeY,
        string biome,
        string groupType
    ) {
        // int lastInsertedId = -1;
        try {
            using IDbCommand dbcmd = dbconn.CreateCommand();
            dbcmd.CommandText = $"INSERT INTO {tableName} (ElementID, DisplayName, SubCategory, Description, SpriteName, SizeX, SizeY, Biome, GroupType) " +
                "VALUES (@ElementID, @DisplayName, @SubCategory, @Description, @SpriteName, @SizeX, @SizeY, @Biome, @GroupType); " +
                "SELECT last_insert_rowid() AS new_id;";
            dbManager.AddParameter(dbcmd, "@ElementID", elementId);
            dbManager.AddParameter(dbcmd, "@DisplayName", displayName);
            dbManager.AddParameter(dbcmd, "@SubCategory", subCategory);
            dbManager.AddParameter(dbcmd, "@Description", description);
            dbManager.AddParameter(dbcmd, "@SpriteName", spriteName);
            dbManager.AddParameter(dbcmd, "@SizeX", sizeX);
            dbManager.AddParameter(dbcmd, "@SizeY", sizeY);
            dbManager.AddParameter(dbcmd, "@Biome", biome);
            dbManager.AddParameter(dbcmd, "@GroupType", groupType);
            dbcmd.ExecuteNonQuery();
            Debug.Log($"{tableName} inserted successfully.");
            /*using IDataReader reader = dbcmd.ExecuteReader();
            if (reader.Read()) {
                lastInsertedId = Convert.ToInt32(reader["new_id"]);
                Debug.Log($"{tableName} inserted successfully. Last inserted ID: {lastInsertedId}");
            }*/
        }
        catch (Exception e) {
            Debug.LogError($"Error inserting {tableName}: {e.Message}");
            // return lastInsertedId;
        }
        // return lastInsertedId;
    }

    public List<Element> GetElementsByElementId(int idElement) {
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
                    string displayName = dbreader.GetString(2);
                    string subCategory = dbreader.GetString(3);
                    string description = dbreader.GetString(4);
                    string spriteName = dbreader.GetString(5);
                    int sizeX = dbreader.GetInt32(6);
                    int sizeY = dbreader.GetInt32(7);
                    string biome = dbreader.GetString(8);
                    string groupType = dbreader.GetString(9);
                    Element item = new Element(
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
                        groupType
                    );
                    elements.Add(item);
                }
            }
            catch (Exception e) {
                Debug.LogError($"Error reading table: {e.Message}");
            }
        }
        return elements;
    }

    public Block GetBlockById(int idElement) {
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
                        string groupType = dbreader.GetString(9);
                        return new Block(
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
                            groupType
                        );
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
