using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class NewMonoBehaviourScript : MonoBehaviour
{
    public Animator animator;
    public Camera playerCamera;
    public float walkSpeed = 6f;
    public float runSpeed = 12f;
    public float jumpPower = 7f;
    public float gravity = 10f;
    public float lookSpeed = 2f;
    public float lookXLimit = 45f;
    public float defaultHeight = 2f;
    public float crouchHeight = 1f;
    public float crouchSpeed = 3f;
    public Texture2D aimPoint;
    public float aimScale = 1.5f;
    public GameObject projectilePrefab;
    public Transform firePoint;
    public float shootForce = 15f;
    public float projectileLifetime = 4f;
    public GameObject explosionEffectPrefab;

    private Vector3 moveDirection = Vector3.zero;
    private float rotationX = 0;
    private CharacterController characterController;
    private bool canMove = true;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 right = transform.TransformDirection(Vector3.right);
        bool isRunning = Input.GetKey(KeyCode.LeftShift);
        bool isGrounded = characterController.isGrounded;
        float vertical = Input.GetAxis("Vertical");
        float horizontal = Input.GetAxis("Horizontal");
        float movementDirectionY = moveDirection.y;
        float curSpeedX = canMove ? (isRunning ? runSpeed : walkSpeed) * vertical : 0;
        float curSpeedY = canMove ? (isRunning ? runSpeed : walkSpeed) * horizontal : 0;
        moveDirection = forward * curSpeedX + right * curSpeedY;

        if (Input.GetButton("Jump") && canMove && isGrounded)
            moveDirection.y = jumpPower;
        else
            moveDirection.y = movementDirectionY;

        if (!isGrounded)
            moveDirection.y -= gravity * Time.deltaTime;

        if (Input.GetKey(KeyCode.R) && canMove)
        {
            characterController.height = crouchHeight;
            walkSpeed = crouchSpeed;
            runSpeed = crouchSpeed;
        }
        else
        {
            characterController.height = defaultHeight;
            walkSpeed = 6f;
            runSpeed = 12f;
        }

        characterController.Move(moveDirection * Time.deltaTime);

        if (canMove)
        {
            rotationX += -Input.GetAxis("Mouse Y") * lookSpeed;
            rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);
            playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
            transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * lookSpeed, 0);
        }

        if (Input.GetButtonDown("Fire1"))
        {
            animator.SetTrigger("magicAttack");
        }

        bool isMoving = vertical != 0 || horizontal != 0;
        animator.SetBool("run", isRunning && isMoving);
        animator.SetBool("Slowrun", !isRunning && isMoving);
    }

    public void CastMagic()
    {
        Vector3 dir = playerCamera.transform.forward;
        GameObject proj = Instantiate(projectilePrefab, firePoint.position, Quaternion.LookRotation(dir));
        Rigidbody rb = proj.GetComponent<Rigidbody>();

        Projectile projectileScript = proj.GetComponent<Projectile>();
        if (projectileScript != null)
        {
            projectileScript.explosionEffectPrefab = explosionEffectPrefab;
        }

        if (rb != null)
            rb.AddForce(dir * shootForce, ForceMode.Impulse);

        Destroy(proj, projectileLifetime);
    }

    void OnGUI()
    {
        if (aimPoint != null)
        {
            float w = aimPoint.width * 0.1f;
            float h = aimPoint.height * 0.1f;
            float x = (Screen.width - w) / 2;
            float y = (Screen.height - h) / 2;
            GUI.DrawTexture(new Rect(x, y, w, h), aimPoint);
        }
    }
}
