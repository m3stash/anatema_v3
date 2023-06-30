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
            Debug.LogWarning("number " + number);
            if (char.IsLetter(seed[number])) {
                return seed[number] - 'A' + 1;
            }
            return Convert.ToInt32(seed[number]);
        }

        public static List<RoomShapeEnum> GenerateRandomShapeByBiome(int numberRooms, string seed, int currentFloor) {

            int current_R2X2 = 0;
            int current_R1X2 = 0;
            int current_R2X1 = 0;
            List<RoomShapeEnum> enumShapes = new List<RoomShapeEnum>();
            int value = GetNumberBySeedIndex(4, seed); // toDo revoir ça avec un md5 pour la value

            for(var i = 0; i < numberRooms; i++) {

                int rng = numberRooms + currentFloor + value + i;
                // bool chanceToCancelRoom = (rng > 9 ? (rng / 10) % 10 : rng) > 4 ? true : false;
                int random = Random.Range(0, 3);
                enumShapes.Add(RoomShapeEnum.R1X1);

                if(random == 2) {
                    if (current_R2X2 < DungeonConsts.max_R2X2) {
                        current_R2X2++;
                        enumShapes.RemoveAt(i);
                        enumShapes.Add(RoomShapeEnum.R2X2);
                    }
                }

                /*if (chanceToCancelRoom) {
                    continue;
                }
                if (rng < 15) {
                    if (current_R2X2 < DungeonConsts.max_R2X2) {
                        current_R2X2++;
                        enumShapes.RemoveAt(i);
                        enumShapes.Add(RoomShapeEnum.R2X2);
                    }
                }
                if (rng >= 15 && UnityEngine.Random.value < 32) {
                    if (current_R1X2 < DungeonConsts.max_R1X2) {
                        current_R1X2++;
                        enumShapes.RemoveAt(i);
                        // enumShapes.Add(RoomShapeEnum.R1X2);
                        enumShapes.Add(RoomShapeEnum.R2X2);
                    }
                }
                if (rng >= 32) {
                    if (current_R2X1 < DungeonConsts.max_R2X1) {
                        current_R2X1++;
                        enumShapes.RemoveAt(i);
                        // enumShapes.Add(RoomShapeEnum.R2X1);
                        enumShapes.Add(RoomShapeEnum.R2X2);
                    }
                }*/
            }
            return enumShapes;
        }

    }
}