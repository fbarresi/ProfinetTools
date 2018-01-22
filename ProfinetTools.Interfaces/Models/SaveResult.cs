namespace ProfinetTools.Interfaces.Models
{
	public class SaveResult
	{
		public SaveResult(bool success, string errorMessage)
		{
			Success = success;
			ErrorMessage = errorMessage;
		}

		public bool Success { get; }
		public string ErrorMessage { get; }
	}
}