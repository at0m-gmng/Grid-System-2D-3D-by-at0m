namespace GridSystem.Samples.Editor
{
    using UnityEngine;

    /// <summary>
    /// Content of the generated sample: items, merges, grids and scene layout.
    /// </summary>
    public static class SampleSpecs
    {
        /// <summary>
        /// Katana item name.
        /// </summary>
        public const string KATANA = "Katana";

        /// <summary>
        /// Bullet item name.
        /// </summary>
        public const string BULLET = "Bullet";

        /// <summary>
        /// Pistol item name.
        /// </summary>
        public const string PISTOL = "Pistol";

        /// <summary>
        /// Laser tower item name.
        /// </summary>
        public const string LASER_TOWER = "LaserTower";

        /// <summary>
        /// Wall item name.
        /// </summary>
        public const string WALL = "Wall";

        /// <summary>
        /// Main inventory name and save key.
        /// </summary>
        public const string MAIN = "Main";

        /// <summary>
        /// Chest inventory name and save key.
        /// </summary>
        public const string CHEST = "Chest";

        /// <summary>
        /// Hand inventory name and save key.
        /// </summary>
        public const string HAND = "Hand";

        /// <summary>
        /// World inventory name and save key.
        /// </summary>
        public const string WORLD = "World";

        /// <summary>
        /// Items of the sample.
        /// </summary>
        public static readonly SampleItemSpec[] Items =
        {
            new SampleItemSpec(KATANA, "Katana", new[] { "x", "P", "x" }, 30f, 1.2f, 1.5f, "Katana.psd", 0.3f),
            new SampleItemSpec(BULLET, "Bullet", new[] { "P" }, 5f, 0f, 0f, "Ammo.psd", 0.3f),
            new SampleItemSpec(PISTOL, "Pistol", new[] { "Px", "x." }, 15f, 2f, 20f, "Pistol.psd", 0.4f),
            new SampleItemSpec(LASER_TOWER, "Laser Tower", new[] { "P" }, 50f, 0.5f, 8f, "Grenade.psd", 1.6f),
            new SampleItemSpec(WALL, "Wall", new[] { "Px", "x." }, 0f, 0f, 0f, "Money.psd", 0.8f)
        };

        /// <summary>
        /// Merge recipes as first, second, result item names.
        /// </summary>
        public static readonly string[][] Merges =
        {
            new[] { KATANA, KATANA, BULLET },
            new[] { BULLET, BULLET, PISTOL },
            new[] { LASER_TOWER, LASER_TOWER, WALL }
        };

        /// <summary>
        /// Main inventory size.
        /// </summary>
        public static readonly Vector2Int MainSize = new Vector2Int(6, 4);

        /// <summary>
        /// Chest inventory size.
        /// </summary>
        public static readonly Vector2Int ChestSize = new Vector2Int(4, 4);

        /// <summary>
        /// Hand inventory size.
        /// </summary>
        public static readonly Vector2Int HandSize = new Vector2Int(6, 2);

        /// <summary>
        /// World inventory size.
        /// </summary>
        public static readonly Vector2Int WorldSize = new Vector2Int(10, 10);

        /// <summary>
        /// Item names spawned into the hand on the first launch.
        /// </summary>
        public static readonly string[] HandSpawnPool = { KATANA, BULLET, PISTOL };

        /// <summary>
        /// Amount of spawned hand items.
        /// </summary>
        public const int HAND_SPAWN_COUNT = 3;

        /// <summary>
        /// Chest start items: item name and pivot cell.
        /// </summary>
        public static readonly (string Name, Vector2Int Origin)[] ChestStartItems =
        {
            (KATANA, new Vector2Int(0, 1)),
            (KATANA, new Vector2Int(1, 1)),
            (BULLET, new Vector2Int(3, 0))
        };

        /// <summary>
        /// World start items: item name and pivot cell.
        /// </summary>
        public static readonly (string Name, Vector2Int Origin)[] WorldStartItems =
        {
            (LASER_TOWER, new Vector2Int(2, 2)),
            (WALL, new Vector2Int(5, 5)),
            (LASER_TOWER, new Vector2Int(7, 3))
        };
    }
}
