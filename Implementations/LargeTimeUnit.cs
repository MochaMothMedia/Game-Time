using Sirenix.OdinInspector;
using UnityEngine;

namespace FedoraDev.GameTime.Implementations
{
    public class LargeTimeUnit : ITimeUnit
    {
		#region Editor Visuals
#if UNITY_EDITOR
		[SerializeField, FoldoutGroup("$Name")] string _name;
		string Name => _name;
#endif
		#endregion

		public ulong Value => _current;
		public string Readable => $"{_current}";

		[SerializeField, FoldoutGroup("$Name")] bool _oneIndexed = false;
		[SerializeField, FoldoutGroup("$Name")] int _conversionRate = 1;
		[SerializeField, FoldoutGroup("$Name")] uint _current = 0;

		int ConversionRate => _conversionRate == -1 ? int.MaxValue : _conversionRate;
		float _currentFloat;

		public int Tick(float tickTime)
		{
			_currentFloat += tickTime;

			if (_currentFloat >= 1f)
			{
				int currentInt = Mathf.FloorToInt(_currentFloat);
				_current += (byte)currentInt;
				_currentFloat -= currentInt;
			}

			int lapses = _oneIndexed ?
				Mathf.FloorToInt((_current - 1) / ConversionRate) :
				Mathf.FloorToInt(_current / ConversionRate);

			_current -= (byte)(ConversionRate * lapses);

			return lapses;
		}
	}
}
