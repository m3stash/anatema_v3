using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DungeonNs {

    public class DungeonValues {

        private int numberOfRoom;

        public void SetNumberOfRooms(int value) {
            numberOfRoom = value;
        }

        public int GetNumberOfRooms() {
            return numberOfRoom;
        }

    }
}