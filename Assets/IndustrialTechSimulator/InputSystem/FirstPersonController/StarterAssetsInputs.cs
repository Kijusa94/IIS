using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using UnityEngine;
#if ENABLE_INPUT_SYSTEM
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

        [Header("Action Map Settings")]
        public bool changeActionMap;
		public string currentActionMap;
        public int currentActionMapIndex;
        public List<string> changeActionMapList = new List<string>(){ "Player", "CableManagementPlayer" };

		[Header("Movement Settings")]
		public bool analogMovement;

		[Header("Mouse Cursor Settings")]
		public bool cursorLocked = true;
		public bool cursorInputForLook = true;

		[Header("Industrial Actions Settings")]
		public bool grab;
		public bool addAnchor;
        public bool removeAnchor;
        public bool activateInteractable;

#if ENABLE_INPUT_SYSTEM
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

		public void OnChangeActionMap(InputValue value)
		{
			ChangeActionMapInput(value.isPressed);
        }

        /// <summary>
        /// Industrial Tech Simulaotr values change.
        /// </summary>
        /// <param name="value"></param>
        public void OnGrab(InputValue value) => GrabInput(value.isPressed);
        public void OnAddAnchor(InputValue value) => AddAnchorInput(value.isPressed);
        public void OnRemoveAnchor(InputValue value) => RemoveAnchorInput(value.isPressed);
        public void OnActivateInteractable(InputValue value) => ActivateInteractableInput(value.isPressed);

#endif
        public void MoveInput(Vector2 newMoveDirection)
		{
			move = newMoveDirection;
		} 

		public void LookInput(Vector2 newLookDirection)
		{
			look = newLookDirection;
		}

		public void JumpInput(bool newJumpState)
		{
			jump = newJumpState;
		}

		public void SprintInput(bool newSprintState)
		{
			sprint = newSprintState;
		}

        public void ChangeActionMapInput(bool newActionMap)
        {
            if (newActionMap == true)
            {
                currentActionMapIndex += 1;
                if (currentActionMapIndex > changeActionMapList.Count - 1) currentActionMapIndex = 0;

                currentActionMap = changeActionMapList[currentActionMapIndex];
            }

            PlayerInput _playerInput = GetComponent<PlayerInput>(); ;            
            _playerInput.SwitchCurrentActionMap(currentActionMap);            
        }

        private void OnApplicationFocus(bool hasFocus)
		{
			SetCursorState(cursorLocked);
		}

		private void SetCursorState(bool newState)
		{
			Cursor.lockState = newState ? CursorLockMode.Locked : CursorLockMode.None;
		}

		/// <summary>
		/// Industrial Tech Simulaotr values change.
		/// </summary>
		/// <param name="newGrab"></param>
		public void GrabInput(bool newGrab) => grab = newGrab;
        public void AddAnchorInput(bool newGrab) => addAnchor = newGrab;
        public void RemoveAnchorInput(bool newGrab) => removeAnchor = newGrab;
        public void ActivateInteractableInput(bool newGrab) => activateInteractable = newGrab;




    }
	
}