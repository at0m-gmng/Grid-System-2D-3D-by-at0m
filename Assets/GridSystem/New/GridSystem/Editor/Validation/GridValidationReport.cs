namespace GridSystem.Editor.Validation
{
    using System.Collections.Generic;
    using UnityEngine;

    /// <summary>
    /// Collects configuration defects of one asset. Reused between validation runs.
    /// </summary>
    public sealed class GridValidationReport
    {
        private readonly List<GridValidationIssue> _issues = new();

        /// <summary>
        /// Collected defects in the order they were found.
        /// </summary>
        public IReadOnlyList<GridValidationIssue> Issues => _issues;

        /// <summary>
        /// True when nothing was found.
        /// </summary>
        public bool IsEmpty => _issues.Count == 0;

        /// <summary>
        /// Reports a defect that breaks at runtime or loses data.
        /// </summary>
        public void AddError(string message, Object context) => _issues.Add(new GridValidationIssue(GridValidationSeverity.Error, message, context));

        /// <summary>
        /// Reports a defect that works, but may be unintended.
        /// </summary>
        public void AddWarning(string message, Object context) => _issues.Add(new GridValidationIssue(GridValidationSeverity.Warning, message, context));

        /// <summary>
        /// Drops every collected defect.
        /// </summary>
        public void Clear() => _issues.Clear();
    }
}
