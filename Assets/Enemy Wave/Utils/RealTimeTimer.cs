using System;

namespace Neon2.SlimeSystem
{
	[System.Serializable]
	public class RealTimeTimer
	{
		public float timer;
		public TimeUnit timeUnit;

		private float timerInSeconds;
		private float currentTimer;
		private Action completedTimerAction;
		private bool timeReached;
		private bool actionStopped;
		private bool running;

		public bool startWithAction;


		public RealTimeTimer()
		{
			timer = 0f;
			timeUnit = TimeUnit.SECONDS;
			startWithAction = false;
			running = false;
		}

		public RealTimeTimer(float timer, TimeUnit timeUnit, bool startWithAction, Action completedTimerAction)
		{
			Setup(timer, timeUnit, startWithAction, completedTimerAction);
		}

		public void Setup(float timer, TimeUnit timeUnit, bool startWithAction, Action completedTimerAction)
		{
			this.timer = timer;
			this.timeUnit = timeUnit;
			this.startWithAction = startWithAction;
			running = false;


			this.completedTimerAction = completedTimerAction;

			if (startWithAction)
			{
				completedTimerAction();
			}

			ResetTimer();
		}

		public void Setup(Action completedTimerAction)
		{
			Setup(this.timer, this.timeUnit, this.startWithAction, completedTimerAction);
		}

		public void ResetTimer()
		{
			this.currentTimer = GetTimerInSeconds();
			this.timerInSeconds = currentTimer;

			this.timeReached = false;
			this.actionStopped = false;
			this.running = false;
		}

		public float GetTimerInSeconds()
		{
			float res = timer;
			switch (timeUnit)
			{
				case TimeUnit.SECONDS:
					res = timer;
					break;
				case TimeUnit.MINUTES:
					res = Converter.FromMinutesToSeconds(timer);
					break;
				case TimeUnit.HOURS:
					res = Converter.FromHoursToSeconds(timer);
					break;
				case TimeUnit.DAYS:
					res = Converter.FromDaysToSecondsFactor(timer);
					break;
			}

			return res;
		}

		public float GetCurrentTimer()
		{
			return currentTimer;
		}

		public float GetNormalizedTimer()
		{
			float res = 1 - (currentTimer / timerInSeconds);
			return res;
		}

		public void SetCurrentTimer(float currentTimer)
		{
			this.currentTimer = currentTimer;
		}

		public void Update(float deltaTime)
		{
			if (!running) { return; }

			if (!timeReached)
			{
				currentTimer -= deltaTime;

				if (currentTimer < 0)
				{
					timeReached = true;
					currentTimer = 0f;

					if (completedTimerAction != null && !actionStopped)
					{
						completedTimerAction();
					}
				}
			}
		}

		public void StopTimer()
		{
			actionStopped = true;
			running = false;
		}

		public void RunTimer()
		{
			actionStopped = false;
			running = true;
		}

		public bool IsTimeReached()
		{
			return timeReached;
		}
	}
}