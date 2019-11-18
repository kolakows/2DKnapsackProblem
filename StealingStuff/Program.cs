using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StealingStuff
{
    class Program
    {
        static void Main(string[] args)
        {
            var allLines = System.IO.File.ReadAllLines("./test2.txt");
            int width = Int32.Parse(allLines[0].Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)[0]), height = Int32.Parse(allLines[0].Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)[1]);
            int numberOfItems = Int32.Parse(allLines[1].Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)[0]);
            var allItems = new List<Item>();
            for (int i = 2; i < allLines.Length; i++)
            {
                var info = allLines[i].Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                allItems.Add(new Item()
                {
                    Height = Int32.Parse(info[0]),
                    Width = Int32.Parse(info[1]),
                    Value = Int32.Parse(info[2])
                });
            }
            var backpack = new Backpack(width, height, numberOfItems);
            var bestSolution = new Backpack(width, height, numberOfItems);
            var sortedItems = allItems.OrderByDescending(x => x.Value / ((double)x.Height * (double)x.Width)).ToList();
            StealSomeMore(backpack, bestSolution, sortedItems);
        }

        static void StealSomeMore(Backpack current, Backpack best, List<Item> allItems)
        {
            if (current.TotalItemsArea > current.Area) //na pewno nie da się zapakować
                return;
            if (current.BackpackCannotFit())
                return;
            if(current.TotalValue > best.TotalValue)
            {
                if (current.FeasibilityCheck())
                {
                    best.BecomeCopyOf(current);
                }
            }
            if (current.R >= allItems.Count - 1) //nie ma więcej przedmiotów do dodania
                return;
            for (int r = current.R+1; r < allItems.Count; r++)
            {
                for (int i = 0; i < current.Width; i++)
                {
                    for (int j = 0; j < current.Height; j++)
                    {
                        for (int k = 0; k < 2; k++)
                        {
                            current.R = r;
                            current.Add(allItems[r], i, j, k == 1);
                            StealSomeMore(current, best, allItems);
                            current.Pop();
                            if (current.BackpackCannotFit())
                                return;
                        }
                    }
                }
            }
        }
    }
}
