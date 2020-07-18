﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class CalamityController : MonoBehaviour
{
    public float CalamityDuration;

    public List<CalamityMovement> CalamityMovements;

    private float _calamityStartTime;

    private float _whiteBalanceStart;
    public float WhiteBalanceTarget;

    private Color _filterStart;
    public Color FilterTarget;

    public List<CalamityParticles> RampUpParticles;

    public List<CalamitySound> CalamitySounds;

    private PostProcessVolume _activePostVolume;
    private ColorGrading _colorGrading;

    void Start()
    {
        _calamityStartTime = Time.time;

        _activePostVolume = FindObjectOfType<PostProcessVolume>();
        _activePostVolume.profile.TryGetSettings(out _colorGrading);
        _whiteBalanceStart = _colorGrading.temperature.value;

        _filterStart = _colorGrading.colorFilter.value;

        foreach (CalamityMovement cm in CalamityMovements)
        {
            cm.Setup();
        }

        foreach (CalamityParticles cp in RampUpParticles)
        {
            cp.Setup();
        }

        foreach (CalamitySound cs in CalamitySounds)
        {
            cs.Setup();
        }
    }

    void Update()
    {
        float elapsedCalamityTime = Time.time - _calamityStartTime;
        float linearCalamityProgress = elapsedCalamityTime / CalamityDuration;
        float calamityProgress = linearCalamityProgress * linearCalamityProgress * linearCalamityProgress * linearCalamityProgress;

        _colorGrading.temperature.value = Mathf.Lerp(_whiteBalanceStart, WhiteBalanceTarget, calamityProgress);
        _colorGrading.colorFilter.value = Color.Lerp(_filterStart, FilterTarget, calamityProgress);

        foreach (CalamityMovement cm in CalamityMovements)
        {
            cm.SetProgress(calamityProgress);
        }

        foreach (CalamityParticles cp in RampUpParticles)
        {
            cp.SetProgress(calamityProgress);
        }

        foreach (CalamitySound cs in CalamitySounds)
        {
            cs.SetProgress(calamityProgress);
        }
    }

    public float EaseInQuad(float start, float end, float value)
    {
        end -= start;
        return end * value * value + start;
    }
}

[System.Serializable]
public class CalamityMovement
{
    public Transform Subject;
    public Vector3 EndPosition;
    [HideInInspector]
    public Vector3 StartPosition;

    public Vector3 Rotation;
    [HideInInspector]
    public Vector3 StartRotation;

    public float TargetScaleFactor;
    [HideInInspector]
    public Vector3 StartScale;

    public void Setup()
    {
        StartPosition = Subject.position;
        StartRotation = Subject.localRotation.eulerAngles;
        StartScale = Subject.localScale;
    }

    public void SetProgress(float progress)
    {
        Subject.localScale = StartScale * Mathf.Lerp(1, TargetScaleFactor, progress);
        Subject.SetPositionAndRotation(Vector3.Lerp(StartPosition, EndPosition, progress), Quaternion.Euler(Vector3.Slerp(StartRotation, StartRotation + Rotation, progress)));
    }
}

[System.Serializable]
public class CalamityParticles
{
    public ParticleSystem ParticleSystem;
    public float TargetSpeed;
    [HideInInspector]
    public float StartSpeed;
    public float TargetGravity;
    [HideInInspector]
    public float StartGravity;

    private ParticleSystem.MainModule _mainModule;

    public void Setup()
    {
        _mainModule = ParticleSystem.main;
        StartSpeed = _mainModule.startSpeedMultiplier;
        StartGravity = _mainModule.gravityModifierMultiplier;
    }

    public void SetProgress(float progress)
    {
        _mainModule.startSpeedMultiplier = Mathf.Lerp(StartSpeed, TargetSpeed, progress);
        _mainModule.gravityModifierMultiplier = Mathf.Lerp(StartGravity, TargetGravity, progress);
    }
}

[System.Serializable]
public class CalamitySound
{
    public AudioSource AudioSource;
    public float TargetVolume;
    [HideInInspector]
    public float StartVolume;

    public void Setup()
    {
        StartVolume = AudioSource.volume;
    }

    public void SetProgress(float progress)
    {
        AudioSource.volume = Mathf.Lerp(StartVolume, TargetVolume, progress);
    }
}