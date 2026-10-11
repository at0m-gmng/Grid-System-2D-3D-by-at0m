namespace GridSystem.Samples.Editor
{
    using Definitions;
    using Runtime.Views;
    using Runtime.Views.Highlight;
    using Runtime.Views.Motion;
    using UnityEditor;
    using UnityEngine;
    using UnityEngine.Rendering;
    using UnityEngine.UI;

    /// <summary>
    /// Creates cell prefabs, item view prefabs and view catalogs.
    /// </summary>
    public sealed class SamplePrefabBuilder
    {
        private const string UI_CELL_NAME = "UI_Cell";
        private const string WORLD_CELL_NAME = "World_Cell";
        private const string UI_PREFIX = "UI_";
        private const string WORLD_PREFIX = "World_";
        private const string BODY_NAME = "Body";
        private const string MODEL_NAME = "Model";
        private const string UI_CATALOG_NAME = "UIItemViews";
        private const string WORLD_CATALOG_NAME = "WorldItemViews";
        private const string MOTION_FIELD = "_motion";
        private const string HIGHLIGHTER_FIELD = "_highlighter";
        private const string BODY_FIELD = "_body";
        private const string RECT_FIELD = "_rect";
        private const string MODEL_FIELD = "_model";
        private const string TARGET_FIELD = "target";
        private const string GRAPHIC_FIELD = "_graphic";
        private const string RENDERERS_FIELD = "_renderers";
        private const string COLOR_PROPERTY_FIELD = "_colorProperty";
        private const string ENTRIES_FIELD = "_entries";
        private const string DEFINITION_FIELD = "Definition";
        private const string PREFAB_FIELD = "Prefab";
        private const string QUAD_MESH = "Quad.fbx";
        private const string CUBE_MESH = "Cube.fbx";
        private const string QUAD_NAME = "Quad";
        private const string CUBE_NAME = "Cube";

        /// <summary>
        /// Creates or replaces prefabs and catalogs and fills the content.
        /// </summary>
        public void Build(SampleContent content)
        {
            content.UICell = CreateUICell();
            content.WorldCell = CreateWorldCell();
            ItemView[] uiViews = new ItemView[SampleSpecs.Items.Length];
            ItemView[] worldViews = new ItemView[SampleSpecs.Items.Length];
            Material material = SampleAssets.Load<Material>(SamplePaths.ITEM_MATERIAL);
            Mesh cube = Resources.GetBuiltinResource<Mesh>(CUBE_MESH);
            string colorProperty = GraphicsSettings.currentRenderPipeline == null ? SampleLayout.BUILT_IN_COLOR : SampleLayout.SRP_COLOR;
            for (int i = 0; i < SampleSpecs.Items.Length; i++)
            {
                uiViews[i] = CreateUIView(SampleSpecs.Items[i]);
                worldViews[i] = CreateWorldView(SampleSpecs.Items[i], cube, material, colorProperty);
            }

            content.UIViews = CreateCatalog(UI_CATALOG_NAME, content, uiViews);
            content.WorldViews = CreateCatalog(WORLD_CATALOG_NAME, content, worldViews);
        }

        private static Transform CreateUICell()
        {
            GameObject root = new GameObject(UI_CELL_NAME, typeof(RectTransform));
            RectTransform rect = (RectTransform)root.transform;
            rect.anchorMin = SampleLayout.Center;
            rect.anchorMax = SampleLayout.Center;
            rect.pivot = SampleLayout.Center;
            Image image = root.AddComponent<Image>();
            image.sprite = SampleAssets.Load<Sprite>(SamplePaths.CELL_SPRITE);
            image.raycastTarget = false;
            return SampleAssets.SavePrefab<Transform>(root, SamplePaths.UI_PREFABS);
        }

        private static Transform CreateWorldCell()
        {
            GameObject root = new GameObject(WORLD_CELL_NAME);
            Transform quad = CreateMesh(QUAD_NAME, Resources.GetBuiltinResource<Mesh>(QUAD_MESH), SampleAssets.Load<Material>(SamplePaths.CELL_MATERIAL),
                root.transform).transform;
            quad.localPosition = SampleLayout.QuadOffset;
            quad.localEulerAngles = SampleLayout.QuadRotation;
            return SampleAssets.SavePrefab<Transform>(root, SamplePaths.WORLD_PREFABS);
        }

        private static MeshRenderer CreateMesh(string name, Mesh mesh, Material material, Transform parent)
        {
            GameObject target = new GameObject(name);
            target.transform.SetParent(parent, false);
            target.AddComponent<MeshFilter>().sharedMesh = mesh;
            MeshRenderer renderer = target.AddComponent<MeshRenderer>();
            renderer.sharedMaterial = material;
            return renderer;
        }

        private static ItemView CreateUIView(in SampleItemSpec spec)
        {
            GameObject root = new GameObject(UI_PREFIX + spec.Name, typeof(RectTransform));
            RectTransform rect = (RectTransform)root.transform;
            rect.anchorMin = SampleLayout.Center;
            rect.anchorMax = SampleLayout.Center;

            GameObject body = new GameObject(BODY_NAME, typeof(RectTransform));
            RectTransform bodyRect = (RectTransform)body.transform;
            bodyRect.SetParent(rect, false);
            bodyRect.anchorMin = Vector2.zero;
            bodyRect.anchorMax = Vector2.one;
            bodyRect.offsetMin = Vector2.zero;
            bodyRect.offsetMax = Vector2.zero;
            Image image = body.AddComponent<Image>();
            image.sprite = SampleAssets.Load<Sprite>(SamplePaths.ITEM_SPRITES + SamplePaths.SEPARATOR + spec.Sprite);
            image.preserveAspect = true;
            image.raycastTarget = false;

            ItemMotion motion = AddMotion(root);
            GraphicItemHighlighter highlighter = root.AddComponent<GraphicItemHighlighter>();
            UIItemView view = root.AddComponent<UIItemView>();
            new SerializedObject(motion).SetObject(TARGET_FIELD, rect).Apply();
            new SerializedObject(highlighter).SetObject(GRAPHIC_FIELD, image).Apply();
            new SerializedObject(view)
                .SetObject(MOTION_FIELD, motion)
                .SetObject(HIGHLIGHTER_FIELD, highlighter)
                .SetObject(BODY_FIELD, body)
                .SetObject(RECT_FIELD, rect)
                .Apply();
            return SampleAssets.SavePrefab<ItemView>(root, SamplePaths.UI_PREFABS);
        }

        private static ItemView CreateWorldView(in SampleItemSpec spec, Mesh cube, Material material, string colorProperty)
        {
            GameObject root = new GameObject(WORLD_PREFIX + spec.Name);
            GameObject model = new GameObject(MODEL_NAME);
            model.transform.SetParent(root.transform, false);
            Renderer[] renderers = CreateModelCells(spec, model.transform, cube, material);

            ItemMotion motion = AddMotion(root);
            RendererItemHighlighter highlighter = root.AddComponent<RendererItemHighlighter>();
            WorldItemView view = root.AddComponent<WorldItemView>();
            new SerializedObject(motion).SetObject(TARGET_FIELD, root.transform).Apply();
            new SerializedObject(highlighter).SetObjects(RENDERERS_FIELD, renderers).SetString(COLOR_PROPERTY_FIELD, colorProperty).Apply();
            new SerializedObject(view)
                .SetObject(MOTION_FIELD, motion)
                .SetObject(HIGHLIGHTER_FIELD, highlighter)
                .SetObject(BODY_FIELD, model)
                .SetObject(MODEL_FIELD, model.transform)
                .Apply();
            return SampleAssets.SavePrefab<ItemView>(root, SamplePaths.WORLD_PREFABS);
        }

        private static Renderer[] CreateModelCells(in SampleItemSpec spec, Transform parent, Mesh cube, Material material)
        {
            int count = 0;
            for (int y = 0; y < spec.Height; y++)
            {
                for (int x = 0; x < spec.Width; x++)
                {
                    if (spec.IsFilled(x, y))
                    {
                        count++;
                    }
                }
            }

            Renderer[] renderers = new Renderer[count];
            int index = 0;
            float centerX = (spec.Width - 1) * SampleLayout.HALF;
            float centerY = (spec.Height - 1) * SampleLayout.HALF;
            for (int y = 0; y < spec.Height; y++)
            {
                for (int x = 0; x < spec.Width; x++)
                {
                    if (spec.IsFilled(x, y))
                    {
                        renderers[index] = CreateMesh(CUBE_NAME, cube, material, parent);
                        Transform cell = renderers[index].transform;
                        cell.localPosition = new Vector3(x - centerX, spec.ModelHeight * SampleLayout.HALF, centerY - y);
                        cell.localScale = new Vector3(SampleLayout.MODEL_FOOTPRINT, spec.ModelHeight, SampleLayout.MODEL_FOOTPRINT);
                        index++;
                    }
                }
            }

            return renderers;
        }

        private static ItemMotion AddMotion(GameObject root)
        {
#if GRID_SYSTEM_PRIMETWEEN
            ItemMotion motion = root.AddComponent<PrimeTweenItemMotion>();
#else
            ItemMotion motion = root.AddComponent<InstantItemMotion>();
#endif
            return motion;
        }

        private static ItemViewCatalog CreateCatalog(string name, SampleContent content, ItemView[] views)
        {
            ItemViewCatalog catalog = SampleAssets.LoadOrCreate<ItemViewCatalog>(SamplePaths.CATALOGS, name);
            SerializedObject serialized = new SerializedObject(catalog);
            SerializedProperty entries = serialized.FindProperty(ENTRIES_FIELD);
            entries.arraySize = views.Length;
            for (int i = 0; i < views.Length; i++)
            {
                SerializedProperty entry = entries.GetArrayElementAtIndex(i);
                entry.FindPropertyRelative(DEFINITION_FIELD).objectReferenceValue = content.Items[SampleSpecs.Items[i].Name];
                entry.FindPropertyRelative(PREFAB_FIELD).objectReferenceValue = views[i];
            }

            serialized.Apply();
            return catalog;
        }
    }
}
