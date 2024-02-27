using UnityEngine;

public class ItemTableManager {

    private ItemTable table;
    private PotionTable potionTable;

    public ItemTableManager(DatabaseManager dbManager, string elementTableName) {
        Setup(dbManager, elementTableName);
    }

    private void Setup(DatabaseManager dbManager, string elementTableName) {
        if (dbManager == null) {
            Debug.LogError("DatabaseManager Serialisable is not set.");
        }
        else if (elementTableName == null) {
            Debug.LogError("Element table name is not set.");
        }
        else {
            SetupItemDb(dbManager, elementTableName);
        }
    }

    private void SetupItemDb(DatabaseManager dbManager, string elementTableName) {
        table = new ItemTable(dbManager);
        table.CreateTable(elementTableName);
        string tableName = table.GetTableName();
        if (tableName == null) {
            Debug.LogError("Item table name is not set.");
        }
        else {
            CreatePotionTable(dbManager, elementTableName);
        }
    }

    private void CreatePotionTable(DatabaseManager dbManager, string elementTableName) {
        potionTable = new PotionTable(dbManager);
        potionTable.CreateTable(elementTableName);
    }

    public ItemTable GetItemTable() {
        return table;
    }

    public PotionTable GetPotionTable() {
        return potionTable;
    }

}