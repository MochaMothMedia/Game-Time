namespace FedoraDev.GameTime
{
	public interface ITimeUnit
    {
        ulong Value { get; }
        string Readable { get; }
        int Tick(float tickTime);
    }
}
