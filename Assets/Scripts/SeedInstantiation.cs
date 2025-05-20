using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Unity.VisualScripting;
public class SeedInstantiation : MonoBehaviour
{
    [SerializeField] ParticleSystem seedStream; // The water stream prefab to spawn
    [SerializeField] float activationAngle; // The angle at which the water stream activates
    [SerializeField] float limitAngle;
    [SerializeField] float eulerX;
    [SerializeField] float eulerZ;

    public GameObject BudPrefab;
    public GameObject FlowerPrefab;

    private int count;
    public bool isSeeding = false;
    private void Awake()
    {
        seedStream.Stop();
        count = 0;
    }
    private void Update()
    {
        count++;
        if (count == 6)
        {
            // Get the current rotation of the watering can
            Vector3 eulerAngles = GetPitchYawRollDeg(transform.rotation);

            eulerX = WrapAngle(eulerAngles.x);
            eulerZ = WrapAngle(eulerAngles.y);


            // Check if the watering can is tilted more than the activation angle
            if (Math.Abs(eulerZ) > activationAngle || (eulerX > activationAngle || eulerX < limitAngle))
            {

                if (!isSeeding)
                {
                    StartSeeding();
                }
            }
            else
            {
                if (isSeeding)
                {
                    StopSeeding();
                }
            }
            count = 0;
        }
    }

    private void StartSeeding()
    {
        isSeeding = true;
        // Instantiate the water stream prefab at the spawn point
        seedStream.Play();
    }

    private void StopSeeding()
    {
        isSeeding = false;
        // Destroy the current water stream
        seedStream.Stop();
    }

    public static Vector3 GetPitchYawRollRad(Quaternion rotation)
    {
        float roll = Mathf.Atan2(2 * rotation.y * rotation.w - 2 * rotation.x * rotation.z, 1 - 2 * rotation.y * rotation.y - 2 * rotation.z * rotation.z);
        float pitch = Mathf.Atan2(2 * rotation.x * rotation.w - 2 * rotation.y * rotation.z, 1 - 2 * rotation.x * rotation.x - 2 * rotation.z * rotation.z);
        float yaw = Mathf.Asin(2 * rotation.x * rotation.y + 2 * rotation.z * rotation.w);

        return new Vector3(pitch, yaw, roll);
    }

    public static Vector3 GetPitchYawRollDeg(Quaternion rotation)
    {
        Vector3 radResult = GetPitchYawRollRad(rotation);
        return new Vector3(radResult.x * Mathf.Rad2Deg, radResult.y * Mathf.Rad2Deg, radResult.z * Mathf.Rad2Deg);
    }

    private static float WrapAngle(float angle)
    {
        angle %= 360;
        if (angle > 180)
            return angle - 360;

        return angle;
    }
}
