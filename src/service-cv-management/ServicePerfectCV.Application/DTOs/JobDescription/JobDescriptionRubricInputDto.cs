namespace ServicePerfectCV.Application.DTOs.JobDescription
{
    public sealed record JobDescriptionRubricInputDto
    {
        public required Guid JobDescriptionId { get; init; }
        public required string Title { get; init; }
        public required string CompanyName { get; init; }
        public required string Responsibility { get; init; }
        public required string Qualification { get; init; }
    }

    public static class JobDescriptionMappings
    {
        public static JobDescriptionRubricInputDto ToRubricInputDto(this Domain.Entities.JobDescription jd) => new()
        {
            JobDescriptionId = jd.Id,
            Title = jd.Title,
            CompanyName = jd.CompanyName,
            Responsibility = jd.Responsibility,
            Qualification = jd.Qualification
        };
    }
}
