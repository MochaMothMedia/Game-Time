using Sirenix.OdinInspector;
using UnityEngine;

namespace FedoraDev.GameTime.Implementations
{
	public class RealMonthTimeUnit : ITimeUnit
	{
		#region Editor Visuals
#if UNITY_EDITOR
		[SerializeField] string _name = "Days/Months/Years";
		[ShowInInspector, ReadOnly] bool LeapYear => IsLeapYear();
#endif
		#endregion

		[SerializeField] int _conversionRate = 1;
		[SerializeField] float _currentDay = 1f;
		[SerializeField] float _currentMonth = 1f;
		[SerializeField] float _currentYear = 2000f;

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


			while (_currentDay > DaysInMonth())
			{
				_currentDay -= DaysInMonth();

				TickMonths(1);
			}
		}

		void TickMonths(float tickTime)
		{
			_currentMonth += tickTime;

			int lapses = Mathf.FloorToInt((_currentMonth - 1) / 12);
			_currentMonth -= 12 * lapses;
			TickYears(lapses);
		}

		void TickYears(float tickTime)
		{
			_currentYear += tickTime;
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
