namespace GridSystem.Samples.Editor
{
    /// <summary>
    /// Generated sample item description.
    /// Shape rows go from top to bottom: 'P' pivot cell, 'x' filled cell, '.' empty cell.
    /// </summary>
    public readonly struct SampleItemSpec
    {
        public SampleItemSpec(string name, string displayName, string[] shape, float damage, float attackSpeed, float range, string sprite,
            float modelHeight)
        {
            Name = name;
            DisplayName = displayName;
            Shape = shape;
            Damage = damage;
            AttackSpeed = attackSpeed;
            Range = range;
            Sprite = sprite;
            ModelHeight = modelHeight;
        }

        /// <summary>
        /// Pivot cell symbol.
        /// </summary>
        public const char PIVOT = 'P';

        /// <summary>
        /// Filled cell symbol.
        /// </summary>
        public const char FILLED = 'x';

        /// <summary>
        /// Asset name.
        /// </summary>
        public readonly string Name;

        /// <summary>
        /// Tooltip name.
        /// </summary>
        public readonly string DisplayName;

        /// <summary>
        /// Shape rows.
        /// </summary>
        public readonly string[] Shape;

        /// <summary>
        /// Tooltip damage.
        /// </summary>
        public readonly float Damage;

        /// <summary>
        /// Tooltip attack speed.
        /// </summary>
        public readonly float AttackSpeed;

        /// <summary>
        /// Tooltip range.
        /// </summary>
        public readonly float Range;

        /// <summary>
        /// Sprite file name inside <see cref="SamplePaths.ITEM_SPRITES"/>.
        /// </summary>
        public readonly string Sprite;

        /// <summary>
        /// Height of the generated 3D model.
        /// </summary>
        public readonly float ModelHeight;

        /// <summary>
        /// Shape columns count.
        /// </summary>
        public int Width => Shape[0].Length;

        /// <summary>
        /// Shape rows count.
        /// </summary>
        public int Height => Shape.Length;

        /// <summary>
        /// Returns true when the cell belongs to the shape.
        /// </summary>
        public bool IsFilled(int x, int y) => Shape[y][x] == FILLED || Shape[y][x] == PIVOT;

        /// <summary>
        /// Returns true when the cell is the pivot.
        /// </summary>
        public bool IsPivot(int x, int y) => Shape[y][x] == PIVOT;
    }
}
