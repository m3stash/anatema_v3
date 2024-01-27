using IDbCommand = System.Data.IDbCommand;
using IDbConnection = System.Data.IDbConnection;
using UnityEngine;
using System.Data;
using System;
using Database;
using Dapper;
using System.Linq;
using System.Collections.Generic;
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
            [DisplayName] TEXT NOT NULL,
            [Category] TEXT NOT NULL,
            [Description] TEXT NOT NULL,
            [IconPath] TEXT NOT NULL,
            [SizeX] INTEGER NOT NULL,
            [SizeY] INTEGER NOT NULL,
            [Biome] TEXT NOT NULL
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
                string displayName = dbreader.GetString(1);
                string category = dbreader.GetString(2);
                string description = dbreader.GetString(3);
                string iconPath = dbreader.GetString(4);
                int sizeX = dbreader.GetInt32(5);
                int sizeY = dbreader.GetInt32(6);
                string biome = dbreader.GetString(7);
                Debug.Log($"ID: {id}, Type: {displayName}, Category: {category}, Description: {description}, IconPath: {iconPath}, SizeX: {sizeX}, SizeY: {sizeY}, Biome: {biome}");
            }
            Debug.Log("Table read successfully.");
        }
        catch (Exception e) {
            Debug.LogError($"Error reading table: {e.Message}");
        }
    }

    public void Insert(
            string displayName,
            string category,
            string description,
            string iconPath,
            int sizeX,
            int sizeY,
            string biome
        ) {
        try {
            using IDbCommand dbcmd = dbconn.CreateCommand();
            dbcmd.CommandText = $"INSERT INTO {tableName} (DisplayName, Category, Description, IconPath, SizeX, SizeY, Biome) "+ 
                "VALUES (@DisplayName, @Category, @Description, @IconPath, @SizeX, @SizeY, @Biome)";
            dbManager.AddParameter(dbcmd, "@DisplayName", displayName);
            dbManager.AddParameter(dbcmd, "@Category", category);
            dbManager.AddParameter(dbcmd, "@Description", description);
            dbManager.AddParameter(dbcmd, "@IconPath", iconPath);
            dbManager.AddParameter(dbcmd, "@SizeX", sizeX);
            dbManager.AddParameter(dbcmd, "@SizeY", sizeY);
            dbManager.AddParameter(dbcmd, "@Biome", biome);
            dbcmd.ExecuteNonQuery();
            Debug.Log("Item inserted successfully.");
        } catch (Exception e) {
            Debug.LogError($"Error inserting item: {e.Message}");
        }
    }

    public int GetIdByType(string type) {
        try {
            using IDbCommand dbcmd = dbconn.CreateCommand();
            dbcmd.CommandText = $"SELECT id FROM {tableName} WHERE Type = @Type";
            dbManager.AddParameter(dbcmd, "@Type", type);

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

    public List<Element> GetElementsWithCategories() {
        var ElementCategoryTypes = new[] { "ITEM"/*, "ENTITY", "BLOCK" */};
        List<Element> allElements = new List<Element>();

        foreach (var ElementCategoryType in ElementCategoryTypes) {
            string categoryTableName = $"{ElementCategoryType.ToLower()}_table";
            
            // get all elements by type
            string mainQuery = $"SELECT * FROM element_table WHERE Type = @ElementCategoryType";
            var mainResult = dbconn.Query<Element>(mainQuery, new { ElementCategoryType = ElementCategoryType }).ToList();
            allElements.AddRange(mainResult);

            // get all categories for elements by type
            string specializedQuery = $"SELECT * FROM {categoryTableName} WHERE ElementID IN @ElementIds";
            var elementIds = mainResult.Select(e => e.Id);
            var specializedResult = dbconn.Query<Element>(specializedQuery, new { ElementIds = elementIds }).ToList();
            allElements.AddRange(specializedResult);

            // Get all sub category by element and type
            foreach (var element in specializedResult) {
                string subTableName = $"{ElementCategoryType.ToLower()}_sub_table";  // Remplace avec le nom correct de la sous-table
                string subQuery = $"SELECT * FROM {subTableName} WHERE {ElementCategoryType}Id = @ElementId";
                var subResult = dbconn.Query<Element>(subQuery, new { ElementId = element.Id }).ToList();
                allElements.AddRange(subResult);
            }
        }

        return allElements;
    }

    public List<Element> GetAll() {
        /*string sqlQuery = @"
            SELECT 
                e.*,
                i.Dropable, i.Consumable, i.Craftable, i.Max, i.Weight, i.SizeX, i.SizeY, i.Icon, i.Category, i.Description,
                p.SubCategory
            FROM element_table e
            LEFT JOIN item_table i ON e.id = i.ElementID AND e.Type = 'ITEM'
            LEFT JOIN potion_table p ON i.id = p.ItemID AND e.Type = 'ITEM'
            WHERE e.Type = @ElementCategoryType";*/

        string sqlQuery = @"
            SELECT 
                e.*,
                i.*,
                p.*
            FROM element_table e
            LEFT JOIN item_table i ON e.id = i.ElementID AND e.Category = 'ITEM'
            LEFT JOIN potion_table p ON i.id = p.ItemID AND e.Category = 'ITEM'
            WHERE e.Category = @ElementCategoryType";

        var ElementCategoryTypes = new[] { "ITEM"/*, "ENTITY", "BLOCK"*/ };

        var result = dbconn.Query<Element, Item, Potion, Element>(
            sqlQuery,
            (element, item, potion) => {
                Debug.Log(item);
                Debug.Log(potion);
                return element;
            },
            splitOn: "id, ElementID, ItemID",
            param: new { ElementCategoryType = "ITEM" }
        );

        return result.ToList();
    }
    public List<Element> GetElementsFromCategory(string category) {
        List<Element> elements = new List<Element>();

        using (IDbCommand dbcmd = dbconn.CreateCommand()) {
            /*
            
            string sqlQuery = $@"
            SELECT 
                e.*,
                i.*,
                p.*,
                c.*  -- Add columns for Container if needed
            FROM element_table e
            LEFT JOIN item_table i ON e.id = i.ElementID AND e.Category = @Category
            LEFT JOIN potion_table p ON i.id = p.ItemID AND e.Category = @Category
            LEFT JOIN container_table c ON i.id = c.ItemID AND e.Category = @Category
            WHERE e.Category = @Category";
            
            using (var connection = dbconn)  // Assume dbconn is your IDbConnection {
                connection.Open();
                var elements = connection.Query(sqlQuery, new { Category = category }).ToList();
                return elements;
            }
            
            */
            dbcmd.CommandText = $@"
                SELECT 
                    e.*,
                    i.*,
                    p.*
                FROM element_table e
                LEFT JOIN item_table i ON e.id = i.ElementID AND e.Category = @Category
                LEFT JOIN potion_table p ON i.id = p.ItemID AND e.Category = @Category
                WHERE e.Category = @Category";

            dbManager.AddParameter(dbcmd, "@Category", category);

            try {
                using (IDataReader dbreader = dbcmd.ExecuteReader()) {
                    while (dbreader.Read()) {
                        string id = dbreader["Id"].ToString();
                        string displayName = dbreader["DisplayName"].ToString();
                        string subCategory = dbreader["SubCategory"]?.ToString();
                        string subCategoryType = dbreader["SubCategoryType"]?.ToString();
                        if(subCategory == null) {
                            Debug.LogError("SubCategory is null");
                        }else{
                            if(subCategory == "POTION") {
                                Element potion = new Potion {
                                    Category = category,
                                    Id = id,
                                    DisplayName = displayName
                                };
                                // elements.Add(new Potion(id, displayName, subCategory));
                            }
                        }
                    }
                }
                Debug.Log("Table read successfully.");
            } catch (Exception e) {
                Debug.LogError($"Error reading table: {e.Message}");
            }
        }
        return elements;
    }

    
}