using RoomNs;
using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace DungeonNs {

    public class DungeonValueGeneration {

        public static DungeonValues CreateRandomValues(string seed, int currentFloor) {
            DungeonValues values = new DungeonValues();
            values.SetNumberOfRooms(GenerateNumberRoomPerFloor(seed, currentFloor));
            GenerateNumberOfChestPerFloor();
            return values;
        }

        private static int GenerateNumberRoomPerFloor(string seed, int currentFloor) {
            int nbOfRooms = 0;
            int roomRnd = GetNumberBySeedIndex(0, seed);
            // toDO à revoir pas assez précis..
            if (roomRnd > 9) {
                int getTens = roomRnd > 9 ? (roomRnd / 10) % 10 : roomRnd;
                nbOfRooms = getTens > currentFloor + 1 ? currentFloor + 1 : getTens;
            } else {
                nbOfRooms = roomRnd > currentFloor + 1 ? currentFloor + 1 : roomRnd;
            }
            //numberRoomForFloor = CalculNumberOfRoomPerfFloor(currentFloor);
            int totalRoom = 5 + nbOfRooms + currentFloor;
            // return totalRoom > 20 ? 20 : totalRoom;
            return 20;
        }

        private static void GenerateNumberOfChestPerFloor() {

        }

        private static int GetNumberBySeedIndex(int index, string seed) {
            int number = index;
            if(number > seed.Length) {
                // Debug.LogError("GetNumberBySeedIndex index: "+ index + "is > than " + seed.Length);
                number = number % seed.Length;
            }
            if (char.IsLetter(seed[number])) {
                return seed[number] - 'A' + 1;
            }
            return Convert.ToInt32(seed[number]);
        }

    }
}