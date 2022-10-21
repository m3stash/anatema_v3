using System.Collections.Generic;
using UnityEngine;

public class Room_1x2 : PseudoRoom {

    public Room_1x2(Vector2Int position) : base(position) {
        roomShape = RoomShape.ROOMSHAPE_1x2;
    }

}