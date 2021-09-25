using System.Globalization;
using System.Text;

namespace Neon2.SlimeSystem
{
	public static class Converter
	{
		//SYMBOLS
		private const string D2 = "D2";
		private const string COLONS = ":";


		//MILLISECONDS
		private const float MILLISECONDS_TO_SECONDS_FACTOR = 0.01f;

		//SECONDS
		private const float SECONDS_TO_MILLISECONDS_FACTOR = 1 / MILLISECONDS_TO_SECONDS_FACTOR;
		private const float SECONDS_TO_MINUTES_FACTOR = 1 / MINUTES_TO_SECONDS_FACTOR;
		private const float SECONDS_TO_HOURS_FACTOR = 1 / HOURS_TO_SECONDS_FACTOR;
		private const float SECONDS_TO_DAYS_FACTOR = 1 / DAYS_TO_SECONDS_FACTOR;

		//MINUTES
		private const float MINUTES_TO_SECONDS_FACTOR = 60f;
		private const float MINUTES_TO_HOURS_FACTOR = 1 / HOURS_TO_MINUTES_FACTOR;
		private const float MINUTES_TO_DAYS_FACTOR = 1 / DAYS_TO_MINUTES_FACTOR;

		//HOURS
		private const float HOURS_TO_SECONDS_FACTOR = 3600f;
		private const float HOURS_TO_MINUTES_FACTOR = 60f;
		private const float HOURS_TO_DAYS_FACTOR = 1 / DAYS_TO_HOURS_FACTOR;

		//DAYS
		private const float DAYS_TO_SECONDS_FACTOR = 86400f;
		private const float DAYS_TO_MINUTES_FACTOR = 1440f;
		private const float DAYS_TO_HOURS_FACTOR = 24f;

		//NUMBER FORMAT
		private static NumberFormatInfo numberFormatInfo;

		public static float FromSecondsToMinutes(float seconds)
		{
			float res = seconds * SECONDS_TO_MINUTES_FACTOR;
			return res;
		}

		public static float FromMinutesToSeconds(float minutes)
		{
			float res = minutes * MINUTES_TO_SECONDS_FACTOR;
			return res;
		}

		public static float FromHoursToSeconds(float hours)
		{
			float res = hours * HOURS_TO_SECONDS_FACTOR;
			return res;
		}

		public static float FromDaysToSecondsFactor(float days)
		{
			float res = days * DAYS_TO_SECONDS_FACTOR;
			return res;
		}

		public static string GetFormattedMinutesAndSeconds(float seconds)
		{
			string res = string.Empty;
			StringBuilder stringBuilder = new StringBuilder();

			int minutes = (int)(seconds * SECONDS_TO_MINUTES_FACTOR);
			int secondsRemaining = (int)(seconds % MINUTES_TO_SECONDS_FACTOR);


			stringBuilder.Append(minutes.ToString(D2));
			stringBuilder.Append(COLONS);
			stringBuilder.Append(secondsRemaining.ToString(D2));

			res = stringBuilder.ToString();

			return res;
		}

		public static string GetFormattedSSff(float seconds)
		{
			string res = string.Empty;
			StringBuilder stringBuilder = new StringBuilder();





			int _seconds = (int)seconds;


			float secondsReminder = seconds - _seconds;
			int _milliseconds = (int)(secondsReminder * SECONDS_TO_MILLISECONDS_FACTOR);


			stringBuilder.Append(_seconds.ToString(D2));
			stringBuilder.Append(COLONS);
			stringBuilder.Append(_milliseconds.ToString(D2));

			res = stringBuilder.ToString();
			return res;
		}

		public static string GetFormattedHHmmSS(float seconds)
		{
			string res = string.Empty;
			StringBuilder stringBuilder = new StringBuilder();


			int _seconds = (int)(seconds % MINUTES_TO_SECONDS_FACTOR);
			float minutesRemainder = seconds * SECONDS_TO_MINUTES_FACTOR;

			int _minutes = (int)(minutesRemainder % HOURS_TO_MINUTES_FACTOR);
			float hoursRemainder = (minutesRemainder * MINUTES_TO_HOURS_FACTOR);

			int _hours = (int)(hoursRemainder % DAYS_TO_HOURS_FACTOR);



			stringBuilder.Append(_hours.ToString(D2));
			stringBuilder.Append(COLONS);
			stringBuilder.Append(_minutes.ToString(D2));
			stringBuilder.Append(COLONS);
			stringBuilder.Append(_seconds.ToString(D2));

			res = stringBuilder.ToString();
			return res;
		}

		public static string GetThousandsFormatted(int number)
		{
			if (numberFormatInfo == null)
			{
				numberFormatInfo = (NumberFormatInfo)CultureInfo.InvariantCulture.NumberFormat.Clone();
				numberFormatInfo.NumberGroupSeparator = " ";
			}

			string res = number.ToString("#,#", numberFormatInfo);

			return res;
		}
	}
}