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
        public event Action<RoomUIFormValues> OnSave;
        public string CurrentShape => shape;
        public string CurrentDifficulty => difficulty;
        public string CurrentBiome => biome;

        public void OnSelectObject(Element selectedObject) {
            OnObjectSelected?.Invoke(selectedObject);
        }

        public void OnChangeShape(string shape) {
            this.shape = shape;
            OnShapeChange.Invoke(shape);
        }

        public void OnChangeBiome(string biome) {
            this.biome = biome;
            OnBiomeChange?.Invoke(biome);
        }

        public void OnChangeDifficulty(string difficulty) {
            this.difficulty = difficulty;
            OnDifficultyChange?.Invoke(difficulty);
        }

        public void OnClickSave(RoomUIFormValues formValues) {
            OnSave?.Invoke(formValues);
        }

    }

}