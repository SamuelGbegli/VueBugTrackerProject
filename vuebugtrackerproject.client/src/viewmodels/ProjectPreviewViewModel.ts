//View model for showing a preview of a project.
import AccountViewModel from "./AccountViewModel";

export default class ProjectPreviewViewModel{

	// Unique identifier for the project.
	ID: string = "";
	// The name of the project.
	Name: string = "";
	// The short description of the project.
	Summary: string = "";
	// The user that created the project.
	Owner: AccountViewModel;
	// The number of bugs in the project are open.
	OpenBugs: number = 0;
	// The total number of bugs in the project.
	TotalBugs: number = 0;
	// The date the project was last updated.
	DateModified: Date;
}
