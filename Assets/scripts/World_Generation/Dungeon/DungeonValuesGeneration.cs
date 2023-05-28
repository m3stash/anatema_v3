using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
            if (roomRnd > 9) {
                int getTens = roomRnd > 9 ? (roomRnd / 10) % 10 : roomRnd;
                nbOfRooms = getTens > currentFloor + 1 ? currentFloor + 1 : getTens;
            } else {
                nbOfRooms = roomRnd > currentFloor + 1 ? currentFloor + 1 : roomRnd;
            }
            //numberRoomForFloor = CalculNumberOfRoomPerfFloor(currentFloor);
            int totalRoom = 5 + nbOfRooms + currentFloor;
            return totalRoom > 20 ? 20 : totalRoom;
        }

        private static void GenerateNumberOfChestPerFloor() {

        }

        private static int GetNumberBySeedIndex(int index, string seed) {
            if (char.IsLetter(seed[index])) {
                return seed[index] - 'A' + 1;
            }
            return Convert.ToInt32(seed[index]);
        }

        /*private int CalculNumberOfRoomPerfFloor(int floorNbr) {
            int floorRandValue = Mathf.FloorToInt(floorNbr * 3.4f + UnityEngine.Random.Range(4, 6));
            return floorRandValue > 20 ? 20 : floorRandValue;
        }*/

    }
}