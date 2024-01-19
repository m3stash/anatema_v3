using UnityEngine;

public class ItemTableManager {

    private ItemTable itemTable;
    private PotionTable potionTable;

    public void Setup(DatabaseManager dbManager) {
        if(dbManager == null) {
            Debug.LogError("DatabaseManager Serialisable is not set.");
        } else {
            SetupItemDb(dbManager);
        }
    }

    private void SetupItemDb(DatabaseManager dbManager) {
        itemTable = new ItemTable(dbManager);
        itemTable.CreateTable();
        potionTable = new PotionTable(dbManager);
        potionTable.CreateTable();
    }

    public ItemTable GetItemTable() {
        return itemTable;
    }

    public PotionTable GetPotionTable() {
        return potionTable;
    }

}