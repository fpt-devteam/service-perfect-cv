using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ServicePerfectCV.Application.Interfaces
{
    public interface ICVSnapshotService
    {
        Task UpdateCVSnapshotIfChangedAsync(Guid cvId);
    }
}