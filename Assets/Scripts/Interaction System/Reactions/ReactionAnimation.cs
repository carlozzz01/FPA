using UnityEngine;

public class ReactionAnimation : Reaction
{
    [Header("Animation Configuration")]
    [SerializeField] private Animator _targetAnimator;
    [SerializeField] public string _key;
    [SerializeField] public float _value;
    [SerializeField] private AnimationReactionType _reactionType;

    public enum AnimationReactionType
    {
        Trigger,
        Bool,
        Float
    }

    void OnValidate()
    {
        if (_reactionType == AnimationReactionType.Bool)
        {
            _value = Mathf.FloorToInt(_value);
        }
    }

    protected override void React()
    {
        switch (_reactionType)
        {
            case AnimationReactionType.Trigger:

                _targetAnimator.SetTrigger(_key);

                break;

            case AnimationReactionType.Bool:

                _targetAnimator.SetBool(_key, _value == 1);

                break;

            case AnimationReactionType.Float:

                _targetAnimator.SetFloat(_key, _value);

                break;

            default:
                break;
        }
    }
}
