using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace MathFunctionWPF.Models
{
    internal class SearchModel
    {
        struct ItemIdx
        {
            public string Word;
            public Item Ref;

            public ItemIdx(string word, Item reference)
            {
                Word = word;
                Ref = reference;
            }
        }

        ItemIdx[] _list;

        public readonly static List<Item> ListItems = new List<Item>()
        {
            new Item(
                "Дихотомия",
                new string[]{ "Бисекциия"},
                "Метод нахождение корней уравнения", Controllers.Drawing.LoaderIcon.GetImageSVG(MathFunctionWPF.Resources.Resource1.d_svgrepo_com)
                ),
            new Item(
                "Ньютон",
                new string[]{ },
                "Метод нахождение корней уравнения", Controllers.Drawing.LoaderIcon.GetImageSVG(MathFunctionWPF.Resources.Resource1.n_svgrepo_com)
                ),
            new Item(
                "Золотое сечение",
                new string[]{ },
                "Метод нахождение корней уравнения", Controllers.Drawing.LoaderIcon.GetImageSVG(MathFunctionWPF.Resources.Resource1.g_svgrepo_com)
                ),
            new Item(
                "Координатного спуска",
                new string[]{ },
                "Метод нахождение корней уравнения", Controllers.Drawing.LoaderIcon.GetImageSVG(MathFunctionWPF.Resources.Resource1.k_svgrepo_com)
                )
        };

        public SearchModel(List<Item> listResult)
        {
            List<ItemIdx> listItems = new List<ItemIdx>();
            Item refrence;
            for (int idx = 0, idxEnd = listResult.Count; idx != idxEnd; ++idx)
            {
                refrence = listResult[idx];
                listItems.Add(new ItemIdx(refrence.Title, refrence));

                for (int idxAliace = 0, idxAliaceCount = refrence.ListAliases.Length; idxAliace != idxAliaceCount; ++idxAliace)
                {
                    listItems.Add(new ItemIdx(refrence.ListAliases[idx], refrence));
                }
            }

            _list = listItems.ToArray();
        }


        List<int> currentIndexses = new List<int>();


        public HashSet<Item> Search(string searchString)
        {
            currentIndexses.Clear();
            // Регулярное выражение для поиска слов (слова состоят из букв, цифр или апострофов)
            string pattern = @"\b\w+\b";  // \b - границы слов, \w+ - последовательность символов (буквы, цифры, подчеркивания)

            // Используем Regex для нахождения всех слов
            MatchCollection matches = Regex.Matches(searchString, pattern);

            List<string> words = new List<string>();

            // Разбиваем поисковую строку на слова
            foreach (Match match in matches)
            {
                words.Add(match.Value);
            }

            Regex[] arrPatterns = new Regex[words.Count];

            for (int idxPatternWord = 0, idxPatternWordEnd = words.Count; idxPatternWord < idxPatternWordEnd; ++idxPatternWord)
            {
                arrPatterns[idxPatternWord] = new Regex(words[idxPatternWord], RegexOptions.IgnoreCase);
            }


            // Выводим все найденные слова
            int idxList, idxListEnd = _list.Length, idxPattern = 0, idxPatternEnd = arrPatterns.Length;
            Regex currentPattern;

            for (idxList = 0; idxList != idxListEnd; ++idxList)
            {
                for (idxPattern = 0; idxPattern != idxPatternEnd; ++idxPattern)
                {
                    currentPattern = arrPatterns[idxPattern];

                    if (currentPattern.IsMatch(_list[idxList].Ref.Title))
                    {
                        currentIndexses.Add(idxList);
                        break;
                    }

                    bool isNeedBreak = false;

                    foreach (var aliaceName in _list[idxList].Ref.ListAliases)
                    {
                        if (currentPattern.IsMatch(aliaceName))
                        {
                            isNeedBreak = true;
                            break;
                        }
                    }

                    if (isNeedBreak)
                    {
                        break;
                    }

                    if (currentPattern.IsMatch(_list[idxList].Ref.ShortDescription))
                    {
                        break;
                    }

                }

            }

            HashSet<Item> items = new HashSet<Item>();

            for (idxList = 0, idxListEnd = currentIndexses.Count; idxList != idxListEnd; ++idxList)
            {
                items.Add(_list[currentIndexses[idxList]].Ref);
            }

            return items;
        }


    }

    class Item
    {
        public string Title
        {
            get; set;
        } = "";

        public string ShortDescription
        {
            get; set;
        } = "";

        public string[] ListAliases { get; set; }
        public BitmapImage Image { get; set; }

        public Item(string title, string[] listAliases, string shortDescription, BitmapImage image)
        {
            Title = title;
            ListAliases = listAliases;
            ShortDescription = shortDescription;
            Image = image;
        }
    }
}
