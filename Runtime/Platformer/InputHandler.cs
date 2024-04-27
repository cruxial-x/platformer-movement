using UnityEngine;
public class InputHandler
{
  public float HorizontalInput { get; private set; }
  public float VerticalInput { get; private set; }
  public bool JumpButtonDown { get; private set; }
  public bool JumpButtonHeld { get; private set; }
  public bool SlideButtonPressed { get; private set; }
  public bool ToggleWeaponButtonPressed { get; private set; }

  public void HandleInput()
  {
    HorizontalInput = Input.GetAxis("Horizontal");
    VerticalInput = Input.GetAxis("Vertical");
    JumpButtonDown = Input.GetButtonDown("Jump");
    JumpButtonHeld = Input.GetButton("Jump");
    SlideButtonPressed = Input.GetKeyDown(KeyCode.LeftShift);
    ToggleWeaponButtonPressed = Input.GetKeyDown(KeyCode.Q);
  }
}