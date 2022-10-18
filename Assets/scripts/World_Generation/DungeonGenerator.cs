using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using System.IO;

public class DungeonGenerator : MonoBehaviour {

    private Vector2Int roomSize = new Vector2Int(61, 31);
    private int current_ROOMSHAPE_2x2, current_ROOMSHAPE_1x2, current_ROOMSHAPE_2x1 = 0;
    private readonly int max_ROOMSHAPE_2x2 = 1;
    private readonly int max_ROOMSHAPE_1x2 = 1;
    private readonly int max_ROOMSHAPE_2x1 = 1;
    // private float luckForSpecialSHape = 0.5f;

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

    public void StartGeneration(GameObject floor, DungeonConfig config) {
        InitValues(floor, config);
        // CreatePool();
        Generate();
        ManageRoomsDoors();
        CreateRooms();
        // SetRoomNeighborsDoors();
    }
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

    private void Generate() {
        InitGenerateValues();
        InitSpawnRoom();
        int totalRoomPlaced = TryToGenerateAllRoomFloor();
        // if all the pieces have not been successfully placed then we start again
        if (totalRoomPlaced < maxRooms) {
            totalLoop++;
            Generate();
        } else {
            Debug.Log("END " + totalLoop);
            Debug.Log("max_ROOMSHAPE_1x2 " + max_ROOMSHAPE_1x2);
            Debug.Log("max_ROOMSHAPE_2x1 " + max_ROOMSHAPE_2x1);
            Debug.Log("max_ROOMSHAPE_1x2 " + max_ROOMSHAPE_1x2);
        }
    }

    private void ManageRoomsDoors() {
        foreach (PseudoRoom room in listOfPseudoRoom) {
            room.SeachNeightboors(listOfPseudoRoom, bound);
        }
    }

    private void CreateRooms() {
        foreach (PseudoRoom room in listOfPseudoRoom) {
            GameObject roomGo = null;
            // long fileInfo = new System.IO.FileInfo(folder).Length;
            // create get room
            if (room.GetIsStartRoom()) {
                roomGo = Resources.Load<GameObject>(currentDungeonConfig.GetStarterPathRoomByBiome() + 0);
            } else {
                // toDO refacto et manage avec un pool de piece déjà prise !!!
                int rnd = 0;
                if (room.GetRoomShape() == RoomShapeEnum.ROOMSHAPE_1x1) {
                    // rnd = Random.Range(0, (int)new System.IO.FileInfo(currentDungeonConfig.GetRoomsFolderPathByBiomeDifficultyAndRoomSize(currentDungeonConfig.GetDifficulty(), RoomShapeEnum.ROOMSHAPE_1x1)).Length);
                    rnd = Random.Range(0, 23);
                }
                if (room.GetRoomShape() == RoomShapeEnum.ROOMSHAPE_1x2) {
                    // rnd = Random.Range(0, (int)new System.IO.FileInfo(currentDungeonConfig.GetRoomsFolderPathByBiomeDifficultyAndRoomSize(currentDungeonConfig.GetDifficulty(), RoomShapeEnum.ROOMSHAPE_1x2)).Length);
                    rnd = Random.Range(0, 1);
                }
                if (room.GetRoomShape() == RoomShapeEnum.ROOMSHAPE_2x2) {
                    // rnd = Random.Range(0, (int)new System.IO.FileInfo(currentDungeonConfig.GetRoomsFolderPathByBiomeDifficultyAndRoomSize(currentDungeonConfig.GetDifficulty(), RoomShapeEnum.ROOMSHAPE_2x1)).Length);
                    rnd = Random.Range(0, 1);
                }
                if (room.GetRoomShape() == RoomShapeEnum.ROOMSHAPE_2x1) {
                    // rnd = Random.Range(0, (int)new System.IO.FileInfo(currentDungeonConfig.GetRoomsFolderPathByBiomeDifficultyAndRoomSize(currentDungeonConfig.GetDifficulty(), RoomShapeEnum.ROOMSHAPE_2x2)).Length);
                    rnd = Random.Range(0, 1);
                }
                roomGo = Resources.Load<GameObject>(currentDungeonConfig.GetRoomsFolderPathByBiomeDifficultyAndRoomSize(currentDungeonConfig.GetDifficulty(), room.GetRoomShape()) + rnd);
            }
            Vector2Int roomPos = room.GetPosition();
            Vector2Int pos = new Vector2Int(roomPos.x * roomSize.x, roomPos.y * roomSize.y);
            Room obj = Instantiate(roomGo, new Vector3(pos.x, pos.y, 0), transform.rotation).GetComponent<Room>();
            obj.transform.parent = floor.transform;
            obj.Setup(roomPos, room.GetRoomShape());
            // room.room = obj;
            // gridOfRoom.Add(obj);
        }
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
    private void InitSpawnRoom() {
        Visit(vectorStart, RoomShapeEnum.ROOMSHAPE_1x1);
        cellQueue.Add(vectorStart);
        listOfPseudoRoom.Add(new PseudoRoom(vectorStart, RoomShapeEnum.ROOMSHAPE_1x1, true));
    }

    /*public Room GetRoomFromVector2Int(Vector2Int position) {
        return gridOfRoom.Find(room => room.GetId() == rooms[position.x + maxCol / 2, position.y + maxCol / 2].id);
    }*/

    private int TryToGenerateAllRoomFloor() {
        floorplanCount = 0;
        // Start BFS pattern
        while (cellQueue.Count > 0 && floorplanCount < maxRooms) {
            RoomShapeEnum roomShape = GetRandomShapeRoom(bound);
            Vector2Int vector = cellQueue[0];
            cellQueue.RemoveAt(0); // get the first of list
            int createdCount = 0;

            if (vector.x > 1 && !CheckLimitOfSpecialRooms(roomShape)) {
                createdCount += CheckIsEmtyPlaceAndAddRoomToQueue(new Vector2Int(vector.x - 1, vector.y), roomShape) ? 1 : 0;
            }
            if (vector.x < roomMaxBound && !CheckLimitOfSpecialRooms(roomShape)) {
                createdCount += CheckIsEmtyPlaceAndAddRoomToQueue(new Vector2Int(vector.x + 1, vector.y), roomShape) ? 1 : 0;
            }
            if (vector.y > 1 && !CheckLimitOfSpecialRooms(roomShape)) {
                createdCount += CheckIsEmtyPlaceAndAddRoomToQueue(new Vector2Int(vector.x, vector.y - 1), roomShape) ? 1 : 0;
            }
            if (vector.y < roomMaxBound && !CheckLimitOfSpecialRooms(roomShape)) {
                createdCount += CheckIsEmtyPlaceAndAddRoomToQueue(new Vector2Int(vector.x, vector.y + 1), roomShape) ? 1 : 0;
            }
            if (createdCount == 0) {
                endRooms.Add(vector);
                listOfPseudoRoom.Find(room => room.GetPosition() == vector).SetIsEndRoom(true);
            }
        }
        return floorplanCount;
    }
    private bool CheckIsEmtyPlaceAndAddRoomToQueue(Vector2Int vector, RoomShapeEnum shape) {
        if (floorplanCount >= maxRooms)
            return false;
        bool isEmptyPlace = Visit(vector, shape);
        if (isEmptyPlace) {
            cellQueue.Add(vector);
            listOfPseudoRoom.Add(new PseudoRoom(vector, shape, false));
            floorplanCount += 1;
        }
        return isEmptyPlace;
    }

    private bool CheckLimitOfSpecialRooms(RoomShapeEnum roomShape) {
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

    private bool Visit(Vector2Int vector, RoomShapeEnum shape) {

        if (Random.value < 0.5f && vector != vectorStart || NeighbourCount(vector, shape) > 1) {
            return false;
        }

        // toDO rajouter le sens T L R B du visite afin de prendre les bonnes distances ex : si 2x2 et sens = L alors on enlève -1 en X 
        // si 2x2 && sens = T alors on ajoute +1 en y
        if (CheckEmptySpace(vector, shape) > 0) {
            return false;
        }

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

        return true;

    }

    private int NeighbourCount(Vector2Int vector, RoomShapeEnum shape) {

        int count = 0;
        Vector2Int[] shapesToCheck = WorldUtils.GetNeighborsByShapes(shape, vector, bound);
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

    private RoomShapeEnum GetRandomShapeRoom(int bound) {
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

}
