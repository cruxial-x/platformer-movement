using System.Collections;
using UnityEditor.EditorTools;
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
  private float jumpBufferCount;
  private float coyoteTime;
  [HideInInspector] public PlatformerState platformerState;
  [Tooltip("Check if using ApplyJumpForce method in an animation event")]
  public bool useJumpAnimationEvent = false;
  [HideInInspector] public bool jumpAnimationFinished = true;
  public BoxCollider2D normalCollider;
  public BoxCollider2D slideCollider;
  private InputHandler inputHandler;
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
    inputHandler = new InputHandler();
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
    if (platformerState.dashing || platformerState.sliding) return;

    inputHandler.HandleInput();
    FlipCharacterBasedOnInput();
    CheckGroundStatus();
    ManageJumpBuffer();
    ToggleWeapon(ref platformerState.weaponSheathed);
    StartSlide();
    normalCollider.enabled = !platformerState.sliding;
    slideCollider.enabled = !normalCollider.enabled;
    // Check if the character has reached the peak of the jump
    if (platformerState.isJumping && character.velocity.y <= 0.01f && jumpAnimationFinished)
    {
      platformerState.isJumping = false;
      platformerState.wallKicking = false;
    }
  }

  void FixedUpdate()
  {
    Slide();
    if (platformerState.dashing || platformerState.sliding) return;
    Move();
    FastFall();
  }
  void ToggleWeapon(ref bool sheathed)
  {
    if (Input.GetKeyDown(KeyCode.Q))
      sheathed = !sheathed;
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
    if (platformerState.IsClimbing) return;
    if (platformerState.isGrounded)
      coyoteTime = coyoteTimeLength;
    else
      coyoteTime -= Time.deltaTime;
    if (inputHandler.JumpButtonDown)
      jumpBufferCount = jumpBufferLength;


    if ((jumpBufferCount > 0 && coyoteTime > 0) || (inputHandler.JumpButtonDown && platformerState.airJumps > 0))
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
    character.velocity = new Vector2(inputHandler.HorizontalInput * speed, character.velocity.y);
    platformerState.isMoving = inputHandler.HorizontalInput != 0;
  }

  private void Jump()
  {
    if (platformerState.wallKicking) return;
    // Only allow jumping if the player is grounded or has air jumps left
    if (platformerState.isGrounded || platformerState.airJumps > 0)
    {

      platformerState.isJumping = true;
      jumpAnimationFinished = false;

      // If not using animation event, apply jump force immediately
      if (!useJumpAnimationEvent)
      {
        ApplyJumpForce();
      }

      // Decrement airJumps if the player is not grounded
      if (!platformerState.isGrounded)
        platformerState.airJumps--;
    }
  }
  private void StartSlide()
  {
    if (platformerState.isGrounded && Input.GetKeyDown(KeyCode.LeftShift) && !platformerState.sliding)
    {
      platformerState.sliding = true;
    }
  }
  private void Slide()
  {
    if (platformerState.sliding)
    {
      character.velocity = new Vector2(transform.localScale.x * speed, character.velocity.y);
    }
  }
#pragma warning disable IDE0051
  private void ApplyJumpForce() // Fires at end of jumpsquat in jump animation
  {
    if (jumpAnimationFinished) return;
    // Check if the jump button is still being held down
    if (inputHandler.JumpButtonHeld)
    {
      // If it is, apply the full jump force
      character.velocity = new Vector2(character.velocity.x, jumpForce);
    }
    else
    {
      // If it isn't, apply half the jump force
      character.velocity = new Vector2(character.velocity.x, jumpForce / 2);
    }
    jumpAnimationFinished = true;
  }
  private void EndSlide() // Fires at end of slide animation
  {
    platformerState.sliding = false;
  }
#pragma warning restore IDE0051


  private void FastFall()
  {
    if (inputHandler.VerticalInput < 0 && platformerState.IsFalling)
    {
      character.velocity = new Vector2(character.velocity.x, -jumpForce * 2);
    }
  }

  private void FlipCharacterBasedOnInput()
  {
    if (inputHandler.HorizontalInput > 0)
    {
      transform.localScale = new Vector2(1, transform.localScale.y);
      platformerState.isFacingRight = true;
    }
    else if (inputHandler.HorizontalInput < 0)
    {
      transform.localScale = new Vector2(-1, transform.localScale.y);
      platformerState.isFacingRight = false;
    }
  }
}
