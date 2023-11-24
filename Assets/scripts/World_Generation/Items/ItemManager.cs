using DungeonNs;

public class ItemManager: IITemManager {

    private static ItemManager instance;

    public static ItemManager GetInstance() {
        instance ??= new ItemManager();
        return instance;
    }

    private ItemManager() {

    }

    public void GenerateItems() {

    }

    private void CreateKey() {

    }

    private void CreateChest() {

    }

    private void CreateOrb() {

    }

    private void CreateBombs() {

    }

}

