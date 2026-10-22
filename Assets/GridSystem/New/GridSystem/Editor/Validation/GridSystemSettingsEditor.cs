namespace GridSystem.Editor.Validation
{
    using System.Collections.Generic;
    using Runtime.Definitions;
    using Runtime.Merge;
    using Runtime.Settings;
    using Runtime.Views;
    using UnityEditor;

    /// <summary>
    /// Inspector of the grid system settings. Validates the referenced catalogs, the merge recipes and
    /// every item view catalog of the project: the settings asset is the only place that sees them together.
    /// </summary>
    [CustomEditor(typeof(GridSystemSettings))]
    public sealed class GridSystemSettingsEditor : GridValidatedEditor
    {
        private const string WITHOUT_ITEMS = "Items catalog is required: a save cannot resolve items without it.";
        private const string WITHOUT_INPUT = "Input config is required: dragging never starts without it.";
        private const string WITHOUT_RULES = "Merge rules are required: dropping an item onto another one throws without them.";

        private readonly List<ItemViewCatalog> _catalogs = new();

        /// <summary>
        /// Validates the settings and the content they reach.
        /// </summary>
        protected override void Validate(GridValidationReport report)
        {
            GridSystemSettings settings = (GridSystemSettings)target;
            if (settings.Items == null)
            {
                report.AddError(WITHOUT_ITEMS, settings);
            }
            else
            {
                GridContentValidator.ValidateDefinitions(settings.Items, report);
            }

            if (settings.Input == null)
            {
                report.AddError(WITHOUT_INPUT, settings);
            }

            if (settings.MergeRules == null)
            {
                report.AddError(WITHOUT_RULES, settings);
            }
            else
            {
                ValidateRules(settings.MergeRules, settings.Items, report);
            }
        }

        private void ValidateRules(MergeRules rules, ItemDefinitionCatalog definitions, GridValidationReport report)
        {
            GridContentValidator.ValidateRules(rules, report);
            if (definitions != null)
            {
                GridContentValidator.ValidateRuleDefinitions(rules, definitions, report);
            }

            GridContentValidator.CollectViewCatalogs(_catalogs);
            for (int i = 0; i < _catalogs.Count; i++)
            {
                GridContentValidator.ValidateViews(_catalogs[i], report);
                GridContentValidator.ValidateMergeResults(rules, _catalogs[i], report);
            }
        }
    }
}
