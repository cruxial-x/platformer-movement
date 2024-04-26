using UnityEngine;

public class PlatformerAnimation : MonoBehaviour
{
  public Animator animator;
  private PlatformerState platformerState;

  [Header("Animation Boolean Parameters")]
  public string jumpingParam = "Jumping";
  public string airJumpParam = "AirJump";
  public string fallingParam = "Falling";
  public string runningParam = "Running";
  public string sheathedParam = "Sheathed";
  public string wallClimbingParam = "WallClimbing";
  public string wallSlidingParam = "WallSliding";
  private int initialAirJumps;

  public void Start()
  {
    if (animator == null)
      animator = GetComponent<Animator>();
    platformerState = GetComponent<PlatformerMovement>().platformerState;
    initialAirJumps = platformerState.airJumps;
  }

  public void Update()
  {
    if (animator != null)
    {
      bool isJumping = platformerState.isJumping;
      animator.SetBool(jumpingParam, isJumping);
      animator.SetBool(airJumpParam, (
        isJumping && platformerState.airJumps < initialAirJumps)
        || platformerState.wallKicking // Wallkick uses air jump animation
        );
      animator.SetBool(fallingParam, platformerState.IsFalling);
      animator.SetBool(runningParam, platformerState.isMoving && platformerState.isGrounded);
      animator.SetBool(sheathedParam, platformerState.weaponSheathed);
      animator.SetBool(wallClimbingParam, platformerState.wallClimbing);
      animator.SetBool(wallSlidingParam, platformerState.wallSliding);
    }
  }
}