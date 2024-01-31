using UnityEngine;

public class ItemTableManager {

    private ItemTable itemTable;
    private PotionTable potionTable;

    public ItemTableManager(DatabaseManager dbManager, string elementTableName) {
        Setup(dbManager, elementTableName);
    }

    private void Setup(DatabaseManager dbManager, string elementTableName) {
        if(dbManager == null) {
            Debug.LogError("DatabaseManager Serialisable is not set.");
        } else if(elementTableName == null){
            Debug.LogError("Element table name is not set.");
        } else {
            SetupItemDb(dbManager, elementTableName);
        }
    }

    private void SetupItemDb(DatabaseManager dbManager, string elementTableName) {
        itemTable = new ItemTable(dbManager);
        itemTable.CreateTable(elementTableName);
        string itemTableName = itemTable.GetTableName();
        if(itemTableName == null) {
            Debug.LogError("Item table name is not set.");
        }else{
            CreatePotionTable(dbManager, itemTableName);
        }
        
    }

    private void CreatePotionTable(DatabaseManager dbManager, string itemTableName){
        potionTable = new PotionTable(dbManager);
        potionTable.CreateTable(itemTableName);
    }

    public ItemTable GetItemTable() {
        return itemTable;
    }

    public PotionTable GetPotionTable() {
        return potionTable;
    }

}