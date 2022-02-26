using Sirenix.OdinInspector;
using UnityEngine;

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

		public ulong Value => GetValue();
		public string Readable => $"{_currentDay}:{_currentMonth}:{_currentYear}";

		[SerializeField, FoldoutGroup("$Name")] int _conversionRate = 1;
		[SerializeField, FoldoutGroup("$Name")] byte _currentDay = 1;
		[SerializeField, FoldoutGroup("$Name")] byte _currentMonth = 1;
		[SerializeField, FoldoutGroup("$Name")] uint _currentYear = 2000;

		int ConversionRate => _conversionRate == -1 ? int.MaxValue : _conversionRate;
		float _currentDayFloat;

		ulong GetValue()
		{
			if (_currentYear > 16777215)
				Debug.Log("Some data from the year may be lost if this time unit uses seconds, minutes, and hours. This time unit only uses 3 bytes for the year which has a maximum value of 16,777,215.");

			ulong value = _currentDay; // 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 dddd dddd
			value += (ulong)_currentMonth << 8; // 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 mmmm mmmm dddd dddd
			value += (ulong)_currentYear << 16; // 0000 0000 0000 0000 0000 0000 yyyy yyyy yyyy yyyy yyyy yyyy mmmm mmmm dddd dddd

			return value;
		}

		public int Tick(float tickTime)
		{
			TickDays(tickTime);

			int lapses = Mathf.FloorToInt(_currentYear / ConversionRate);
			_currentYear -= (uint)(ConversionRate * lapses);

			return lapses;
		}

		void TickDays(float tickTime)
		{
			_currentDayFloat += tickTime;

			if (_currentDayFloat >= 1f)
			{
				int currentDayInt = Mathf.FloorToInt(_currentDayFloat);
				_currentDay += (byte)currentDayInt;
				_currentDayFloat -= currentDayInt;
			}

			while (_currentDay > DaysInMonth())
			{
				_currentDay -= (byte)DaysInMonth();

				TickMonths();
			}
		}

		void TickMonths()
		{
			_currentMonth += 1;

			if (_currentMonth >= 12)
			{
				_currentMonth -= 12;
				TickYears();
			}
		}

		void TickYears()
		{
			_currentYear += 1;
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
