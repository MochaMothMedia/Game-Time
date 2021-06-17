using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace FedoraDev.GameTime.Implementations
{
	public class DefaultGameTime : IGameTime
	{
		public ulong Value => GetValue();
		public string ReadableTime => GetReadableTime();

		[SerializeField, HideLabel, BoxGroup("Time Units")] List<ITimeUnit> _timeUnit;
		[SerializeField, HideLabel, BoxGroup("Change Event")] UnityEvent<string> _changeEvent = new UnityEvent<string>();

		public void Tick(float tickTime)
		{
			bool lapsedAtLeastOnce = false;

			for (int i = 0; i < _timeUnit.Count; i++)
			{
				tickTime = _timeUnit[i].Tick(tickTime);

				if (tickTime <= 0)
					break;
				lapsedAtLeastOnce = true;
			}

			if (lapsedAtLeastOnce)
				_changeEvent?.Invoke(GetReadableTime());
		}

		ulong GetValue()
		{
			ulong value = 0;
			ulong multiplier = 10;

			for (int i = 0; i < _timeUnit.Count; i++)
			{
				value += _timeUnit[i].Value * multiplier * ((ulong)i + 1);
			}

			return value;
		}

		string GetReadableTime()
		{
			List<string> times = new List<string>();

			for (int i = 0; i < _timeUnit.Count; i++)
				times.Add(_timeUnit[i].Readable);

			return string.Join(":", times);
		}
	}
}
