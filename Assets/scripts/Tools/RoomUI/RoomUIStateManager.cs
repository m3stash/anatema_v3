using System;
using UnityEngine;

public class RoomUIStateManager: MonoBehaviour {
    private string shape;
    private string biome;
    private string difficulty;

    // Events
    public event Action<string> OnShapeChange;
    public event Action<string> OnBiomeChange;
    public event Action<string> OnDifficultyChange;

    public string CurrentShape => shape;
    public string CurrentDifficulty => difficulty;
    public string CurrentBiome => biome;

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

}

