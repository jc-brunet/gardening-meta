using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UIElements;

public class FlamingInstantiation : MonoBehaviour
    {
    [SerializeField] ParticleSystem FlameStream; // The water stream prefab to spawn
    [SerializeField] AudioSource FlameAudio;
    [SerializeField] float ActivationAngle; // The angle at which the water stream activates
    [SerializeField] float LimitAngle; // The angle at which the water stream deactivates

    private bool _isFlaming = false;

    private void Awake()
    {
        FlameStream.Stop();
    }
    private void Update()
        {
        // Get the current rotation of the watering can
        float forwardTilt = transform.localEulerAngles.x;

        // Check if the watering can is tilted more than the activation angle
        if (forwardTilt > ActivationAngle && forwardTilt < LimitAngle)
            {
            if (!_isFlaming)
                {
                StartFlaming();
                }
            }
        else 
            {
            if (_isFlaming)
                {
                StopFlaming();
                }
            }
        }

    private void StartFlaming()
        {
        _isFlaming = true;
        FlameStream.Play();
        FlameAudio.Play();
    }

    private void StopFlaming()
        {
        _isFlaming = false;
        FlameStream.Stop();
        FlameAudio.Stop();

        }
    }
