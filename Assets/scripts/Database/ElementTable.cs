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
            [Category] TEXT NOT NULL,
            [DisplayName] TEXT NOT NULL,
            [SubCategory] TEXT NOT NULL,
            [Description] TEXT NOT NULL,
            [SpriteName] TEXT NOT NULL,
            [SizeX] INTEGER NOT NULL,
            [SizeY] INTEGER NOT NULL,
            [Biome] TEXT NOT NULL,
            [GroupType] TEXT NOT NULL
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
            Debug.LogError($"Error reading table: {e.Message}");
        }
    }

    public void Insert(
            string category,
            string displayName,
            string subCategory,
            string description,
            string spriteName,
            int sizeX,
            int sizeY,
            string biome,
            string groupType
        ) {
        try {
            using IDbCommand dbcmd = dbconn.CreateCommand();
            dbcmd.CommandText = $"INSERT INTO {tableName} (Category, DisplayName, SubCategory, Description, SpriteName, SizeX, SizeY, Biome, GroupType) " +
                "VALUES (@Category, @DisplayName, @SubCategory, @Description, @SpriteName, @SizeX, @SizeY, @Biome, @GroupType); ";
            dbManager.AddParameter(dbcmd, "@Category", category);
            dbManager.AddParameter(dbcmd, "@DisplayName", displayName);
            dbManager.AddParameter(dbcmd, "@SubCategory", subCategory);
            dbManager.AddParameter(dbcmd, "@Description", description);
            dbManager.AddParameter(dbcmd, "@SpriteName", spriteName);
            dbManager.AddParameter(dbcmd, "@SizeX", sizeX);
            dbManager.AddParameter(dbcmd, "@SizeY", sizeY);
            dbManager.AddParameter(dbcmd, "@Biome", biome);
            dbManager.AddParameter(dbcmd, "@GroupType", groupType);
            dbcmd.ExecuteNonQuery();
            Debug.Log("Item inserted successfully.");
        }
        catch (Exception e) {
            Debug.LogError($"Error inserting item: {e.Message}");
        }
    }

    public List<Element> GetElementsByCategory(string Category) {
        try {
            using IDbCommand dbcmd = dbconn.CreateCommand();
            dbcmd.CommandText = $"SELECT * FROM {tableName} WHERE Category = @Category";
            dbManager.AddParameter(dbcmd, "@Category", Category);

            using IDataReader dbreader = dbcmd.ExecuteReader();
            List<Element> elements = new List<Element>();
            while (dbreader.Read()) {
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
                Element elt = new Element(
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
            return elements;
        }
        catch (Exception e) {
            Debug.LogError($"Error getting elements by category: {e.Message}");
            return null;
        }
    }

    public List<Element> GetElementsByIds(List<int> elementIds) {
        List<Element> elements = new List<Element>();
        string sqlQuery = $"SELECT * FROM {tableName} WHERE id IN (";
        sqlQuery += string.Join(",", elementIds);
        sqlQuery += ");";
        try {
            using IDbCommand dbcmd = dbconn.CreateCommand();
            dbcmd.CommandText = sqlQuery;
            using IDataReader dbreader = dbcmd.ExecuteReader();
            while (dbreader.Read()) {
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
                Element elt = new Element(
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
            return elements;
        }
        catch (Exception e) {
            Debug.LogError($"Error reading table: {e.Message}");
            return null;
        }
    }

}