namespace GridSystem.Samples.Editor
{
    using System.Collections.Generic;
    using UnityEditor;
    using UnityEngine;

    /// <summary>
    /// Fluent setters of serialized fields by property path.
    /// </summary>
    public static class SerializedObjectExtensions
    {
        /// <summary>
        /// Sets object reference.
        /// </summary>
        public static SerializedObject SetObject(this SerializedObject target, string path, Object value)
        {
            target.FindProperty(path).objectReferenceValue = value;
            return target;
        }

        /// <summary>
        /// Sets object reference array.
        /// </summary>
        public static SerializedObject SetObjects<T>(this SerializedObject target, string path, IReadOnlyList<T> values) where T : Object
        {
            SerializedProperty property = target.FindProperty(path);
            property.arraySize = values.Count;
            for (int i = 0; i < values.Count; i++)
            {
                property.GetArrayElementAtIndex(i).objectReferenceValue = values[i];
            }

            return target;
        }

        /// <summary>
        /// Sets integer.
        /// </summary>
        public static SerializedObject SetInt(this SerializedObject target, string path, int value)
        {
            target.FindProperty(path).intValue = value;
            return target;
        }

        /// <summary>
        /// Sets float.
        /// </summary>
        public static SerializedObject SetFloat(this SerializedObject target, string path, float value)
        {
            target.FindProperty(path).floatValue = value;
            return target;
        }

        /// <summary>
        /// Sets boolean.
        /// </summary>
        public static SerializedObject SetBool(this SerializedObject target, string path, bool value)
        {
            target.FindProperty(path).boolValue = value;
            return target;
        }

        /// <summary>
        /// Sets string.
        /// </summary>
        public static SerializedObject SetString(this SerializedObject target, string path, string value)
        {
            target.FindProperty(path).stringValue = value;
            return target;
        }

        /// <summary>
        /// Sets Vector2.
        /// </summary>
        public static SerializedObject SetVector2(this SerializedObject target, string path, Vector2 value)
        {
            target.FindProperty(path).vector2Value = value;
            return target;
        }

        /// <summary>
        /// Sets Vector2Int.
        /// </summary>
        public static SerializedObject SetVector2Int(this SerializedObject target, string path, Vector2Int value)
        {
            target.FindProperty(path).vector2IntValue = value;
            return target;
        }

        /// <summary>
        /// Writes changes without undo and returns nothing to allow chaining at the end.
        /// </summary>
        public static void Apply(this SerializedObject target) => target.ApplyModifiedPropertiesWithoutUndo();
    }
}
