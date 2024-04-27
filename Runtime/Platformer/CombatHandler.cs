using UnityEngine;
public class CombatHandler : MonoBehaviour
{
  private PlatformerState platformerState;
  private InputHandler inputHandler;

  void Start()
  {
    platformerState = GetComponent<PlatformerMovement>().PlatformerState;
    inputHandler = platformerState.InputHandler;
  }

  void Attack()
  {
    Debug.Log("Attacking");
  }
}