using System.Collections.Generic;
using UnityEngine;

public class Room_2x2 : PseudoRoom {

    public Room_2x2(Vector2Int position) : base(position) {
        roomShape = RoomShape.ROOMSHAPE_2x2;
    }

}