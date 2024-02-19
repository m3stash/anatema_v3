using IDbCommand = System.Data.IDbCommand;
using IDbConnection = System.Data.IDbConnection;
using UnityEngine;
using System.Data;
using System;
using Database;
using System.Collections.Generic;
using System.Linq;
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

    public string GetTableName() {
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
            dbcmd.CommandText = $"INSERT INTO {tableName} (Category) " +
                "VALUES (@Category)";
            dbManager.AddParameter(dbcmd, "@Category", category);
            dbcmd.ExecuteNonQuery();
            Debug.Log("Item inserted successfully.");
        }
        catch (Exception e) {
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
            }
            else {
                return -1;
            }
        }
        catch (Exception e) {
            Debug.LogError($"Error getting ID by type: {e.Message}");
            return -1;
        }
    }

    public List<string> GetCategories() {
        try {
            using IDbCommand dbcmd = dbconn.CreateCommand();
            dbcmd.CommandText = $"SELECT Category FROM {tableName}";

            using IDataReader dbreader = dbcmd.ExecuteReader();
            List<string> categories = new List<string>();
            while (dbreader.Read()) {
                categories.Add(dbreader.GetString(0));
            }
            return categories;
        }
        catch (Exception e) {
            Debug.LogError($"Error getting categories: {e.Message}");
            return null;
        }
    }

    public List<Element> GetAllElementsByElementId(List<Tuple<int, int>> idsList, ItemTableManager itemTableManager, BlockTableManager blockTableManager, EntityTableManager entityTableManager) {
        List<Element> elements = new List<Element>();
        using (IDbCommand dbcmd = dbconn.CreateCommand()) {
            string itemTableName = itemTableManager.GetItemTable().GetTableName();
            string blockTableName = blockTableManager.GetBlockTable().GetTableName();
            string entityTableName = entityTableManager.GetEntityTable().GetTableName();
            string paramNames = string.Join(",", idsList.Select((tuple, index) => $"@elementId{index}, @itemId{index}"));

            dbcmd.CommandText = $@"
            SELECT 
                {tableName}.*,
                {itemTableName}.*,
                {blockTableName}.*,
                {entityTableName}.*
            FROM {tableName}
            LEFT JOIN {itemTableName} ON {tableName}.id = {itemTableName}.ElementID AND {itemTableName}.id IN (@id0, @id1, @id2, @id3)
            LEFT JOIN {blockTableName} ON {tableName}.id = {blockTableName}.ElementID AND {blockTableName}.id IN (@id0, @id1, @id2, @id3)
            LEFT JOIN {entityTableName} ON {tableName}.id = {entityTableName}.ElementID AND {entityTableName}.id IN (@id0, @id1, @id2, @id3)
            WHERE {tableName}.id IN (@elementId0, @elementId1, @elementId2, @elementId3)";

            for (int i = 0; i < idsList.Count; i++) {
                var tuple = idsList[i];
                int elementId = tuple.Item1;
                int itemId = tuple.Item2;
                dbManager.AddParameter(dbcmd, $"@elementId{i}", elementId);
                dbManager.AddParameter(dbcmd, $"@id{i}", itemId);
            }
            try {
                using IDataReader dbreader = dbcmd.ExecuteReader();
                while (dbreader.Read()) {
                    int elementID = dbreader.GetInt32(0);
                    string category = dbreader.GetString(1);
                    int id = dbreader.GetInt32(2);
                    //int elementID = dbreader.GetInt32(3); // from child table
                    string displayName = dbreader.GetString(4);
                    string subCategory = dbreader.GetString(5);
                    string description = dbreader.GetString(6);
                    string spriteName = dbreader.GetString(7);
                    int sizeX = dbreader.GetInt32(8);
                    int sizeY = dbreader.GetInt32(9);
                    string biome = dbreader.GetString(10);
                    //bool dropable = dbreader.GetBoolean(11);
                    //bool consumable = dbreader.GetBoolean(12);
                    //bool craftable = dbreader.GetBoolean(13);
                    string groupType = dbreader.GetString(14);
                    Element elt = new Element(
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
                    elements.Add(elt);
                }
            }
            catch (Exception e) {
                Debug.LogError($"Error reading table: {e.Message}");
            }
        }
        return elements;
    }

}