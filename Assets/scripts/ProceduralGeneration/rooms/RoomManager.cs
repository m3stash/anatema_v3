using RoomNs;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static DungeonNs.SpecialRoomManager;

namespace DungeonNs {
    public class RoomManager : IRoomManager {

        private static RoomManager instance;
        private IDungeonFloorValues dungeonFloorValues;
        private IRoomFactory roomFactory;
        private List<Room> listOfRoom;
        private const float ratio = 0.25f;
        private IFloorPlanManager floorPlanManager;
        private List<RoomShapeEnum> roomShapes;
        private SpecialRoomManager specialRoomManager;

        public static RoomManager GetInstance(IDungeonFloorValues dungeonFloorValues, IFloorPlanManager floorPlanManager) {
            instance ??= new RoomManager(dungeonFloorValues, floorPlanManager);
            return instance;
        }

        private RoomManager(IDungeonFloorValues dungeonFloorValues, IFloorPlanManager floorPlanManager) {
            listOfRoom = new List<Room>();
            this.dungeonFloorValues = dungeonFloorValues;
            roomFactory = RoomFactory.GetInstance();
            this.floorPlanManager = floorPlanManager;
            Setup();
        }

        private void Setup() {
            roomShapes = GetListOfSpecialShapes();
            specialRoomManager = new SpecialRoomManager(dungeonFloorValues.GetVectorStart(), floorPlanManager);
        }

        public void InitializeRooms() {
            CreateStandardRooms();
            CreateSpecialRooms();
        }

        private void CreateSpecialRooms() {
            List<SpecialRoom> specialRooms = specialRoomManager.CreateSpecialRooms();
            specialRooms.ForEach(specialRoom => {
                Vector2Int vector = specialRoom.GetVector();
                Room room = InstantiateRoomImplWithProperties(specialRoom.GetShape(), vector, specialRoom.GetTypeEnum());
                AddRoom(room);
                floorPlanManager.SetFloorPlanValue(vector.x, vector.y, 1);
            });
        }

        private void CreateStandardRooms() {
            int currentShapeIndex = 0;

            Vector2Int vectorStart = dungeonFloorValues.GetVectorStart();
            InitializeStarterRoom(vectorStart);

            Queue<Vector2Int> roomPositions = new Queue<Vector2Int>();
            roomPositions.Enqueue(vectorStart);

            while (ShouldContinueCreatingRooms(roomPositions)) {
                Vector2Int currentRoomPosition = DequeueRandomRoomPosition(roomPositions);
                Room room = GenerateRoom(roomShapes, ref currentShapeIndex);

                TryPlaceAndAddRoom(room, currentRoomPosition, roomPositions);
            }
        }

        private void TryPlaceAndAddRoom(Room room, Vector2Int currentRoomPosition, Queue<Vector2Int> roomPositions) {
            if (TryPlaceRoom(room, currentRoomPosition, out Vector2Int newRoomPosition)) {
                EnqueueNewRoomPosition(newRoomPosition, roomPositions);
                AddRoomAndUpdateFloorPlan(room, newRoomPosition);
            }
        }

        private void EnqueueNewRoomPosition(Vector2Int newRoomPosition, Queue<Vector2Int> roomPositions) {
            roomPositions.Enqueue(newRoomPosition);
        }

        private void AddRoomAndUpdateFloorPlan(Room room, Vector2Int newRoomPosition) {
            AddRoom(room);
            SetFloorPlanByRoom(room, newRoomPosition, GetListOfRoom().Count + 1);
        }

        private Vector2Int DequeueRandomRoomPosition(Queue<Vector2Int> queue) {
            int randomIndex = dungeonFloorValues.GetNextRandomValue(queue.Count);
            Vector2Int[] array = queue.ToArray();
            Vector2Int randomElement = array[randomIndex];
            queue = new Queue<Vector2Int>(array.Where(element => element != randomElement));
            return randomElement;
        }

        private void SetFloorPlanByRoom(Room room, Vector2Int vector, int index) {
            foreach (var cell in room.GetSections(vector)) {
                floorPlanManager.SetFloorPlanValue(cell.x, cell.y, index);
            }
        }

        private List<Vector2Int> GetEmptySpaces(Room room, Vector2Int position) {
            return room.GetDirections(position)
                .Where(Vector2Int => CanAddShape(Vector2Int, room))
                .ToList();
        }

        private int NeighborCount(Vector2Int vector, Room room) {
            int count = 0;
            Vector2Int[] shapesToCheck = room.GetNeighborsCells(vector);

            if (shapesToCheck.Length == 0) {
                return -1;
            }
            foreach (var checkNewPlace in shapesToCheck) {
                if (!floorPlanManager.CheckIsOutOfBound(checkNewPlace, floorPlanManager.GetFloorPlanBound())) {
                    int neighbour = floorPlanManager.GetFloorPlanValue(checkNewPlace.x, checkNewPlace.y) > 0 ? 1 : 0;
                    count += neighbour;
                }
            }
            return count;
        }

        private bool CanAddShape(Vector2Int vector, Room room) {
            if (!CheckIsEmptySpace(vector, room)) {
                return false;
            }
            if (NeighborCount(vector, room) > 1) {
                return false;
            }
            return true;
        }

        private bool CheckIsEmptySpace(Vector2Int vector, Room room) {
            Vector2Int[] cells = room.GetSections(vector);
            int usedCells = cells.Sum(cell => floorPlanManager.CheckIsOutOfBound(cell, floorPlanManager.GetFloorPlanBound()) ? 1 : floorPlanManager.GetFloorPlanValue(cell.x, cell.y));
            return usedCells == 0;
        }

        private bool TryPlaceRoom(Room room, Vector2Int roomPosition, out Vector2Int newRoomPosition) {
            List<Vector2Int> listOfEmptySpaces = GetEmptySpaces(room, roomPosition);

            if (listOfEmptySpaces.Count == 0) {
                newRoomPosition = default;
                return false;
            }

            int randomNeighbor = dungeonFloorValues.GetNextRandomValue(listOfEmptySpaces.Count);
            newRoomPosition = listOfEmptySpaces[randomNeighbor];
            room.SetPosition(newRoomPosition);
            room.SetRoomType(RoomTypeEnum.STANDARD);
            return true;
        }

        private bool ShouldContinueCreatingRooms(Queue<Vector2Int> roomPositions) {
            return roomPositions.Count > 0 && GetListOfRoom().Count < dungeonFloorValues.GetNumberOfRooms();
        }

        private void InitializeStarterRoom(Vector2Int vectorStart) {
            Room starterRoom = new Room_R1X1(vectorStart);
            starterRoom.SetRoomType(RoomTypeEnum.STARTER);
            AddRoom(starterRoom);
            //TODO debug ici pkoi appeler une boucle ???
            SetFloorPlanByRoom(starterRoom, vectorStart, 1);
        }

        private List<RoomShapeEnum> GetListOfSpecialShapes() {
            return Enum.GetValues(typeof(RoomShapeEnum))
               .Cast<RoomShapeEnum>()
               .Where(shape => shape != RoomShapeEnum.R1X1)
               .ToList();
        }

        private bool CheckProportionalShapeDistribution(List<Room> rooms) {
            int specials = rooms.Count(r => r.GetShape() != RoomShapeEnum.R1X1);
            double currentRatio = (double)specials / rooms.Count;
            return currentRatio <= ratio;
        }

        // use Knuth Algorithm to random shuffle to ensure that room shapes are evenly distributed throughout the level.
        private void ShuffleShapes<T>(List<T> list, System.Random random) {
            int n = list.Count;
            while (n > 1) {
                n--;
                int k = random.Next(n + 1);
                (list[n], list[k]) = (list[k], list[n]); // tuples desconstruction
            }
        }

        // Todo error : Principe de responsabilité unique (Single Responsibility Principle)
        public Room GenerateRoom(List<RoomShapeEnum> roomShapes, ref int currentShapeIndex) {
            try {
                RoomShapeEnum newRoomShape = RoomShapeEnum.R1X1;
                if (CheckProportionalShapeDistribution(listOfRoom)) {
                    ShuffleShapes(roomShapes, dungeonFloorValues.GetRandomFromSeedHash());
                    newRoomShape = roomShapes[currentShapeIndex];
                    currentShapeIndex = (currentShapeIndex + 1) % roomShapes.Count;
                }
                return roomFactory.InstantiateRoomImpl(newRoomShape);
            } catch (TypeLoadException ex) {
                Debug.Log("Error generating Room: " + ex.Message);
                return null;
            }
        }

        public Room InstantiateRoomImplWithProperties(RoomShapeEnum shape, Vector2Int vector, RoomTypeEnum type) {
            Room room = roomFactory.InstantiateRoomImpl(shape);
            room.SetPosition(vector);
            room.SetRoomType(type);
            return room;
        }

        public GameObject InstantiateRoomPrefab(DifficultyEnum diff, RoomShapeEnum shape, RoomTypeEnum type, IDungeonFloorValues dungeonFloorValues, BiomeEnum biome) {
            return roomFactory.InstantiateRoomPrefab(diff, shape, type, dungeonFloorValues, biome);
        }

        public GameObject InstantiateRoomGO(GameObject roomPrefab, Vector3 vector3, Transform transform, GameObject floorContainer) {
            return roomFactory.InstantiateRoomGO(roomPrefab, vector3, transform, floorContainer);
        }

        public List<Room> GetListOfRoom() {
            return listOfRoom;
        }

        public void AddRoom(Room room) {
            listOfRoom.Add(room);
        }

        public Room GetNextRoom() {
            if (listOfRoom.Count > 0) {
                Room room = listOfRoom[0];
                listOfRoom.RemoveAt(0);
                return room;
            }
            return null;
        }
    }
}
