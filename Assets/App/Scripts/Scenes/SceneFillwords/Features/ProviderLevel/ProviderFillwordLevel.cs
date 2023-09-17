using System;
using System.Collections.Generic;
using System.Linq;
using App.Scripts.Scenes.SceneFillwords.Features.FillwordModels;
using UnityEngine;

namespace App.Scripts.Scenes.SceneFillwords.Features.ProviderLevel
{
    public class ProviderFillwordLevel : IProviderFillwordLevel
    {
        private static readonly List<string> WORD_CATALOG = GetTextAssetData("Fillwords/words_list");
        private static readonly List<string> LEVEL_CATALOG = GetTextAssetData("Fillwords/pack_0");

        public GridFillWords LoadModel(int index)
        {
            string levelData = LEVEL_CATALOG[--index];

            var indexedLetters = ParseLevelData(levelData.Split(' '));

            var grid = CreateGrid(indexedLetters);

            return grid;
        }

        private static List<string> GetTextAssetData(string path)
        {
            List<string> result = new List<string>();
            TextAsset asset = Resources.Load<TextAsset>(path);
            if (asset != null && asset.text != null)
            {
                result = asset.text.Split("\r\n").ToList();
            }
            return result;
        }

        private Vector2Int GetGridSize(int elements)
        {
            Vector2Int result = new Vector2Int(elements, 1);
            int denominatorPrev = 1;
            int iPrev = elements;
            for (int i = elements; i > 1; i--)
            {
                int denominator = elements / i;
                if (denominator > 0)
                {
                    int remainder = elements % i;
                    if (remainder == 0)
                    {
                        if (denominator > denominatorPrev && denominator < iPrev)
                        {
                            denominatorPrev = denominator;
                            iPrev = i;
                            result = new Vector2Int(i, denominator);
                        }
                    }
                }
            }

            return result;
        }

        private string GetWordFromCatalog(string index)
        {
            return WORD_CATALOG[int.Parse(index)];
        }

        public int GetTotalLevelCount()
        {
            return LEVEL_CATALOG.Count;
        }

        private List<FillwordLetter> ParseLevelData(string[] data)
        {
            var result = new List<FillwordLetter>();

            for (int i = 0; i < data.Length; i += 2)
            {
                string word = GetWordFromCatalog(data[i]);
                string[] letterIndexesOnGrid = data[i + 1].Split(";");

                for (int j = 0; j < letterIndexesOnGrid.Length; j++)
                {
                    var letterData = new FillwordLetter(word[j], int.Parse(letterIndexesOnGrid[j]));

                    result.Add(letterData);
                }
            }

            return result;
        }

        private GridFillWords CreateGrid(List<FillwordLetter> indexedLetters)
        {
            Vector2Int gridDimentions = GetGridSize(indexedLetters.Count);

            GridFillWords grid = new GridFillWords(gridDimentions);

            indexedLetters.Sort();

            int lettersIndex = 0;
            for (int i = 0; i < grid.Size.y; i++)
            {
                for (int j = 0; j < grid.Size.x; j++)
                {
                    grid.Set(i, j, new CharGridModel(indexedLetters[lettersIndex].Letter));
                    lettersIndex++;
                }
            }

            return grid;
        }
    }
    internal class FillwordLetter : IComparable<FillwordLetter>
    {
        public char Letter { get; private set; }
        public int GridIndex { get; private set; }

        public FillwordLetter(char letter, int gridIndex)
        {
            Letter = letter;
            GridIndex = gridIndex;
        }

        public int CompareTo(FillwordLetter other)
        {
            return GridIndex.CompareTo(other.GridIndex);
        }

        public override bool Equals(object obj)
        {
            return obj is FillwordLetter letter &&
                   Letter == letter.Letter &&
                   GridIndex == letter.GridIndex;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Letter, GridIndex);
        }
    }
}