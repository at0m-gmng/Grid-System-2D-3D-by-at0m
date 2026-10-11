namespace GridSystem.Runtime.Pointer
{
    using UnityEngine;
    using UnityEngine.InputSystem;

    /// <summary>
    /// Input actions used by pointer interaction.
    /// </summary>
    [CreateAssetMenu(menuName = MENU_NAME, fileName = FILE_NAME)]
    public sealed class PointerInputConfig : ScriptableObject
    {
        private const string MENU_NAME = "Grid System/Pointer Input Config";
        private const string FILE_NAME = "PointerInputConfig";

        [SerializeField]
        private InputActionReference _point;

        [SerializeField]
        private InputActionReference _press;

        [SerializeField]
        private InputActionReference _rotate;

        /// <summary>
        /// Vector2 screen position action.
        /// </summary>
        public InputAction Point => _point.action;

        /// <summary>
        /// Button action, started on press and canceled on release.
        /// </summary>
        public InputAction Press => _press.action;

        /// <summary>
        /// Button action rotating the dragged item.
        /// </summary>
        public InputAction Rotate => _rotate.action;
    }
}
