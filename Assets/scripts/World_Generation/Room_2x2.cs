using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class Room_2x2 : PseudoRoom {
    public Room_2x2(Vector2Int position, RoomShapeEnum roomShape) : base(position, roomShape) {}

    public override void SeachNeightboors(List<PseudoRoom> listOfPseudoRoom, int bound) {
        // base.SeachNeightboors(listOfPseudoRoom);
        foreach (PseudoRoom room in listOfPseudoRoom) {
            // room.GetCurrentNeightboorsByShape(room)
            Vector2Int[] neighbors = WorldUtils.GetNeighborsByShapes(room.GetRoomShape(), room.GetPosition(), bound);
            int i = 0;
            foreach (var neighborVector in neighbors) {
                i++;
                if(room.GetPosition() == neighborVector) {
                    // doors.Add(Resources.Load<GameObject>('Prefabs/Doors'));
                }
            }
        }
    }

}