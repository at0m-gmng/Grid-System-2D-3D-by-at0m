namespace GridSystem.InventorySystem.Inventory.Configs
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Data;
    using UnityEngine;
    using Random = System.Random;

    [CreateAssetMenu(menuName = "Inventory/Configs/SelectableItemConfig", fileName = "SelectableItemConfig")]
    public sealed class SelectableItemConfig : ScriptableObject
    {
        [field: SerializeField] public List<SelectableItem> SelectableItems { get; private set; }

        private Random _random = new();
        private int _itemsLength;
        private ItemData[] _itemsCopy;
        private List<ItemData> _result;

        /// <summary>
        /// Get Random Items for Target Level.
        /// </summary>
        /// <param name="level"></param>
        /// <returns></returns>
        public List<ItemData> GetRandomItemsForLevel(int level)
        {
            SelectableItem selectableItem = SelectableItems.FirstOrDefault(x => x.Level == level);
            if (selectableItem == null)
            {
                int closestLevel = FindClosestLevel(level);
                selectableItem = SelectableItems.First(x => x.Level == closestLevel);
            }

            if (selectableItem.RandomItems == 0)
            {
                return new List<ItemData>(selectableItem.SelectedItems);
            }

            _itemsLength = selectableItem.SelectedItems.Length;
            _itemsCopy = new ItemData[_itemsLength];
            Array.Copy(selectableItem.SelectedItems, _itemsCopy, _itemsLength);
            _result.Clear();
            _result = new List<ItemData>(selectableItem.RandomItems);

            for (int i = 0; i < selectableItem.RandomItems; i++)
            {
                int randomIndex = _random.Next(i, _itemsLength);
                (_itemsCopy[i], _itemsCopy[randomIndex]) = (_itemsCopy[randomIndex], _itemsCopy[i]);
                _result.Add(_itemsCopy[i]);
            }

            return _result;
        }

        private int FindClosestLevel(int targetLevel)
        {
            int closestLevel = -1;
            int minDifference = int.MaxValue;

            foreach (SelectableItem item in SelectableItems)
            {
                int difference = Math.Abs(item.Level - targetLevel);
                if (difference < minDifference)
                {
                    minDifference = difference;
                    closestLevel = item.Level;
                }
            }

            return closestLevel;
        }
    }

    [Serializable]
    public class SelectableItem
    {
        [field: SerializeField, Min(1)] public int Level { get; private set; }
        [field: SerializeField, Min(0)] public int RandomItems { get; private set; }
        [field: SerializeField] public ItemData[] SelectedItems { get; private set; }
    }
}