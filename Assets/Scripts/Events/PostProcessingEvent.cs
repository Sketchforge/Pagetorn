using CoffeyUtils;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

[CreateAssetMenu(menuName = "Pagetorn/Events/PostProcessingEvent")]
public class PostProcessingEvent : Event
{
    [Header("Fog")]
    [SerializeField] private bool _affectFog;
    [SerializeField, ShowIf("_affectFog")] private float _fogDensity = 0.02f;
    [SerializeField, ShowIf("_affectFog")] private Color _fogColor = Color.gray;

    [Header("Post Processing")]
    [SerializeField] private bool _switchPostProcessingVolume;
    [SerializeField, ShowIf("_switchPostProcessingVolume")] private PostProcessProfile _profile;

    [Header("Animations")]
    [SerializeField] private AnimationCurve _fadeIn = new AnimationCurve(new Keyframe(0, 0), new Keyframe(1, 1));
    [SerializeField] private float _duration = 4;
    [SerializeField] private AnimationCurve _fadeOut = new AnimationCurve(new Keyframe(0, 1), new Keyframe(1, 0));
    public float Duration { get { return _duration; } }

    public override void ActivateEvent(System.Action onFinished = null)
    {

        if (_affectFog)
        {
            GameManager.PostProcessingManager.SetFog(_fogDensity, _fogColor, _fadeIn, _duration, _fadeOut, onFinished);
        }
        if (_switchPostProcessingVolume)
        {
            GameManager.PostProcessingManager.SetSecondaryVolume(_profile, _fadeIn, _duration, _fadeOut);
        }
    }
}