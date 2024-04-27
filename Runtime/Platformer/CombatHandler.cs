using UnityEngine;
using System.Collections.Generic;

public class CombatHandler : MonoBehaviour
{
  private PlatformerState platformerState;
  private InputHandler inputHandler;
  private bool attackButtonPressed;

  void Start()
  {
    PlatformerMovement platformerMovement = GetComponent<PlatformerMovement>();
    platformerState = platformerMovement.PlatformerState;
    inputHandler = platformerMovement.InputHandler;
  }

  void Update()
  {
    if (platformerState.dashing || platformerState.sliding || platformerState.weaponSheathed) return;
    attackButtonPressed = inputHandler.AttackButtonPressed;
    if (attackButtonPressed && platformerState.isAttacking && platformerState.attackCounter < 3)
    {
      // Increment the attack counter if the attack button is pressed during an attack
      platformerState.attackCounter++;
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
  }

#pragma warning disable IDE0051
  void EndAttack() // Used as an animation event
  {
    if (!attackButtonPressed)
    {
      platformerState.attackCounter = 0;
      platformerState.isAttacking = false;
    }
  }
#pragma warning restore IDE0051
}