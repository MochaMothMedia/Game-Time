namespace MochaMoth.GameTime
{
	public interface IGameTime
    {
        void Tick(float tickTime);
        ulong Value { get; }
        string ReadableTime { get; }
    }
}
