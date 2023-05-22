using System.Collections.Generic;
using UnityEngine;
using RoomNs;

namespace DungeonNs {

    public class RoomRepartition : MonoBehaviour {
        /*
         * toDO => rajouter la répartition des rooms dans la gestion de la difficulté des ennemies au moment de la génération de l'étage !
         */
        private Dictionary<DifficultyEnum, float> roomRepartition = new Dictionary<DifficultyEnum, float>();

        [SerializeField] private Config config;

        public void Setup(Config config) {
            this.config = config;
            // GetRoomRepartition(config.GetDifficulty(), config.GetRoomSize());
        }

        public Config GetConfig() {
            return config;
        }

        /*public void InitBackgroundContainer() {
            Instantiate(Resources.Load<GameObject>("Prefabs/Backgrounds/Background_3/BackgroundParallax_" + config.GetBiomeType()));
        }*/

        private void GetRoomRepartition(DifficultyEnum difficulty, RoomSizeEnum size) {
            int sizeFloor = (int)size;
            switch (difficulty) {
                case (DifficultyEnum.Easy):
                /*
                 * 70% Easy
                 * 20% Normal
                 * 10% Hard
                 */
                roomRepartition[DifficultyEnum.Easy] = Mathf.Round(sizeFloor * 0.7f);
                roomRepartition[DifficultyEnum.Normal] = Mathf.Round(sizeFloor * 0.2f);
                roomRepartition[DifficultyEnum.Hard] = Mathf.Round(sizeFloor * 0.1f);
                break;
                case (DifficultyEnum.Normal):
                /*
                * 20% Easy
                * 50% Normal
                * 30% Hard
                */
                roomRepartition[DifficultyEnum.Easy] = Mathf.Round(sizeFloor * 0.2f);
                roomRepartition[DifficultyEnum.Normal] = Mathf.Round(sizeFloor * 0.5f);
                roomRepartition[DifficultyEnum.Hard] = Mathf.Round(sizeFloor * 0.3f);
                break;
                case (DifficultyEnum.Hard):
                /*
                * 10% Easy
                * 30% Normal
                * 60% Hard
                */
                roomRepartition[DifficultyEnum.Easy] = Mathf.Round(sizeFloor * 0.1f);
                roomRepartition[DifficultyEnum.Normal] = Mathf.Round(sizeFloor * 0.3f);
                roomRepartition[DifficultyEnum.Hard] = Mathf.Round(sizeFloor * 0.6f);
                break;
            }
            /*Debug.Log("TOTAL ROOM FLOOR - " + (int)(roomRepartition[DifficultyEnum.Easy] + roomRepartition[DifficultyEnum.Normal] + roomRepartition[DifficultyEnum.Hard]));
            Debug.Log("EASY ROOM - " + roomRepartition[DifficultyEnum.Easy]);
            Debug.Log("NORML ROOM - " + roomRepartition[DifficultyEnum.Normal]);
            Debug.Log("HARD ROOM - " + roomRepartition[DifficultyEnum.Hard]);*/
        }

    }
}