using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using DoorNs;
using DungeonNs;
using System;

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
        public abstract Vector2Int[] GetOccupiedCells(Vector2Int vector);

        /*
         * Get GetNeighbors of cell
         */
        public abstract Vector2Int[] GetNeighborsCells(Vector2Int vector);

        public RoomTypeEnum GetRoomTypeEnum { get { return roomType; } }

        public void SeachNeighborsAndCreatePseudoDoor(int[,] floorplan, int bound, BiomeEnum biome) {
            Vector2Int[] sections = GetOccupiedCells(position);
            List<Vector2Int> filteredNeighbors = GetNeighborsCells(position)
                .Where(neighborPosition => !Utilities.CheckIsOutOfBound(neighborPosition, bound) && floorplan[neighborPosition.x, neighborPosition.y] > 0)
                .ToList();

            foreach (Vector2Int section in sections) {
                foreach (DirectionalEnum direction in Enum.GetValues(typeof(DirectionalEnum))) {
                    Vector2Int offset = Utilities.GetOffsetForDirection(direction);

                    if (filteredNeighbors.Contains(section + offset)) {
                        PseudoDoor newDoor = new PseudoDoor(CalculateDoorPosition(section - position, direction), direction, biome);
                        newDoor.DoorNeighbor = new Vector3(section.x + offset.x, section.y + offset.y, 0);
                        doors.Add(newDoor);
                    }
                }
            }
        }

        private Vector3 CalculateDoorPosition(Vector2Int position, DirectionalEnum direction) {
            float middleH = (DungeonConsts.roomSize.x / 2) * (position.x + 1);
            float middleV = (DungeonConsts.roomSize.y / 2) * (position.y + 1);

            switch (direction) {
                case DirectionalEnum.T:
                return new Vector3(middleH, DungeonConsts.roomSize.y * (position.y + 1) - 0.5f, 0);
                case DirectionalEnum.R:
                return new Vector3(DungeonConsts.roomSize.x * (position.x + 1) - .5f, middleV, 0);
                case DirectionalEnum.B:
                return new Vector3(middleH, 0.5f, 0);
                case DirectionalEnum.L:
                return new Vector3(0.5f, middleV, 0);
                default:
                Debug.LogError("ERROR CalculDoorPosition");
                return Vector3Int.zero;
            }
        }

        private void SetWorldPosition(Vector2Int pos) {
            worldPosition = new Vector2Int(pos.x * roomSize.x, pos.y * roomSize.y);
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