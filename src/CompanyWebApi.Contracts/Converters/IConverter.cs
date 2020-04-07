namespace CompanyWebApi.Contracts.Converters
{
	/// <summary>
	/// Generic converter interface contract
	/// </summary>
	public interface IConverter<in TFrom, out TTo>
	{
		TTo Convert(TFrom source);
	}
}
