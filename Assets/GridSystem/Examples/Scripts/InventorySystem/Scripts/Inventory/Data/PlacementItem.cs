namespace GridSystem.InventorySystem.Inventory.Data
{
    using System;
    using System.Collections.Generic;
    using Enums;
    using UnityEngine;
    using Grid = GridSystem.Core.Grid;

    [Serializable]
    public sealed class PlacementItem
    {
        public ItemType ID;
        public Vector2Int PlacementCenter;
        public List<Vector2Int> PlacementCells = new List<Vector2Int>();
        public Grid Shape;
        public int RotationIndex;
    }
}