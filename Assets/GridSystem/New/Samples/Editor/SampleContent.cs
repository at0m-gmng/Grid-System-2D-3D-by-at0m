namespace GridSystem.Samples.Editor
{
    using System.Collections.Generic;
    using Definitions;
    using Runtime.Definitions;
    using Runtime.Settings;
    using Runtime.Views;
    using UnityEngine;
    using UnityEngine.InputSystem;

    /// <summary>
    /// References to generated assets and prefabs used by the scene builder.
    /// </summary>
    public sealed class SampleContent
    {
        /// <summary>
        /// Grid system settings.
        /// </summary>
        public GridSystemSettings Settings;

        /// <summary>
        /// Main window toggle action.
        /// </summary>
        public InputActionReference ToggleInventory;

        /// <summary>
        /// Item definitions by name.
        /// </summary>
        public readonly Dictionary<string, SampleItemDefinition> Items = new();

        /// <summary>
        /// Main inventory size.
        /// </summary>
        public GridDefinition MainGrid;

        /// <summary>
        /// Hand inventory size.
        /// </summary>
        public GridDefinition HandGrid;

        /// <summary>
        /// World inventory size.
        /// </summary>
        public GridDefinition WorldGrid;

        /// <summary>
        /// UI item views.
        /// </summary>
        public ItemViewCatalog UIViews;

        /// <summary>
        /// World item views.
        /// </summary>
        public ItemViewCatalog WorldViews;

        /// <summary>
        /// UI cell prefab.
        /// </summary>
        public Transform UICell;

        /// <summary>
        /// World cell prefab.
        /// </summary>
        public Transform WorldCell;
    }
}
