using UnityEngine;
using System.Collections.Generic;

public class CombatHandler : MonoBehaviour
{
  private PlatformerState platformerState;
  private InputHandler inputHandler;
  private bool attackButtonPressed;
  private Rigidbody2D character;
  private int attackEndCounter = 0;
  private float comboTimingWindow = 0.5f; // 0.5 seconds to perform the next hit
  private float lastAttackTime = 0; // Time when the last attack was registered
  public bool airSlam = true;

  void Start()
  {
    PlatformerMovement platformerMovement = GetComponent<PlatformerMovement>();
    platformerState = platformerMovement.PlatformerState;
    inputHandler = platformerMovement.InputHandler;
    character = GetComponent<Rigidbody2D>();
  }

  void Update()
  {
    if (platformerState.IsGroundAttacking)
    {
      character.velocity = new Vector2(0, character.velocity.y);
    }
    if (platformerState.dashing || platformerState.sliding || platformerState.weaponSheathed) return;

    attackButtonPressed = inputHandler.AttackButtonPressed;
    if (attackButtonPressed && platformerState.isAttacking && platformerState.attackCounter < 3)
    {
      // Check if the attack is within the allowed timing window
      if (Time.time - lastAttackTime <= comboTimingWindow)
      {
        // Increment the attack counter if the attack button is pressed during an attack
        platformerState.attackCounter++;
      }
    }
    else if (attackButtonPressed && !platformerState.isAttacking)
    {
      // Start a new attack if the attack button is pressed and no attack is in progress
      StartAttack();
    }
  }

  void StartAttack()
  {
    platformerState.isAttacking = true;
    platformerState.attackCounter = (platformerState.attackCounter % 3) + 1; // Cycle through 1, 2, 3
    attackEndCounter = platformerState.attackCounter;
    lastAttackTime = Time.time; // Reset the last attack time
  }

#pragma warning disable IDE0051
  void EndAttack() // Used as an animation event
  {
    Debug.Log(platformerState.attackCounter);
    if (platformerState.attackCounter >= attackEndCounter)
    {
      // Special case for the last attack in the air combo
      if (airSlam && inputHandler.AttackButtonHeld && platformerState.attackCounter < 3 && !platformerState.isGrounded)
      {
        platformerState.attackCounter = (platformerState.attackCounter % 3) + 1;
        return;
      }
      platformerState.attackCounter = 0;
      attackEndCounter = 0;
      platformerState.isAttacking = false;
    }
  }
#pragma warning restore IDE0051
}
