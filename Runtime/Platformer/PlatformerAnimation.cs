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
  public string slidingParam = "Sliding";
  public string groundAttackingParam = "GroundAttacking";
  public string airAttackingParam = "AirAttacking";
  private int initialAirJumps;

  public void Start()
  {
    if (animator == null)
      animator = GetComponent<Animator>();
    platformerState = GetComponent<PlatformerMovement>().PlatformerState;
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
      animator.SetBool(slidingParam, platformerState.sliding);
      animator.SetBool(groundAttackingParam, platformerState.GroundAttacking);
      animator.SetBool(airAttackingParam, platformerState.AirAttacking);
    }
  }
}