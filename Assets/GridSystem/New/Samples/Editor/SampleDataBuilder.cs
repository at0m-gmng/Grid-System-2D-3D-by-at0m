namespace GridSystem.Samples.Editor
{
    using Definitions;
    using Runtime.Definitions;
    using Runtime.Merge;
    using Runtime.Pointer;
    using Runtime.Settings;
    using UnityEditor;
    using UnityEngine;

    /// <summary>
    /// Creates item definitions, catalog, merge rules, grid sizes and settings.
    /// </summary>
    public sealed class SampleDataBuilder
    {
        private const string CATALOG_NAME = "ItemDefinitionCatalog";
        private const string MERGE_RULES_NAME = "MergeRules";
        private const string SETTINGS_NAME = "GridSystemSettings";
        private const string GRID_SUFFIX = "Grid";
        private const string ID_FIELD = "_id";
        private const string ROTATABLE_FIELD = "_isRotatable";
        private const string SHAPE_WIDTH_FIELD = "_shape._width";
        private const string SHAPE_HEIGHT_FIELD = "_shape._height";
        private const string SHAPE_CELLS_FIELD = "_shape._cells";
        private const string SHAPE_PIVOT_FIELD = "_shape._pivot";
        private const string DISPLAY_NAME_FIELD = "_displayName";
        private const string DAMAGE_FIELD = "_damage";
        private const string ATTACK_SPEED_FIELD = "_attackSpeed";
        private const string RANGE_FIELD = "_range";
        private const string ITEMS_FIELD = "_items";
        private const string RULES_FIELD = "_rules";
        private const string FIRST_FIELD = "First";
        private const string SECOND_FIELD = "Second";
        private const string RESULT_FIELD = "Result";
        private const string WIDTH_FIELD = "_width";
        private const string HEIGHT_FIELD = "_height";
        private const string MERGE_RULES_FIELD = "_mergeRules";
        private const string INPUT_FIELD = "_input";
        private const int FIRST_INDEX = 0;
        private const int SECOND_INDEX = 1;
        private const int RESULT_INDEX = 2;

        /// <summary>
        /// Creates or updates data assets and fills the content.
        /// </summary>
        public void Build(SampleContent content, PointerInputConfig input)
        {
            SampleItemDefinition[] items = new SampleItemDefinition[SampleSpecs.Items.Length];
            for (int i = 0; i < items.Length; i++)
            {
                items[i] = CreateItem(SampleSpecs.Items[i]);
                content.Items.Add(SampleSpecs.Items[i].Name, items[i]);
            }

            ItemDefinitionCatalog catalog = SampleAssets.LoadOrCreate<ItemDefinitionCatalog>(SamplePaths.DEFINITIONS, CATALOG_NAME);
            new SerializedObject(catalog).SetObjects(ITEMS_FIELD, items).Apply();
            MergeRules mergeRules = CreateMergeRules(content);
            content.MainGrid = CreateGrid(SampleSpecs.MAIN, SampleSpecs.MainSize);
            content.ChestGrid = CreateGrid(SampleSpecs.CHEST, SampleSpecs.ChestSize);
            content.HandGrid = CreateGrid(SampleSpecs.HAND, SampleSpecs.HandSize);
            content.WorldGrid = CreateGrid(SampleSpecs.WORLD, SampleSpecs.WorldSize);
            content.Settings = SampleAssets.LoadOrCreate<GridSystemSettings>(SamplePaths.CONTENT, SETTINGS_NAME);
            new SerializedObject(content.Settings)
                .SetObject(ITEMS_FIELD, catalog)
                .SetObject(MERGE_RULES_FIELD, mergeRules)
                .SetObject(INPUT_FIELD, input)
                .Apply();
        }

        private static SampleItemDefinition CreateItem(in SampleItemSpec spec)
        {
            SampleItemDefinition item = SampleAssets.LoadOrCreate<SampleItemDefinition>(SamplePaths.DEFINITIONS, spec.Name);
            SerializedObject serialized = new SerializedObject(item)
                .SetString(ID_FIELD, AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(item)))
                .SetBool(ROTATABLE_FIELD, true)
                .SetInt(SHAPE_WIDTH_FIELD, spec.Width)
                .SetInt(SHAPE_HEIGHT_FIELD, spec.Height)
                .SetString(DISPLAY_NAME_FIELD, spec.DisplayName)
                .SetFloat(DAMAGE_FIELD, spec.Damage)
                .SetFloat(ATTACK_SPEED_FIELD, spec.AttackSpeed)
                .SetFloat(RANGE_FIELD, spec.Range);
            SerializedProperty cells = serialized.FindProperty(SHAPE_CELLS_FIELD);
            cells.arraySize = spec.Width * spec.Height;
            for (int y = 0; y < spec.Height; y++)
            {
                for (int x = 0; x < spec.Width; x++)
                {
                    cells.GetArrayElementAtIndex(y * spec.Width + x).boolValue = spec.IsFilled(x, y);
                    if (spec.IsPivot(x, y))
                    {
                        serialized.SetVector2Int(SHAPE_PIVOT_FIELD, new Vector2Int(x, y));
                    }
                }
            }

            serialized.Apply();
            return item;
        }

        private static MergeRules CreateMergeRules(SampleContent content)
        {
            MergeRules rules = SampleAssets.LoadOrCreate<MergeRules>(SamplePaths.DEFINITIONS, MERGE_RULES_NAME);
            SerializedObject serialized = new SerializedObject(rules);
            SerializedProperty array = serialized.FindProperty(RULES_FIELD);
            array.arraySize = SampleSpecs.Merges.Length;
            for (int i = 0; i < SampleSpecs.Merges.Length; i++)
            {
                SerializedProperty rule = array.GetArrayElementAtIndex(i);
                rule.FindPropertyRelative(FIRST_FIELD).objectReferenceValue = content.Items[SampleSpecs.Merges[i][FIRST_INDEX]];
                rule.FindPropertyRelative(SECOND_FIELD).objectReferenceValue = content.Items[SampleSpecs.Merges[i][SECOND_INDEX]];
                rule.FindPropertyRelative(RESULT_FIELD).objectReferenceValue = content.Items[SampleSpecs.Merges[i][RESULT_INDEX]];
            }

            serialized.Apply();
            return rules;
        }

        private static GridDefinition CreateGrid(string name, Vector2Int size)
        {
            GridDefinition grid = SampleAssets.LoadOrCreate<GridDefinition>(SamplePaths.GRIDS, name + GRID_SUFFIX);
            new SerializedObject(grid).SetInt(WIDTH_FIELD, size.x).SetInt(HEIGHT_FIELD, size.y).Apply();
            return grid;
        }
    }
}
