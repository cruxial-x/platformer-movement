using System.Collections;
using UnityEngine;

public class PlatformerDash : MonoBehaviour
{
  public float dashSpeed = 30;
  public float dashTime = 0.5f;
  public float dashCooldown = 0.5f;
  private bool canDash = true;
  private bool dashed = false;
  private Rigidbody2D character;
  private float originalGravity;
  private PlatformerState platformerState;

  void Start()
  {
    character = GetComponent<Rigidbody2D>();
    originalGravity = character.gravityScale;
    platformerState = GetComponent<PlatformerMovement>().platformerState;
  }

  void Update()
  {
    TryStartDash();
  }

  private void TryStartDash()
  {
    if (canDash && Input.GetKeyDown(KeyCode.LeftShift) && !dashed)
    {
      StartCoroutine(Dash());
      dashed = true;
    }

    if (dashed && platformerState.isGrounded)
      dashed = false;
  }

  IEnumerator Dash()
  {
    canDash = false;
    platformerState.dashing = true;
    character.gravityScale = 0;
    character.velocity = new Vector2(transform.localScale.x * dashSpeed, 0);
    yield return new WaitForSeconds(dashTime);
    character.gravityScale = originalGravity;
    platformerState.dashing = false;
    yield return new WaitForSeconds(dashCooldown);
    canDash = true;
  }
}