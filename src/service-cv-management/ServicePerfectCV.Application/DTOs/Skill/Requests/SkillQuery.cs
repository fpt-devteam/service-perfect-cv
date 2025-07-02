using ServicePerfectCV.Application.DTOs.Pagination.Requests;
using ServicePerfectCV.Domain.Constants;

namespace ServicePerfectCV.Application.DTOs.Skill.Requests
{
    public class SkillQuery : PaginationQuery
    {
        public SkillSort? Sort { get; set; } = null;
    }

    public class SkillSort
    {
        public SortOrder? Category { get; set; } = null;
    }
}
