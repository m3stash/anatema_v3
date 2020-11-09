using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour {

    Vector2 worldSize = new Vector2(4, 4);
    RoomModel[,] rooms;
    List<Vector2> takenPositions = new List<Vector2>();
    int gridSizeX, gridSizeY, numberOfRooms = 20;
    public GameObject roomWhiteObj;
    public Transform mapRoot;

    void Start() {
        if (numberOfRooms >= (worldSize.x * 2) * (worldSize.y * 2)) {
            numberOfRooms = Mathf.RoundToInt((worldSize.x * 2) * (worldSize.y * 2));
        }
        gridSizeX = Mathf.RoundToInt(worldSize.x);
        gridSizeY = Mathf.RoundToInt(worldSize.y);
        Generate();
        CreateRooms();
        SetRoomNeighboorsDoors();
    }

    void Generate() {
        rooms = new RoomModel[gridSizeX * 2, gridSizeY * 2];
        // creation of first room
        rooms = new RoomModel[gridSizeX * 2, gridSizeY * 2];
        rooms[gridSizeX, gridSizeY] = new RoomModel(Vector2.zero, 1);
        takenPositions.Insert(0, Vector2.zero);
        Vector2 checkPos;
        //magic numbers
        float randomCompare = 0.2f, randomCompareStart = 0.2f, randomCompareEnd = 0.01f;
        //add rooms
        for (int i = 0; i < numberOfRooms - 1; i++) {
            float randomPerc = i / (float)numberOfRooms - 1;
            randomCompare = Mathf.Lerp(randomCompareStart, randomCompareEnd, randomPerc);
            checkPos = NewPosition();
            //test new position
            if (NumberOfNeighbors(checkPos, takenPositions) > 1 && Random.value > randomCompare) {
                int iterations = 0;
                do {
                    checkPos = SelectiveNewPosition();
                    iterations++;
                } while (NumberOfNeighbors(checkPos, takenPositions) > 1 && iterations < 100);
                if (iterations >= 50)
                    print("error: could not create with fewer neighbors than : " + NumberOfNeighbors(checkPos, takenPositions));
            }
            //finalize position
            rooms[(int)checkPos.x + gridSizeX, (int)checkPos.y + gridSizeY] = new RoomModel(checkPos, 0);
            takenPositions.Insert(0, checkPos);
        }
    }
    Vector2 NewPosition() {
        int x, y;
        Vector2 checkingPos;
        do {
            int index = Mathf.RoundToInt(Random.value * (takenPositions.Count - 1)); // pick a random room
            x = (int)takenPositions[index].x;//capture its x, y position
            y = (int)takenPositions[index].y;
            // randomize between 0 to 1
            bool UpDown = (Random.value < 0.5f); // randomize vertical or horizontal axis
            bool positive = (Random.value < 0.5f); // Randomize if x or y is positive or negative
            if (UpDown) {
                if (positive) {
                    y += 1;
                } else {
                    y -= 1;
                }
            } else {
                if (positive) {
                    x += 1;
                } else {
                    x -= 1;
                }
            }
            checkingPos = new Vector2(x, y);
        } while (takenPositions.Contains(checkingPos) || x >= gridSizeX || x < -gridSizeX || y >= gridSizeY || y < -gridSizeY); //make sure the position is valid
        return checkingPos;
    }
    Vector2 SelectiveNewPosition() { // method differs from the above in the two commented ways
        int index, inc, x, y;
        Vector2 checkingPos;
        do {
            inc = 0;
            do {
                //instead of getting a room to find an adject empty space, we start with one that only 
                //as one neighbor. This will make it more likely that it returns a room that branches out
                index = Mathf.RoundToInt(Random.value * (takenPositions.Count - 1));
                inc++;
            } while (NumberOfNeighbors(takenPositions[index], takenPositions) > 1 && inc < 100);
            x = (int)takenPositions[index].x;
            y = (int)takenPositions[index].y;
            bool UpDown = (Random.value < 0.5f);
            bool positive = (Random.value < 0.5f);
            if (UpDown) {
                if (positive) {
                    y += 1;
                } else {
                    y -= 1;
                }
            } else {
                if (positive) {
                    x += 1;
                } else {
                    x -= 1;
                }
            }
            checkingPos = new Vector2(x, y);
        } while (takenPositions.Contains(checkingPos) || x >= gridSizeX || x < -gridSizeX || y >= gridSizeY || y < -gridSizeY);
        if (inc >= 100) { // break loop if it takes too long: this loop isnt garuanteed to find solution, which is fine for this
            print("Error: could not find position with only one neighbor");
        }
        return checkingPos;
    }
    int NumberOfNeighbors(Vector2 checkingPos, List<Vector2> usedPositions) {
        int ngb = 0; // start at zero, add 1 for each side there is already a room
        if (usedPositions.Contains(checkingPos + Vector2.right)) {
            ngb++;
        }
        if (usedPositions.Contains(checkingPos + Vector2.left)) {
            ngb++;
        }
        if (usedPositions.Contains(checkingPos + Vector2.up)) {
            ngb++;
        }
        if (usedPositions.Contains(checkingPos + Vector2.down)) {
            ngb++;
        }
        return ngb;
    }

    private void CreateRooms() {
        foreach (RoomModel room in rooms) {
            //skip where there is no room
            if (room == null) {
                continue;
            }
            Room obj = Instantiate((GameObject)Resources.Load("Prefabs/Rooms/Room_1x1"), new Vector3(room.gridPos.x * 61, room.gridPos.y * 31, 0), transform.rotation).GetComponent<Room>();
            obj.Setup(room.gridPos, 0);
            room.room = obj;
        }
    }

    void SetRoomNeighboorsDoors() {
        for (int x = 0; x < ((gridSizeX * 2)); x++) {
            for (int y = 0; y < ((gridSizeY * 2)); y++) {
                if (rooms[x, y] == null) {
                    continue;
                }
                Vector2 gridPosition = new Vector2(x, y);
                if (y - 1 >= 0) { // if not outOfBound
                    rooms[x, y].doorBot = (rooms[x, y - 1] != null);
                    rooms[x, y].room.doorBot = (rooms[x, y - 1] != null);
                }
                if (y + 1 < gridSizeY * 2) { // if not outOfBound
                    rooms[x, y].doorTop = (rooms[x, y + 1] != null);
                    rooms[x, y].room.doorTop = (rooms[x, y + 1] != null);
                }
                if (x - 1 >= 0) { // if not outOfBound
                    rooms[x, y].doorLeft = (rooms[x - 1, y] != null);
                    rooms[x, y].room.doorLeft = (rooms[x - 1, y] != null);
                }
                if (x + 1 < gridSizeX * 2) { // if not outOfBound
                    rooms[x, y].doorRight = (rooms[x + 1, y] != null);
                    rooms[x, y].room.doorRight = (rooms[x + 1, y] != null);
                }
            }
        }
    }
}
