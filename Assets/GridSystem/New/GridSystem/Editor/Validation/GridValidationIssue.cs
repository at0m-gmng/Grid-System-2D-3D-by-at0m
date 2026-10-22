namespace GridSystem.Editor.Validation
{
    using UnityEngine;

    /// <summary>
    /// Single configuration defect found by a validator.
    /// </summary>
    public readonly struct GridValidationIssue
    {
        public GridValidationIssue(GridValidationSeverity severity, string message, Object context)
        {
            Severity = severity;
            Message = message;
            Context = context;
        }

        /// <summary>
        /// How bad the defect is.
        /// </summary>
        public readonly GridValidationSeverity Severity;

        /// <summary>
        /// Description of the defect and of its runtime consequence.
        /// </summary>
        public readonly string Message;

        /// <summary>
        /// Asset to highlight when the issue is clicked.
        /// </summary>
        public readonly Object Context;
    }

    /// <summary>
    /// Weight of a configuration defect.
    /// </summary>
    public enum GridValidationSeverity
    {
        /// <summary>
        /// Works, but may be unintended.
        /// </summary>
        Warning,

        /// <summary>
        /// Breaks at runtime or loses data.
        /// </summary>
        Error
    }
}
