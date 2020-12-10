using UnityEngine;

public class PlayerController : MonoBehaviour
{
    float mouseSensitivity = 1f;
    float walkingSpeed = 5f;

    [SerializeField]
    Camera cam;

    float xRotation = 0;
    CharacterController cc;

    Vector2 movementInput;
    Vector3 desiredMove;
    CollisionFlags characterCollisionFlag;
    Vector3 moveDir;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        cc = GetComponent<CharacterController>();
    }

    void Update()
    {
        MouseRotation();
    }

    void MouseRotation()
    {
        float x = Input.GetAxisRaw("Mouse X") * mouseSensitivity;
        float y = Input.GetAxisRaw("Mouse Y") * mouseSensitivity;
        xRotation -= y;
        xRotation = Mathf.Clamp(xRotation, -80f, 80f);
        cam.transform.localRotation = Quaternion.Euler(xRotation, 0, 0);
        transform.localRotation = Quaternion.Euler(0, transform.localRotation.eulerAngles.y + x, 0);
    }

    private void FixedUpdate()
    {
        PlayerMovement();
    }

    void PlayerMovement()
    {
        float vertical = Input.GetAxisRaw("Vertical");
        float horizontal = Input.GetAxisRaw("Horizontal");
        movementInput = new Vector2(horizontal, vertical);

        if (movementInput.sqrMagnitude > 1)
        {
            movementInput.Normalize();
        }

        desiredMove = transform.forward * movementInput.y + transform.right * movementInput.x;

        RaycastHit hitInfo;
        Physics.SphereCast(transform.position, cc.radius, Vector3.down, out hitInfo, cc.height / 2f, Physics.AllLayers, QueryTriggerInteraction.Ignore);
        Debug.DrawLine(transform.position, transform.position + desiredMove, Color.yellow);
        desiredMove = Vector3.ProjectOnPlane(desiredMove, hitInfo.normal).normalized;
        Debug.DrawLine(transform.position, transform.position + desiredMove*0.8f, Color.cyan);

        moveDir.x = desiredMove.x * walkingSpeed;
        moveDir.z = desiredMove.z * walkingSpeed;

        if (cc.isGrounded)
        {
            //moveDir.y = 10f; // stick to ground here if needed
        }
        else
        {
            moveDir += Physics.gravity * Time.fixedDeltaTime;
        }

        cc.Move(moveDir * Time.fixedDeltaTime);
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        Rigidbody rb = hit.collider.attachedRigidbody;
        //dont move the rigidbody if the character is on top of it
        if (characterCollisionFlag == CollisionFlags.Below)
        {
            return;
        }

        if (rb == null || rb.isKinematic)
        {
            return;
        }
        rb.AddForceAtPosition(cc.velocity * 0.1f, hit.point, ForceMode.Impulse);
    }

}
