using System;
using UnityEngine;

namespace RoomUI {

    public class RoomUIStateManager : MonoBehaviour {
        private string shape;
        private string biome;
        private string difficulty;
        // Events
        public event Action<string> OnShapeChange;
        public event Action<string> OnBiomeChange;
        public event Action<string> OnDifficultyChange;
        public event Action<Element> OnObjectSelected;
        public event Action OnSaveClick;
        public event Action OnOpenClick;
        public event Action<RoomUIModel> OnRoomLoad;
        public string CurrentShape => shape;
        public string CurrentDifficulty => difficulty;
        public string CurrentBiome => biome;

        public void OnSelectObject(Element selectedObject) {
            OnObjectSelected?.Invoke(selectedObject);
        }

        public void OnLoadRoom(RoomUIModel roomUIModel) {
            OnRoomLoad?.Invoke(roomUIModel);
        }

        public void OnChangeShape(string shape, bool canEmit = true) {
            this.shape = shape;
            if (canEmit)
                OnShapeChange.Invoke(shape);
        }

        public void OnChangeBiome(string biome, bool canEmit = true) {
            this.biome = biome;
            if (canEmit)
                OnBiomeChange?.Invoke(biome);
        }

        public void OnChangeDifficulty(string difficulty, bool canEmit = true) {
            this.difficulty = difficulty;
            if (canEmit)
                OnDifficultyChange?.Invoke(difficulty);
        }

        public void OnClickSave() {
            OnSaveClick?.Invoke();
        }

        public void OnClickOpen() {
            OnOpenClick?.Invoke();
        }

    }

}