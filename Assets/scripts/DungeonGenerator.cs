using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using System.IO;

public class DoorWithVector2 {
    public Vector2Int vector2;
    public DoorEnum door;

    public DoorWithVector2(Vector2Int vector2, DoorEnum door) {
        this.vector2 = vector2;
        this.door = door;
    }
}

public enum NeighboorToCheckEnum {
    T,
    TL,
    TR,
    L,
    LT,
    LB,
    R,
    RT,
    RB,
    B,
    BL,
    BR,
}

public class GridOfRooms {
    public Vector2Int position;
    public RoomShapeEnum roomShape;
    public Room room;
    public List<int[,]> sides = new List<int[,]>();

    public GridOfRooms(Vector2Int position, RoomShapeEnum roomShape, Room room) {
        this.position = position;
        this.roomShape = roomShape;
        this.room = room;
    }
}

public class PseudoRoom {
    public Vector2Int position;
    public RoomShapeEnum roomShape;
    public bool isStartRoom;

    public PseudoRoom(Vector2Int position, RoomShapeEnum roomShape, bool isStartRoom = false) {
        this.position = position;
        this.roomShape = roomShape;
        this.isStartRoom = isStartRoom;
    }
}

public class DungeonGenerator : MonoBehaviour {

    private int maxCol = 8;
    private RoomModel[,] rooms;
    private List<Room> gridOfRoom = new List<Room>();
    private readonly List<Vector2Int> roomListPositions = new List<Vector2Int>();
    private GridOfRooms[,] gridOfRooms;
    private readonly List<Room> roomList = new List<Room>();
    private List<Vector2Int> listOfUsingSpace;
    private int maxTry = 4;
    private int currentTry = 0;
    private Dictionary<int, List<int>> tree = new Dictionary<int, List<int>>();
    private List<Vector2Int> roomGrid = new List<Vector2Int>();
    private List<GameObject> room_1x1 = new List<GameObject>();
    private List<GameObject> room_1x2 = new List<GameObject>();
    private List<GameObject> room_2x1 = new List<GameObject>();
    private List<GameObject> room_2x2 = new List<GameObject>();
    private int numberOfRooms;


    private int current_ROOMSHAPE_2x2, current_ROOMSHAPE_1x2, current_ROOMSHAPE_2x1 = 0;
    private readonly int max_ROOMSHAPE_2x2 = 1;
    private readonly int max_ROOMSHAPE_1x2 = 1;
    private readonly int max_ROOMSHAPE_2x1 = 1;
    private float luckForSpecialSHape = 0.5f;

    // InitValues
    private int[,] floorplan;
    private int bound;
    private int roomMaxBound;
    private List<Vector2Int> cellQueue;
    private List<Vector2Int> endRooms;
    private int floorplanCount;
    private DungeonConfig currentDungeonConfig;
    private int maxRooms;
    private Vector2Int vectorStart;
    private int totalLoop = 0;
    private GameObject floor;
    private List<PseudoRoom> listOfPseudoRoom;

    private void InitValues(GameObject floor, DungeonConfig config) {
        floorplan = new int[12, 12];
        bound = floorplan.GetLength(0);
        roomMaxBound = floorplan.GetLength(0) - 2;
        currentDungeonConfig = config;
        maxRooms = (int)config.GetRoomSize();
        vectorStart = new Vector2Int(bound / 2, bound / 2);
        totalLoop = 0;
        this.floor = floor;
    }

    private void InitGenerateValues() {
        floorplan = new int[12, 12];
        current_ROOMSHAPE_2x2 = 0;
        current_ROOMSHAPE_1x2 = 0;
        current_ROOMSHAPE_2x1 = 0;
        cellQueue = new List<Vector2Int>();
        endRooms = new List<Vector2Int>();
        listOfPseudoRoom = new List<PseudoRoom>();
        floorplanCount = 0;
    }

    public void StartGeneration(GameObject floor, DungeonConfig config) {
        InitValues(floor, config);
        // CreatePool();
        Generate();
        CreateRooms();
        // SetRoomNeighboorsDoors();
    }

    public Room GetRoomFromVector2Int(Vector2Int position) {
        return gridOfRoom.Find(room => room.GetId() == rooms[position.x + maxCol / 2, position.y + maxCol / 2].id);
    }

    /*private string getPathRoom(string roomSize) {
        return roomPath + roomSize + "/" + currentDungeonConfig.GetBiomeType();
    }

    private void CreatePool() {
        GameObject[] go_1x1 = Resources.LoadAll<GameObject>(getPathRoom("1x1"));
        for (var i = 0; i < go_1x1.Length; i++) {
            room_1x1.Add(go_1x1[i]);
        }
        GameObject[] go_1x2 = Resources.LoadAll<GameObject>(getPathRoom("1x2"));
        for (var i = 0; i < go_1x2.Length; i++) {
            room_1x2.Add(go_1x2[i]);
        }
        GameObject[] go_2x1 = Resources.LoadAll<GameObject>(getPathRoom("2x1"));
        for (var i = 0; i < go_2x1.Length; i++) {
            room_2x1.Add(go_2x1[i]);
        }
        GameObject[] go_2x2 = Resources.LoadAll<GameObject>(getPathRoom("2x2"));
        for (var i = 0; i < go_2x2.Length; i++) {
            room_2x2.Add(go_2x2[i]);
        }
    }*/

    private void Generate() {
        InitGenerateValues();
        // Start Breadth First Pattern
        Visit(vectorStart, RoomShapeEnum.ROOMSHAPE_1x1, true);

        while (cellQueue.Count > 0 && floorplanCount < maxRooms) {
            RoomShapeEnum roomShape = GetRandomShapeRoom();
            Vector2Int vector = cellQueue[0];
            cellQueue.RemoveAt(0);
            int createdCount = 0;

            if (vector.x > 1 && !checkLimitOfSpecialRooms(roomShape)) {
                createdCount += Visit(new Vector2Int(vector.x - 1, vector.y), roomShape, false);
            }
            if (vector.x < roomMaxBound && !checkLimitOfSpecialRooms(roomShape)) {
                createdCount += Visit(new Vector2Int(vector.x + 1, vector.y), roomShape, false);
            }
            if (vector.y > 1 && !checkLimitOfSpecialRooms(roomShape)) {
                createdCount += Visit(new Vector2Int(vector.x, vector.y - 1), roomShape, false);
            }
            if (vector.y < roomMaxBound && !checkLimitOfSpecialRooms(roomShape)) {
                createdCount += Visit(new Vector2Int(vector.x, vector.y + 1), roomShape, false);
            }
            if (createdCount == 0) {
                endRooms.Add(vector);
            }
        }
        if (floorplanCount < maxRooms) {
            totalLoop++;
            Generate();
        } else {
            Debug.Log("END " + totalLoop);
            Debug.Log("max_ROOMSHAPE_1x2 " + max_ROOMSHAPE_1x2);
            Debug.Log("max_ROOMSHAPE_2x1 " + max_ROOMSHAPE_2x1);
            Debug.Log("max_ROOMSHAPE_1x2 " + max_ROOMSHAPE_1x2);
        }
        /*
         * 
         * RAJOUTER LA GESTION DES PIECES SPECIALES !!!
         * 
         if(floorplanCount < minrooms) {
                start.apply(this);
                return;
            }

            placedSpecial = true;
            bossl = endrooms.pop();
            var cellImage = img(this, bossl, 'boss');
            cellImage.x += 1;
            
            var rewardl = poprandomendroom();
            var cellImage = img(this, rewardl, 'reward');

            var coinl = poprandomendroom();
            img(this, coinl, 'coin');

            var secretl = picksecretroom();
            img(this, secretl, 'cell');
            img(this, secretl, 'secret');

            if (!rewardl || !coinl || !secretl) {
                start.apply(this);
                return;
            }
         * 
        */

    }

    private bool checkLimitOfSpecialRooms(RoomShapeEnum roomShape) {
        switch (roomShape) {
            case RoomShapeEnum.ROOMSHAPE_2x2: {
                if (current_ROOMSHAPE_2x2 > max_ROOMSHAPE_2x2) {
                    return true;
                }
                break;
            }
            case RoomShapeEnum.ROOMSHAPE_1x2: {
                if (current_ROOMSHAPE_1x2 > max_ROOMSHAPE_1x2) {
                    return true;
                }
                break;
            }
            case RoomShapeEnum.ROOMSHAPE_2x1: {
                if (current_ROOMSHAPE_2x1 > max_ROOMSHAPE_2x1) {
                    return true;
                }
                break;
            }
        }
        return false;
    }

    private int Visit(Vector2Int vector, RoomShapeEnum shape, bool firstRoom) {

        if (floorplanCount >= maxRooms) {
            return 0;
        }

        if (Random.value < 0.5f && vector != vectorStart) {
            return 0;
        }

        // toDO rajouter le sens T L R B du visite afin de prendre les bonnes distances ex : si 2x2 et sens = L alors on enlève -1 en X 
        // si 2x2 && sens = T alors on ajoute +1 en y
        if (CheckEmptySpace(vector, shape) > 0) {
            return 0;
        }

        if (neighbourCount(vector, shape) > 1) {
            return 0;
        }

        cellQueue.Add(vector);

        switch (shape) {
            case RoomShapeEnum.ROOMSHAPE_1x1: {
                floorplan[vector.x, vector.y] = 1;
                break;
            }
            case RoomShapeEnum.ROOMSHAPE_2x2: {
                floorplan[vector.x, vector.y] = 1;
                floorplan[vector.x + 1, vector.y] = 1;
                floorplan[vector.x, vector.y + 1] = 1;
                floorplan[vector.x + 1, vector.y + 1] = 1;
                current_ROOMSHAPE_2x2++;
                break;
            }
            case RoomShapeEnum.ROOMSHAPE_1x2: {
                floorplan[vector.x, vector.y] = 1;
                floorplan[vector.x, vector.y + 1] = 1;
                current_ROOMSHAPE_1x2++;
                break;
            }
            case RoomShapeEnum.ROOMSHAPE_2x1: {
                floorplan[vector.x, vector.y] = 1;
                floorplan[vector.x + 1, vector.y] = 1;
                current_ROOMSHAPE_2x1++;
                break;
            }
        }

        listOfPseudoRoom.Add(new PseudoRoom(vector, shape, firstRoom));
        floorplanCount += 1;

        return 1;

    }

    private int neighbourCount(Vector2Int vector, RoomShapeEnum shape) {

        int count = 0;
        Vector2Int[] shapesToCheck = SearchNeighboorsByShapes(shape, vector);
        /* case
        * if x or y == 1 
        * || x or y == bound -1 
        * if search to check neightboor of big room like 2x2 so -1 + -2 is out of bound!
        */
        if (shapesToCheck.Length == 0) {
            return 99;
        }
        foreach (var checkNewPlace in shapesToCheck) {
            count += floorplan[checkNewPlace.x, checkNewPlace.y];
        }
        return count;
    }

    private int CheckEmptySpace(Vector2Int vector, RoomShapeEnum shape) {
        switch (shape) {
            case RoomShapeEnum.ROOMSHAPE_1x1: {
                return floorplan[vector.x, vector.y];
            }
            case RoomShapeEnum.ROOMSHAPE_2x2: {
                return floorplan[vector.x, vector.y] +
                    floorplan[vector.x + 1, vector.y] +
                    floorplan[vector.x, vector.y + 1] +
                    floorplan[vector.x + 1, vector.y + 1];
            }
            case RoomShapeEnum.ROOMSHAPE_1x2: {
                return floorplan[vector.x, vector.y] +
                    floorplan[vector.x, vector.y + 1];
            }
            case RoomShapeEnum.ROOMSHAPE_2x1: {
                return floorplan[vector.x, vector.y] +
                    floorplan[vector.x + 1, vector.y];
            }
        }
        return 0;
    }

    private Vector2Int[] SearchNeighboorsByShapes(RoomShapeEnum shape, Vector2Int vector) {
        switch (shape) {
            case RoomShapeEnum.ROOMSHAPE_1x1: {
                Vector2Int t = new Vector2Int(vector.x, vector.y + 1);
                Vector2Int b = new Vector2Int(vector.x, vector.y - 1);
                Vector2Int l = new Vector2Int(vector.x - 1, vector.y);
                Vector2Int r = new Vector2Int(vector.x + 1, vector.y);
                return new Vector2Int[] { t, b, l, r };
            }
            case RoomShapeEnum.ROOMSHAPE_2x2: {
                if(vector.y + 2 >= bound) {
                    return new Vector2Int[] { };
                }
                Vector2Int tl = new Vector2Int(vector.x, vector.y + 2);
                Vector2Int tr = new Vector2Int(vector.x + 1, vector.y + 2);
                Vector2Int lt = new Vector2Int(vector.x - 1, vector.y + 1);
                Vector2Int lb = new Vector2Int(vector.x - 1, vector.y);
                Vector2Int rt = new Vector2Int(vector.x + 1, vector.y + 1);
                Vector2Int rb = new Vector2Int(vector.x + 1, vector.y);
                Vector2Int bl = new Vector2Int(vector.x, vector.y - 1);
                Vector2Int br = new Vector2Int(vector.x + 1, vector.y - 1);
                return new Vector2Int[] { tl, tr, lt, lb, rt, rb, bl, br };
            }
            case RoomShapeEnum.ROOMSHAPE_2x1: {
                if (vector.x + 2 >= bound) {
                    return new Vector2Int[] { };
                }
                Vector2Int tl = new Vector2Int(vector.x, vector.y + 1);
                Vector2Int tr = new Vector2Int(vector.x + 1, vector.y + 1);
                Vector2Int bl = new Vector2Int(vector.x, vector.y - 1);
                Vector2Int br = new Vector2Int(vector.x + 1, vector.y - 1);
                Vector2Int l = new Vector2Int(vector.x - 1, vector.y);
                Vector2Int r = new Vector2Int(vector.x + 2, vector.y);
                return new Vector2Int[] { tl, tr, bl, br, l, r };
            }
            case RoomShapeEnum.ROOMSHAPE_1x2: {
                if (vector.y + 2 >= bound) {
                    return new Vector2Int[] { };
                }
                Vector2Int t = new Vector2Int(vector.x, vector.y + 2);
                Vector2Int b = new Vector2Int(vector.x, vector.y - 1);
                Vector2Int lt = new Vector2Int(vector.x - 1, vector.y + 1);
                Vector2Int lb = new Vector2Int(vector.x - 1, vector.y);
                Vector2Int rt = new Vector2Int(vector.x + 1, vector.y + 1);
                Vector2Int rb = new Vector2Int(vector.x + 1, vector.y);
                return new Vector2Int[] { t, b, lt, lb, rt, rb };
            }
        }
        return null;
    }

    private RoomShapeEnum GetRandomShapeRoom() {
        float rng = Random.value;
        if (rng < 0.3) {
            if (Random.value < 0.5f) {
                return 0;
            }
            if (current_ROOMSHAPE_2x2 < max_ROOMSHAPE_2x2) {
                return RoomShapeEnum.ROOMSHAPE_2x2;
            }
        }
        if (rng > 0.3 && Random.value < 0.6) {
            if (Random.value < 0.5f) {
                return 0;
            }
            if (current_ROOMSHAPE_1x2 < max_ROOMSHAPE_1x2) {
                return RoomShapeEnum.ROOMSHAPE_1x2;
            }
        }
        if (rng > 0.6) {
            if (Random.value < 0.5f) {
                return 0;
            }
            if (current_ROOMSHAPE_2x1 < max_ROOMSHAPE_2x1) {
                return RoomShapeEnum.ROOMSHAPE_2x1;
            }
        }
        return RoomShapeEnum.ROOMSHAPE_1x1;
    }

    private void CreateRooms() {
        foreach (PseudoRoom room in listOfPseudoRoom) {
            GameObject roomGo = null;
            // long fileInfo = new System.IO.FileInfo(folder).Length;
            // create get room
            if (room.isStartRoom) {
                roomGo = Resources.Load<GameObject>(currentDungeonConfig.GetStarterPathRoomByBiome() + 0);
            } else {
                // toDO refacto et manage avec un pool de piece déjà prise !!!
                int rnd = 0;
                if (room.roomShape == RoomShapeEnum.ROOMSHAPE_1x1) {
                    // rnd = Random.Range(0, (int)new System.IO.FileInfo(currentDungeonConfig.GetRoomsFolderPathByBiomeDifficultyAndRoomSize(currentDungeonConfig.GetDifficulty(), RoomShapeEnum.ROOMSHAPE_1x1)).Length);
                    rnd = Random.Range(0, 23);
                }
                if (room.roomShape == RoomShapeEnum.ROOMSHAPE_1x2) {
                    // rnd = Random.Range(0, (int)new System.IO.FileInfo(currentDungeonConfig.GetRoomsFolderPathByBiomeDifficultyAndRoomSize(currentDungeonConfig.GetDifficulty(), RoomShapeEnum.ROOMSHAPE_1x2)).Length);
                    rnd = Random.Range(0, 1);
                }
                if (room.roomShape == RoomShapeEnum.ROOMSHAPE_2x2) {
                    // rnd = Random.Range(0, (int)new System.IO.FileInfo(currentDungeonConfig.GetRoomsFolderPathByBiomeDifficultyAndRoomSize(currentDungeonConfig.GetDifficulty(), RoomShapeEnum.ROOMSHAPE_2x1)).Length);
                    rnd = Random.Range(0, 1);
                }
                if (room.roomShape == RoomShapeEnum.ROOMSHAPE_2x1) {
                    // rnd = Random.Range(0, (int)new System.IO.FileInfo(currentDungeonConfig.GetRoomsFolderPathByBiomeDifficultyAndRoomSize(currentDungeonConfig.GetDifficulty(), RoomShapeEnum.ROOMSHAPE_2x2)).Length);
                    rnd = Random.Range(0, 1);
                }
                roomGo = Resources.Load<GameObject>(currentDungeonConfig.GetRoomsFolderPathByBiomeDifficultyAndRoomSize(currentDungeonConfig.GetDifficulty(), room.roomShape) + rnd);
            }
            Vector2Int pos = new Vector2Int(room.position.x * 61, room.position.y * 31);
            Room obj = Instantiate(roomGo, new Vector3(pos.x, pos.y, 0), transform.rotation).GetComponent<Room>();
            obj.transform.parent = floor.transform;
            obj.Setup(room.position, room.roomShape);
            // room.room = obj;
            // gridOfRoom.Add(obj);
        }
    }

    /*private DoorWithVector2[] GetDoorsByShape(RoomShapeEnum shape) {
        switch (shape) {
            case RoomShapeEnum.ROOMSHAPE_1x1:
            return new DoorWithVector2[] {
                new DoorWithVector2(new Vector2Int(0, 1), DoorEnum.T),
                new DoorWithVector2(new Vector2Int(0, -1), DoorEnum.B),
                new DoorWithVector2(new Vector2Int(- 1, 0), DoorEnum.L),
                new DoorWithVector2(new Vector2Int(+ 1, 0), DoorEnum.R),
            };
            case RoomShapeEnum.ROOMSHAPE_2x2: {
                return new DoorWithVector2[] {
                    new DoorWithVector2(new Vector2Int(-1, 0), DoorEnum.LB),
                    new DoorWithVector2(new Vector2Int(-1, 1), DoorEnum.LT),
                    new DoorWithVector2(new Vector2Int(0, 2), DoorEnum.TL),
                    new DoorWithVector2(new Vector2Int(1, 2), DoorEnum.TR),
                    new DoorWithVector2(new Vector2Int(2, 0), DoorEnum.RB),
                    new DoorWithVector2(new Vector2Int(2, 1), DoorEnum.RT),
                    new DoorWithVector2(new Vector2Int(0, -1), DoorEnum.BL),
                    new DoorWithVector2(new Vector2Int(1, -1), DoorEnum.BR),
                };
            }
            case RoomShapeEnum.ROOMSHAPE_1x2: {
                return new DoorWithVector2[] {
                    new DoorWithVector2(new Vector2Int(-1, 0), DoorEnum.LB),
                    new DoorWithVector2(new Vector2Int(-1, 1), DoorEnum.LT),
                    new DoorWithVector2(new Vector2Int(1, 0), DoorEnum.RB),
                    new DoorWithVector2(new Vector2Int(1, 1), DoorEnum.RT),
                    new DoorWithVector2(new Vector2Int(0, 2), DoorEnum.T),
                    new DoorWithVector2(new Vector2Int(0, -1), DoorEnum.B),
                };
            }
            case RoomShapeEnum.ROOMSHAPE_2x1: {
                return new DoorWithVector2[] {
                    new DoorWithVector2(new Vector2Int(-1, 0), DoorEnum.L),
                    new DoorWithVector2(new Vector2Int(2, 0), DoorEnum.R),
                    new DoorWithVector2(new Vector2Int(0, 1), DoorEnum.TL),
                    new DoorWithVector2(new Vector2Int(0, -1), DoorEnum.BL),
                    new DoorWithVector2(new Vector2Int(1, 1), DoorEnum.TR),
                    new DoorWithVector2(new Vector2Int(1, -1), DoorEnum.BR),
                };
            }
            default:
            return null;
        }
    }*/

    /*private Room CreateRoom(Vector2Int position, RoomShapeEnum shape, bool isStartRoom) {
        Room newRoom = gameObject.AddComponent<Room>();
        newRoom.Setup(position, shape);
        newRoom.isStartRoom = isStartRoom;
        // newRoom.getDoorsByShape(shape);
        return newRoom;
    }*/

    /*private void InsertNewRoomInListPosition(Vector2Int rootPos, RoomShapeEnum roomToCreate, int id) {
        id++;
        roomListPositions.Insert(0, rootPos);
        RoomModel rootRoom = new RoomModel(rootPos, roomToCreate) {
            worldPosition = new Vector2Int(rootPos.x + maxCol / 2, rootPos.y + maxCol / 2),
            id = id,
            isRootRoom = true,
        };
        rooms[rootPos.x + maxCol / 2, rootPos.y + maxCol / 2] = rootRoom;
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
                rooms[rootPos.x + 1 + maxCol / 2, rootPos.y + maxCol / 2] = new RoomModel(br, roomToCreate) {
                    id = id,
                };
                roomListPositions.Insert(0, br);
                tl = new Vector2Int(rootPos.x, rootPos.y + 1);
                rooms[rootPos.x + maxCol / 2, rootPos.y + 1 + maxCol / 2] = new RoomModel(tl, roomToCreate) {
                    id = id,
                };
                roomListPositions.Insert(0, tl);
                tr = new Vector2Int(rootPos.x + 1, rootPos.y + 1);
                rooms[rootPos.x + 1 + maxCol / 2, rootPos.y + 1 + maxCol / 2] = new RoomModel(tr, roomToCreate) {
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
                rooms[rootPos.x + maxCol / 2, rootPos.y + 1 + maxCol / 2] = new RoomModel(t, roomToCreate) {
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
                rooms[rootPos.x + 1 + maxCol / 2, rootPos.y + maxCol / 2] = new RoomModel(r, roomToCreate) {
                    id = id,
                };
                roomListPositions.Insert(0, r);
            }
            break;
        }
    }*/

    /*
     * Instead of SearchNewPositionForRoom, we start with one that only as one neighbor.
     * This will make it more likely that it returns a room that branches out
     */

    /*private int GetNumberOfNeighborsByRoomShape(Vector2Int checkingPos, List<Vector2Int> usedPositions, RoomShapeEnum roomToCreate) {
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
    }*/

    /*private int GetNumberOfNeighbors(Vector2Int checkingPos, List<Vector2Int> usedPositions) {
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
    }*/

    /*private void CreateRooms() {
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
                if (room.isStartRoom) {
                    roomGo = Resources.Load<GameObject>(roomPath + "Starter/Room_1x1_" + currentDungeonConfig.GetBiomeType());
                } else {
                    roomGo = GetRandomRoomFromPool(room_1x1);
                }
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
            // obj.Setup(room.rootPos, room.roomShape, room.id);
            room.room = obj;
            gridOfRoom.Add(obj);
        }
    }*/

    /*private GameObject GetRandomRoomFromPool(List<GameObject> rooms) {
        int index = Random.Range(0, rooms.Count - 1);
        GameObject room = rooms[index];
        rooms.RemoveAt(index);
        return room;
    }*/

    private bool CheckRightDoor(int x, int y) {
        return x < maxCol && y >= 0 && y < maxCol && rooms[x, y] != null;
    }

    private bool CheckLeftDoor(int x, int y) {
        return x >= 0 && y >= 0 && y < maxCol && rooms[x, y] != null;
    }

    private bool CheckTopDoor(int x, int y) {
        return y < maxCol && x < maxCol && x >= 0 && rooms[x, y] != null;
    }

    private bool CheckBottomDoor(int x, int y) {
        return y >= 0 && x < maxCol && x >= 0 && rooms[x, y] != null;
    }

    private void SetRoomNeighboorsDoors() {
        for (int x = 0; x < (maxCol); x++) {
            for (int y = 0; y < (maxCol); y++) {
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
