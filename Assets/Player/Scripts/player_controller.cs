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

    private Vector2 moveInput;
    private PlayerInput playerInput;
    private bool isRunButtonPressed = false;

    void Awake()
    {
        playerInput = GetComponent<PlayerInput>();

        // ✅ Add event listeners for the UI button
        if (runButton != null)
        {
            runButton.onClick.AddListener(() => StartRunning());
        }
    }

    void Update()
    {
        // Read input from joystick or mobile UI
        moveInput = playerInput.actions["Move"].ReadValue<Vector2>();

        // ✅ Only override moveInput if a keyboard key is pressed
        if (Keyboard.current != null)
        {
            if (Keyboard.current.wKey.isPressed) moveInput.y = 1; // Move forward
            if (Keyboard.current.sKey.isPressed) moveInput.y = -1; // Move backward
            if (Keyboard.current.aKey.isPressed) moveInput.x = -1; // Turn left
            if (Keyboard.current.dKey.isPressed) moveInput.x = 1; // Turn right
        }

        float horizontalMove = moveInput.x * turnSpeed * Time.deltaTime;
        float verticalMove = moveInput.y * moveSpeed * Time.deltaTime;

        // ✅ Running activates when holding "R" or the UI button is pressed
        isRunning = (Keyboard.current != null && Keyboard.current.rKey.isPressed) || isRunButtonPressed;

        if (isRunning)
        {
            verticalMove = moveInput.y * runSpeed * Time.deltaTime;
        }

        // Handle movement
        if (moveInput.magnitude > 0.1f)
        {
            isWalking = true;
            transform.Rotate(0, horizontalMove, 0);
            transform.Translate(Vector3.forward * verticalMove);

            // ✅ Play correct animation
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

        // ✅ Press 'H' to wave first, then talk
        if (Keyboard.current != null && Keyboard.current.hKey.wasPressedThisFrame)
        {
            isWalking = false;
            isRunning = false;
            StartCoroutine(WaveThenTalk());
        }
    }

    IEnumerator WaveThenTalk()
    {
        player.GetComponent<Animation>().Play("Waving");

        // Wait for the Waving animation to finish
        yield return new WaitForSeconds(player.GetComponent<Animation>()["Waving"].length);

        // Start Talking animation
        player.GetComponent<Animation>().Play("Talking");
    }

    // ✅ UI Button functions
    public void StartRunning()
    {
        isRunButtonPressed = true;
        Invoke("StopRunning", 0.5f); // Simulate button hold
    }

    public void StopRunning()
    {
        isRunButtonPressed = false;
    }
}
