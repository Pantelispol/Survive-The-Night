using UnityEngine;

    public class FreeLookCamera : MonoBehaviour
    {
        [SerializeField] private float rotationSpeed = 5f; // Speed of camera rotation
        [SerializeField] private float moveSpeed = 5f;     // Speed of camera movement

        void Update()
        {
            // Rotate the camera based on mouse input
            float mouseX = Input.GetAxis("Mouse X") * rotationSpeed;
            float mouseY = Input.GetAxis("Mouse Y") * rotationSpeed;

            transform.Rotate(Vector3.up, mouseX, Space.World); // Rotate around the Y-axis
            transform.Rotate(Vector3.right, -mouseY, Space.Self); // Rotate around the X-axis

            // Move the camera based on keyboard input
            float moveX = Input.GetAxis("Horizontal"); // A/D or Left/Right Arrow
            float moveZ = Input.GetAxis("Vertical");   // W/S or Up/Down Arrow

            Vector3 moveDirection = new Vector3(moveX, 0, moveZ).normalized;
            transform.Translate(moveDirection * moveSpeed * Time.deltaTime, Space.Self);
        }
    }
