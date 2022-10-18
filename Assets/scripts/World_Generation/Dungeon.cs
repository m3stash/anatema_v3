using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dungeon : MonoBehaviour {

    private Dictionary<DifficultyEnum, float> roomRepartition = new Dictionary<DifficultyEnum, float>();

    [SerializeField] private DungeonConfig config;

    public void Setup(DungeonConfig config) {
        this.config = config;
        RoomRepartition(config.GetDifficulty(), config.GetRoomSize());
    }

    public DungeonConfig GetConfig() {
        return config;
    }

    /*public void InitBackgroundContainer() {
        Instantiate(Resources.Load<GameObject>("Prefabs/Backgrounds/Background_3/BackgroundParallax_" + config.GetBiomeType()));
    }*/

    public void RoomRepartition(DifficultyEnum difficulty, RoomSizeEnum size) {
        int sizeFloor = (int)size;
        switch (difficulty) {
            case (DifficultyEnum.Easy):
            /*
             * 70% Easy
             * 20% Normal
             * 10% Hard
             */
            roomRepartition[DifficultyEnum.Easy] = Mathf.Round((float)sizeFloor * 70 / 100);
            roomRepartition[DifficultyEnum.Normal] = Mathf.Round((float)sizeFloor * 20 / 100);
            roomRepartition[DifficultyEnum.Hard] = Mathf.Round((float)sizeFloor * 10 / 100);
            break;
            case (DifficultyEnum.Normal):
            /*
            * 20% Easy
            * 50% Normal
            * 30% Hard
            */
            roomRepartition[DifficultyEnum.Easy] = Mathf.Round((float)sizeFloor * 20 / 100);
            roomRepartition[DifficultyEnum.Normal] = Mathf.Round((float)sizeFloor * 50 / 100);
            roomRepartition[DifficultyEnum.Hard] = Mathf.Round((float)sizeFloor * 30 / 100);
            break;
            case (DifficultyEnum.Hard):
            /*
            * 10% Easy
            * 30% Normal
            * 60% Hard
            */
            roomRepartition[DifficultyEnum.Easy] = Mathf.Round((float)sizeFloor * 10 / 100);
            roomRepartition[DifficultyEnum.Normal] = Mathf.Round((float)sizeFloor * 30 / 100);
            roomRepartition[DifficultyEnum.Hard] = Mathf.Round((float)sizeFloor * 60 / 100);
            break;
            default:
            break;
        }
        /*Debug.Log("TOTAL ROOM FLOOR - " + (int)(roomRepartition[DifficultyEnum.Easy] + roomRepartition[DifficultyEnum.Normal] + roomRepartition[DifficultyEnum.Hard]));
        Debug.Log("EASY ROOM - " + roomRepartition[DifficultyEnum.Easy]);
        Debug.Log("NORML ROOM - " + roomRepartition[DifficultyEnum.Normal]);
        Debug.Log("HARD ROOM - " + roomRepartition[DifficultyEnum.Hard]);*/
    }

}
