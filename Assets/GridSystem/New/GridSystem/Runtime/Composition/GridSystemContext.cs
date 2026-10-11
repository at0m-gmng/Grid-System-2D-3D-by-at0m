namespace GridSystem.Runtime.Composition
{
    using Settings;
    using UnityEngine;

    /// <summary>
    /// Scene composition data shared by all container adapters.
    /// </summary>
    public sealed class GridSystemContext : MonoBehaviour
    {
        [SerializeField]
        private GridSystemSettings _settings;

        [SerializeField]
        private Transform _poolRoot;

        [SerializeField]
        private GridModule[] _modules;

        /// <summary>
        /// Registers grid system and all scene modules.
        /// </summary>
        public void Install(IGridRegistrar registrar)
        {
            GridSystemComposition.Install(registrar, _settings, _poolRoot);
            for (int i = 0; i < _modules.Length; i++)
            {
                _modules[i].Install(registrar);
            }
        }
    }
}
