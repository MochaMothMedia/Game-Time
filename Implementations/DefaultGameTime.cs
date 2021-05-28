using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

namespace FedoraDev.GameTime.Implementations
{
	public class DefaultGameTime : IGameTime
	{
		[SerializeField, HideLabel, BoxGroup("Time Scale")] float _timeScale;
		[SerializeField, HideLabel, BoxGroup("Time Units")] List<ITimeUnit> _timeUnit;

		public void Tick(float tickTime)
		{
			tickTime *= _timeScale;

			for (int i = 0; i < _timeUnit.Count; i++)
			{
				tickTime = _timeUnit[i].Tick(tickTime);

				if (tickTime <= 0)
					break;
			}
		}
	}
}