using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Zenject;

public class SaveLoadProgressService : ISaveLoadProgressService
{
    private const string _saveFileName = "tower_save.json";

    private ITowerService _towerService;

    [Inject]
    private void Construct(ITowerService towerService)
    {
        _towerService = towerService;
    }

    public void SaveTower(List<DraggingObject> stackedObjects)
    {
        TowerSaveData saveData = new TowerSaveData();

        foreach (var figure in stackedObjects)
        {
            saveData.Figures.Add(new FigureSaveData
            {
                X = figure.RectTransform.position.x,
                Y = figure.RectTransform.position.y,
                SpriteName = figure.Image.sprite.name
            });
        }

        saveData.MaxHeight = _towerService.MaxHeight.Value;
        saveData.CurrentTowerHeight = _towerService.CurrentTowerHeight.Value;

        string json = JsonUtility.ToJson(saveData, true);
        File.WriteAllText(Application.persistentDataPath + "/" + _saveFileName, json);

        Debug.Log("Tower is saved");
    }

    public TowerSaveData LoadTower()
    {
        string path = Application.persistentDataPath + "/" + _saveFileName;

        if (!File.Exists(path))
        {
            return new TowerSaveData();
        }

        string json = File.ReadAllText(path);
        TowerSaveData saveData = JsonUtility.FromJson<TowerSaveData>(json);

        Debug.Log("Tower is loaded");
        return saveData;
    }

    public void ResetSave()
    {
        string path = Application.persistentDataPath + "/" + _saveFileName;

        if (File.Exists(path))
        {
            File.Delete(path);
            _towerService.DeleteAllTowerData();
            Debug.Log("Save is deleted");
        }
        else
        {
            Debug.Log("Save did not exist");
        }
    }
}