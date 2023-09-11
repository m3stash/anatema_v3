using System;
using System.Security.Cryptography;
using System.Text;

namespace DungeonNs {

    public class DungeonValueGeneration {

        private const int MAX_ROOMS = 20;

        public static int GenerateNumberRoomPerFloor(string seed, int currentFloor) {
            int roomRnd = GetValueFromSeedCharacter(0, seed);
            int decidingFactor = roomRnd > 9 ? roomRnd / 10 : roomRnd;
            int nbOfRooms = Math.Min(decidingFactor, currentFloor + 1);
            int totalRoom = 5 + nbOfRooms + currentFloor;
            // return Math.Min(totalRoom, 20);
            return MAX_ROOMS;
        }

        private static void GenerateNumberOfChestPerFloor() {

        }

        private static int GetValueFromSeedCharacter(int index, string seed) {
            char character = seed[index % seed.Length];

            if (char.IsLetter(character)) {
                return char.ToUpper(character) - 'A' + 1;
            } else if (char.IsDigit(character)) {
                return character - '0';
            }

            return -1;
        }

        public static int GetSeedHash(string seed) {
            using SHA256 sha256Hash = SHA256.Create();
            byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(seed));
            // Takes the first 4 bytes and converts them to int
            int hashValue = BitConverter.ToInt32(bytes, 0);
            return hashValue;
        }
    }
}