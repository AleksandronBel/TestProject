using System;
using System.Collections.Generic;

[Serializable]
public class FigureSaveData
{
    public float X;   
    public float Y;
    public string SpriteName; 
}

[Serializable]
public class TowerSaveData
{
    public List<FigureSaveData> Figures = new();

    public float MaxHeight;
    public float CurrentTowerHeight;
}