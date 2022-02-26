using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace FedoraDev.GameTime.Implementations
{
	public class DefaultGameTime : IGameTime
	{
#if UNITY_EDITOR
		[ShowInInspector, ReadOnly, HideLabel, BoxGroup("Bit Representation")]
		public string ByteTime => GetBinaryTime();
#endif

		[ShowInInspector, ReadOnly, HideLabel, BoxGroup("ULong Representation")]
		public ulong Value => GetValue();

		[ShowInInspector, ReadOnly, HideLabel, BoxGroup("Readable Representation")]
		public string ReadableTime => GetReadableTime();

		[SerializeField, HideLabel, BoxGroup("Time Units")] List<ITimeUnit> _timeUnit;
		[SerializeField, HideLabel, BoxGroup("Change => Readable String")] UnityEvent<string> _changeReadableEvent = new UnityEvent<string>();
		[SerializeField, HideLabel, BoxGroup("Change => Value")] UnityEvent<ulong> _changeValueEvent = new UnityEvent<ulong>();

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
			{
				_changeReadableEvent?.Invoke(GetReadableTime());
				_changeValueEvent?.Invoke(GetValue());
			}
		}
		
		ulong GetValue()
		{
			ulong value = 0;

			for (int i = 0; i < _timeUnit.Count; i++)
			{
				ulong unit = _timeUnit[i].Value << (8 * i);
				value += unit;
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

#if UNITY_EDITOR
		string GetBinaryTime()
		{
			string binary = Convert.ToString((long)Value, 2).PadLeft(64, '0');
			List<string> segments = new List<string>();

			for (int i = 0; i < 64; i += 4)
				segments.Add(binary.Substring(i, 4));

			return string.Join(" ", segments);
		}
#endif
	}
}
