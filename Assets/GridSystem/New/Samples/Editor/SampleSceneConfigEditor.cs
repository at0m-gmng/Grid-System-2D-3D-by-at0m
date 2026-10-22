namespace GridSystem.Samples.Editor
{
    using System.Collections.Generic;
    using Bootstrap;
    using Core.Inventories;
    using Core.Items;
    using GridSystem.Editor.Validation;
    using Runtime.Surfaces;
    using UnityEditor;

    /// <summary>
    /// Inspector of the sample configuration. Reports the defects that only show up at runtime:
    /// start items the surface cannot show or cannot fit, inventories sharing a save key
    /// and a world surface that outranks the UI.
    /// </summary>
    [CustomEditor(typeof(SampleSceneConfig))]
    public sealed class SampleSceneConfigEditor : GridValidatedEditor
    {
        private const int FIRST_ID = 1;
        private const string WITHOUT_UI = "UI prefab is required: the sample has no surfaces without it.";
        private const string WITHOUT_WORLD = "World prefab is required: the sample has no world surface without it.";
        private const string WITHOUT_GRID = "'{0}' has no grid definition.";
        private const string WITHOUT_SURFACE = "'{0}' has no surface in the UI prefab.";
        private const string WITHOUT_VIEWS = "'{0}' has no item view catalog, so showing an item throws.";
        private const string WITHOUT_KEY = "An inventory has no save key, so it shares a save slot with every other keyless one.";
        private const string DUPLICATE_KEY = "'{0}' is used as a save key twice, so the two inventories overwrite each other.";
        private const string EMPTY_START_ITEM = "'{0}': start item {1} has no definition.";
        private const string NOT_DISPLAYABLE = "'{0}': start item '{1}' is missing from '{2}', so opening the surface throws.";
        private const string DOES_NOT_FIT = "'{0}': start item '{1}' does not fit at ({2}, {3}), so it is dropped silently.";
        private const string LOW_PRIORITY = "'{0}' priority {1} does not outrank the world surface '{2}' priority {3}, so a drop over the UI may land in the world.";

        /// <summary>
        /// Validates the sample content against the surfaces showing it.
        /// </summary>
        protected override void Validate(GridValidationReport report)
        {
            SampleSceneConfig config = (SampleSceneConfig)target;
            SampleSceneRoot ui = config.UiPrefab;
            GridSurface world = config.WorldPrefab;
            if (ui == null)
            {
                report.AddError(WITHOUT_UI, config);
            }
            else
            {
                ValidateInventory(config, config.Main, ui.Main, report);
                ValidateInventory(config, config.Chest, ui.Chest, report);
                ValidateInventory(config, config.Hand, ui.Hand, report);
            }

            if (world == null)
            {
                report.AddError(WITHOUT_WORLD, config);
            }
            else
            {
                ValidateInventory(config, config.World, world, report);
            }

            if (ui != null && world != null)
            {
                ValidatePriority(config, ui.Main, world, report);
                ValidatePriority(config, ui.Chest, world, report);
                ValidatePriority(config, ui.Hand, world, report);
            }

            ValidateKeys(config, report);
        }

        private static void ValidateInventory(SampleSceneConfig config, SampleInventory inventory, GridSurface surface, GridValidationReport report)
        {
            if (inventory.Definition == null)
            {
                report.AddError(string.Format(WITHOUT_GRID, inventory.Key), config);
            }
            else if (surface == null)
            {
                report.AddError(string.Format(WITHOUT_SURFACE, inventory.Key), config);
            }
            else if (surface.Views == null)
            {
                report.AddError(string.Format(WITHOUT_VIEWS, surface.name), config);
            }
            else
            {
                ValidateStartItems(config, inventory, surface, report);
            }
        }

        private static void ValidateStartItems(SampleSceneConfig config, SampleInventory inventory, GridSurface surface, GridValidationReport report)
        {
            SampleStartItem[] items = inventory.StartItems;
            using (GridInventory replay = new GridInventory(FIRST_ID, inventory.Key, inventory.Definition.Width, inventory.Definition.Height))
            {
                for (int i = 0; i < items.Length; i++)
                {
                    if (items[i].Definition == null)
                    {
                        report.AddError(string.Format(EMPTY_START_ITEM, inventory.Key, i), config);
                    }
                    else
                    {
                        if (!surface.CanDisplay(items[i].Definition))
                        {
                            report.AddError(string.Format(NOT_DISPLAYABLE, inventory.Key, items[i].Definition.name, surface.Views.name), items[i].Definition);
                        }

                        if (!replay.TryAdd(new GridItem(i + FIRST_ID, items[i].Definition), items[i].Origin, items[i].Rotation))
                        {
                            report.AddError(string.Format(DOES_NOT_FIT, inventory.Key, items[i].Definition.name, items[i].Origin.x, items[i].Origin.y), items[i].Definition);
                        }
                    }
                }
            }
        }

        private static void ValidatePriority(SampleSceneConfig config, GridSurface surface, GridSurface world, GridValidationReport report)
        {
            if (surface != null && surface.Priority <= world.Priority)
            {
                report.AddError(string.Format(LOW_PRIORITY, surface.name, surface.Priority, world.name, world.Priority), config);
            }
        }

        private static void ValidateKeys(SampleSceneConfig config, GridValidationReport report)
        {
            SampleInventory[] inventories = { config.Main, config.Chest, config.Hand, config.World };
            HashSet<string> keys = new HashSet<string>();
            for (int i = 0; i < inventories.Length; i++)
            {
                if (string.IsNullOrEmpty(inventories[i].Key))
                {
                    report.AddError(WITHOUT_KEY, config);
                }
                else if (!keys.Add(inventories[i].Key))
                {
                    report.AddError(string.Format(DUPLICATE_KEY, inventories[i].Key), config);
                }
            }
        }
    }
}
