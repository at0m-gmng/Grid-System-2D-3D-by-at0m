namespace InputSystem
{
    using UnityEngine;
    using UnityEngine.InputSystem;

    [CreateAssetMenu(menuName = "Configs/InputConfig", fileName = "InputConfig")]
    public sealed class InputConfig : ScriptableObject
    {
        [field: SerializeField] public InputActionReference BeginDrag { get; private set; }
        [field: SerializeField] public InputActionReference Drag { get; private set; }
        [field: SerializeField] public InputActionReference EndDrag { get; private set; }
        [field: SerializeField] public InputActionReference Rotation { get; private set; }
    }
}