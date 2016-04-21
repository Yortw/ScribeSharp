namespace ScribeSharp
{
	public interface ILogEventContextProvider
	{

		string PropertyName { get; }
		object GetValue();

		ILogEventFilter Filter { get; }

	}
}