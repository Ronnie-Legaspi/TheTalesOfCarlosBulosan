using System.Collections;
using UnityEngine;

public class CamLookAt : MonoBehaviour
{
    public Transform player, cameraTrans;
    public float forwardOffset = 2f; // How much the camera moves forward when stopping
    public float smoothSpeed = 3f; // How smoothly the camera transitions

    private Vector3 originalPosition;
    private bool isMoving = true; // Tracks if the player is moving

    void Start()
    {
        originalPosition = cameraTrans.position; // Store initial camera position
    }

    void Update()
    {
        cameraTrans.LookAt(player); // Always look at the player

        // Check if player is moving (assumes player has Rigidbody)
        if (player.TryGetComponent<Rigidbody>(out Rigidbody rb))
        {
            if (rb.linearVelocity.magnitude > 0.1f) // Player is moving
            {
                if (!isMoving) // Transition back to normal
                {
                    isMoving = true;
                    StartCoroutine(MoveCamera(originalPosition));
                }
            }
            else // Player stopped
            {
                if (isMoving)
                {
                    isMoving = false;
                    Vector3 forwardPosition = originalPosition + player.forward * forwardOffset;
                    StartCoroutine(MoveCamera(forwardPosition));
                }
            }
        }
    }

    IEnumerator MoveCamera(Vector3 targetPosition)
    {
        float elapsedTime = 0f;
        Vector3 startPosition = cameraTrans.position;

        while (elapsedTime < 1f)
        {
            cameraTrans.position = Vector3.Lerp(startPosition, targetPosition, elapsedTime * smoothSpeed);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        cameraTrans.position = targetPosition;
    }
}
