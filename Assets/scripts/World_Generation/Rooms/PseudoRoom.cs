using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using DoorNs;
using DungeonNs;
namespace RoomNs {
    public class PseudoRoom {

        private Vector2Int position;
        protected RoomShape roomShape;
        private bool isStartRoom;
        private bool isEndRoom;
        private List<PseudoDoor> doors = new List<PseudoDoor>();

        public PseudoRoom(Vector2Int position, bool isStartRoom = false) {
            this.position = position;
            this.isStartRoom = isStartRoom;
        }

        public void SeachNeighborsAndCreateDoor(List<PseudoRoom> pseudoRooms) {
            List<PseudoRoom> roomList = new List<PseudoRoom>(pseudoRooms);
            Vector2Int[] currentSectionsRoom = WorldUtils.GetSectionPerRoom(GetRoomShape(), GetPosition());
            // base.SeachNeighbors(listOfPseudoRoom);
            foreach (PseudoRoom room in roomList.ToList()) {
                if (room != this) {
                    Vector2Int[] neighborSectionsRoom = WorldUtils.GetSectionPerRoom(room.GetRoomShape(), room.GetPosition());
                    foreach (var currentSections in currentSectionsRoom) {
                        foreach (var neighborSection in neighborSectionsRoom) {
                            DirectionalEnum doorDirection = SearchNeighborAndGetDoorDirection(currentSections, neighborSection);
                            if (doorDirection != DirectionalEnum.DEFAULT) {
                                PseudoDoor newDoor = new PseudoDoor(CalculDoorPosition(doorDirection), doorDirection);
                                newDoor.SetDoorNeighbor(new Vector3(neighborSection.x, neighborSection.y, 0));
                                doors.Add(newDoor);
                            }
                        }
                    }
                }
                roomList.Remove(room);
            }
        }

        private DirectionalEnum SearchNeighborAndGetDoorDirection(Vector2Int sectionPos, Vector2Int neightborSectionPos) {
            if (new Vector2Int(sectionPos.x, sectionPos.y + 1) == neightborSectionPos) {
                return DirectionalEnum.T;
            }
            if (new Vector2Int(sectionPos.x + 1, sectionPos.y) == neightborSectionPos) {
                return DirectionalEnum.R;
            }
            if (new Vector2Int(sectionPos.x - 1, sectionPos.y) == neightborSectionPos) {
                return DirectionalEnum.L;
            }
            if (new Vector2Int(sectionPos.x, sectionPos.y - 1) == neightborSectionPos) {
                return DirectionalEnum.B;
            }
            return DirectionalEnum.DEFAULT;
        }

        private Vector3 CalculDoorPosition(DirectionalEnum direction) {
            switch (direction) {
                case DirectionalEnum.T:
                return new Vector3((float)(DungeonConsts.roomSize.x / 2) + .5f, DungeonConsts.roomSize.y - .5f, 0);
                case DirectionalEnum.R:
                return new Vector3((float)DungeonConsts.roomSize.x - .5f, (float)DungeonConsts.roomSize.y / 2, 0);
                case DirectionalEnum.B:
                return new Vector3((float)DungeonConsts.roomSize.x / 2, .5f, 0);
                case DirectionalEnum.L:
                return new Vector3(.5f, (float)DungeonConsts.roomSize.y / 2, 0);
                default:
                Debug.Log("ERROR CalculateDoorPosition");
                return Vector3Int.zero;
            }
        }

        public void SetIsEndRoom(bool value) {
            isEndRoom = value;
        }

        public bool GetIsStartRoom() {
            return isStartRoom;
        }

        public bool GetIsEndRoom() {
            return isEndRoom;
        }

        public Vector2Int GetPosition() {
            return position;
        }

        public RoomShape GetRoomShape() {
            return roomShape;
        }

        public List<PseudoDoor> GetDoors() {
            return doors;
        }
    }
}