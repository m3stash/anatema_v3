using System;
using System.Security.Cryptography;
using System.Text;

namespace DungeonNs {

    public class DungeonSeedGenerator: IDungeonSeedGenerator {

        private const int MAX_ROOMS = 20;
        private static DungeonSeedGenerator instance;

        public static IDungeonSeedGenerator GetInstance() {
            instance ??= new DungeonSeedGenerator();
            return instance;
        }

        public int GenerateNumberRoomPerFloor(string seed, int currentFloor) {
            int roomRnd = GetValueFromSeedCharacter(0, seed);
            int decidingFactor = roomRnd > 9 ? roomRnd / 10 : roomRnd;
            int nbOfRooms = Math.Min(decidingFactor, currentFloor + 1);
            int totalRoom = 5 + nbOfRooms + currentFloor;
            // return Math.Min(totalRoom, MAX_ROOMS);
            return MAX_ROOMS;
        }

        public int GetSeedHash(string seed) {
            using SHA256 sha256Hash = SHA256.Create();
            byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(seed));
            // Takes the first 4 bytes and converts them to int
            int hashValue = BitConverter.ToInt32(bytes, 0);
            return hashValue;
        }

        public string GetNewSeed(int length) {
            const string characters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            StringBuilder seed = new StringBuilder(length);

            for (int i = 0; i < length; i++) {
                int randomIndex = UnityEngine.Random.Range(0, characters.Length);
                seed.Append(characters[randomIndex]);
            }

            return seed.ToString();
        }

        private int GetValueFromSeedCharacter(int index, string seed) {
            char character = seed[index % seed.Length];

            if (char.IsLetter(character)) {
                return char.ToUpper(character) - 'A' + 1;
            } else if (char.IsDigit(character)) {
                return character - '0';
            }

            return -1;
        }
    }
}