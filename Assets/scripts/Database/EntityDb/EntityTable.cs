using IDbCommand = System.Data.IDbCommand;
using IDbConnection = System.Data.IDbConnection;
using UnityEngine;
using System.Data;
using System;
using Database;

public class EntityTable {
    private IDbConnection dbconn;
    private readonly string tableName = "entity_table";
    private readonly string category = ElementCategoryType.ENTITY.ToString();
    private TableManager tableManager;
    private DatabaseManager dbManager;

    public EntityTable(DatabaseManager dbManager) {
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
            [Life] INTEGER NOT NULL,
            [Type] TEXT NOT NULL,
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
                int life = dbreader.GetInt32(2);
                string type = dbreader.GetString(3);
            }
            Debug.Log("Table read successfully.");
        }
        catch (Exception e) {
            Debug.LogError($"Error reading table ${tableName}: {e.Message}");
        }
    }

    public int Insert(
        int elementId,
        int life,
        string type
    ) {
        int lastInsertedId = -1;
        try {
            using IDbCommand dbcmd = dbconn.CreateCommand();
            dbcmd.CommandText = $"INSERT INTO {tableName} (ElementID, Life, Type) " +
                "VALUES (@ElementID, @Life, @Type); " +
                "SELECT last_insert_rowid() AS new_id;";
            dbManager.AddParameter(dbcmd, "@ElementID", elementId);
            dbManager.AddParameter(dbcmd, "@Life", life);
            dbManager.AddParameter(dbcmd, "@Type", type);
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

    public Entity GetEntityById(int idElement) {
        try {
            using (IDbCommand dbcmd = dbconn.CreateCommand()) {
                dbcmd.CommandText = $"SELECT * FROM {tableName} WHERE id = @ElementID";
                dbManager.AddParameter(dbcmd, "@ElementID", idElement);

                using (IDataReader dbreader = dbcmd.ExecuteReader()) {
                    if (dbreader.Read()) {
                        int id = dbreader.GetInt32(0);
                        string category = dbreader.GetString(1);
                        string displayName = dbreader.GetString(2);
                        string subCategory = dbreader.GetString(3);
                        string description = dbreader.GetString(4);
                        string spriteName = dbreader.GetString(5);
                        int sizeX = dbreader.GetInt32(6);
                        int sizeY = dbreader.GetInt32(7);
                        string biome = dbreader.GetString(8);
                        string groupType = dbreader.GetString(9);
                        string type = dbreader.GetString(10);
                        return new Entity(
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
                            type
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
