using System;
using UnityEngine;

public class AttackerEffects: MonoBehaviour
{
    private static readonly int ANIM_ATTACK = Animator.StringToHash("attackTrigger");
    
    [SerializeField] private Animator _animator;
    [SerializeField] private AudioSource _audioSource;
    private Animator[] _animatorWeapons;

    private void Awake()
    {
        _animatorWeapons = _animator.transform.GetComponentsInChildren<Animator>();
    }

    public void Play()
    {
        _animator.SetTrigger(ANIM_ATTACK);
        _audioSource.Play();
    }
}
