using UnityEngine;

public class ReactionContainer : MonoBehaviour
{
    [Header("Configuration")]
    [SerializeField] private Reaction[] _reactions;

    private void OnValidate()
    {
        GetReactionsInChildren();
    }

    public Reaction[] GetReactions()
    {
        return _reactions;
    }

    [ContextMenu("GetReactionsInChildren")]
    public void GetReactionsInChildren()
    {
        _reactions = GetComponentsInChildren<Reaction>();
    }
}
