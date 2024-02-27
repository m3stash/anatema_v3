using UnityEngine;

public class BlockTableManager {

    private BlockTable table;

    public BlockTableManager(DatabaseManager dbManager, string elementTableName) {
        Setup(dbManager, elementTableName);
    }

    private void Setup(DatabaseManager dbManager, string elementTableName) {
        if(dbManager == null) {
            Debug.LogError("DatabaseManager Serialisable is not set.");
        } else if(elementTableName == null){
            Debug.LogError("Element table name is not set.");
        } else {
            SetupBlockDb(dbManager, elementTableName);
        }
    }

    private void SetupBlockDb(DatabaseManager dbManager, string elementTableName) {
        table = new BlockTable(dbManager);
        table.CreateTable(elementTableName);
        string tableName = table.GetTableName();
        if(tableName == null) {
            Debug.LogError("Block table name is not set.");
        }else{
            //
        }
    }

    public BlockTable GetBlockTable() {
        return table;
    }

}