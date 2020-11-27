using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class DoorWithVector2 {
    public Vector2Int vector2;
    public DoorEnum door;

    public DoorWithVector2(Vector2Int vector2, DoorEnum door) {
        this.vector2 = vector2;
        this.door = door;
    }
}

public class DungeonGenerator : MonoBehaviour {

    private Vector2Int worldSize = new Vector2Int(4, 4);
    private RoomModel[,] rooms;
    private List<Room> gridOfRoom = new List<Room>();
    private readonly List<Vector2Int> roomListPositions = new List<Vector2Int>();
    private List<GameObject> room_1x1 = new List<GameObject>();
    private List<GameObject> room_1x2 = new List<GameObject>();
    private List<GameObject> room_2x1 = new List<GameObject>();
    private List<GameObject> room_2x2 = new List<GameObject>();
    private int gridSizeX, gridSizeY, numberOfRooms = 20;
    private readonly int max_ROOMSHAPE_2x2 = 1;
    private readonly int max_ROOMSHAPE_1x2 = 1;
    private readonly int max_ROOMSHAPE_2x1 = 1;
    private int current_ROOMSHAPE_2x2 = 0;
    private int current_ROOMSHAPE_1x2, current_ROOMSHAPE_2x1 = 0;
    private GameObject level;
    private Vector2Int spawnPoint;
    private int id;

    public void StartGeneration(GameObject level) {
        this.level = level;
        if (numberOfRooms >= (worldSize.x * 2) * (worldSize.y * 2)) {
            numberOfRooms = Mathf.RoundToInt((worldSize.x * 2) * (worldSize.y * 2));
        }
        gridSizeX = worldSize.x;
        gridSizeY = worldSize.y;
        CreatePool();
        Generate();
        CreateRooms();
        SetRoomNeighboorsDoors();
    }

    public Room GetRoomFromVector2Int(Vector2Int position) {
        return gridOfRoom.Find(room => room.GetId() == rooms[position.x + gridSizeX, position.y + gridSizeY].id);
    }

    private void CreatePool() {
        GameObject[] go_1x1 = Resources.LoadAll<GameObject>("Prefabs/Rooms/1x1");
        for (var i = 0; i < go_1x1.Length; i++) {
            room_1x1.Add(go_1x1[i]);
        }
        GameObject[] go_1x2 = Resources.LoadAll<GameObject>("Prefabs/Rooms/1x2");
        for (var i = 0; i < go_1x2.Length; i++) {
            room_1x2.Add(go_1x2[i]);
        }
        GameObject[] go_2x1 = Resources.LoadAll<GameObject>("Prefabs/Rooms/2x1");
        for (var i = 0; i < go_2x1.Length; i++) {
            room_2x1.Add(go_2x1[i]);
        }
        GameObject[] go_2x2 = Resources.LoadAll<GameObject>("Prefabs/Rooms/2x2");
        for (var i = 0; i < go_2x2.Length; i++) {
            room_2x2.Add(go_2x2[i]);
        }
    }

    private void Generate() {
        id = 0;
        rooms = new RoomModel[gridSizeX * 2, gridSizeY * 2];
        // creation of first room with fixed position => ToDo find a better way
        spawnPoint = new Vector2Int(gridSizeX, gridSizeY);
        InsertNewRoomInListPosition(Vector2Int.zero, RoomShapeEnum.ROOMSHAPE_1x1);
        roomListPositions.Insert(0, Vector2Int.zero);
        Vector2Int newPos;
        float randomCompare = 0.2f;
        //add rooms
        for (int i = 0; i < numberOfRooms - 1; i++) {
            RoomShapeEnum roomToCreate = GetRandomShapeRoom();
            newPos = SearchNewPositionForRoomFromEmptySpace(roomToCreate);
            if (GetNumberOfNeighborsByRoomShape(newPos, roomListPositions, roomToCreate) > 1 && Random.value > randomCompare) {
                int iterations = 0;
                do {
                    newPos = SearchNewPositionForRoomWithOneNeighboor(roomToCreate);
                    iterations++;
                } while (GetNumberOfNeighborsByRoomShape(newPos, roomListPositions, roomToCreate) > 1 && iterations < 100);
                // Debug.Log(iterations);
            }
            InsertNewRoomInListPosition(newPos, roomToCreate);
        }
    }

    private void InsertNewRoomInListPosition(Vector2Int rootPos, RoomShapeEnum roomToCreate) {
        id++;
        roomListPositions.Insert(0, rootPos);
        RoomModel rootRoom = new RoomModel(rootPos, roomToCreate) {
            worldPosition = new Vector2Int(rootPos.x + gridSizeX, rootPos.y + gridSizeY),
            id = id,
            isRootRoom = true,
        };
        rooms[rootPos.x + gridSizeX, rootPos.y + gridSizeY] = rootRoom;
        switch (roomToCreate) {
            case RoomShapeEnum.ROOMSHAPE_1x1:
            rootRoom.doorsToCheck = new DoorWithVector2[] {
                new DoorWithVector2(new Vector2Int(0, 1), DoorEnum.T),
                new DoorWithVector2(new Vector2Int(0, -1), DoorEnum.B),
                new DoorWithVector2(new Vector2Int(- 1, 0), DoorEnum.L),
                new DoorWithVector2(new Vector2Int(+ 1, 0), DoorEnum.R),
            };
            break;
            case RoomShapeEnum.ROOMSHAPE_2x2: {
                rootRoom.doorsToCheck = new DoorWithVector2[] {
                    new DoorWithVector2(new Vector2Int(-1, 0), DoorEnum.LB),
                    new DoorWithVector2(new Vector2Int(-1, 1), DoorEnum.LT),
                    new DoorWithVector2(new Vector2Int(0, 2), DoorEnum.TL),
                    new DoorWithVector2(new Vector2Int(1, 2), DoorEnum.TR),
                    new DoorWithVector2(new Vector2Int(2, 0), DoorEnum.RB),
                    new DoorWithVector2(new Vector2Int(2, 1), DoorEnum.RT),
                    new DoorWithVector2(new Vector2Int(0, -1), DoorEnum.BL),
                    new DoorWithVector2(new Vector2Int(1, -1), DoorEnum.BR),
                };
                Vector2Int tl, tr, br;
                br = new Vector2Int(rootPos.x + 1, rootPos.y);
                rooms[rootPos.x + 1 + gridSizeX, rootPos.y + gridSizeY] = new RoomModel(br, roomToCreate) {
                    id = id,
                };
                roomListPositions.Insert(0, br);
                tl = new Vector2Int(rootPos.x, rootPos.y + 1);
                rooms[rootPos.x + gridSizeX, rootPos.y + 1 + gridSizeY] = new RoomModel(tl, roomToCreate) {
                    id = id,
                };
                roomListPositions.Insert(0, tl);
                tr = new Vector2Int(rootPos.x + 1, rootPos.y + 1);
                rooms[rootPos.x + 1 + gridSizeX, rootPos.y + 1 + gridSizeY] = new RoomModel(tr, roomToCreate) {
                    id = id,
                };
                roomListPositions.Insert(0, tr);
            }
            break;
            case RoomShapeEnum.ROOMSHAPE_1x2: {
                rootRoom.doorsToCheck = new DoorWithVector2[] {
                    new DoorWithVector2(new Vector2Int(-1, 0), DoorEnum.LB),
                    new DoorWithVector2(new Vector2Int(-1, 1), DoorEnum.LT),
                    new DoorWithVector2(new Vector2Int(1, 0), DoorEnum.RB),
                    new DoorWithVector2(new Vector2Int(1, 1), DoorEnum.RT),
                    new DoorWithVector2(new Vector2Int(0, 2), DoorEnum.T),
                    new DoorWithVector2(new Vector2Int(0, -1), DoorEnum.B),
                };
                Vector2Int t = new Vector2Int(rootPos.x, rootPos.y + 1);
                rooms[rootPos.x + gridSizeX, rootPos.y + 1 + gridSizeY] = new RoomModel(t, roomToCreate) {
                    id = id,
                };
                roomListPositions.Insert(0, t);
            }
            break;
            case RoomShapeEnum.ROOMSHAPE_2x1: {
                rootRoom.doorsToCheck = new DoorWithVector2[] {
                    new DoorWithVector2(new Vector2Int(-1, 0), DoorEnum.L),
                    new DoorWithVector2(new Vector2Int(2, 0), DoorEnum.R),
                    new DoorWithVector2(new Vector2Int(0, 1), DoorEnum.TL),
                    new DoorWithVector2(new Vector2Int(0, -1), DoorEnum.BL),
                    new DoorWithVector2(new Vector2Int(1, 1), DoorEnum.TR),
                    new DoorWithVector2(new Vector2Int(1, -1), DoorEnum.BR),
                };
                Vector2Int r = new Vector2Int(rootPos.x + 1, rootPos.y);
                rooms[rootPos.x + 1 + gridSizeX, rootPos.y + gridSizeY] = new RoomModel(r, roomToCreate) {
                    id = id,
                };
                roomListPositions.Insert(0, r);
            }
            break;
        }
    }

    private RoomShapeEnum GetRandomShapeRoom() {
        if (Random.value > 0.7 && current_ROOMSHAPE_2x2 < max_ROOMSHAPE_2x2) {
            current_ROOMSHAPE_2x2 = 1;
            return RoomShapeEnum.ROOMSHAPE_2x2;
        }
        if (Random.value > 0.7 && current_ROOMSHAPE_1x2 < max_ROOMSHAPE_1x2) {
            current_ROOMSHAPE_1x2 = 1;
            return RoomShapeEnum.ROOMSHAPE_1x2;
        }
        if (Random.value > 0.7 && current_ROOMSHAPE_2x1 < max_ROOMSHAPE_2x1) {
            current_ROOMSHAPE_2x1 = 1;
            return RoomShapeEnum.ROOMSHAPE_2x1;
        }
        return RoomShapeEnum.ROOMSHAPE_1x1;
    }

    /*
     * Getting a room to find an adject empty space
     */
    private Vector2Int SearchNewPositionForRoomFromEmptySpace(RoomShapeEnum roomToCreate) {
        int x, y;
        int iterations = 0;
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
            iterations++;
            // while => if already exist or out of bound search another place in Do iteration else return new position
        } while (SearchNewPositionForRoomFromEmptySpaceByRoomShape(checkingPos, roomToCreate, x, y));
        if (iterations >= 100) {
            Debug.Log("STOP IT");
        }
        return checkingPos;
    }

    private bool SearchNewPositionForRoomFromEmptySpaceByRoomShape(Vector2Int checkingPos, RoomShapeEnum roomToCreate, int x, int y) {
        bool canPlaceRoom = false;
        switch (roomToCreate) {
            case RoomShapeEnum.ROOMSHAPE_1x1: {
                canPlaceRoom = roomListPositions.Contains(checkingPos) || x >= gridSizeX || x < -gridSizeX || y >= gridSizeY || y < -gridSizeY;
            }
            break;
            case RoomShapeEnum.ROOMSHAPE_2x2: {
                bool bl = roomListPositions.Contains(checkingPos) || x >= gridSizeX || x < -gridSizeX || y >= gridSizeY || y < -gridSizeY;
                bool br = roomListPositions.Contains(new Vector2Int(checkingPos.x + 1, checkingPos.y)) || x + 1 >= gridSizeX || x + 1 < -gridSizeX || y >= gridSizeY || y < -gridSizeY;
                bool tl = roomListPositions.Contains(new Vector2Int(checkingPos.x, checkingPos.y + 1)) || x >= gridSizeX || x < -gridSizeX || y + 1 >= gridSizeY || y + 1 < -gridSizeY;
                bool tr = roomListPositions.Contains(new Vector2Int(checkingPos.x + 1, checkingPos.y + 1)) || x + 1 >= gridSizeX || x + 1 < -gridSizeX || y + 1 >= gridSizeY || y + 1 < -gridSizeY;
                canPlaceRoom = bl || br || tl || tr;
            }
            break;
            case RoomShapeEnum.ROOMSHAPE_1x2: {
                bool bl = roomListPositions.Contains(checkingPos) || x >= gridSizeX || x < -gridSizeX || y >= gridSizeY || y < -gridSizeY;
                bool tl = roomListPositions.Contains(new Vector2Int(checkingPos.x, checkingPos.y + 1)) || x >= gridSizeX || x < -gridSizeX || y + 1 >= gridSizeY || y + 1 < -gridSizeY;
                canPlaceRoom = bl || tl;
            }
            break;
            case RoomShapeEnum.ROOMSHAPE_2x1: {
                bool bl = roomListPositions.Contains(checkingPos) || x >= gridSizeX || x < -gridSizeX || y >= gridSizeY || y < -gridSizeY;
                bool br = roomListPositions.Contains(new Vector2Int(checkingPos.x + 1, checkingPos.y)) || x + 1 >= gridSizeX || x + 1 < -gridSizeX || y >= gridSizeY || y < -gridSizeY;
                canPlaceRoom = bl || br;
            }
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
        if (inc >= 100) { // break loop if it takes too long: this loop isnt garuanteed to find solution, which is fine for this
            // print("Error: ErrorErrorErrorErrorErrorErrorError");
        }
        // if too many check so we break => ToDo find better way
        return checkingPos;
    }

    private int GetNumberOfNeighborsByRoomShape(Vector2Int checkingPos, List<Vector2Int> usedPositions, RoomShapeEnum roomToCreate) {
        int numberOfNeighboor = 0;
        switch (roomToCreate) {
            case RoomShapeEnum.ROOMSHAPE_1x1:
            numberOfNeighboor = GetNumberOfNeighbors(checkingPos, usedPositions);
            break;
            case RoomShapeEnum.ROOMSHAPE_2x2: {
                int bl = GetNumberOfNeighbors(checkingPos, usedPositions);
                int br = GetNumberOfNeighbors(new Vector2Int(checkingPos.x + 1, checkingPos.y), usedPositions);
                int tl = GetNumberOfNeighbors(new Vector2Int(checkingPos.x, checkingPos.y + 1), usedPositions);
                int tr = GetNumberOfNeighbors(new Vector2Int(checkingPos.x + 1, checkingPos.y + 1), usedPositions);
                numberOfNeighboor = bl + br + tl + tr;
            }
            break;
            case RoomShapeEnum.ROOMSHAPE_1x2: {
                int bl = GetNumberOfNeighbors(checkingPos, usedPositions);
                int tl = GetNumberOfNeighbors(new Vector2Int(checkingPos.x, checkingPos.y + 1), usedPositions);
                numberOfNeighboor = bl + tl;
            }
            break;
            case RoomShapeEnum.ROOMSHAPE_2x1: {
                int bl = GetNumberOfNeighbors(checkingPos, usedPositions);
                int br = GetNumberOfNeighbors(new Vector2Int(checkingPos.x + 1, checkingPos.y), usedPositions);
                numberOfNeighboor = bl + br;
            }
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
            if (room == null || !room.isRootRoom) {
                continue;
            }
            GameObject roomGo = null;
            string prefabRoom = "";
            Vector2Int pos = new Vector2Int(room.worldPosition.x * 61, room.worldPosition.y * 31);
            switch (room.roomShape) {
                case RoomShapeEnum.ROOMSHAPE_1x1:
                prefabRoom = "Room_1x1";
                roomGo = GetRandomRoomFromPool(room_1x1);
                break;
                case RoomShapeEnum.ROOMSHAPE_1x2:
                prefabRoom = "Room_1x2";
                roomGo = GetRandomRoomFromPool(room_1x2);
                break;
                case RoomShapeEnum.ROOMSHAPE_2x1:
                prefabRoom = "Room_2x1";
                roomGo = GetRandomRoomFromPool(room_2x1);
                break;
                case RoomShapeEnum.ROOMSHAPE_2x2:
                prefabRoom = "Room_2x2";
                roomGo = GetRandomRoomFromPool(room_2x2);
                break;
            }
            Room obj = Instantiate(roomGo, new Vector3(pos.x, pos.y, 0), transform.rotation).GetComponent<Room>();
            obj.transform.parent = level.transform;
            obj.Setup(room.rootPos, room.roomShape, room.id);
            room.room = obj;
            gridOfRoom.Add(obj);
        }
    }

    private GameObject GetRandomRoomFromPool(List<GameObject> rooms) {
        int index = Random.Range(0, rooms.Count - 1);
        GameObject room = rooms[index];
        rooms.RemoveAt(index);
        return room;
    }

    private bool CheckRightDoor(int x, int y) {
        return x < gridSizeX * 2 && y >= 0 && y < gridSizeY * 2 && rooms[x, y] != null;
    }

    private bool CheckLeftDoor(int x, int y) {
        return x >= 0 && y >= 0 && y < gridSizeY * 2 && rooms[x, y] != null;
    }

    private bool CheckTopDoor(int x, int y) {
        return y < gridSizeY * 2 && x < gridSizeX * 2 && x >= 0 && rooms[x, y] != null;
    }

    private bool CheckBottomDoor(int x, int y) {
        return y >= 0 && x < gridSizeX * 2 && x >= 0 &&  rooms[x, y] != null;
    }

    private void SetRoomNeighboorsDoors() {
        for (int x = 0; x < ((gridSizeX * 2)); x++) {
            for (int y = 0; y < ((gridSizeY * 2)); y++) {
                RoomModel roomModel = rooms[x, y];
                if (roomModel == null || !rooms[x, y].isRootRoom) {
                    continue;
                }
                Room room = roomModel.room;
                for (var i = 0; i < roomModel.doorsToCheck.Length; i++) {
                    DoorWithVector2 dwv = roomModel.doorsToCheck[i];
                    switch (dwv.door) {
                        case DoorEnum.L:
                        room.enable_door_L = CheckBottomDoor(dwv.vector2.x + x, dwv.vector2.y + y);
                        break;
                        case DoorEnum.LB:
                        room.enable_door_LB = CheckLeftDoor(dwv.vector2.x + x, dwv.vector2.y + y);
                        break;
                        case DoorEnum.LT:
                        room.enable_door_LT = CheckLeftDoor(dwv.vector2.x + x, dwv.vector2.y + y);
                        break;
                        case DoorEnum.B:
                        room.enable_door_B = CheckBottomDoor(dwv.vector2.x + x, dwv.vector2.y + y);
                        break;
                        case DoorEnum.BL:
                        room.enable_door_BL = CheckBottomDoor(dwv.vector2.x + x, dwv.vector2.y + y);
                        break;
                        case DoorEnum.BR:
                        room.enable_door_BR = CheckBottomDoor(dwv.vector2.x + x, dwv.vector2.y + y);
                        break;
                        case DoorEnum.T:
                        room.enable_door_T = CheckTopDoor(dwv.vector2.x + x, dwv.vector2.y + y);
                        break;
                        case DoorEnum.TL:
                        room.enable_door_TL = CheckTopDoor(dwv.vector2.x + x, dwv.vector2.y + y);
                        break;
                        case DoorEnum.TR:
                        room.enable_door_TR = CheckTopDoor(dwv.vector2.x + x, dwv.vector2.y + y);
                        break;
                        case DoorEnum.R:
                        room.enable_door_R = CheckRightDoor(dwv.vector2.x + x, dwv.vector2.y + y);
                        break;
                        case DoorEnum.RB:
                        room.enable_door_RB = CheckRightDoor(dwv.vector2.x + x, dwv.vector2.y + y);
                        break;
                        case DoorEnum.RT:
                        room.enable_door_RT = CheckRightDoor(dwv.vector2.x + x, dwv.vector2.y + y);
                        break;
                    }
                }
            }
        }
    }
}
