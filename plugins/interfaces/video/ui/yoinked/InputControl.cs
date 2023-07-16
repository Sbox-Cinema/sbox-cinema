namespace CinemaTeam.Plugins.Media.Yoinked
{
	/// <summary>
	/// A UI element that can have invalid user input, such as a <see cref="TextEntry"/>.
	/// </summary>
	public interface IInputControl
	{
		/// <summary>
		/// Whether the current user input is valid, or not.
		/// </summary>
		bool HasValidationErrors { get; }
	}

}
