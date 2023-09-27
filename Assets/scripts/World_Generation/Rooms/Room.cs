using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using DoorNs;
using DungeonNs;
using System;

namespace RoomNs {
    public abstract class Room {

        private readonly Vector2Int roomSize = new Vector2Int(61, 31);

        private Vector2Int worldPosition;
        protected Vector2Int position;
        protected RoomShapeEnum roomShape;
        protected RoomTypeEnum roomType;
        private bool isEndRoom;
        private List<Door> doors = new List<Door>();

        public Room() { }

        public Room(Vector2Int pos) {
            SetWorldPosition(pos);
        }

        /*
         * Get index position of shape
         */
        public abstract Vector2Int GetSizeOfRoom();

        /*
         * Get Size of Room
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

        public void SearchNeighborsAndCreateDoor(IFloorPlanManager floorPlanManager, int bound, BiomeEnum biome, IDungeonUtils dungeonUtils) {
            Vector2Int[] sections = GetOccupiedCells(position);
            List<Vector2Int> filteredNeighbors = GetNeighborsCells(position)
                .Where(neighborPosition => !dungeonUtils.CheckIsOutOfBound(neighborPosition, bound) && floorPlanManager.GetFloorPlanValue(neighborPosition.x, neighborPosition.y) > 0)
                .ToList();

            foreach (Vector2Int section in sections) {
                foreach (DirectionalEnum direction in Enum.GetValues(typeof(DirectionalEnum))) {
                    // get neightbor cell by direction for each section
                    Vector2Int offset = GetOffsetForDirection(direction);

                    if (filteredNeighbors.Contains(section + offset)) {
                        Door newDoor = new Door(CalculateDoorPosition(direction, section, position), direction, biome) {
                            DoorNeighbor = new Vector3(section.x + offset.x, section.y + offset.y, 0)
                        };
                        doors.Add(newDoor);
                    }
                }
            }
        }

        private Vector2Int GetOffsetForDirection(DirectionalEnum direction) {
            switch (direction) {
                case DirectionalEnum.T:
                    return Vector2Int.up;
                case DirectionalEnum.B:
                    return Vector2Int.down;
                case DirectionalEnum.L:
                    return Vector2Int.left;
                case DirectionalEnum.R:
                    return Vector2Int.right;
                default:
                    return Vector2Int.zero;
            }
        }

        private Vector3 CalculateDoorPosition(DirectionalEnum direction, Vector2Int section, Vector2Int roomPosition) {
            int sectionPositionX = section.x - roomPosition.x;
            int sectionPositionY = section.y - roomPosition.y;
            int sectionSizeX = sectionPositionX * roomSize.x;
            int sectionSizeY = sectionPositionY * roomSize.y;
            
            float middleH = (roomSize.x / 2) + (sectionSizeX);
            float middleV = (roomSize.y / 2) + (sectionSizeY);

            switch (direction) {
                case DirectionalEnum.T:
                return new Vector3(middleH , roomSize.y + sectionSizeY - 0.5f, 0);
                case DirectionalEnum.R:
                return new Vector3(roomSize.x + sectionSizeX - .5f, middleV, 0);
                case DirectionalEnum.B:
                return new Vector3(middleH, 0.5f + sectionSizeY, 0);
                case DirectionalEnum.L:
                return new Vector3(sectionSizeX + 0.5f, middleV, 0);
                default:
                Debug.LogError("ERROR on method CalculateDoorPosition no direction find for DirectionalEnum " + direction);
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

        public List<Door> GetDoors() {
            return doors;
        }

        public Vector2Int GetWorldPosition() {
            return worldPosition;
        }

    }
}