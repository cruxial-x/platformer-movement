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
  private float horizontalInput;
  private float verticalInput;
  private float jumpBufferCount;
  private float coyoteTime;
  [HideInInspector] public PlatformerState platformerState;
  [Tooltip("Check if using ApplyJumpForce method in an animation event")]
  public bool useJumpAnimationEvent = false;
  private bool jumpAnimationFinished = true;
  public LayerMask whatIsWall;
  public Transform wallCheck;
  public float wallCheckDistance = 0.1f;
  private bool isTouchingWall;
  private float climbingSpeed = 5;
  private bool ignoreInput = false;

  private IEnumerator IgnoreInputForSeconds(float seconds)
  {
    ignoreInput = true;
    yield return new WaitForSeconds(seconds);
    ignoreInput = false;
  }
  void OnDrawGizmos()
  {
    Gizmos.color = Color.red;
    Gizmos.DrawWireSphere(groundCheck.position, checkRadius);
    Gizmos.color = Color.blue;
    Gizmos.DrawWireSphere(wallCheck.position, wallCheckDistance);
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
    if (platformerState.isJumping && character.velocity.y <= 0.01f && jumpAnimationFinished)
    {
      platformerState.isJumping = false;
      platformerState.wallKicking = false;
    }

    if (platformerState.isGrounded)
    {
      platformerState.wallClimbing = false;
      platformerState.wallSliding = false;
    }
    if (!isTouchingWall)
    {
      platformerState.wallClimbing = false;
      platformerState.wallSliding = false;
    }
  }
  void ManageWalls()
  {
    if (platformerState.wallKicking)
    {
      return;
    }
    isTouchingWall = Physics2D.Raycast(wallCheck.position, Vector2.right * transform.localScale.x, wallCheckDistance, whatIsWall);
    if (isTouchingWall && !platformerState.isGrounded)
    {
      if (verticalInput > 0)
      {
        platformerState.wallClimbing = true;
        platformerState.wallSliding = false;
        platformerState.isJumping = false;
        character.velocity = new Vector2(0, climbingSpeed);
      }
      else
      {
        platformerState.wallClimbing = false;
        platformerState.wallSliding = true;
        platformerState.isJumping = false;
        character.velocity = new Vector2(0, -climbingSpeed / 2);
      }
    }
  }

  void FixedUpdate()
  {
    if (!platformerState.dashing)
      Move();
    ManageWalls();
    FastFall();
  }
  void ToggleWeapon(ref bool sheathed)
  {
    if (Input.GetKeyDown(KeyCode.Q))
      sheathed = !sheathed;
  }

  private void HandleInput()
  {
    horizontalInput = Input.GetAxis("Horizontal");
    verticalInput = Input.GetAxis("Vertical");
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
    if (platformerState.IsClimbing)
    {
      if (Input.GetButtonDown("Jump"))
      {
        platformerState.wallClimbing = false;
        platformerState.wallSliding = false;
        platformerState.isJumping = true;
        platformerState.wallKicking = true;
        StartCoroutine(IgnoreInputForSeconds(0.1f));
        Debug.Log("Transform.localScale.x " + transform.localScale.x);
        character.velocity = new Vector2(-transform.localScale.x * speed, jumpForce);
      }
      return;
    }
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
    if (ignoreInput) return;
    character.velocity = new Vector2(horizontalInput * speed, character.velocity.y);
    platformerState.isMoving = horizontalInput != 0;
  }

  private void Jump()
  {
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

#pragma warning disable IDE0051 
  private void ApplyJumpForce() // Fires at end of jumpsquat in jump animation
  {
    if (jumpAnimationFinished) return;
    // Check if the jump button is still being held down
    if (Input.GetButton("Jump"))
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
#pragma warning restore IDE0051

  private void FastFall()
  {
    if (verticalInput < 0 && platformerState.IsFalling)
    {
      character.velocity = new Vector2(character.velocity.x, -jumpForce * 2);
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
