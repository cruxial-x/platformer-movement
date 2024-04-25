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

  public void Start()
  {
    if (animator == null)
      animator = GetComponent<Animator>();
    platformerState = GetComponent<PlatformerMovement>().platformerState;
  }

  public void Update()
  {
    if (animator != null)
    {
      bool isJumping = platformerState.isJumping;
      animator.SetBool(jumpingParam, isJumping);
      animator.SetBool(airJumpParam, isJumping && platformerState.airJumps < 1);
      animator.SetBool(fallingParam, !platformerState.isGrounded && !isJumping);
      animator.SetBool(runningParam, platformerState.isMoving);
      animator.SetBool(sheathedParam, platformerState.weaponSheathed);
    }
  }
}