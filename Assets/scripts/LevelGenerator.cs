using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour {

    private Vector2Int worldSize = new Vector2Int(4, 4);
    private RoomModel[,] rooms;
    private readonly int max_ROOMSHAPE_2x2 = 1;
    private readonly List<Vector2Int> roomListPositions = new List<Vector2Int>();
    private int gridSizeX, gridSizeY, numberOfRooms = 20;
    private int current_ROOMSHAPE_2x2 = 0;

    private void Start() {
        if (numberOfRooms >= (worldSize.x * 2) * (worldSize.y * 2)) {
            numberOfRooms = Mathf.RoundToInt((worldSize.x * 2) * (worldSize.y * 2));
        }
        gridSizeX = worldSize.x;
        gridSizeY = worldSize.y;
        Generate();
        CreateRooms();
        SetRoomNeighboorsDoors();
    }

    private void Generate() {
        rooms = new RoomModel[gridSizeX * 2, gridSizeY * 2];
        rooms = new RoomModel[gridSizeX * 2, gridSizeY * 2];
        // creation of first room
        rooms[gridSizeX, gridSizeY] = new RoomModel(Vector2Int.zero, 1, RoomShapeEnum.ROOMSHAPE_1x1);
        roomListPositions.Insert(0, Vector2Int.zero);
        Vector2Int newPos;
        float randomCompare = 0.2f;
        //add rooms
        for (int i = 0; i < numberOfRooms - 1; i++) {
            RoomShapeEnum roomToCreate = getTypeOfRoomToCreate();
            newPos = SearchNewPositionForRoomFromEmptySpace(roomToCreate);
            if (GetNumberOfNeighborsByRoomShape(newPos, roomListPositions, roomToCreate) > 1 && Random.value > randomCompare) {
                int iterations = 0;
                do {
                    newPos = SearchNewPositionForRoomWithOneNeighboor(roomToCreate);
                    iterations++;
                } while (GetNumberOfNeighborsByRoomShape(newPos, roomListPositions, roomToCreate) > 1 && iterations < 100);
            }
            InsertNewRoomInListPosition(newPos, roomToCreate);
        }
    }

    private void InsertNewRoomInListPosition(Vector2Int newPos, RoomShapeEnum roomToCreate) {
        switch (roomToCreate) {
            case RoomShapeEnum.ROOMSHAPE_1x1:
            rooms[newPos.x + gridSizeX, newPos.y + gridSizeY] = new RoomModel(newPos, 0, roomToCreate);
            roomListPositions.Insert(0, newPos);
            break;
            case RoomShapeEnum.ROOMSHAPE_2x2:
            rooms[newPos.x + gridSizeX, newPos.y + gridSizeY] = new RoomModel(newPos, 0, roomToCreate);
            roomListPositions.Insert(0, newPos);

            //rooms[newPos.x + 1 + gridSizeX, newPos.y + gridSizeY] = new RoomModel(new Vector2Int(newPos.x + 1, newPos.y), 0, roomToCreate);
            roomListPositions.Insert(0, new Vector2Int(newPos.x + 1, newPos.y));

            //rooms[newPos.x + gridSizeX, newPos.y + gridSizeY] = new RoomModel(new Vector2Int(newPos.x, newPos.y + 1), 0, roomToCreate);
            roomListPositions.Insert(0, new Vector2Int(newPos.x, newPos.y + 1));

            //rooms[newPos.x + gridSizeX, newPos.y + gridSizeY] = new RoomModel(new Vector2Int(newPos.x + 1, newPos.y + 1), 0, roomToCreate);
            roomListPositions.Insert(0, new Vector2Int(newPos.x + 1, newPos.y + 1));
            break;
        }
    }

    private RoomShapeEnum getTypeOfRoomToCreate() {
        if (Random.value > 0.7 && current_ROOMSHAPE_2x2 < max_ROOMSHAPE_2x2) {
            current_ROOMSHAPE_2x2 = 1;
            return RoomShapeEnum.ROOMSHAPE_2x2;
        }
        return RoomShapeEnum.ROOMSHAPE_1x1;
    }

    /*
     * Getting a room to find an adject empty space
     */
    private Vector2Int SearchNewPositionForRoomFromEmptySpace(RoomShapeEnum roomToCreate) {
        int x, y;
        Vector2Int checkingPos;
        do {
            int index = Mathf.RoundToInt(Random.value * (roomListPositions.Count - 1)); // pick a random room
            x = roomListPositions[index].x;
            y = roomListPositions[index].y;
            // randomize between 0 to 1 for vertical or horizontal axis and positive or negative value
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
            checkingPos = new Vector2Int(x, y);
            // while => if already exist or out of bound search another place in Do iteration else return new position
        } while (SearchNewPositionForRoomFromEmptySpaceByRoomShape(checkingPos, roomToCreate, x, y));
        return checkingPos;
    }

    private bool SearchNewPositionForRoomFromEmptySpaceByRoomShape(Vector2Int checkingPos, RoomShapeEnum roomToCreate, int x, int y) {
        bool canPlaceRoom = false;
        switch (roomToCreate) {
            case RoomShapeEnum.ROOMSHAPE_1x1:
            canPlaceRoom = roomListPositions.Contains(checkingPos) || x >= gridSizeX || x < -gridSizeX || y >= gridSizeY || y < -gridSizeY;
            break;
            case RoomShapeEnum.ROOMSHAPE_2x2:
            bool bl = roomListPositions.Contains(checkingPos) || x >= gridSizeX || x < -gridSizeX || y >= gridSizeY || y < -gridSizeY;
            bool br = roomListPositions.Contains(new Vector2Int(checkingPos.x + 1, checkingPos.y)) || x + 1 >= gridSizeX || x + 1 < -gridSizeX || y >= gridSizeY || y < -gridSizeY;
            bool tl = roomListPositions.Contains(new Vector2Int(checkingPos.x, checkingPos.y + 1)) || x >= gridSizeX || x < -gridSizeX || y + 1 >= gridSizeY || y + 1 < -gridSizeY;
            bool tr = roomListPositions.Contains(new Vector2Int(checkingPos.x + 1, checkingPos.y + 1)) || x + 1 >= gridSizeX || x + 1 < -gridSizeX || y + 1 >= gridSizeY || y + 1 < -gridSizeY;
            canPlaceRoom = bl ||  br || tl || tr;
            break;
        }
        return canPlaceRoom;
    }

    /*
     * Instead of SearchNewPositionForRoom, we start with one that only as one neighbor.
     * This will make it more likely that it returns a room that branches out
     */
    private Vector2Int SearchNewPositionForRoomWithOneNeighboor(RoomShapeEnum roomToCreate) {
        int index, inc, x, y;
        Vector2Int checkingPos;
        do {
            inc = 0;
            do {
                index = Mathf.RoundToInt(Random.value * (roomListPositions.Count - 1));
                inc++;
            } while (GetNumberOfNeighborsByRoomShape(roomListPositions[index], roomListPositions, roomToCreate) > 1 && inc < 100);
            x = roomListPositions[index].x;
            y = roomListPositions[index].y;
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
            checkingPos = new Vector2Int(x, y);
        } while (SearchNewPositionForRoomFromEmptySpaceByRoomShape(checkingPos, roomToCreate, x, y));
        // if too many check so we break => toDo find better way
        return checkingPos;
    }

    private int GetNumberOfNeighborsByRoomShape(Vector2Int checkingPos, List<Vector2Int> usedPositions, RoomShapeEnum roomToCreate) {
        int numberOfNeighboor = 0;
        switch (roomToCreate) {
            case RoomShapeEnum.ROOMSHAPE_1x1:
            numberOfNeighboor = GetNumberOfNeighbors(checkingPos, usedPositions);
            break;
            case RoomShapeEnum.ROOMSHAPE_2x2:
            int bl = GetNumberOfNeighbors(checkingPos, usedPositions);
            int br = GetNumberOfNeighbors(new Vector2Int(checkingPos.x + 1, checkingPos.y), usedPositions);
            int tl = GetNumberOfNeighbors(new Vector2Int(checkingPos.x, checkingPos.y + 1), usedPositions);
            int tr = GetNumberOfNeighbors(new Vector2Int(checkingPos.x + 1, checkingPos.y + 1), usedPositions);
            numberOfNeighboor = bl + br + tl + tr;
            break;
        }
        return numberOfNeighboor;
    }

    private int GetNumberOfNeighbors(Vector2Int checkingPos, List<Vector2Int> usedPositions) {
        int ngbh = 0; // start at zero, add 1 for each side there is already a room
        if (usedPositions.Contains(checkingPos + Vector2Int.right)) {
            ngbh++;
        }
        if (usedPositions.Contains(checkingPos + Vector2Int.left)) {
            ngbh++;
        }
        if (usedPositions.Contains(checkingPos + Vector2Int.up)) {
            ngbh++;
        }
        if (usedPositions.Contains(checkingPos + Vector2Int.down)) {
            ngbh++;
        }
        return ngbh;
    }

    private void CreateRooms() {
        foreach (RoomModel room in rooms) {
            //skip where there is no room
            if (room == null) {
                continue;
            }
            string prefabRoom = "";
            Vector2Int pos = Vector2Int.zero;
            switch (room.roomShape) {
                case RoomShapeEnum.ROOMSHAPE_1x1:
                pos.x = room.gridPos.x * 61;
                pos.y = room.gridPos.y * 31;
                prefabRoom = "Room_1x1";
                break;
                case RoomShapeEnum.ROOMSHAPE_2x2:
                /*pos.x = room.gridPos.x * (61 * 2);
                pos.y = room.gridPos.y * (31 * 2);*/
                pos.x = room.gridPos.x * 61;
                pos.y = room.gridPos.y * 31;
                prefabRoom = "Room_2x2";
                break;
            }
            Room obj = Instantiate((GameObject)Resources.Load("Prefabs/Rooms/" + prefabRoom), new Vector3(pos.x, pos.y, 0), transform.rotation).GetComponent<Room>();
            obj.Setup(room.gridPos, 0, room.roomShape);
            room.room = obj;
        }
    }

    private void SetRoomNeighboorsDoors() {
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
