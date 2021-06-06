using Sirenix.OdinInspector;
using UnityEngine;

namespace FedoraDev.GameTime.Implementations
{
	[HideMonoScript]
    public class GameTimeBehaviour : SerializedMonoBehaviour, IGameTime
    {
		public ulong Value => _gameTime.Value;
		public string ReadableTime => _gameTime.ReadableTime;

		[SerializeField] float _timeScale = 1f;
        [SerializeField] IGameTime _gameTime;

		public void Tick(float tickTime) => _gameTime.Tick(tickTime);

		private void Update()
		{
			Tick(Time.deltaTime * _timeScale);
		}
	}
}
