using UnityEngine;
#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
using UnityEngine.InputSystem;
#endif

namespace StarterAssets
{
    public class StarterAssetsInputs : MonoBehaviour
	{
		[Header("Character Input Values")]
		public Vector2 move;
		public Vector2 look;
		public bool jump;
		public bool sprint;
		public bool fire;
		public bool explode;
		public Vector2 rotate;

		[Header("Movement Settings")]
		public bool analogMovement;

		[Header("Mouse Cursor Settings")]
		public bool cursorLocked = true;
		public bool cursorInputForLook = true;

#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
		public void OnMove(InputValue value)
		{
			MoveInput(value.Get<Vector2>());
		}

		public void OnLook(InputValue value)
		{
			if(cursorInputForLook)
			{
				LookInput(value.Get<Vector2>());
			}
		}

		public void OnJump(InputValue value)
		{
			JumpInput(value.isPressed);
		}

		public void OnSprint(InputValue value)
		{
			SprintInput(value.isPressed);
		}

		public void OnFire(InputValue value) 
		{
			OnFireInput(value.isPressed);
		}

		public void OnExplode(InputValue value)
		{
			OnExplodeInput(value.isPressed);
		}

		public void OnRotate(InputValue value)
		{
			OnRotateInput(value.Get<Vector2>());
		}
#endif


		public void MoveInput(Vector2 newMoveDirection)
		{
			move = newMoveDirection;
		} 

		public void LookInput(Vector2 newLookDirection)
		{
			//print("Looking " + newLookDirection);
			look = newLookDirection;
		}

		public void JumpInput(bool newJumpState)
		{
			//print("JumpInput ");
			jump = newJumpState;
		}

		public void SprintInput(bool newSprintState)
		{
			//print("SprintInput ");
			sprint = newSprintState;
		}

		public void OnFireInput(bool newLeftMouseState)
		{
			//print("check this one " + newLeftMouseState);
			fire = newLeftMouseState;
		}

		public void OnExplodeInput(bool newExplodeState)
		{
			explode = newExplodeState;
		}

		private void OnRotateInput(Vector2 newRotateDirection)
        {
			print("rotate " + newRotateDirection);
			rotate = newRotateDirection;
		}


		private void OnApplicationFocus(bool hasFocus)
		{
			SetCursorState(cursorLocked);
		}

		private void SetCursorState(bool newState)
		{
			Cursor.lockState = newState ? CursorLockMode.Locked : CursorLockMode.None;
		}
	}
	
}