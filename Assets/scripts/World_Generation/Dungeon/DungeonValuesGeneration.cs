using System;
using System.Security.Cryptography;
using System.Text;

namespace DungeonNs {

    public class DungeonValueGeneration {

        public static DungeonValues CreateRandomValues(string seed, int currentFloor) {
            DungeonValues values = new DungeonValues();
            values.SetNumberOfRooms(GenerateNumberRoomPerFloor(seed, currentFloor));
            GenerateNumberOfChestPerFloor();
            return values;
        }

        private static int GenerateNumberRoomPerFloor(string seed, int currentFloor) {
            int roomRnd = GetNumberBySeedIndex(0, seed);
            int decidingFactor = roomRnd > 9 ? roomRnd / 10 : roomRnd;
            int nbOfRooms = Math.Min(decidingFactor, currentFloor + 1);
            int totalRoom = 5 + nbOfRooms + currentFloor;
            // return Math.Min(totalRoom, 20);
            return 20;
        }

        private static void GenerateNumberOfChestPerFloor() {

        }

        private static int GetNumberBySeedIndex(int index, string seed) {
            int number = index;

            if (number >= seed.Length) {
                number = number % seed.Length;
            }

            if (char.IsLetter(seed[number])) {
                return seed[number] - 'A' + 1;
            } else if (char.IsDigit(seed[number])) {
                return seed[number] - '0';
            }

            return -1;
        }

        public static int GetSeedHash(string seed) {
            using (SHA256 sha256Hash = SHA256.Create()) {
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(seed));
                // Takes the first 4 bytes and converts them to int (this is a simplification)
                int hashValue = BitConverter.ToInt32(bytes, 0);
                return hashValue;
            }
        }
    }
}