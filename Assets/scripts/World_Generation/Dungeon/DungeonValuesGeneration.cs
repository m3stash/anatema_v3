using RoomNs;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEditor.Rendering.BuiltIn.ShaderGraph;
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
            // toDO à revoir pas assez précis..
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

        public static RoomShapeEnum[] GenerateRandomShapeByBiome(int numberRooms, string seed, int currentFloor) {

            int current_R2X2 = 0;
            int current_R1X2 = 0;
            int current_R2X1 = 0;
            RoomShapeEnum[] enumShapes = new RoomShapeEnum[numberRooms];
            int value = GetNumberBySeedIndex(4, seed);

            for(var i = 0; i < numberRooms; i++) {

                int rng = numberRooms + currentFloor + value + i;
                bool chanceToCancelRoom = (rng > 9 ? (rng / 10) % 10 : rng) > 4 ? true : false;
                enumShapes[i] = RoomShapeEnum.R1X1;

                if (chanceToCancelRoom) {
                    continue;
                }
                if (rng < 16) {
                    if (current_R2X2 < DungeonConsts.max_R2X2) {
                        current_R2X2++;
                        enumShapes[i] = RoomShapeEnum.R2X2;
                    }
                }
                if (rng > 15 && UnityEngine.Random.value < 31) {
                    if (current_R1X2 < DungeonConsts.max_R1X2) {
                        current_R1X2++;
                        enumShapes[i] = RoomShapeEnum.R1X2;
                    }
                }
                if (rng > 32) {
                    if (current_R2X1 < DungeonConsts.max_R2X1) {
                        current_R2X1++;
                        enumShapes[i] = RoomShapeEnum.R2X1;
                    }
                }
            }
            return enumShapes;
        }

        public static bool TossUp(int index, string seed) {
            int rng = GetNumberBySeedIndex(index, seed);
            return (rng > 9 ? (rng / 10) % 10 : rng) > 4 ? true : false;
        }

        /*private int CalculNumberOfRoomPerfFloor(int floorNbr) {
            int floorRandValue = Mathf.FloorToInt(floorNbr * 3.4f + UnityEngine.Random.Range(4, 6));
            return floorRandValue > 20 ? 20 : floorRandValue;
        }*/

    }
}