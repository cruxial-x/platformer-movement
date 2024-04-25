using System.Collections;
using UnityEngine;

public class PlatformerMovement : MonoBehaviour
{
  [Header("Movement Properties")]
  public float speed = 10;
  public float jumpForce = 5;
  public float jumpBufferLength = 0.1f;
  public float coyoteTimeLength = 0.2f;
  public float friction = 0;

  [Header("Physics")]
  public Transform groundCheck;
  public float checkRadius;
  public LayerMask whatIsGround;

  private Rigidbody2D character;
  private float horizontalInput;
  private float jumpBufferCount;
  private float coyoteTime;
  [HideInInspector] public PlatformerState platformerState;
  public Animator animator;
  public string horizontalAnimatorBool = "Running";

  void OnDrawGizmos()
  {
    Gizmos.color = Color.red;
    Gizmos.DrawWireSphere(groundCheck.position, checkRadius);
  }
  void Awake()
  {
    character = GetComponent<Rigidbody2D>();
    SetFriction(friction);
    platformerState = new PlatformerState();
    if (animator == null)
      animator = GetComponent<Animator>();
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
  }

  void FixedUpdate()
  {
    if (!platformerState.dashing)
      Move();
  }
  void ToggleWeapon(ref bool sheathed)
  {
    if (Input.GetKeyDown(KeyCode.Q))
      sheathed = !sheathed;
    if (animator != null)
      animator.SetBool("Sheathed", sheathed);
  }

  private void HandleInput()
  {
    horizontalInput = Input.GetAxis("Horizontal");
    FlipCharacterBasedOnInput();
    if (animator != null)
      animator.SetBool(horizontalAnimatorBool, horizontalInput != 0);
  }

  private void CheckGroundStatus()
  {
    platformerState.isGrounded = Physics2D.OverlapCircle(groundCheck.position, checkRadius, whatIsGround);
  }

  private void ManageJumpBuffer()
  {
    if (platformerState.isGrounded)
      coyoteTime = coyoteTimeLength;
    else
      coyoteTime -= Time.deltaTime;
    if (Input.GetButtonDown("Jump"))
      jumpBufferCount = jumpBufferLength;

    if (jumpBufferCount > 0 && coyoteTime > 0)
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
    character.velocity = new Vector2(horizontalInput * speed, character.velocity.y);
  }

  private void Jump()
  {
    character.velocity = new Vector2(character.velocity.x, jumpForce);
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
