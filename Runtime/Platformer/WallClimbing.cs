using UnityEngine;

public class WallClimbing : MonoBehaviour
{
  public LayerMask whatIsWall;
  public Transform wallCheck;
  public float wallCheckDistance = 0.1f;
  private bool isTouchingWall;
  private float climbingSpeed = 5;
  private Rigidbody2D character;
  private PlatformerState platformerState;
  private PlatformerMovement platformerMovement;
  private int initialAirJumps;
  private InputHandler inputHandler;

  // Initialization
  void Start()
  {
    character = GetComponent<Rigidbody2D>();
    platformerMovement = GetComponent<PlatformerMovement>();
    platformerState = platformerMovement.PlatformerState;
    initialAirJumps = platformerState.airJumps;
    inputHandler = new InputHandler();
  }

  // Debugging
  void OnDrawGizmos()
  {
    Gizmos.DrawWireSphere(wallCheck.position, wallCheckDistance);
  }

  // Update is called once per frame
  void Update()
  {
    inputHandler.HandleInput();
    HandleWallInteraction();
  }

  // Fixed update for physics related calculations
  void FixedUpdate()
  {
    if (platformerState.dashing || platformerState.sliding) return;
    ManageWalls();
  }

  // Handle interaction with walls
  private void HandleWallInteraction()
  {
    isTouchingWall = Physics2D.Raycast(wallCheck.position, Vector2.right * transform.localScale.x, wallCheckDistance, whatIsWall);

    if (platformerState.dashing || platformerState.sliding) return;

    if (!isTouchingWall || platformerState.isGrounded)
    {
      platformerState.wallClimbing = false;
      platformerState.wallSliding = false;
    }
    else if (platformerState.IsClimbing && Input.GetButtonDown("Jump"))
    {
      JumpOffWall();
    }
  }

  // Jump off the wall
  private void JumpOffWall()
  {
    platformerState.wallClimbing = false;
    platformerState.wallSliding = false;
    platformerState.isJumping = true;
    platformerState.wallKicking = true;
    character.velocity = new Vector2(-transform.localScale.x * platformerMovement.speed, platformerMovement.jumpForce);
  }

  // Manage wall climbing and sliding
  private void ManageWalls()
  {
    if (platformerState.wallKicking) return;

    if (isTouchingWall && !platformerState.isGrounded)
    {
      Debug.Log("Air Jumps: " + platformerState.airJumps + " Initial Air Jumps: " + initialAirJumps);
      platformerState.airJumps = initialAirJumps;
      if (inputHandler.VerticalInput > 0)
      {
        ClimbWall();
      }
      else
      {
        SlideDownWall();
      }
    }
  }

  // Climb the wall
  private void ClimbWall()
  {
    platformerState.wallClimbing = true;
    platformerState.wallSliding = false;
    platformerState.isJumping = false;
    character.velocity = new Vector2(0, climbingSpeed);
  }

  // Slide down the wall
  private void SlideDownWall()
  {
    platformerState.wallClimbing = false;
    platformerState.wallSliding = true;
    platformerState.isJumping = false;
    character.velocity = new Vector2(0, -climbingSpeed / 2);
  }
}
