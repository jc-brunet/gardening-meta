using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UIElements;

public class WateringInstantiation : MonoBehaviour
    {
    public bool IsEmpty;

    [SerializeField] ParticleSystem waterStream; // The water stream prefab to spawn
    [SerializeField] float activationAngle; // The angle at which the water stream activates
    [SerializeField] float limitAngle; // The angle at which the water stream deactivates
    [SerializeField] float EulerX;
    [SerializeField] float WaterDecreasingRate;
    [SerializeField] int MaxWaterAmount;

    [SerializeField] GameObject[] WaterLevel; 

    private int _waterAmountLeft;
    private bool _isWatering;
    private int _waterLevelStep;
    private int _currentWaterLevel;

    [SerializeField] AudioFade WaterSoundFade;

    private void Awake()
    {
        waterStream.Stop();
        MaxWaterAmount -= MaxWaterAmount % 5;
        _waterAmountLeft = MaxWaterAmount;
        _waterLevelStep = MaxWaterAmount / 5;
        _currentWaterLevel = 5;
        _isWatering = false;
    }
    private void Update()
        {
        if (!IsEmpty)
        {

            // Get the current rotation of the watering can
            Vector3 eulerAngles = GetPitchYawRollDeg(transform.rotation);

            EulerX = eulerAngles.x;


            if (!_isWatering)
            {
                if (_waterAmountLeft > 0 && EulerX > activationAngle && EulerX < limitAngle)
                {
                    StartWatering();
                }
            }
            else
            {
                if (_waterAmountLeft <= 0 || !(EulerX > activationAngle && EulerX < limitAngle))
                {
                    StopWatering();
                    if (_waterAmountLeft <= 0)
                    {
                        IsEmpty = true;
                    }
                }
            }
        }

        //// Check if the watering can is tilted more than the activation angle
        //if ((EulerX > activationAngle || EulerX < limitAngle))
        // Check if the watering can is tilted more than the activation angle

    }

    private void StartWatering()
        {
        _isWatering = true;
        waterStream.Play();
        InvokeRepeating(nameof(_DecreaseWater), WaterDecreasingRate, WaterDecreasingRate);
        WaterSoundFade.FadeIn();
    }

    private void StopWatering()
        {
        waterStream.Stop();
        _isWatering = false;
        CancelInvoke(nameof(_DecreaseWater));
        WaterSoundFade.FadeOut();
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

    private void _DecreaseWater()
    {
        if(_waterAmountLeft > 0)
        {
            _waterAmountLeft--;
            _CheckWaterLevel(_currentWaterLevel);
        }
    }

    public void FillTank()
    {
        _waterAmountLeft = MaxWaterAmount;
        IsEmpty = false;
        _ResetWaterLevel();
    }

    private void _CheckWaterLevel(int lastWaterLevel)
    {
        _currentWaterLevel = _waterAmountLeft / _waterLevelStep + ((_waterAmountLeft % _waterLevelStep==0) ? 0:1);
        if (lastWaterLevel > _currentWaterLevel)
        {
            WaterLevel[_currentWaterLevel].SetActive(false);
        }
    }

    private void _ResetWaterLevel()
    {
        foreach (GameObject level in WaterLevel)
        {
            level.SetActive(true);
        }
    }
}
