using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace OnlineVehicleRentalSystem.Models
{
    /// <summary>
    /// Represents a user in the Online Vehicle Rental System.
    /// Inherits from IdentityUser to include identity management properties.
    /// </summary>
    public class User : IdentityUser
    {
        /// <summary>
        /// Gets or sets the name of the user. This property is required.
        /// </summary>
        public required string Name { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the collection of bookings associated with the user.
        /// Navigation property to the Booking entity.
        /// </summary>
        public ICollection<Booking> Bookings { get; set; } = new List<Booking>();

        /// <summary>
        /// Gets or sets the collection of roles associated with the user.
        /// Navigation property to the IdentityUserRole entity.
        /// </summary>
        public ICollection<IdentityUserRole<string>> UserRoles { get; set; } = new List<IdentityUserRole<string>>();
    }
}
