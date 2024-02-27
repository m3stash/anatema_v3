using UnityEngine.UIElements;

public class RoomUITableManager {

    private ElementTable elementTable;
    private ItemTableManager itemTableManager;
    private BlockTableManager blockTableManager;
    RoomUITableManager() {
        Setup();
    }

    private void Setup() {
        DatabaseManager dbManager = new DatabaseManager();
        elementTable = new ElementTable(dbManager);
        string elementTableName = elementTable.GetTableName();
        itemTableManager = new ItemTableManager(dbManager, elementTableName); 
        blockTableManager = new BlockTableManager(dbManager, elementTableName);
    }

}
