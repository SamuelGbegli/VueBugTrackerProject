using VueBugTrackerProject.Classes;

using Sodium;

namespace VueBugTrackerProject.Server
{
    /// <summary>
    /// Static class to generate data in the application.
    /// </summary>
    public static class SeedData
    {
        public static void SeedAll(DatabaseContext context)
        {
            SeedSuperUser(context);
        }
        /// <summary>
        /// Adds a super user account to the app's database, if one doesn't
        /// exist.
        /// </summary>
        /// <param name="context">The database context.</param>
        public static void SeedSuperUser(DatabaseContext context)
        {
            //Exits function if context is null or a super user already exists
            if(context == null || context.Accounts.Any(a => a.Role == AccountRole.SuperUser)) return;

            //Creates super user account
            var superUser = new Account
            {
                Username = "Super User",
                PasswordHash = PasswordHash.ArgonHashString("TestPassword"),
                Email = "SuperUser@VueBugTracker.com",
                Role = AccountRole.SuperUser,
                DateCreated = DateTime.Now
            };

            //Adds and saves user
            context.Accounts.Add(superUser);
            context.SaveChanges();
        }
    }
}
