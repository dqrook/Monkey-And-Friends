using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Ryzm.EndlessRunner 
{
    public class SwipeZone: MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler, IPointerEnterHandler
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
				_angle = AngleBetween(_deltaSwipe, Vector2.right);
				_swipeDirection = AngleToSwipeDirection(_angle);
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

		/// <summary>
		/// Determines a MMPossibleSwipeDirection out of an angle in degrees. 
		/// </summary>
		/// <returns>The to swipe direction.</returns>
		/// <param name="angle">Angle in degrees.</param>
		Direction AngleToSwipeDirection(float angle)
		{
			if ((angle < 45) || (angle >= 315))
			{
				return Direction.Right;
			}
			if ((angle >= 45) && (angle < 135))
			{
				return Direction.Up;
			}
			if ((angle >= 135) && (angle < 225))
			{
				return Direction.Left;
			}
			if ((angle >= 225) && (angle < 315))
			{
				return Direction.Down;
			}
			return Direction.Right;
		}

        void Swipe()
		{
			InputManager.Instance.Shift(_swipeDirection);
		}

        float AngleBetween(Vector2 vectorA, Vector2 vectorB)
		{
			float angle = Vector2.Angle(vectorA, vectorB);
			Vector3 cross = Vector3.Cross(vectorA, vectorB);

			if (cross.z > 0)
			{
				angle = 360 - angle;
			}

			return angle;
		}
    }
}