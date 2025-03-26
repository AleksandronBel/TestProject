using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class TowerSaveSystem
{
    private const string _saveFileName = "tower_save.json";

    public void SaveTower(List<DraggingObject> stackedObjects, float maxHeight, float currentHeight)
    {
        TowerSaveData saveData = new TowerSaveData();

        foreach (var figure in stackedObjects)
        {
            saveData.Figures.Add(new FigureSaveData
            {
                X = figure.RectTransform.anchoredPosition.x,
                Y = figure.RectTransform.anchoredPosition.y,
                SpriteName = figure.Image.sprite.name
            });
        }

        saveData.MaxHeight = maxHeight;
        saveData.CurrentTowerHeight = currentHeight;

        string json = JsonUtility.ToJson(saveData, true);
        File.WriteAllText(Application.persistentDataPath + "/" + _saveFileName, json);
    }

    public TowerSaveData LoadTower()
    {
        string path = Application.persistentDataPath + "/" + _saveFileName;

        if (!File.Exists(path))
            return new TowerSaveData();

        string json = File.ReadAllText(path);
        TowerSaveData saveData = JsonUtility.FromJson<TowerSaveData>(json);

        return saveData;
    }

    public void ResetSave()
    {
        string path = Application.persistentDataPath + "/" + _saveFileName;

        if (File.Exists(path))
            File.Delete(path);
    }
}