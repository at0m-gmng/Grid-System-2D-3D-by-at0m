namespace GridSystem.InventorySystem.Inventory.Configs
{
    using UnityEngine;

    [CreateAssetMenu(menuName = "Inventory/Configs/InventoryConfig", fileName = "InventoryConfig")]
    public sealed class InventoryConfig : ScriptableObject
    {
        public RectTransform CellRect => CellPrefab as RectTransform;

        [Header("Core size")]
        [field: SerializeField]
        public int Columns { get; private set; }

        [field: SerializeField] public int Rows { get; private set; }
        [field: SerializeField] public Transform CellPrefab { get; private set; }
    }
}