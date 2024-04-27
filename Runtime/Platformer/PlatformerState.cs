public class PlatformerState
{
  public InputHandler InputHandler { get; set; }
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
}