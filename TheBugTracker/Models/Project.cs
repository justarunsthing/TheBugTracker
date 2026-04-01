using TheBugTracker.Client.Enums;
using TheBugTracker.Client.Models;
using System.ComponentModel.DataAnnotations;

namespace TheBugTracker.Models
{
    public class Project
    {
        // Fields
        private DateTimeOffset _created;
        private DateTimeOffset _startDate;
        private DateTimeOffset _endDate;

        public int Id { get; set; }

        [Required]
        public string? Name { get; set; }

        [Required]
        public string? Description { get; set; }

        // From auto property to full property
        public DateTimeOffset Created 
        { 
            get => _created; 
            set => _created = value.ToUniversalTime(); 
        }

        public DateTimeOffset StartDate 
        { 
            get => _startDate; 
            set => _startDate = value.ToUniversalTime(); 
        }

        public DateTimeOffset EndDate 
        { 
            get => _endDate; 
            set => _endDate = value.ToUniversalTime(); 
        }

        public ProjectPriority Priority { get; set; }
        public bool IsArchived { get; set; } = false;

        // Navigational Properties
        public int CompanyId { get; set; }
        public virtual Company? Company { get; set; }
        public virtual ICollection<ApplicationUser> Members { get; set; } = [];
        public virtual ICollection<Ticket> Tickets { get; set; } = [];
    }

    public static class ProjectExtensions
    {
        public static ProjectDTO ToDTO(this Project project)
        {
            foreach (var ticket in project.Tickets)
            {
                ticket.Project = null;
            }

            return new ProjectDTO
            {
                Id = project.Id,
                Name = project.Name,
                Description = project.Description,
                Created = project.Created,
                StartDate = project.StartDate,
                EndDate = project.EndDate,
                Priority = project.Priority,
                IsArchived = project.IsArchived,
                Tickets = [.. project.Tickets.Select(t => t.ToDTO())],
                Members = [.. project.Members.Select(m => m.ToDTO())]
            };
        }
    }
}