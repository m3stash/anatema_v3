using RoomNs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;

namespace DungeonNs {
    public class RoomManager {
        private IDungeonInitializer dungeonInitializer;
        private IRoomFactory roomFactory;
        private List<Room> listOfRoom;

        public RoomManager(IDungeonInitializer initializer, IRoomFactory factory) {
            listOfRoom = new List<Room>();
            dungeonInitializer = initializer;
            roomFactory = factory;
        }

        public Room GenerateRoom(List<RoomShapeEnum> roomShapes, ref int currentShapeIndex) {
            try {
                RoomShapeEnum newRoomShape = RoomShapeEnum.R1X1;
                if (CheckProportionalShapeDistribution(listOfRoom)) {
                    ShuffleShapes(roomShapes, dungeonInitializer.GetRandomFromSeedHash());
                    newRoomShape = roomShapes[currentShapeIndex];
                    currentShapeIndex = (currentShapeIndex + 1) % roomShapes.Count;
                }
                return roomFactory.CreateRoom(newRoomShape);
            } catch (TypeLoadException ex) {
                Debug.Print("Error generating Room: " + ex.Message);
                return null;
            }
        }

        private bool CheckProportionalShapeDistribution(List<Room> rooms) {
            int specials = rooms.Count(r => r.GetShape() != RoomShapeEnum.R1X1);
            double currentRatio = (double)specials / rooms.Count;
            return currentRatio <= 0.25;
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
