import AccountRole from "@/enumConsts/Role";

export default class AccountViewModel{

	// Unique identifier for the account.
	ID: string = "";
	// The account's username.
	Username: string = "";
	// The icon that will be shown with the account's username.
	Icon: string = "";
	// The role and privileges the account has in the application.
	Role: number = AccountRole.Normal;
	// If true, the user cannot login with the account.
	Suspended: boolean = false;
	// The date and time the account was created.
	DateCreated: Date = new Date();
}
