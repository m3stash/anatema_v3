using System.Collections.Generic;
using UnityEngine;

namespace RoomNs {
    public class Room_1x1 : PseudoRoom {

        public Room_1x1(Vector2Int position) : base(position) {
            roomShape = RoomShape.ROOMSHAPE_1x1;
        }

    }
}

