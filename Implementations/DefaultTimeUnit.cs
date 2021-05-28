using UnityEngine;

namespace FedoraDev.GameTime.Implementations
{
	public class DefaultTimeUnit : ITimeUnit
	{
		#region Editor Visuals
	#if UNITY_EDITOR
			[SerializeField] string _name;
#endif
		#endregion

		[SerializeField] bool _oneIndexed = false;
		[SerializeField] int _conversionRate = 1;
		[SerializeField] float _current = 0f;

		int ConversionRate => _conversionRate == -1 ? int.MaxValue : _conversionRate;

		public int Tick(float tickTime)
		{
			_current += tickTime;

			int lapses = _oneIndexed ?
				Mathf.FloorToInt((_current - 1) / ConversionRate) :
				Mathf.FloorToInt(_current / ConversionRate);

			_current -= ConversionRate * lapses;
			return lapses;
		}
	}
}
