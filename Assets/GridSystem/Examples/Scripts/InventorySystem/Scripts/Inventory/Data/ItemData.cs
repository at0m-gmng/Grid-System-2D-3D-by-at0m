namespace GridSystem.InventorySystem.Inventory.Data
{
    using System;
    using UnityEngine;
    using Grid = GridSystem.Core.Grid;
#if UNITY_EDITOR
    using UnityEditor;
#endif

    [CreateAssetMenu(menuName = "Inventory/ItemData", fileName = "ItemData")]
    public sealed class ItemData : ScriptableObject
    {
        [field: SerializeField] public BaseItem Item { get; private set; }
        [field: SerializeField] public BaseItemView BaseItemView { get; private set; }

        private string _id;

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (string.IsNullOrWhiteSpace(Item.Id) || Item.Id == default)
            {
                string path = AssetDatabase.GetAssetPath(this);
                if (!string.IsNullOrEmpty(path))
                {
                    _id = AssetDatabase.AssetPathToGUID(path);
                }
                else
                {
                    _id = Guid.NewGuid().ToString();
                }

                Item = new BaseItem(_id);
            }

            Item = new BaseItem(Item.Id, Item.Type, Item.Level, Item.UIPrefab, Item.WorldPrefab, Item.EditorGrid, Item.IsRotatable, Item.IsMergable)
            {
                Grid = Item.EditorGrid != null ? Grid.Clone(Item.EditorGrid.GetGrid()) : Item.Grid
            };
            EditorUtility.SetDirty(this);
        }
#endif
    }
}