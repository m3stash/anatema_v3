using RoomNs;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DungeonNs {
    public class RoomManager : IRoomManager {
        private IDungeonFloorValues dungeonFloorValues;
        private IRoomFactory roomFactory;
        private List<Room> listOfRoom;
        private const float ratio = 0.25f;


        public RoomManager(IDungeonFloorValues dungeonFloorValues, IRoomFactory factory) {
            listOfRoom = new List<Room>();
            this.dungeonFloorValues = dungeonFloorValues;
            roomFactory = factory;
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

        public void SetRoomProperties(Room room, Vector2Int vector) {
            
        }

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
