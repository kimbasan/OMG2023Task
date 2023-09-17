using System.Collections.Generic;
using App.Scripts.Libs.Factory;
using App.Scripts.Scenes.SceneWordSearch.Features.Level.Models.Level;

namespace App.Scripts.Scenes.SceneWordSearch.Features.Level.BuilderLevelModel
{
    public class FactoryLevelModel : IFactory<LevelModel, LevelInfo, int>
    {
        public LevelModel Create(LevelInfo value, int levelNumber)
        {
            var model = new LevelModel();

            model.LevelNumber = levelNumber;
            model.Words = value.words;
            model.InputChars = BuildListChars(value.words);

            return model;
        }

        private List<char> BuildListChars(List<string> words)
        {
            Dictionary<char, int> allWordsOccurences = new();
            foreach (string word in words)
            {
                Dictionary<char, int> wordOccurences = new();
                foreach (char ch in word)
                {
                    if (wordOccurences.ContainsKey(ch))
                    {
                        wordOccurences[ch]++;
                    }
                    else
                    {
                        wordOccurences[ch] = 1;
                    }

                }
                foreach (var charCount in wordOccurences)
                {
                    if (allWordsOccurences.ContainsKey(charCount.Key))
                    {
                        if (allWordsOccurences[charCount.Key] < charCount.Value)
                        {
                            allWordsOccurences[charCount.Key] = charCount.Value;
                        }
                    }
                    else
                    {
                        allWordsOccurences[charCount.Key] = charCount.Value;
                    }
                }
            }

            List<char> result = new();
            foreach (var charCount in allWordsOccurences)
            {
                for (int i = 0; i < charCount.Value; i++)
                {
                    result.Add(charCount.Key);
                }
            }

            return result;
        }
    }
}