namespace GridSystem.Runtime.Surfaces
{
    using UnityEngine;

    /// <summary>
    /// Finds opened surfaces under screen points.
    /// </summary>
    public interface ISurfaceLocator
    {
        /// <summary>
        /// Picks the highest priority opened surface containing the point.
        /// </summary>
        public bool TryPick(Vector2 screenPosition, out SurfaceHit hit);
    }
}
