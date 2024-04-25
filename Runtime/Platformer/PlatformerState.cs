public class PlatformerState
{
  public bool isGrounded = false;
  public bool isMoving = false;
  public bool dashing = false;
  public bool isFacingRight = true;
  public int airJumps = 1;
  public bool isJumping = false;
  public bool wallClimbing = false;
  public bool wallSliding = false;
  public bool weaponSheathed = true;
  public bool IsFalling
  {
    get
    {
      return !isGrounded && !isJumping;
    }
  }
}