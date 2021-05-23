using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Ryzm.EndlessRunner.UI
{
    public class SwipeZone: EndlessMenu, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler, IPointerEnterHandler
    {
		/// the minimal length of a swipe
		public float MinimalSwipeLength = 50f;
        Vector2 _firstTouchPosition;
		float _angle;
		float _length;
		Vector2 _destination;
		Vector2 _deltaSwipe;
		protected Direction _swipeDirection;

        /// <summary>
		/// Triggers the bound pointer down action
		/// </summary>
		public void OnPointerDown(PointerEventData data)
		{
			_firstTouchPosition = Input.mousePosition;
		}

		/// <summary>
		/// Triggers the bound pointer up action
		/// </summary>
		public void OnPointerUp(PointerEventData data)
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

        void Swipe()
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
}