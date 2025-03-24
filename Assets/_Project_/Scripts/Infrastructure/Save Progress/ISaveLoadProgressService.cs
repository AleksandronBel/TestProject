using System.Collections.Generic;

public interface ISaveLoadProgressService
{
    public void SaveTower(List<DraggingObject> stackedObjects);
    public TowerSaveData LoadTower();
    public void ResetSave();
}
