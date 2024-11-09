using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Controls;
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
        ItemIdx[] list;
        SearchModel(List<Item> listResult)
        {
            List<ItemIdx> listItems = new List<ItemIdx>();
            Item refrence;
            for (int idx = 0, idxEnd = listResult.Count; idx != idxEnd; ++idx)
            {
                refrence = listResult[idx];
                listItems.Add(new ItemIdx(refrence.Title, refrence));

                for (int idxAliace = 0, idxAliaceCount = refrence.ListAliases.Length; idxAliace != idxAliaceCount;  ++idxAliace)
                {
                    listItems.Add(new ItemIdx(refrence.ListAliases[idx], refrence));
                }
            }

            list = listItems.ToArray();
        }



        List<int> currentIndexses = new List<int>();
        HashSet<Item> Search(string searchString)
        {
            currentIndexses.Clear();
            // Регулярное выражение для поиска слов (слова состоят из букв, цифр или апострофов)
            string pattern = @"\b\w+\b";  // \b - границы слов, \w+ - последовательность символов (буквы, цифры, подчеркивания)

            // Используем Regex для нахождения всех слов
            MatchCollection matches = Regex.Matches(searchString, pattern);

            List<string> words = new List<string>();

            // Добавляем все найденные слова в список
            foreach (Match match in matches)
            {
                words.Add(match.Value);
            }

            var regex = new Regex(pattern, RegexOptions.IgnoreCase);

            // Выводим все найденные слова
            int idxList, idxListEnd = list.Length;

            for (idxList = 0; idxList != idxListEnd; ++idxList)
            {
                foreach (string patternWord in words)
                {
                    if (regex.IsMatch(patternWord))
                    {
                        currentIndexses.Add(idxList);
                        break;
                    }
                }
            }

            HashSet<Item> items = new HashSet<Item>();

            for (idxList = 0, idxListEnd = currentIndexses.Count; idxList != idxListEnd; ++idxList)
            {
                items.Add(list[currentIndexses[idxList]].Ref);
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
        Image Image { get; set; }

        public Item(string title, string[] listAliases, string shortDescription, Image image)
        {
            Title = title;
            ListAliases = listAliases;
            ShortDescription = shortDescription;
            Image = image;
        }
    }
}
