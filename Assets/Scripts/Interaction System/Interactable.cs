using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    [Header("Configuration")]
    // [SerializeField] private bool _interactOnEnter;
    // [SerializeField] private ReactionContainer _positiveReactions;
    [SerializeField] private ReactionContainer _defaultReactions;

    [Header("Components")]
    [SerializeField] private Collider _collider;

    private bool _isReacting;
    private Queue<Reaction> _reactions = new Queue<Reaction>();

    private void Awake()
    {
        if (_collider == null) _collider = GetComponent<Collider>();
    }

    private void Start()
    {
        _collider.isTrigger = true;
    }

    // private void OnTriggerEnter(Collider other)
    // {
    //     if (other.CompareTag("Player"))
    //     {
    //         if (_interactOnEnter && !_isReacting) Interact();
    //     }
    // }

    public void Interact()
    {
        if (_isReacting) return;

        _isReacting = true;

        QueueReactions(_defaultReactions);

        NextReaction();
    }

    private void QueueReactions(ReactionContainer reactionContainer)
    {
        _reactions.Clear();

        foreach (Reaction reaction in reactionContainer.GetReactions())
        {
            reaction.SetInteractable(this);

            _reactions.Enqueue(reaction);
        }
    }

    public void NextReaction()
    {
        if (_reactions.Count > 0)
        {
            _reactions.Dequeue().ExecuteReaction();
        }
        else
        {
            _isReacting = false;
        }
    }
}
