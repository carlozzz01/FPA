using UnityEngine;
using Managers;

public class ReactionScene : Reaction
{
    [Header("Scene Configuration")]
    [SerializeField] private string _sceneName;

    protected override void React()
    {
        // SceneController.Instance.LoadScene(_sceneName);
    }
}
