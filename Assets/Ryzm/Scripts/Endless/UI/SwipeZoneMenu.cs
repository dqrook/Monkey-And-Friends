using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Ryzm.EndlessRunner;
using Ryzm.EndlessRunner.Messages;
using CodeControl;

namespace Ryzm.UI
{
    public class SwipeZoneMenu: RyzmMenu, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler, IPointerEnterHandler
    {
		#region Public Variables
		/// the minimal length of a swipe
		public float minimalSwipeLength = 20f;
		public float doubleTapTime = 0.5f;
		#endregion

		#region Private Variables
        Vector2 _firstTouchPosition;
		Direction _swipeDirection;
		GameStatus gameStatus;
		float tapTime;
		Vector2 tapPosition;
		int taps;
		IEnumerator trackPosition;
		bool checkingSwipe;
		#endregion

		#region Properties
		public override bool IsActive 
        { 
            get
            { 
                return base.IsActive;
            }
            set 
            {
				if(ShouldUpdate(value))
				{
                	base.IsActive = value;
				}
            }
        }
		#endregion

		#region Event Functions
        /// <summary>
		/// Triggers the bound pointer down action
		/// </summary>
		public void OnPointerDown(PointerEventData data)
		{
			_OnPointerDown();
		}

		/// <summary>
		/// Triggers the bound pointer up action
		/// </summary>
		public void OnPointerUp(PointerEventData data)
		{
			_OnPointerUp();
		}

		/// <summary>
		/// Triggers the bound pointer enter action when touch enters zone
		/// </summary>
		public void OnPointerEnter(PointerEventData data)
		{
			_OnPointerDown();
		}

		/// <summary>
		/// Triggers the bound pointer exit action when touch is out of zone
		/// </summary>
		public void OnPointerExit(PointerEventData data)
		{
			_OnPointerUp();	
		}
		#endregion

		#region Private Functions
		void _OnPointerDown()
		{
			bool specialAttack = false;
			taps++;
			if(taps == 1)
			{
				tapTime = Time.time;
				tapPosition = Input.mousePosition;
			}
			else
			{
				float timeDiff = Time.time - tapTime;
				float length = GetLength(tapPosition);
				if(taps == 2 && timeDiff < doubleTapTime && length < minimalSwipeLength * 0.5f)
				{
					// todo: ask for special attack
					Debug.Log("special attack");
					specialAttack = true;
				}
				taps = 0;
			}
			_firstTouchPosition = Input.mousePosition;
			checkingSwipe = true;
			trackPosition = TrackPosition();
			StartCoroutine(trackPosition);
		}

		void _OnPointerUp()
		{
			if(trackPosition != null)
			{
				StopCoroutine(trackPosition);
			}
			CheckForSwipe();
			checkingSwipe = false;
		}

		void CheckForSwipe()
		{
			if(checkingSwipe)
			{
				Vector2 _destination = Input.mousePosition;
				Vector2 _deltaSwipe = _destination - _firstTouchPosition;
				float _length = _deltaSwipe.magnitude;

				// if the swipe has been long enough
				if (_length > minimalSwipeLength)
				{
					float _angle = EndlessUtils.AngleBetweenVectors(_deltaSwipe, Vector2.right);
					_swipeDirection = EndlessUtils.AngleToSwipeDirection(_angle);
					Swipe();
					checkingSwipe = false;
				}
			}
		}

		float GetLength(Vector2 position)
		{
			Vector2 _destination = Input.mousePosition;
			Vector2 _deltaSwipe = _destination - position;
			
			return _deltaSwipe.magnitude;
		}

        void Swipe()
		{
			if(_swipeDirection == Direction.Up)
			{
				InputManager.Instance.Jump();
			}
			else if(_swipeDirection == Direction.Down)
			{
				InputManager.Instance.Down();
			}
			else
			{
				InputManager.Instance.Shift(_swipeDirection);
			}
		}
		#endregion

		IEnumerator TrackPosition()
		{
			while(checkingSwipe)
			{
				CheckForSwipe();
				yield return null;
			}
		}
    }
}