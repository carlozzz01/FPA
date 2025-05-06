using System.Collections;
using UnityEngine;

public class Reaction : MonoBehaviour
{
    [Header("Description")]
    [TextArea]
    [SerializeField] private string _description;

    [Header("Configuration")]
    [SerializeField] private float _reactionDelay;

    private Interactable _interactable;

    protected virtual void React()
    {

    }

    protected virtual void PostReact()
    {
        _interactable.NextReaction();
    }

    protected virtual IEnumerator DelayPostReaction()
    {
        yield return new WaitForSeconds(_reactionDelay);

        PostReact();
    }

    public void ExecuteReaction()
    {
        React();

        StartCoroutine(DelayPostReaction());
    }

    public void SetInteractable(Interactable interactable)
    {
        _interactable = interactable;
    }
}
