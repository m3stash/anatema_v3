using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using DoorNs;
using DungeonNs;
using System;

namespace RoomNs {
    public abstract class Room {

        private Vector2Int roomSize = new Vector2Int(61, 31);
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

        public void SearchNeighborsAndCreateDoor(int[,] floorplan, int bound, BiomeEnum biome) {
            Vector2Int[] sections = GetOccupiedCells(position);
            List<Vector2Int> filteredNeighbors = GetNeighborsCells(position)
                .Where(neighborPosition => !Utilities.CheckIsOutOfBound(neighborPosition, bound) && floorplan[neighborPosition.x, neighborPosition.y] > 0)
                .ToList();

            foreach (Vector2Int section in sections) {
                foreach (DirectionalEnum direction in Enum.GetValues(typeof(DirectionalEnum))) {
                    // get neightbor cell by direction for each section
                    Vector2Int offset = Utilities.GetOffsetForDirection(direction);

                    if (filteredNeighbors.Contains(section + offset)) {
                        Door newDoor = new Door(CalculateDoorPosition(direction, section, position), direction, biome) {
                            DoorNeighbor = new Vector3(section.x + offset.x, section.y + offset.y, 0)
                        };
                        doors.Add(newDoor);
                    }
                }
            }
        }

        private Vector3 CalculateDoorPosition(DirectionalEnum direction, Vector2Int section, Vector2Int roomPosition) {
            int sectionPositionX = section.x - roomPosition.x;
            int sectionPositionY = section.y - roomPosition.y;
            int sectionSizeX = sectionPositionX * DungeonConsts.roomSize.x;
            int sectionSizeY = sectionPositionY * DungeonConsts.roomSize.y;
            
            float middleH = (DungeonConsts.roomSize.x / 2) + (sectionSizeX);
            float middleV = (DungeonConsts.roomSize.y / 2) + (sectionSizeY);

            switch (direction) {
                case DirectionalEnum.T:
                return new Vector3(middleH , DungeonConsts.roomSize.y + sectionSizeY - 0.5f, 0);
                case DirectionalEnum.R:
                return new Vector3(DungeonConsts.roomSize.x + sectionSizeX - .5f, middleV, 0);
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