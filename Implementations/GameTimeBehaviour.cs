using Sirenix.OdinInspector;
using UnityEngine;

namespace FedoraDev.GameTime.Implementations
{
    public class GameTimeBehaviour : SerializedMonoBehaviour, IGameTime
    {
        [SerializeField] IGameTime _gameTime;

		public void Tick(float tickTime) => _gameTime.Tick(tickTime);

		private void Update()
		{
			Tick(Time.deltaTime);
		}
	}
}
