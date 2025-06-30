using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ServicePerfectCV.Domain.ValueObjects
{
    public sealed record JobDetail(string JobTitle, string CompanyName, string Description);
}