using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class PostProcessingManager : MonoBehaviour
{
    [SerializeField] private PostProcessVolume _masterVolume;
    [SerializeField] private PostProcessVolume _secondaryVolume;

    private bool _fogEnabled;
    private float _fogDensity;
    private Color _fogColor;
    private Coroutine _fogRoutine;
    private Coroutine _volumeRoutine;

    private void Start()
    {
        _fogEnabled = RenderSettings.fog;
        _fogDensity = RenderSettings.fogDensity;
        _fogColor = RenderSettings.fogColor;
    }

    public void SetFog(float fogDensity, Color fogColor, AnimationCurve fadeIn, float duration, AnimationCurve fadeOut)
    {
        if (_fogRoutine != null) StopCoroutine(_fogRoutine);
        _fogRoutine = StartCoroutine(SetFogRoutine(fogDensity, fogColor, fadeIn, duration, fadeOut));
    }

    private IEnumerator SetFogRoutine(float fogDensity, Color fogColor, AnimationCurve fadeIn, float duration, AnimationCurve fadeOut)
    {
        RenderSettings.fog = true;
        SetWeight(0);
        for (float t = 0; t < 1; t += Time.deltaTime)
        {
            SetWeight(fadeIn.Evaluate(t));
            yield return null;
        }
        SetWeight(1);
        for (float t = 0; t < duration; t += Time.deltaTime)
        {
            yield return null;
        }
        for (float t = 0; t < 1; t += Time.deltaTime)
        {
            SetWeight(fadeOut.Evaluate(t));
            yield return null;
        }
        SetWeight(0);
        RenderSettings.fog = _fogEnabled;
        _fogRoutine = null;

        void SetWeight(float weight)
        {
            RenderSettings.fogDensity = Mathf.Lerp(_fogDensity, fogDensity, weight);
            RenderSettings.fogColor = Color.Lerp(_fogColor, fogColor, weight);
        }
    }

    public void SetSecondaryVolume(PostProcessProfile profile, AnimationCurve fadeIn, float duration, AnimationCurve fadeOut)
    {
        if (_volumeRoutine != null) StopCoroutine(_volumeRoutine);
        _volumeRoutine = StartCoroutine(SetSecondaryVolumeRoutine(profile, fadeIn, duration, fadeOut));
    }

    private IEnumerator SetSecondaryVolumeRoutine(PostProcessProfile profile, AnimationCurve fadeIn, float duration, AnimationCurve fadeOut)
    {
        _secondaryVolume.profile = profile;
        SetWeight(0);
        for (float t = 0; t < 1; t += Time.deltaTime)
        {
            SetWeight(fadeIn.Evaluate(t));
            yield return null;
        }
        SetWeight(1);
        for (float t = 0; t < duration; t += Time.deltaTime)
        {
            yield return null;
        }
        for (float t = 0; t < 1; t += Time.deltaTime)
        {
            SetWeight(fadeOut.Evaluate(t));
            yield return null;
        }
        SetWeight(0);
        _volumeRoutine = null;

        void SetWeight(float weight)
        {
            weight = Mathf.Clamp01(weight);
            _masterVolume.weight = 1 - weight;
            _secondaryVolume.weight = weight;
        }
    }
}
