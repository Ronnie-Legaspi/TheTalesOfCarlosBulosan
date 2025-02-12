using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f; // Walking speed
    public float runSpeed = 10f; // Running speed
    public float turnSpeed = 150f;
    public bool isWalking;
    public bool isRunning;
    public GameObject player; // Assign your character GameObject in Inspector
    public Button runButton; // Assign this in the Inspector

    public LayerMask groundLayer; // Select 'Ground' layer in Inspector
    public float raycastHeightOffset = 1.5f; // Adjust based on your player model
    public float groundFollowSpeed = 10f; // How fast player follows ground

    private Vector2 moveInput;
    private PlayerInput playerInput;
    private bool isRunButtonPressed = false;
    private Rigidbody rb;

    void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        rb = GetComponent<Rigidbody>();

        if (runButton != null)
        {
            runButton.onClick.AddListener(() => StartRunning());
        }
    }

    void Update()
    {
        // Read joystick or mobile UI input
        moveInput = playerInput.actions["Move"].ReadValue<Vector2>();

        // ✅ Override with keyboard input if available
        if (Keyboard.current != null)
        {
            moveInput.y = Keyboard.current.wKey.isPressed ? 1 : Keyboard.current.sKey.isPressed ? -1 : 0;
            moveInput.x = Keyboard.current.aKey.isPressed ? -1 : Keyboard.current.dKey.isPressed ? 1 : 0;
        }

        float horizontalMove = moveInput.x * turnSpeed * Time.deltaTime;
        float verticalMove = moveInput.y * moveSpeed * Time.deltaTime;

        isRunning = (Keyboard.current != null && Keyboard.current.rKey.isPressed) || isRunButtonPressed;

        if (isRunning)
        {
            verticalMove = moveInput.y * runSpeed * Time.deltaTime;
        }

        if (moveInput.magnitude > 0.1f)
        {
            isWalking = true;
            transform.Rotate(0, horizontalMove, 0);
            transform.Translate(Vector3.forward * verticalMove);

            if (isRunning)
            {
                if (!player.GetComponent<Animation>().IsPlaying("Running"))
                {
                    player.GetComponent<Animation>().CrossFade("Running");
                }
            }
            else
            {
                if (!player.GetComponent<Animation>().IsPlaying("Walking"))
                {
                    player.GetComponent<Animation>().CrossFade("Walking");
                }
            }
        }
        else
        {
            isWalking = false;
            isRunning = false;

            if (!player.GetComponent<Animation>().IsPlaying("Waving") &&
                !player.GetComponent<Animation>().IsPlaying("Talking"))
            {
                player.GetComponent<Animation>().Play("Idle");
            }
        }

        if (Keyboard.current != null && Keyboard.current.hKey.wasPressedThisFrame)
        {
            isWalking = false;
            isRunning = false;
            StartCoroutine(WaveThenTalk());
        }
    }

    void FixedUpdate()
    {
        AdjustToGround();
    }

    void AdjustToGround()
    {
        RaycastHit hit;

        Vector3 rayOrigin = transform.position + Vector3.up * raycastHeightOffset;

        if (Physics.Raycast(rayOrigin, Vector3.down, out hit, raycastHeightOffset * 2f, groundLayer))
        {
            Vector3 newPosition = transform.position;
            newPosition.y = Mathf.Lerp(transform.position.y, hit.point.y, Time.deltaTime * groundFollowSpeed);
            transform.position = newPosition;
        }
    }

    IEnumerator WaveThenTalk()
    {
        player.GetComponent<Animation>().Play("Waving");
        yield return new WaitForSeconds(player.GetComponent<Animation>()["Waving"].length);
        player.GetComponent<Animation>().Play("Talking");
    }

    public void StartRunning()
    {
        isRunButtonPressed = true;
        Invoke("StopRunning", 0.5f);
    }

    public void StopRunning()
    {
        isRunButtonPressed = false;
    }
}
