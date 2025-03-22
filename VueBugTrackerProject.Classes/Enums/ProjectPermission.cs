using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VueBugTrackerProject.Classes
{
    /// <summary>
    /// Describes a role a user can have regarding a project they don't own.
    /// </summary>
    public enum ProjectPermission
    {
        /// <summary>
        /// Grants the user permission to view a project. Only used if the project is restricted.
        /// </summary>
        Viewer,
        /// <summary>
        /// Sorts projects by when they were created.
        /// </summary>
        Editor

    }
}
