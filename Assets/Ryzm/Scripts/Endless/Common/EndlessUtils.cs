using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ryzm.EndlessRunner
{
    public static class EndlessUtils
    {
        public static System.Random r = new System.Random();
        public static void Shuffle<T>(this IList<T> list)
        {
            int n = list.Count;
            while(n > 1)
            {
                n--;
                int k = r.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }

        public static Vector3 ScreenToWorld(Camera camera, Vector3 position)
        {
            position.z = camera.nearClipPlane;
            return camera.ScreenToWorldPoint(position);
        }

        /// <summary>
		/// Determines a Direction out of an angle in degrees. 
		/// </summary>
		/// <returns>The to swipe direction.</returns>
		/// <param name="angle">Angle in degrees.</param>
		public static Direction AngleToSwipeDirection(float angle)
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

        public static float AngleBetweenVectors(Vector2 vectorA, Vector2 vectorB)
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
