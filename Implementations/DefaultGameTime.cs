using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

namespace FedoraDev.GameTime.Implementations
{
	public class DefaultGameTime : IGameTime
	{
		public ulong Value => GetValue();
		public string ReadableTime => GetReadableTime();

		[SerializeField, HideLabel, BoxGroup("Time Units")] List<ITimeUnit> _timeUnit;

		public void Tick(float tickTime)
		{
			for (int i = 0; i < _timeUnit.Count; i++)
			{
				tickTime = _timeUnit[i].Tick(tickTime);

				if (tickTime <= 0)
					break;
			}
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
