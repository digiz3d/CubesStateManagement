using UnityEngine;

public class PlayerRotate : MonoBehaviour
{
    float mouseSensitivity = 1f;
    float walkingSpeed = 5f;

    [SerializeField]
    Camera cam;

    float xRotation = 0;
    CharacterController cc;

    Vector3 desiredMovementDirection;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        cc = GetComponent<CharacterController>();
    }

    void Update()
    {
        float x = Input.GetAxisRaw("Mouse X") * mouseSensitivity;
        float y = Input.GetAxisRaw("Mouse Y") * mouseSensitivity;

        float forward = Input.GetAxisRaw("Vertical");
        float shift = Input.GetAxisRaw("Horizontal");

        desiredMovementDirection = (Vector3.zero + transform.forward * forward + transform.right * shift);

        xRotation -= y;
        xRotation = Mathf.Clamp(xRotation, -80f, 80f);
        cam.transform.localRotation = Quaternion.Euler(xRotation, 0, 0);
        transform.localRotation = Quaternion.Euler(0, transform.localRotation.eulerAngles.y + x, 0);
        cc.SimpleMove(desiredMovementDirection.normalized * walkingSpeed);
    }
}
