﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CalamityController : MonoBehaviour
{
    public float CalamityDuration;

    public List<CalamityMovement> CalamityMovements;

    private float _calamityStartTime;

    void Start()
    {
        _calamityStartTime = Time.time;

        foreach (CalamityMovement cm in CalamityMovements)
        {
            cm.Setup();
        }
    }

    void Update()
    {
        float elapsedCalamityTime = Time.time - _calamityStartTime;
        float calamityProgress = elapsedCalamityTime / CalamityDuration;

        foreach (CalamityMovement cm in CalamityMovements)
        {
            cm.SetProgress(calamityProgress);
        }
    }
}

[System.Serializable]
public class CalamityMovement
{
    public Transform Subject;
    public Vector3 Movement;
    [HideInInspector]
    public Vector3 StartPosition;

    public Vector3 Rotation;
    [HideInInspector]
    public Vector3 StartRotation;

    public void Setup()
    {
        StartPosition = Subject.localPosition;
        StartRotation = Subject.localRotation.eulerAngles;
    }

    public void SetProgress(float progress)
    {
        Subject.localPosition = Vector3.Lerp(StartPosition, Movement, progress);
        Subject.SetPositionAndRotation(Vector3.Lerp(StartPosition, Movement, progress), Quaternion.Euler(Vector3.Slerp(StartRotation, Rotation, progress)));
    }
}
