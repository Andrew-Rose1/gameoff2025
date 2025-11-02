using UnityEngine;

namespace WaveMagicSurvivor.Utils
{
    public class CameraController : MonoBehaviour
    {
        [Header("Target")]
        [SerializeField] private Transform target;
        [SerializeField] private string targetTag = "Player";

        [Header("Settings")]
        [SerializeField] private float smoothSpeed = 5f;
        [SerializeField] private Vector3 offset = new Vector3(0, 0, -10);
        [SerializeField] private bool useFixedUpdate = false;

        private void Start()
        {
            if (target == null)
            {
                GameObject targetObj = GameObject.FindGameObjectWithTag(targetTag);
                if (targetObj != null)
                    target = targetObj.transform;
            }
        }

        private void LateUpdate()
        {
            if (!useFixedUpdate)
                UpdateCameraPosition();
        }

        private void FixedUpdate()
        {
            if (useFixedUpdate)
                UpdateCameraPosition();
        }

        private void UpdateCameraPosition()
        {
            if (target == null) return;

            Vector3 desiredPosition = target.position + offset;
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
            transform.position = smoothedPosition;
        }
    }
}

