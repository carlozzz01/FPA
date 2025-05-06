using UnityEngine;
using UnityEngine.Events;

public class ReactionEvent : Reaction
{
    [Header("Event Configuration")]
    public UnityEvent OnInteract;

    protected override void React()
    {
        OnInteract?.Invoke();
    }
}
