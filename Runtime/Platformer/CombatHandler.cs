using UnityEngine;
using System.Collections.Generic;

public class CombatHandler : MonoBehaviour
{
  private PlatformerState platformerState;
  private InputHandler inputHandler;
  private int attackCounter = 0;

  void Start()
  {
    PlatformerMovement platformerMovement = GetComponent<PlatformerMovement>();
    platformerState = platformerMovement.PlatformerState;
    inputHandler = platformerMovement.InputHandler;
  }

  void Update()
  {
    if (inputHandler.AttackButtonPressed && platformerState.isAttacking && attackCounter < 3)
    {
      // Increment the attack counter if the attack button is pressed during an attack
      attackCounter++;
    }
    else if (inputHandler.AttackButtonPressed && !platformerState.isAttacking)
    {
      // Start a new attack if the attack button is pressed and no attack is in progress
      StartAttack();
    }
  }

  void StartAttack()
  {
    platformerState.isAttacking = true;
    attackCounter = (attackCounter % 3) + 1; // Cycle through 1, 2, 3
    // Pass attackCounter to the animator to select the correct attack animation
    GetComponent<Animator>().SetInteger("AttackState", attackCounter);
  }

#pragma warning disable IDE0051
  void EndAttack() // Used as an animation event
  {
    platformerState.isAttacking = false;
    // If the attack button is not being pressed, reset the attack counter
    if (!inputHandler.AttackButtonPressed)
    {
      attackCounter = 0;
    }
  }
#pragma warning restore IDE0051
}