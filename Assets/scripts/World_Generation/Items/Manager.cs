using System;
using UnityEngine;

namespace Item {
    public class Manager : IManager {

        private static Manager instance;
        private IDungeonFloorValues dungeonFloorValues;

        public static Manager GetInstance(IDungeonFloorValues dungeonFloorValues) {
            instance ??= new Manager(dungeonFloorValues);
            return instance;
        }

        private Manager(IDungeonFloorValues dungeonFloorValues) {
            this.dungeonFloorValues = dungeonFloorValues;
        }

        public void GenerateItems() {
            foreach (TypeEnum value in Enum.GetValues(typeof(TypeEnum))) {
                switch (value) {
                    case TypeEnum.CAPACITY:
                        Configuration capacity = new Capacity();
                        break;
                    case TypeEnum.CHEST:
                        Configuration chest = new Chest();
                        break;
                    case TypeEnum.CONSUMABLE:
                        break;
                    case TypeEnum.PEDESTRAL:
                        break;
                    case TypeEnum.RESOURCE:
                        break;
                    default:
                        Debug.LogError("Generate Items not exist for this value: " + value);
                        break;
                }
            }
        }
    }
}


