using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Pagetorn/Events/SfxEvent")]
public class SoundEvent : Event
{
    [Header("Sfx")]
    [SerializeField] private SfxReference _sfx;

    [Header("Music")]
    [SerializeField] private MusicTrack _music;

    public override void ActivateEvent()
    {
        _sfx.Play();
        if (_music) _music.Play();
    }
}