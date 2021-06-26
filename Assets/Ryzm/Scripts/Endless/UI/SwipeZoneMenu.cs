using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ryzm.EndlessRunner;
using CodeControl;
using Ryzm.EndlessRunner.Messages;

namespace Ryzm.UI
{
    public class SwipeZoneMenu : RyzmMenu
    {
        public float minimumSwipeLength = 0.2f;
        
        EndlessController monkey;
        EndlessController dragon;
        Vector2 startPosition;
        float startTime;
        Vector2 endPosition;
        float endTime;
		float _angle;
		float _length;
		Vector2 _deltaSwipe;
		Direction _swipeDirection;

        public override bool IsActive 
        { 
            get
            { 
                return base.IsActive;
            }
            set 
            {
                // if(value)
                // {
                //     if(monkey == null || dragon == null)
                //     {
                //         Message.Send(new ControllersRequest());
                //     }
                //     else
                //     {
                //         monkey.OnStartTouch += SwipeStart;
                //         monkey.OnEndTouch += SwipeEnd;
                //         dragon.OnStartTouch += SwipeStart;
                //         dragon.OnEndTouch += SwipeEnd;
                //     }
                // }
                // else
                // {
                //     if(monkey != null && dragon != null)
                //     {
                //         monkey.OnStartTouch -= SwipeStart;
                //         monkey.OnEndTouch -= SwipeEnd;
                //         dragon.OnStartTouch -= SwipeStart;
                //         dragon.OnEndTouch -= SwipeEnd;
                //     }
                // }
                base.IsActive = value;
            }
        }

        protected override void Awake()
        {
            base.Awake();
            Message.AddListener<ControllersResponse>(OnControllersResponse);
        }

        void Start()
        {
            Message.Send(new ControllersRequest());
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            Message.RemoveListener<ControllersResponse>(OnControllersResponse);
        }

        void OnControllersResponse(ControllersResponse response)
        {
            if(monkey == null || dragon == null)
            {
                monkey = response.monkey;
                dragon = response.dragon;
                monkey.OnStartTouch += SwipeStart;
                monkey.OnEndTouch += SwipeEnd;
                dragon.OnStartTouch += SwipeStart;
                dragon.OnEndTouch += SwipeEnd;
                if(IsActive)
                {
                }
            }
        }

        void SwipeStart(Vector2 position, float time)
        {
            Debug.Log("go");
            startPosition = position;
            startTime = time;
        }

        void SwipeEnd(Vector2 positon, float time)
        {
            endPosition = positon;
            endTime = time;
            Debug.Log("ended");
            DetectSwipe();
        }

        void DetectSwipe()
        {
            _deltaSwipe = endPosition - startPosition;
			_length = _deltaSwipe.magnitude;

			// if the swipe has been long enough
			if (_length > minimumSwipeLength)
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
				InputManager.Instance.Slide();
			}
			else
			{
				InputManager.Instance.Shift(_swipeDirection);
			}
		}
    }
}
