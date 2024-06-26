using UnityEngine;
internal class Dev
{
  public static bool loggingEnabled = false;
  // Only log messages in debug builds
  public static void Log<T>(T message)
  {
    if (Debug.isDebugBuild && loggingEnabled)
    {
      Debug.Log(message.ToString());
    }
  }
}

public class PlatformerMovement : MonoBehaviour
{
  public PlatformerState PlatformerState { get; private set; }
  public InputHandler InputHandler { get; private set; }
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
  [Tooltip("Check if using ApplyJumpForce method in an animation event")]
  public bool useJumpAnimationEvent = false;
  [HideInInspector] public bool jumpAnimationFinished = true;
  public BoxCollider2D normalCollider;
  public BoxCollider2D slideCollider;
  public float detachBuffer = 0.5f;
  private float detachTimer;
  private bool isDetaching;

  void OnDrawGizmos()
  {
    Gizmos.color = Color.red;
    Gizmos.DrawWireSphere(groundCheck.position, checkRadius);
  }
  void Awake()
  {
    character = GetComponent<Rigidbody2D>();
    SetFriction(friction);
    PlatformerState = new PlatformerState(airJumps);
    InputHandler = new InputHandler();
  }

  void SetFriction(float rbFriction)
  {
    PhysicsMaterial2D physicsMaterial = new()
    {
      friction = rbFriction
    };
    character.sharedMaterial = physicsMaterial;
  }

  void Update()
  {
    if (PlatformerState.dashing || PlatformerState.sliding) return;

    InputHandler.HandleInput();
    FlipCharacterBasedOnInput();
    CheckGroundStatus();
    ManageJumpBuffer();
    ToggleWeapon(ref PlatformerState.weaponSheathed);
    StartSlide();
    normalCollider.enabled = !PlatformerState.sliding;
    slideCollider.enabled = !normalCollider.enabled;
    // Check if the character has reached the peak of the jump
    if (PlatformerState.isJumping && character.velocity.y <= 0.01f && jumpAnimationFinished)
    {
      PlatformerState.isJumping = false;
      PlatformerState.wallKicking = false;
    }
  }

  void FixedUpdate()
  {
    Slide();
    if (PlatformerState.dashing || PlatformerState.sliding) return;
    Move();
    FastFall();
  }
  void ToggleWeapon(ref bool sheathed)
  {
    if (PlatformerState.isAttacking) return;
    if (Input.GetKeyDown(KeyCode.Q))
      sheathed = !sheathed;
  }

  private void CheckGroundStatus()
  {
    bool wasGrounded = PlatformerState.isGrounded;
    PlatformerState.isGrounded = Physics2D.OverlapCircle(groundCheck.position, checkRadius, whatIsGround);

    // If the player was not grounded but is now, they have landed
    if (!wasGrounded && PlatformerState.isGrounded)
    {
      PlatformerState.isJumping = false;
      PlatformerState.wallKicking = false;
      PlatformerState.isAttacking = false;
      PlatformerState.attackCounter = 0;
      PlatformerState.airJumps = airJumps; // Reset airJumps when the player lands
    }
  }

  private void ManageJumpBuffer()
  {
    if (PlatformerState.IsClimbing) return;
    if (PlatformerState.isGrounded)
      coyoteTime = coyoteTimeLength;
    else
      coyoteTime -= Time.deltaTime;
    if (InputHandler.JumpButtonDown)
      jumpBufferCount = jumpBufferLength;


    if ((jumpBufferCount > 0 && coyoteTime > 0) || (InputHandler.JumpButtonDown && PlatformerState.airJumps > 0))
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
    if (PlatformerState.IsGroundAttacking || PlatformerState.IsClimbing) return;
    character.velocity = new Vector2(InputHandler.HorizontalInput * speed, character.velocity.y);
    PlatformerState.isMoving = InputHandler.HorizontalInput != 0;
  }

  private void Jump()
  {
    if (PlatformerState.wallKicking) return;
    // Only allow jumping if the player is grounded or has air jumps left
    if (PlatformerState.isGrounded || PlatformerState.airJumps > 0)
    {

      PlatformerState.isJumping = true;
      jumpAnimationFinished = false;

      // If not using animation event, apply jump force immediately
      if (!useJumpAnimationEvent)
      {
        ApplyJumpForce();
      }

      // Decrement airJumps if the player is not grounded
      if (!PlatformerState.isGrounded)
        PlatformerState.airJumps--;
    }
  }
  private void StartSlide()
  {
    if (PlatformerState.isGrounded && Input.GetKeyDown(KeyCode.LeftShift) && !PlatformerState.sliding && jumpAnimationFinished)
    {
      PlatformerState.sliding = true;
    }
  }
  private void Slide()
  {
    if (PlatformerState.sliding)
    {
      character.velocity = new Vector2(transform.localScale.x * speed, character.velocity.y);
    }
  }
#pragma warning disable IDE0051
  private void ApplyJumpForce() // Fires at end of jumpsquat in jump animation
  {
    if (jumpAnimationFinished) return;
    // Check if the jump button is still being held down
    if (InputHandler.JumpButtonHeld)
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
    PlatformerState.sliding = false;
  }
#pragma warning restore IDE0051


  private void FastFall()
  {
    if (InputHandler.VerticalInput < 0 && PlatformerState.IsFalling)
    {
      character.velocity = new Vector2(character.velocity.x, -jumpForce * 2);
    }
  }
  private void FlipCharacterBasedOnInput()
  {
    if (PlatformerState.isAttacking || (!PlatformerState.isGrounded && !PlatformerState.ShouldAirJump && !PlatformerState.wallSliding)) return;

    float horizontalInput = InputHandler.HorizontalInput;

    // If the player starts to move in the opposite direction while wall sliding, start the detach timer
    bool isFacingRight = PlatformerState.isFacingRight;
    bool isMovingRight = InputHandler.HorizontalInput > 0;
    bool isFacingLeft = !isFacingRight;
    bool isMovingLeft = InputHandler.HorizontalInput < 0;
    if (PlatformerState.wallSliding && ((isFacingLeft && isMovingRight) || (isFacingRight && isMovingLeft)))
    {
      if (!isDetaching)
      {
        isDetaching = true;
        detachTimer = Time.time;
      }
      else if (Time.time - detachTimer > detachBuffer)
      {
        isDetaching = false;
        FlipCharacter(horizontalInput);
      }
    }
    else
    {
      isDetaching = false;
      FlipCharacter(horizontalInput);
    }
  }
  private void FlipCharacter(float horizontalInput)
  {
    if (horizontalInput > 0)
    {
      transform.localScale = new Vector2(1, transform.localScale.y);
      PlatformerState.isFacingRight = true;
    }
    else if (horizontalInput < 0)
    {
      transform.localScale = new Vector2(-1, transform.localScale.y);
      PlatformerState.isFacingRight = false;
    }
  }
}
