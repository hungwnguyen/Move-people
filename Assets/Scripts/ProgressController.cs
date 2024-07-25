using SuperPack;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class ProgressController : MonoBehaviour
{
    private Animator _animator;

    void Awake()
    {
        _animator = GetComponent<Animator>();
    }
    /// <summary>
    /// Update the position of the progress bar requires a value between 0 and 1
    /// </summary>
    /// <param name="position"></param>
    public void UpdatePosition(float position)
    {
        SuperAnimator.SetRuntimeAnimationKeyFrame(_animator, "slide", position);
    }
}
