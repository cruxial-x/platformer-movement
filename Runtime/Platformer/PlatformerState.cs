public class PlatformerState
{
  public bool isGrounded = false;
  public bool isMoving = false;
  public bool dashing = false;
  public bool sliding = false;
  public bool isFacingRight = true;
  public int airJumps = 1;
  public bool isJumping = false;
  public bool wallClimbing = false;
  public bool wallSliding = false;
  public bool weaponSheathed = true;
  public bool wallKicking = false;
  public bool isAttacking = false;
  public int attackCounter = 0;
  public bool IsFalling
  {
    get
    {
      return !isGrounded && !isJumping && !wallSliding && !wallClimbing;
    }
  }
  public bool IsClimbing
  {
    get
    {
      return wallClimbing || wallSliding;
    }
  }
  public bool GroundAttacking
  {
    get
    {
      return isAttacking && isGrounded;
    }
  }
  public bool AirAttacking
  {
    get
    {
      return isAttacking && !isGrounded && !wallClimbing;
    }
  }
}