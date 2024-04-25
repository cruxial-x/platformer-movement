using System.Collections;
using UnityEngine;

public class PlatformerMovement : MonoBehaviour
{
  [Header("Movement Properties")]
  public float speed = 10;
  public float jumpForce = 5;
  public float jumpBufferLength = 0.1f;
  public float coyoteTimeLength = 0.2f;

  [Header("Physics")]
  public Transform groundCheck;
  public float checkRadius;
  public LayerMask whatIsGround;

  private Rigidbody2D character;
  private float horizontalInput;
  private float jumpBufferCount;
  private float coyoteTime;
  [HideInInspector] public PlatformerState platformerState;

  void Awake()
  {
    character = GetComponent<Rigidbody2D>();
    platformerState = new PlatformerState();
  }

  void Update()
  {
    if (platformerState.dashing) return;

    HandleInput();
    CheckGroundStatus();
    ManageJumpBuffer();
  }

  void FixedUpdate()
  {
    if (!platformerState.dashing)
      Move();
  }

  private void HandleInput()
  {
    horizontalInput = Input.GetAxis("Horizontal");
    FlipCharacterBasedOnInput();
  }

  private void CheckGroundStatus()
  {
    platformerState.isGrounded = Physics2D.OverlapCircle(groundCheck.position, checkRadius, whatIsGround);
  }

  private void ManageJumpBuffer()
  {
    if (platformerState.isGrounded)
      coyoteTime = coyoteTimeLength;
    else
      coyoteTime -= Time.deltaTime;
    if (Input.GetButtonDown("Jump"))
      jumpBufferCount = jumpBufferLength;

    if (jumpBufferCount > 0 && coyoteTime > 0)
    {
      Jump();
      jumpBufferCount = 0;
      coyoteTime = 0;
    }
    else if (jumpBufferCount > 0)
      jumpBufferCount -= Time.deltaTime;
  }

  private void Move()
  {
    character.velocity = new Vector2(horizontalInput * speed, character.velocity.y);
  }

  private void Jump()
  {
    character.velocity = new Vector2(character.velocity.x, jumpForce);
  }

  private void FlipCharacterBasedOnInput()
  {
    if (horizontalInput > 0)
    {
      transform.localScale = new Vector2(1, transform.localScale.y);
      platformerState.isFacingRight = true;
    }
    else if (horizontalInput < 0)
    {
      transform.localScale = new Vector2(-1, transform.localScale.y);
      platformerState.isFacingRight = false;
    }
  }
}
