using Microsoft.AspNetCore.Identity;
using System;

namespace GCCS.Mvc.Security
{
    public class ApplicationRole : IdentityRole<int>
    {
        public ApplicationRole() : base()
        {
            base.ConcurrencyStamp = string.Empty;
        }

        public override string ConcurrencyStamp {
            get => string.Empty;
            set => throw new NotSupportedException();
        }
    }

    public class ApplicationUser : IdentityUser<int>
    {
        public ApplicationUser() : base()
        {
            base.ConcurrencyStamp = string.Empty;
            base.SecurityStamp = Guid.NewGuid().ToString();
        }

        /// <summary>
        /// Our version of IdentityUser does not make use of this property.
        /// </summary>
        public override string ConcurrencyStamp
        {
            get => string.Empty;
        }

        public override string NormalizedEmail {
            get => Email?.ToUpperInvariant() ?? string.Empty;
        }

        public override string NormalizedUserName
        {
            get => UserName?.ToUpperInvariant() ?? string.Empty;
        }

        public byte[] LastUpdatedStamp { get; set; }
    }
}
