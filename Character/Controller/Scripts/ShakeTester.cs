using UnityEngine;

namespace Script.Controller
{
    public class ShakeTrigger : MonoBehaviour
    {
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                CameraShake.Instance.ShakeCamera(5f, 0.5f);
            }
        }
    }
}