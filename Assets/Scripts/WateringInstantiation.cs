using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UIElements;

public class WateringInstantiation : MonoBehaviour
    {
    [SerializeField] ParticleSystem waterStream; // The water stream prefab to spawn
    [SerializeField] float activationAngle; // The angle at which the water stream activates
    [SerializeField] float limitAngle; // The angle at which the water stream deactivates

    private bool isWatering = false;
    private Vector3 initialForward;

    private void Awake()
    {

        waterStream.Stop();
        initialForward = transform.forward;
    }
    private void Update()
        {
        // Get the current rotation of the watering can
        float forwardTilt = transform.localEulerAngles.x;

        // Check if the watering can is tilted more than the activation angle
        if (forwardTilt > activationAngle && forwardTilt < limitAngle)
            {
            if (!isWatering)
                {
                StartWatering();
                }
            }
        else 
            {
            if (isWatering)
                {
                StopWatering();
                }
            }
        }

    private void StartWatering()
        {
        isWatering = true;
        waterStream.Play();
    }

    private void StopWatering()
        {
        waterStream.Stop();
        isWatering = false;
        }
    }
