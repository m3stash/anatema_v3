using UnityEngine;

public class BlockTableManager {

    private BlockTable blockTable;

    public BlockTableManager(DatabaseManager dbManager, string elementTableName) {
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
        blockTable = new BlockTable(dbManager);
        blockTable.CreateTable(elementTableName);
        string blockTableName = blockTable.GetTableName();
        if(blockTable == null) {
            Debug.LogError("Item table name is not set.");
        }else{
            //
        }
    }

    public BlockTable GetItemTable() {
        return blockTable;
    }

}