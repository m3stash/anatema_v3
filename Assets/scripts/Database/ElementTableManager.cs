using System;
using System.Collections.Generic;
using UnityEngine;

public class ElementTableManager {

    private ElementTable elementTable;
    private ItemTableManager itemTableManager;
    private BlockTableManager blockTableManager;
    private EntityTableManager entityTableManager;
    private DatabaseManager dbManager;

    public ElementTableManager(DatabaseManager dbManager) {
        elementTable = new ElementTable(dbManager);
        string elementTableName = elementTable.GetTableName();
        CreateTables(dbManager, elementTableName);
    }

    public ElementTable GetElementTable() {
        return elementTable;
    }

    private void CreateTables(DatabaseManager dbManager, string elementTableName) {
        CreateItemTable(dbManager, elementTableName);
        CreateBlockTable(dbManager, elementTableName);
        CreateEntityTable(dbManager, elementTableName);
    }

    private void CreateItemTable(DatabaseManager dbManager, string elementTableName) {
        itemTableManager = new ItemTableManager(dbManager, elementTableName);
    }

    private void CreateBlockTable(DatabaseManager dbManager, string elementTableName) {
        blockTableManager = new BlockTableManager(dbManager, elementTableName);
    }

    private void CreateEntityTable(DatabaseManager dbManager, string elementTableName) {
        entityTableManager = new EntityTableManager(dbManager, elementTableName);
    }

    public List<Element> GetElementsByIdList(List<int> elementIdList) {
        return elementTable.GetElementsByIds(elementIdList);
    }

    /*private List<Element> CallTableByCategoryAndElementID(string category, int elementID) {
        switch (category) {
            case "ITEM":
                ItemTable itemTable = itemTableManager.GetItemTable();
                //return itemTable.GetElementsByElementId(elementID);
                return null;
            case "BLOCK":
                BlockTable blockTable = blockTableManager.GetBlockTable();
                // return blockTable.GetElementsByElementId(elementID);
                return null;
            case "ENTITY":
                EntityTable entityTable = entityTableManager.GetEntityTable();
                // return entityTable.GetElementsByElementId(elementID);
                return null;
            default:
                Debug.Log($"Category {category} not managed.");
                return null;
        }
    }*/

    /*public List<Element> GetAllElementsByElementIdAndID(List<Tuple<int, int>> idsList) {
        return elementTable.GetAllElementsByElementId(idsList, itemTableManager, blockTableManager, entityTableManager);
    }*/

    /*public List<Element> GetElementsByCategory(string category) {
        Debug.LogWarning("voir pour récupérer la liste en dur depuis le service !!!");
        int elementID = elementTable.GetIdByType(category);
        // return CallTableByCategoryAndElementID(category, elementID);
        return null;
    }*/

    /*public List<string> GetCategories() {
        return elementTable.GetCategories();
    }*/

}
