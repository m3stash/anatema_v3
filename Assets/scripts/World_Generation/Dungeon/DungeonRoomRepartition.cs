﻿using System.Collections.Generic;
using UnityEngine;
using RoomNs;

namespace DungeonNs {

    public class RoomRepartition {

        /*public void InitBackgroundContainer() {
            Instantiate(Resources.Load<GameObject>("Prefabs/Backgrounds/Background_3/BackgroundParallax_" + config.GetBiomeType()));
        }*/

        public static Dictionary<DifficultyEnum, float> SetRoomRepartition(DifficultyEnum difficulty, int sizeFloor, Dictionary<DifficultyEnum, float> roomRepartition) {
            switch (difficulty) {
                case (DifficultyEnum.EASY):
                /*
                 * 70% Easy
                 * 20% Normal
                 * 10% Hard
                 */
                roomRepartition[DifficultyEnum.EASY] = Mathf.Round(sizeFloor * 0.7f);
                roomRepartition[DifficultyEnum.NORMAL] = Mathf.Round(sizeFloor * 0.3f);
                roomRepartition[DifficultyEnum.HARD] = 0;
                break;
                case (DifficultyEnum.NORMAL):
                /*
                * 20% Easy
                * 50% Normal
                * 30% Hard
                */
                roomRepartition[DifficultyEnum.EASY] = Mathf.Round(sizeFloor * 0.2f);
                roomRepartition[DifficultyEnum.NORMAL] = Mathf.Round(sizeFloor * 0.5f);
                roomRepartition[DifficultyEnum.HARD] = Mathf.Round(sizeFloor * 0.3f);
                break;
                case (DifficultyEnum.HARD):
                /*
                * 10% Easy
                * 30% Normal
                * 60% Hard
                */
                roomRepartition[DifficultyEnum.EASY] = Mathf.Round(sizeFloor * 0.1f);
                roomRepartition[DifficultyEnum.NORMAL] = Mathf.Round(sizeFloor * 0.3f);
                roomRepartition[DifficultyEnum.HARD] = Mathf.Round(sizeFloor * 0.6f);
                break;
            }

            return roomRepartition;
            /*Debug.Log("TOTAL ROOM FLOOR - " + (int)(roomRepartition[DifficultyEnum.Easy] + roomRepartition[DifficultyEnum.Normal] + roomRepartition[DifficultyEnum.Hard]));
            Debug.Log("EASY ROOM - " + roomRepartition[DifficultyEnum.Easy]);
            Debug.Log("NORML ROOM - " + roomRepartition[DifficultyEnum.Normal]);
            Debug.Log("HARD ROOM - " + roomRepartition[DifficultyEnum.Hard]);*/
        }

    }
}