using App.Scripts.Scenes.SceneWordSearch.Features.Level.Models.Level;
using UnityEngine;

namespace App.Scripts.Scenes.SceneWordSearch.Features.Level.BuilderLevelModel.ProviderWordLevel
{
    public class ProviderWordLevel : IProviderWordLevel
    {
        public LevelInfo LoadLevelData(int levelIndex)
        {
            TextAsset level = Resources.Load<TextAsset>("WordSearch/Levels/" + levelIndex);
            LevelInfo result = null;
            if (level != null)
            {
                result = JsonUtility.FromJson<LevelInfo>(level.text);
            }
            return result;
        }
    }
}