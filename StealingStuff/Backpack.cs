using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StealingStuff
{
    public class Backpack
    {
        private Placement[] Placements { get; set; }
        private ItemStatus[] Status { get; set; }
        private bool[,] SpotTaken { get; set; }
        private List<Item> Items { get; set; }
        public double TotalValue { get; set; }
        public int R { get; set; }
        

        public int TotalItemsArea { get; set; }

        public readonly int Width;
        public readonly int Height;
        public readonly int Area;

        public Backpack(int width, int height,int capacity)
        {
            SpotTaken = new bool[width, height];
            Status = new ItemStatus[capacity];
            for (int i = 0; i < Status.Length; i++)
                Status[i] = ItemStatus.NotChecked;
            Placements = new Placement[capacity];
            Items = new List<Item>();
            R = -1;
            Width = width;
            Height = height;
            Area = Width * Height;
        }

        public void BecomeCopyOf(Backpack other)
        {
            Array.Copy(other.Placements, Placements, other.Placements.Length);
            Array.Copy(other.Status, Status, other.Status.Length);
            Array.Copy(other.SpotTaken, SpotTaken, other.SpotTaken.Length);
            Items = other.Items.ToList();
            TotalValue = other.TotalValue;
            TotalItemsArea = other.TotalItemsArea;
        }

        public void Add(Item item, int width, int height, bool rotation)
        {
            Items.Add(item);
            TotalValue += item.Value;
            TotalItemsArea += item.Width * item.Height;
            Placements[Items.Count - 1].x = width;
            Placements[Items.Count - 1].y = height;
            Placements[Items.Count - 1].rotation = rotation;

        }

        public void Pop()
        {
            if(Items.Count > 0)
            {
                var item = Items[Items.Count - 1];
                if(Status[Items.Count-1] == ItemStatus.Placed)
                    TakeItemOff(Items.Count - 1);
                Status[Items.Count - 1] = ItemStatus.NotChecked;
                TotalValue -= item.Value;
                TotalItemsArea -= item.Height * item.Width;
                Items.RemoveAt(Items.Count - 1); //musi być na końcu, edytuje indeks
            }
        }

        public bool PlaceItem(int ind)
        {
            var item = Items[ind];
            var itemWidth = Placements[ind].rotation ? item.Height : item.Width;
            var itemHeight = Placements[ind].rotation ? item.Width : item.Height;

            if (Placements[ind].x + itemWidth - 1 >= Width || Placements[ind].y + itemHeight - 1 >= Height)
                return false;

            for (int i = 0; i < itemWidth; i++)
                for (int j = 0; j < itemHeight; j++)
                    if (SpotTaken[Placements[ind].x + i, Placements[ind].y + j])
                        return false;

            for (int i = 0; i < itemWidth; i++)
                for (int j = 0; j < itemHeight; j++)
                    SpotTaken[Placements[ind].x + i, Placements[ind].y + j] = true;
            return true;
        }

        public bool FeasibilityCheck()
        {
            if (Status[Items.Count - 1] == ItemStatus.Placed)
                return true;
            for (int i = 0; i < Items.Count; i++)
            {
                switch (Status[i])
                {
                    case ItemStatus.Placed:
                        continue;
                    case ItemStatus.DoesntFit:
                        return false;
                    case ItemStatus.NotChecked:
                        if (PlaceItem(i))
                        {
                            Status[i] = ItemStatus.Placed;
                            continue;
                        }     
                        else
                        {
                            Status[i] = ItemStatus.DoesntFit;
                            for (int j = i+1; j < Items.Count; j++)
                                Status[j] = ItemStatus.DoesntFit;
                            return false;
                        }
                }
            }
            return true;
        }

        public bool BackpackCannotFit()
        {
            if (Items.Count == 0)
                return false;
            return Status[Items.Count - 1] == ItemStatus.DoesntFit;
        }

        private void TakeItemOff(int ind)
        {
            var item = Items[ind];
            var itemWidth = Placements[ind].rotation ? item.Height : item.Width;
            var itemHeight = Placements[ind].rotation ? item.Width : item.Height;
            for (int i = 0; i < itemWidth; i++)
                for (int j = 0; j < itemHeight; j++)
                    SpotTaken[Placements[ind].x + i, Placements[ind].y + j] = false;
        }
    }

    public enum ItemStatus
    {
        NotChecked,
        Placed,
        DoesntFit
    }

    public struct Placement
    {
        public int x;
        public int y;
        public bool rotation;
    }
}
