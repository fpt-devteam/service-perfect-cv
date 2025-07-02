using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ServicePerfectCV.Domain.Enums
{
    public enum AuthenticationMethod
    {
        /// <summary>
        /// User created via JWT registration with password
        /// </summary>
        JWT = 1,

        /// <summary>
        /// User created via Google OAuth
        /// </summary>
        Google = 2,

        /// <summary>
        /// User created via LinkedIn OAuth
        /// </summary>
        LinkedIn = 3
    }
}
