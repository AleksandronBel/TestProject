using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class SpriteService : ISpriteService
{
    private Dictionary<string, Sprite> _spriteDictionary;

    [Inject]
    public SpriteService(SpritesSO spriteConfig)
    {
        _spriteDictionary = new Dictionary<string, Sprite>();

        foreach (var spriteData in spriteConfig.Sprites)
            _spriteDictionary[spriteData.Name] = spriteData.Sprite;
    }

    public Sprite GetSprite(string spriteName)
    {
        if (_spriteDictionary.TryGetValue(spriteName, out var sprite))
            return sprite;

        return null;
    }
}