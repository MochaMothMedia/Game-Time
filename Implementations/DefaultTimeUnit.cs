using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace FedoraDev.GameTime.Implementations
{
	public class DefaultTimeUnit : ITimeUnit
	{
		#region Editor Visuals
#if UNITY_EDITOR
		[SerializeField, FoldoutGroup("$Name")] string _name;
		string Name => _name;
#endif
		#endregion

		[SerializeField, FoldoutGroup("$Name")] bool _oneIndexed = false;
		[SerializeField, FoldoutGroup("$Name")] int _conversionRate = 1;
		[SerializeField, FoldoutGroup("$Name")] float _current = 0f;
		[SerializeField, HideLabel, BoxGroup("$Name/Change Event")] UnityEvent<float> _timeChanged = new UnityEvent<float>();

		int ConversionRate => _conversionRate == -1 ? int.MaxValue : _conversionRate;
		public ulong Value => (ulong)(_current / _conversionRate * 100);

		public int Tick(float tickTime)
		{
			_current += tickTime;

			int lapses = _oneIndexed ?
				Mathf.FloorToInt((_current - 1) / ConversionRate) :
				Mathf.FloorToInt(_current / ConversionRate);

			_current -= ConversionRate * lapses;

			if (lapses > 0)
				_timeChanged?.Invoke(_current);

			return lapses;
		}
	}
}
