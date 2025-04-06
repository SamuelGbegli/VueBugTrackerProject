namespace VueBugTrackerProject.Classes
{
	/// <summary>
	/// View model for a project.
	/// </summary>
	public class ProjectViewModel
	{
		/// <summary>
		/// Unique identifier for the project.
		/// </summary>
		public string ID { get; set; }
	
		/// <summary>
		/// The name of the project.
		/// </summary>
		public string Name { get; set; }
	
		/// <summary>
		/// The short description of the project.
		/// </summary>
		public string Summary { get; set; }
	
		/// <summary>
		/// The user that created the project.
		/// </summary>
		public AccountViewModel Owner { get; set; }
	
		/// <summary>
		/// The number of bugs in the project are open.
		/// </summary>
		public int OpenBugs { get; set; }
	
		/// <summary>
		/// The total number of bugs in the project.
		/// </summary>
		public int TotalBugs { get; set; }
	
		/// <summary>
		/// The date the project was last updated.
		/// </summary>
		public DateTime DateModified { get; set; }
	
		/// <summary>
		/// The formatted project description.
		/// </summary>
		public string Description { get; set; }
	
		/// <summary>
		/// The project's tags.
		/// </summary>
		public List<string> Tags { get; set; }
	
	}
}
