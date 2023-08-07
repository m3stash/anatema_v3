using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using DoorNs;
using DungeonNs;

namespace RoomNs {
    public abstract class PseudoRoom {

        private Vector2Int roomSize = new Vector2Int(61, 31);
        private Vector2Int worldPosition;
        protected Vector2Int position;
        protected RoomShapeEnum roomShape;
        protected RoomTypeEnum roomType;
        private bool isEndRoom;
        private List<PseudoDoor> doors = new List<PseudoDoor>();

        public PseudoRoom() { }

        public PseudoRoom(Vector2Int pos) {
            SetWorldPosition(pos);
        }

        /*
         * Get index position of shape
         */
        public abstract Vector2Int[] GetDirections(Vector2Int vector);

        /*
         * Get occuped Cell's for shape
         */
        public abstract Vector2Int[] GetOccupedCells(Vector2Int vector);

        /*
         * Get GetNeighbors of cell
         */
        public abstract Vector2Int[] GetNeighborsCells(Vector2Int vector);

        public RoomTypeEnum GetRoomTypeEnum { get { return roomType; } }

        /*public void SeachNeighborsAndCreateDoor(List<PseudoRoom> pseudoRooms) {
            List<PseudoRoom> roomList = new List<PseudoRoom>(pseudoRooms);
            Vector2Int[] currentSectionsRoom = WorldUtils.GetSectionPerRoom(GetShape(), GetPosition());
            foreach (PseudoRoom room in roomList.ToList()) {
                if (room != this) {
                    Vector2Int[] neighborSectionsRoom = WorldUtils.GetSectionPerRoom(room.GetShape(), room.GetPosition());
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
        }*/

        // to do enlever la duplication !!
        public bool CheckIsOutOfBound(Vector2Int vector, int floorplanBound) {
            return vector.x < 0 || vector.x > floorplanBound || vector.y > floorplanBound || vector.y < 0;
        }

        public void SeachNeighborsAndCreateDoor(int[,] floorPlan, int bound) {
            List<Vector2Int> filteredNeighbors = new List<Vector2Int>();
            Vector2Int[] sections = GetOccupedCells(position);
            filteredNeighbors.AddRange(
                GetNeighborsCells(position)
                    .Where(neighborPosition => !CheckIsOutOfBound(neighborPosition, bound) && floorPlan[neighborPosition.x, neighborPosition.y] == 1)
            );

            foreach (Vector2Int section in sections) {
                // top
                if (filteredNeighbors.Contains(new Vector2Int(section.x, section.y + 1))) {
                    PseudoDoor newDoor = new PseudoDoor(CalculDoorPosition(DirectionalEnum.T), DirectionalEnum.T);
                    newDoor.SetDoorNeighbor(new Vector3(section.x, section.y + 1, 0));
                    doors.Add(newDoor);
                }
                /*// bottom
                if (filteredNeighbors.Contains(new Vector2Int(section.x, section.y - 1))) {
                    PseudoDoor newDoor = new PseudoDoor(CalculDoorPosition(DirectionalEnum.B), DirectionalEnum.B);
                    newDoor.SetDoorNeighbor(new Vector3(0, - 1, 0));
                    doors.Add(newDoor);
                }*/
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

        private void SetWorldPosition(Vector2Int pos) {
            worldPosition = new Vector2Int(pos.x * roomSize.x, pos.y * roomSize.y);
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
                Debug.LogError("ERROR CalculateDoorPosition");
                return Vector3Int.zero;
            }
        }

        public void SetIsEndRoom(bool value) {
            isEndRoom = value;
        }

        public bool GetIsEndRoom() {
            return isEndRoom;
        }

        public Vector2Int GetPosition() {
            return position;
        }

        public void SetPosition(Vector2Int pos) {
            SetWorldPosition(pos);
            position = pos;
        }

        public void SetRoomType(RoomTypeEnum type) {
            roomType = type;
        }

        public RoomShapeEnum GetShape() {
            return roomShape;
        }

        public List<PseudoDoor> GetDoors() {
            return doors;
        }

        public Vector2Int GetWorldPosition() {
            return worldPosition;
        }

    }
}