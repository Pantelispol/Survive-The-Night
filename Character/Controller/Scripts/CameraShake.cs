using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Cinemachine;

    public class CameraShake : MonoBehaviour
    {
        public static CameraShake Instance { get; private set; }
        CinemachineCamera cinemachineVirtualCamera;
        float shakerTimer;
        float shakerTimerTotal;
        float startingIntensity;
        void Awake()
        {
            Instance = this;
            cinemachineVirtualCamera = GetComponent<CinemachineCamera>();

        }

        public void ShakeCamera(float intensity, float time)
        {
            var cinemachineBasicMultiChannelPerlin = cinemachineVirtualCamera.GetCinemachineComponent(CinemachineCore.Stage.Noise) as CinemachineBasicMultiChannelPerlin;
            if (cinemachineBasicMultiChannelPerlin == null)
            {
                Debug.LogError("CinemachineBasicMultiChannelPerlin component is missing on the CinemachineCamera.");
                return;
            }

            cinemachineBasicMultiChannelPerlin.AmplitudeGain = intensity;

            startingIntensity = intensity;
            shakerTimerTotal = time;
            shakerTimer = time;
        }


        private void Update()
        {
            if (shakerTimer > 0)
            {
                shakerTimer -= Time.deltaTime;
                var cinemachineBasicMultiChannelPerlin = cinemachineVirtualCamera.GetCinemachineComponent(CinemachineCore.Stage.Noise) as CinemachineBasicMultiChannelPerlin;
                cinemachineBasicMultiChannelPerlin.AmplitudeGain = Mathf.Lerp(startingIntensity, 0f, 1f - shakerTimer / shakerTimerTotal);
            }
        }
    }