using System.Collections;
using UnityEngine;

public class PlatformerMovement : MonoBehaviour
{
  [Header("Movement Properties")]
  public float speed = 10;
  public float jumpForce = 5;
  public int airJumps = 1;
  public float jumpBufferLength = 0.1f;
  public float coyoteTimeLength = 0.2f;

  [Header("Physics")]
  public Transform groundCheck;
  public float checkRadius;
  public LayerMask whatIsGround;
  public float friction = 0;

  private Rigidbody2D character;
  private float horizontalInput;
  private float jumpBufferCount;
  private float coyoteTime;
  [HideInInspector] public PlatformerState platformerState;
  void OnDrawGizmos()
  {
    Gizmos.color = Color.red;
    Gizmos.DrawWireSphere(groundCheck.position, checkRadius);
  }
  void Awake()
  {
    character = GetComponent<Rigidbody2D>();
    SetFriction(friction);
    platformerState = new PlatformerState
    {
      airJumps = airJumps
    };
  }

  void SetFriction(float rbFriction)
  {
    PhysicsMaterial2D physicsMaterial = new PhysicsMaterial2D
    {
      friction = rbFriction
    };
    character.sharedMaterial = physicsMaterial;
  }

  void Update()
  {
    if (platformerState.dashing) return;

    HandleInput();
    CheckGroundStatus();
    ManageJumpBuffer();
    ToggleWeapon(ref platformerState.weaponSheathed);

    // Check if the character has reached the peak of the jump
    if (platformerState.isJumping && character.velocity.y <= 0.01f)
      platformerState.isJumping = false;
  }

  void FixedUpdate()
  {
    if (!platformerState.dashing)
      Move();
  }
  void ToggleWeapon(ref bool sheathed)
  {
    if (Input.GetKeyDown(KeyCode.Q))
      sheathed = !sheathed;
  }

  private void HandleInput()
  {
    horizontalInput = Input.GetAxis("Horizontal");
    platformerState.isMoving = horizontalInput != 0;
    FlipCharacterBasedOnInput();
  }

  private void CheckGroundStatus()
  {
    bool wasGrounded = platformerState.isGrounded;
    platformerState.isGrounded = Physics2D.OverlapCircle(groundCheck.position, checkRadius, whatIsGround);

    // If the player was not grounded but is now, they have landed
    if (!wasGrounded && platformerState.isGrounded)
    {
      platformerState.isJumping = false;
      platformerState.airJumps = airJumps; // Reset airJumps when the player lands
    }
  }

  private void ManageJumpBuffer()
  {
    if (platformerState.isGrounded)
      coyoteTime = coyoteTimeLength;
    else
      coyoteTime -= Time.deltaTime;
    if (Input.GetButtonDown("Jump"))
      jumpBufferCount = jumpBufferLength;


    if ((jumpBufferCount > 0 && coyoteTime > 0) || (Input.GetButtonDown("Jump") && platformerState.airJumps > 0))
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
    // Only allow jumping if the player is grounded or has air jumps left
    if (platformerState.isGrounded || platformerState.airJumps > 0)
    {
      character.velocity = new Vector2(character.velocity.x, jumpForce);
      platformerState.isJumping = true;

      // Decrement airJumps if the player is not grounded
      if (!platformerState.isGrounded)
        platformerState.airJumps--;
    }
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
