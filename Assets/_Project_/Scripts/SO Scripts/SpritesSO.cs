using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SpritesConfig", menuName = "ScriptableObjects/SpritesConfig")]
public class SpritesSO : ScriptableObject
{
    public List<SpriteData> Sprites;

    public Sprite GetSpriteByName(string spriteName)
    {
        foreach (var spriteData in Sprites)
        {
            if (spriteData.Name == spriteName)
                return spriteData.Sprite;
        }
        return null;
    }
}

[Serializable]
public class SpriteData
{
    public string Name;
    public Sprite Sprite;
}