using System.ComponentModel.DataAnnotations;

namespace TheBugTracker.Client.Enums
{
    public enum Role
    {
        Admin,
        [Display(Name = "Project Manager")]
        ProjectManager,
        Developer,
        Submitter,
        [Display(Name = "Demo User")]
        DemoUser
    }
}