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
			if(!checkingSwipe)
			{
				taps++;
				// Debug.Log("pointer down " + taps + " " + tapTime + " " + Time.time);
				if(taps == 1)
				{
					tapTime = Time.time;
					tapPosition = Input.mousePosition;
					// Debug.Log("tap time and pos " + tapTime + " " + tapPosition);
				}
				else
				{
					float timeDiff = Time.time - tapTime;
					float length = GetLength(tapPosition);
					// Debug.Log("pointer down 2 " + timeDiff + " " + length + " " + tapPosition + " " + Input.mousePosition);
					if(taps == 2 && timeDiff < doubleTapTime)
					{
						// Debug.Log("special attack");
						Message.Send(new SpecialAttackRequest());
						taps = 0;
					}
					else
					{
						tapTime = Time.time;
						tapPosition = Input.mousePosition;
						taps = 1;
					}
					// taps = 0;
				}
				_firstTouchPosition = Input.mousePosition;
				checkingSwipe = true;
				trackPosition = TrackPosition();
				StartCoroutine(trackPosition);
			}
		}

		void _OnPointerUp()
		{
			if(checkingSwipe)
			{
				if(trackPosition != null)
				{
					StopCoroutine(trackPosition);
					trackPosition = null;
				}
				CheckForSwipe();
				checkingSwipe = false;
			}
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
			InputManager.Instance.SetInput(_swipeDirection);
			taps = 0;
		}
		#endregion

		#region Coroutines
		IEnumerator TrackPosition()
		{
			while(checkingSwipe)
			{
				CheckForSwipe();
				yield return null;
			}
		}
		#endregion
    }
}