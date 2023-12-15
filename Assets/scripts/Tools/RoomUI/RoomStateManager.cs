using System;
using System.Collections.Generic;
using RoomNs;
using TMPro;
using UnityEngine;

public class RoomStateManager: MonoBehaviour {
    private RoomShapeEnum shape;
    private BiomeEnum biome;
    private DifficultyEnum difficulty;


    // Events
    public event Action<RoomShapeEnum> OnShapeChange;
    public event Action<BiomeEnum> OnBiomeChange;
    public event Action<DifficultyEnum> OnDifficultyChange;

    public RoomShapeEnum CurrentShape => shape;
    public DifficultyEnum CurrentDifficulty => difficulty;
    public BiomeEnum CurrentBiome => biome;

    public void OnChangeShape(RoomShapeEnum shape) {
        this.shape = shape;
        OnShapeChange.Invoke(shape);
    }

    public void OnChangeBiome(BiomeEnum biome) {
        this.biome = biome;
        OnBiomeChange?.Invoke(biome);
    }

    public void OnChangeDifficulty(DifficultyEnum difficulty) {
        this.difficulty = difficulty;
        OnDifficultyChange?.Invoke(difficulty);
    }

}

