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
        /// Main inventory toggle action.
        /// </summary>
        public InputActionReference ToggleMain;

        /// <summary>
        /// Chest inventory toggle action.
        /// </summary>
        public InputActionReference ToggleChest;

        /// <summary>
        /// World inventory toggle action.
        /// </summary>
        public InputActionReference ToggleWorld;

        /// <summary>
        /// Item definitions by name.
        /// </summary>
        public readonly Dictionary<string, SampleItemDefinition> Items = new();

        /// <summary>
        /// Main inventory size.
        /// </summary>
        public GridDefinition MainGrid;

        /// <summary>
        /// Chest inventory size.
        /// </summary>
        public GridDefinition ChestGrid;

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
