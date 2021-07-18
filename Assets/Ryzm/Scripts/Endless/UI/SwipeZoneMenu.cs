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
		public float MinimalSwipeLength = 50f;
		#endregion

		#region Private Variables
        Vector2 _firstTouchPosition;
		float _angle;
		float _length;
		Vector2 _destination;
		Vector2 _deltaSwipe;
		Direction _swipeDirection;
		GameStatus gameStatus;
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
					if(value)
					{
						Message.AddListener<GameStatusResponse>(OnGameStatusResponse);
					}
					else
					{
						Message.RemoveListener<GameStatusResponse>(OnGameStatusResponse);
					}
                	base.IsActive = value;
				}
            }
        }
		#endregion

		#region Listener Functions
		void OnGameStatusResponse(GameStatusResponse response)
		{
			gameStatus = response.status;
		}
		#endregion

		#region Event Functions
        /// <summary>
		/// Triggers the bound pointer down action
		/// </summary>
		public void OnPointerDown(PointerEventData data)
		{
			if(GameActive())
			{
				_firstTouchPosition = Input.mousePosition;
				Debug.Log("onpointerdown");
			}
		}

		/// <summary>
		/// Triggers the bound pointer up action
		/// </summary>
		public void OnPointerUp(PointerEventData data)
		{
			if(GameActive())
			{
				_destination = Input.mousePosition;
				_deltaSwipe = _destination - _firstTouchPosition;
				_length = _deltaSwipe.magnitude;

				// if the swipe has been long enough
				if (_length > MinimalSwipeLength)
				{
					_angle = EndlessUtils.AngleBetweenVectors(_deltaSwipe, Vector2.right);
					_swipeDirection = EndlessUtils.AngleToSwipeDirection(_angle);
					Swipe();
				}
			}
		}

		/// <summary>
		/// Triggers the bound pointer enter action when touch enters zone
		/// </summary>
		public void OnPointerEnter(PointerEventData data)
		{
			OnPointerDown(data);
		}

		/// <summary>
		/// Triggers the bound pointer exit action when touch is out of zone
		/// </summary>
		public void OnPointerExit(PointerEventData data)
		{
			OnPointerUp(data);	
		}
		#endregion

		#region Private Functions
        void Swipe()
		{
			if(GameActive())
			{
				if(_swipeDirection == Direction.Up)
				{
					InputManager.Instance.Jump();
				}
				else if(_swipeDirection == Direction.Down)
				{
					InputManager.Instance.Slide();
				}
				else
				{
					InputManager.Instance.Shift(_swipeDirection);
				}
			}
		}

		bool GameActive()
		{
			return gameStatus == GameStatus.Active;
		}
		#endregion
    }
}