namespace FedoraDev.GameTime
{
	public interface ITimeUnit
    {
        ulong Value { get; }
        int Tick(float tickTime);
    }
}
