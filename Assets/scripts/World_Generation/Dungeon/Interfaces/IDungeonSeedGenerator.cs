using DungeonNs;

public interface IDungeonSeedGenerator {
    public int GenerateNumberRoomPerFloor(string seed, int currentFloor);
    public int GetSeedHash(string seed);
    public string GetNewSeed(int length);
}

