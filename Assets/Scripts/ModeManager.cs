using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

[Serializable]
public class ModeManager
{
    [SerializeField]
    private string pathToControlPatterns;
    private Dictionary<Modes, ControlPattern> patterns;
    public Modes mode;
    
    public void GeneratePatternsDictionary() => GeneratePatternsDictionary(pathToControlPatterns);
    public void GeneratePatternsDictionary(string path) {
        patterns = new();
        foreach (ControlPattern controlPattern in Resources.LoadAll<ControlPattern>(path)) {
            if (!patterns.TryAdd(controlPattern.mode, controlPattern)) {
                Debug.LogWarning($"ControlPattern {controlPattern.name} has the same mode as {patterns[controlPattern.mode].name}. \n" +
                                 $"{controlPattern.name} will not be accessible.");
            }
        }
    }

    [CanBeNull]
    public ControlPattern GetPattern() {
        return GetPattern(mode);
    }
    
    [CanBeNull]
    public ControlPattern GetPattern(Modes mode) {
        if (patterns == null) GeneratePatternsDictionary();
        if (patterns.TryGetValue(mode, out ControlPattern pattern)) return pattern;
        else return null;
    }

    public Action<IGeometryBody, float> GetPatternGizmo() {
        ControlPattern pattern = GetPattern();
        if (pattern != null) return pattern.SelectedGizmos;
        else return (body, f) => { };
    }
    
    public enum Modes
    {
        Standard,
        Rocket,
    }
}