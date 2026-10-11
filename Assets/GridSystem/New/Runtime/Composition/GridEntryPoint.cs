namespace GridSystem.Runtime.Composition
{
    using System.Collections.Generic;

    /// <summary>
    /// Invokes every registered <see cref="IGridInitializable"/> once. Used by container adapters.
    /// </summary>
    public sealed class GridEntryPoint
    {
        public GridEntryPoint(IEnumerable<IGridInitializable> initializables)
        {
            _initializables = initializables;
        }

        private readonly IEnumerable<IGridInitializable> _initializables;

        /// <summary>
        /// Initializes services in registration order.
        /// </summary>
        public void Initialize()
        {
            foreach (IGridInitializable initializable in _initializables)
            {
                initializable.Initialize();
            }
        }
    }
}
