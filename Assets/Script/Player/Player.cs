using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEditor.Timeline.TimelinePlaybackControls;

public class Player : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed;
    private Vector2 curMovementInput;
    public float jumptForce;
    public LayerMask groundLayerMask;

    private Rigidbody rigidbody;
    private Animator animation;
    private int isWalk = Animator.StringToHash("IsWalking");
    private int isJump = Animator.StringToHash("Jumping");
    Queue<Item> items = new Queue<Item>();

    private bool isJumpPower = false;
    public bool IsJumpPower { get { return isJumpPower; } set { isJumpPower = value; } }



    [Header("Look")]
    public Transform cameraContainer;
    public float minXLook;
    public float maxXLook;
    private float camCurXRot;
    public float lookSensitivity;
    private Vector2 mouseDelta;

    void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
        animation = GetComponentInChildren<Animator>();
    }


    private void FixedUpdate()
    {
        Move();
        if (isJumpPower)
        {
            float save = jumptForce;
            jumptForce += jumptForce;
            jumptForce = save;

            if (IsGrounded())
            {
                rigidbody.AddForce(Vector2.up * jumptForce, ForceMode.Impulse);
                animation.SetBool(isJump, true);
                animation.SetBool(isWalk, false);
                Invoke("JumpDelay", 1.1f);
                isJumpPower = false;
            }
        }

    }

    void Update()
    {
        CameraLook();
    }

    public void OnLookInput(InputAction.CallbackContext context)
    {
        mouseDelta = context.ReadValue<Vector2>();
    }

    public void OnMoveInput(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            curMovementInput = context.ReadValue<Vector2>();
            animation.SetBool(isWalk, true);
        }
        else if (context.phase == InputActionPhase.Canceled)
        {
            curMovementInput = Vector2.zero;
            animation.SetBool(isWalk, false);
        }
    }

    public void OnJumpInput(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started && IsGrounded())
        {
            rigidbody.AddForce(Vector2.up * jumptForce, ForceMode.Impulse);
            animation.SetBool(isJump, true);
            animation.SetBool(isWalk, false);
            Invoke("JumpDelay", 1.1f);
        }
    }


    private void Move()
    {
        Vector3 dir = transform.forward * curMovementInput.y + transform.right * curMovementInput.x;
        dir *= moveSpeed;
        dir.y = rigidbody.velocity.y;

        rigidbody.velocity = dir;

    }

    bool IsGrounded()
    {
        Ray rays = new Ray(transform.position + (transform.forward * 0.2f) + (transform.up * 0.01f), Vector3.down);
        if (Physics.Raycast(rays, 0.1f, groundLayerMask))
        {
            return true;
        }
        return false;
    }

    void JumpDelay()
    {
        //yield return new WaitForSeconds(0.5f);

        animation.SetBool(isJump, false);

    }

    void CameraLook()
    {
        camCurXRot += mouseDelta.y * lookSensitivity;
        camCurXRot = Mathf.Clamp(camCurXRot, minXLook, maxXLook);
        cameraContainer.localEulerAngles = new Vector3(-camCurXRot, 0, 0);

        transform.eulerAngles += new Vector3(0, mouseDelta.x * lookSensitivity, 0);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Item"))
        {
            items.Enqueue(other.gameObject.GetComponent<Item>());
            Destroy(other.gameObject);
        }

    }
    public void OnUseItem(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            Debug.Log(items.Count);
            if (items.Count > 0)
            {
                Item item = items.Dequeue();
                for (int i = 0; i < item.data.consumables.Length; ++i)
                {
                    switch (item.data.consumables[i].type)
                    {
                        case ConsumableType.SPEED:
                            {
                                StartCoroutine(BuffRest(moveSpeed, item.data.consumables[i].type));
                                moveSpeed = moveSpeed * item.data.consumables[i].value;
                                break;
                            }
                        case ConsumableType.HEALTH:
                            break;
                        case ConsumableType.STAMINA:
                            break;
                        case ConsumableType.ATTACK:
                            break;
                    }
                }
            }
        }
    }

    IEnumerator BuffRest(float down, ConsumableType Type)
    {
        yield return new WaitForSeconds(3f);

        switch (Type)
        {
            case ConsumableType.SPEED:
                {
                    moveSpeed = down;
                    break;
                }

            case ConsumableType.ATTACK:
                {
                    break;
                }
        }
    }
}
