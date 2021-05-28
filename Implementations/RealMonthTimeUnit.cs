using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace FedoraDev.GameTime.Implementations
{
	public class RealMonthTimeUnit : ITimeUnit
	{
		#region Editor Visuals
#if UNITY_EDITOR
		[SerializeField, FoldoutGroup("$Name")] string _name = "Days/Months/Years";
		[ShowInInspector, ReadOnly, FoldoutGroup("$Name")] bool LeapYear => IsLeapYear();
		string Name => _name;
#endif
		#endregion

		[SerializeField, FoldoutGroup("$Name")] int _conversionRate = 1;
		[SerializeField, FoldoutGroup("$Name")] float _currentDay = 1f;
		[SerializeField, FoldoutGroup("$Name")] float _currentMonth = 1f;
		[SerializeField, FoldoutGroup("$Name")] float _currentYear = 2000f;
		[SerializeField, HideLabel, BoxGroup("$Name/Day Event")] UnityEvent<float> _dayChanged = new UnityEvent<float>();
		[SerializeField, HideLabel, BoxGroup("$Name/Month Event")] UnityEvent<float> _monthChanged = new UnityEvent<float>();
		[SerializeField, HideLabel, BoxGroup("$Name/Year Event")] UnityEvent<float> _yearChanged = new UnityEvent<float>();

		int ConversionRate => _conversionRate == -1 ? int.MaxValue : _conversionRate;

		public int Tick(float tickTime)
		{
			TickDays(tickTime);

			int lapses = Mathf.FloorToInt(_currentYear / ConversionRate);
			_currentYear -= ConversionRate * lapses;

			return lapses;
		}

		void TickDays(float tickTime)
		{
			_currentDay += tickTime;
			bool dayChanged = false;

			while (_currentDay > DaysInMonth())
			{
				dayChanged = true;
				_currentDay -= DaysInMonth();

				TickMonths(1);
			}

			if (dayChanged)
				_dayChanged?.Invoke(_currentDay);
		}

		void TickMonths(float tickTime)
		{
			_currentMonth += tickTime;

			int lapses = Mathf.FloorToInt((_currentMonth - 1) / 12);
			_currentMonth -= 12 * lapses;
			TickYears(lapses);

			if (lapses > 0)
				_monthChanged?.Invoke(_currentMonth);
		}

		void TickYears(float tickTime)
		{
			_currentYear += tickTime;

			_yearChanged?.Invoke(_currentYear);
		}

		int DaysInMonth()
		{
			// Month is 1-indexed
			switch (_currentMonth)
			{
				case 4:  // April
				case 6:  // June
				case 9:  // September
				case 11: // November
					return 30;

				case 2: // February
					return IsLeapYear() ? 29 : 28;

				default: // January, March, May, July, August, October, December
					return 31;
			}
		}

		bool IsLeapYear()
		{
			// Truth Table
			// Quarter		Century		QtrCentury		 Out
			//	false		false		false			false
			//	false		false		true			false
			//	false		true		false			false
			//	false		true		true			false

			//	true		false		false			true
			//	true		false		true			true
			
			//	true		true		false			false
			//	true		true		true			true

			bool isQuarter = _currentYear % 4 == 0; //Every 4 years we have a leap year
			bool isCentury = _currentYear % 100 == 0; //Every 100 years we skip leap year
			bool isQuarterCentury = _currentYear % 400 == 0; //Every 400 years, we skip skipping leap year

			return isQuarter && (!isCentury || isQuarterCentury);
		}
	}
}
