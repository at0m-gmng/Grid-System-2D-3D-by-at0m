namespace GridSystem.Runtime.Composition
{
    using UnityEngine;

    /// <summary>
    /// Scene extension point. Registers additional services and scene references.
    /// </summary>
    public abstract class GridModule : MonoBehaviour
    {
        /// <summary>
        /// Adds module bindings.
        /// </summary>
        public abstract void Install(IGridRegistrar registrar);
    }
}
