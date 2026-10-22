namespace GridSystem.Editor.Validation
{
    using System.Collections.Generic;
    using Core.Items;
    using Runtime.Definitions;
    using Runtime.Merge;
    using Runtime.Views;
    using UnityEditor;

    /// <summary>
    /// Reusable checks of grid system assets. Every check describes a defect that the runtime cannot
    /// report by itself, because it either corrupts a save or silently drops an item.
    /// </summary>
    public static class GridContentValidator
    {
        private const string ITEMS_FIELD = "_items";
        private const string ENTRIES_FIELD = "_entries";
        private const string RULES_FIELD = "_rules";
        private const string DEFINITION_FIELD = "Definition";
        private const string PREFAB_FIELD = "Prefab";
        private const string FIRST_FIELD = "First";
        private const string SECOND_FIELD = "Second";
        private const string RESULT_FIELD = "Result";
        private const string VIEW_CATALOG_FILTER = "t:ItemViewCatalog";
        private const string EMPTY_SLOT = "{0}: slot {1} is empty.";
        private const string WITHOUT_ID = "{0}: '{1}' has no persistent id, so a save cannot restore it.";
        private const string DUPLICATE_ID = "{0}: '{1}' shares the persistent id '{2}' with another definition, so a save restores the wrong item.";
        private const string WITHOUT_DEFINITION = "{0}: entry {1} has no definition.";
        private const string WITHOUT_PREFAB = "{0}: '{1}' has no view prefab, so showing it throws.";
        private const string DUPLICATE_DEFINITION = "{0}: '{1}' is listed twice, so the last view wins.";
        private const string INCOMPLETE_RULE = "{0}: rule {1} is incomplete.";
        private const string CONFLICTING_RULE = "{0}: '{1}' with '{2}' produces both '{3}' and '{4}', so the last rule wins.";
        private const string UNKNOWN_DEFINITION = "{0}: '{1}' is missing from '{2}', so every save holding it is dropped on load.";
        private const string UNREACHABLE_RESULT = "{0}: merging '{1}' with '{2}' produces '{3}', which has no view here, so the merge is refused.";
        private const string WITHOUT_ANY_VIEW = "{0}: '{1}' has no view in any catalog, so it can never appear on a surface.";

        /// <summary>
        /// Reports definitions that saves cannot restore: empty slots, missing and shared ids.
        /// </summary>
        public static void ValidateDefinitions(ItemDefinitionCatalog catalog, GridValidationReport report)
        {
            using (SerializedObject serialized = new SerializedObject(catalog))
            {
                SerializedProperty items = serialized.FindProperty(ITEMS_FIELD);
                HashSet<string> ids = new HashSet<string>();
                for (int i = 0; i < items.arraySize; i++)
                {
                    ItemDefinition definition = items.GetArrayElementAtIndex(i).objectReferenceValue as ItemDefinition;
                    if (definition == null)
                    {
                        report.AddError(string.Format(EMPTY_SLOT, catalog.name, i), catalog);
                    }
                    else if (string.IsNullOrEmpty(definition.Id))
                    {
                        report.AddError(string.Format(WITHOUT_ID, catalog.name, definition.name), definition);
                    }
                    else if (!ids.Add(definition.Id))
                    {
                        report.AddError(string.Format(DUPLICATE_ID, catalog.name, definition.name, definition.Id), definition);
                    }
                }
            }
        }

        /// <summary>
        /// Reports catalog entries that cannot produce a view: empty slots, missing prefabs and shadowed duplicates.
        /// </summary>
        public static void ValidateViews(ItemViewCatalog views, GridValidationReport report)
        {
            using (SerializedObject serialized = new SerializedObject(views))
            {
                SerializedProperty entries = serialized.FindProperty(ENTRIES_FIELD);
                HashSet<ItemDefinition> known = new HashSet<ItemDefinition>();
                for (int i = 0; i < entries.arraySize; i++)
                {
                    SerializedProperty entry = entries.GetArrayElementAtIndex(i);
                    ItemDefinition definition = entry.FindPropertyRelative(DEFINITION_FIELD).objectReferenceValue as ItemDefinition;
                    ItemView prefab = entry.FindPropertyRelative(PREFAB_FIELD).objectReferenceValue as ItemView;
                    if (definition == null)
                    {
                        report.AddError(string.Format(WITHOUT_DEFINITION, views.name, i), views);
                    }
                    else if (prefab == null)
                    {
                        report.AddError(string.Format(WITHOUT_PREFAB, views.name, definition.name), views);
                    }
                    else if (!known.Add(definition))
                    {
                        report.AddWarning(string.Format(DUPLICATE_DEFINITION, views.name, definition.name), views);
                    }
                }
            }
        }

        /// <summary>
        /// Reports incomplete recipes and pairs producing two different results.
        /// </summary>
        public static void ValidateRules(MergeRules rules, GridValidationReport report)
        {
            using (SerializedObject serialized = new SerializedObject(rules))
            {
                SerializedProperty array = serialized.FindProperty(RULES_FIELD);
                Dictionary<(ItemDefinition, ItemDefinition), ItemDefinition> results = new();
                for (int i = 0; i < array.arraySize; i++)
                {
                    SerializedProperty rule = array.GetArrayElementAtIndex(i);
                    ItemDefinition first = GetDefinition(rule, FIRST_FIELD);
                    ItemDefinition second = GetDefinition(rule, SECOND_FIELD);
                    ItemDefinition result = GetDefinition(rule, RESULT_FIELD);
                    if (first == null || second == null || result == null)
                    {
                        report.AddError(string.Format(INCOMPLETE_RULE, rules.name, i), rules);
                    }
                    else if (results.TryGetValue((first, second), out ItemDefinition known) && known != result)
                    {
                        report.AddError(string.Format(CONFLICTING_RULE, rules.name, first.name, second.name, known.name, result.name), rules);
                    }
                    else
                    {
                        results[(first, second)] = result;
                        results[(second, first)] = result;
                    }
                }
            }
        }

        /// <summary>
        /// Reports recipe ingredients and results that the definition catalog does not know,
        /// so an inventory holding them loses its save.
        /// </summary>
        public static void ValidateRuleDefinitions(MergeRules rules, ItemDefinitionCatalog definitions, GridValidationReport report)
        {
            using (SerializedObject serialized = new SerializedObject(rules))
            {
                SerializedProperty array = serialized.FindProperty(RULES_FIELD);
                HashSet<ItemDefinition> reported = new HashSet<ItemDefinition>();
                for (int i = 0; i < array.arraySize; i++)
                {
                    SerializedProperty rule = array.GetArrayElementAtIndex(i);
                    ValidateKnown(GetDefinition(rule, FIRST_FIELD), definitions, rules, reported, report);
                    ValidateKnown(GetDefinition(rule, SECOND_FIELD), definitions, rules, reported, report);
                    ValidateKnown(GetDefinition(rule, RESULT_FIELD), definitions, rules, reported, report);
                }
            }
        }

        /// <summary>
        /// Reports recipes reachable inside the catalog whose result the same catalog cannot show.
        /// </summary>
        public static void ValidateMergeResults(MergeRules rules, ItemViewCatalog views, GridValidationReport report)
        {
            List<ItemDefinition> definitions = new List<ItemDefinition>();
            CollectDefinitions(views, definitions);
            for (int a = 0; a < definitions.Count; a++)
            {
                for (int b = a; b < definitions.Count; b++)
                {
                    if (rules.TryGetResult(definitions[a], definitions[b], out IGridItemDefinition result) && !views.Contains(result))
                    {
                        ItemDefinition produced = result as ItemDefinition;
                        report.AddWarning(string.Format(UNREACHABLE_RESULT, views.name, definitions[a].name, definitions[b].name, produced.name), views);
                    }
                }
            }
        }

        /// <summary>
        /// Reports definitions no catalog can show, so they can never appear on any surface.
        /// A definition present in a single catalog is normal: that is how a surface refuses foreign items.
        /// </summary>
        public static void ValidateReachability(ItemDefinitionCatalog definitions, IReadOnlyList<ItemViewCatalog> views, GridValidationReport report)
        {
            using (SerializedObject serialized = new SerializedObject(definitions))
            {
                SerializedProperty items = serialized.FindProperty(ITEMS_FIELD);
                for (int i = 0; i < items.arraySize; i++)
                {
                    ItemDefinition definition = items.GetArrayElementAtIndex(i).objectReferenceValue as ItemDefinition;
                    if (definition != null && !IsDisplayable(definition, views))
                    {
                        report.AddError(string.Format(WITHOUT_ANY_VIEW, definitions.name, definition.name), definition);
                    }
                }
            }
        }

        /// <summary>
        /// Fills the list with definitions the catalog can show.
        /// </summary>
        public static void CollectDefinitions(ItemViewCatalog views, List<ItemDefinition> definitions)
        {
            definitions.Clear();
            using (SerializedObject serialized = new SerializedObject(views))
            {
                SerializedProperty entries = serialized.FindProperty(ENTRIES_FIELD);
                for (int i = 0; i < entries.arraySize; i++)
                {
                    ItemDefinition definition = entries.GetArrayElementAtIndex(i).FindPropertyRelative(DEFINITION_FIELD).objectReferenceValue as ItemDefinition;
                    if (definition != null)
                    {
                        definitions.Add(definition);
                    }
                }
            }
        }

        /// <summary>
        /// Fills the list with every item view catalog of the project.
        /// </summary>
        public static void CollectViewCatalogs(List<ItemViewCatalog> catalogs)
        {
            catalogs.Clear();
            string[] guids = AssetDatabase.FindAssets(VIEW_CATALOG_FILTER);
            for (int i = 0; i < guids.Length; i++)
            {
                catalogs.Add(AssetDatabase.LoadAssetAtPath<ItemViewCatalog>(AssetDatabase.GUIDToAssetPath(guids[i])));
            }
        }

        private static bool IsDisplayable(ItemDefinition definition, IReadOnlyList<ItemViewCatalog> views)
        {
            bool result = false;
            for (int i = 0; !result && i < views.Count; i++)
            {
                result = views[i].Contains(definition);
            }

            return result;
        }

        private static ItemDefinition GetDefinition(SerializedProperty rule, string field)
            => rule.FindPropertyRelative(field).objectReferenceValue as ItemDefinition;

        private static void ValidateKnown(ItemDefinition definition, ItemDefinitionCatalog definitions, MergeRules rules,
            HashSet<ItemDefinition> reported, GridValidationReport report)
        {
            if (definition != null && !definitions.TryGet(definition.Id, out _) && reported.Add(definition))
            {
                report.AddError(string.Format(UNKNOWN_DEFINITION, rules.name, definition.name, definitions.name), definition);
            }
        }
    }
}
