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
		float _angle;
		float _length;
		Vector2 _destination;
		Vector2 _deltaSwipe;
		Direction _swipeDirection;
		GameStatus gameStatus;
		float tapTime;
		int taps;
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
			_OnPointerDown(data);
			taps++;
			if(taps == 1)
			{
				tapTime = Time.time;
			}
			else
			{
				float timeDiff = Time.time - tapTime;
				if(taps == 2 && timeDiff < doubleTapTime)
				{
					// todo: ask for special attack
					Debug.Log("special attack");
				}
				taps = 0;
			}
		}

		/// <summary>
		/// Triggers the bound pointer up action
		/// </summary>
		public void OnPointerUp(PointerEventData data)
		{
			_OnPointerUp(data);
		}

		/// <summary>
		/// Triggers the bound pointer enter action when touch enters zone
		/// </summary>
		public void OnPointerEnter(PointerEventData data)
		{
			_OnPointerDown(data);
		}

		/// <summary>
		/// Triggers the bound pointer exit action when touch is out of zone
		/// </summary>
		public void OnPointerExit(PointerEventData data)
		{
			_OnPointerUp(data);	
		}
		#endregion

		#region Private Functions
		void _OnPointerDown(PointerEventData data)
		{
			_firstTouchPosition = Input.mousePosition;
		}

		void _OnPointerUp(PointerEventData data)
		{
			Debug.Log("up");
			_destination = Input.mousePosition;
			_deltaSwipe = _destination - _firstTouchPosition;
			_length = _deltaSwipe.magnitude;

			// if the swipe has been long enough
			if (_length > minimalSwipeLength)
			{
				_angle = EndlessUtils.AngleBetweenVectors(_deltaSwipe, Vector2.right);
				_swipeDirection = EndlessUtils.AngleToSwipeDirection(_angle);
				Swipe();
			}
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
    }
}