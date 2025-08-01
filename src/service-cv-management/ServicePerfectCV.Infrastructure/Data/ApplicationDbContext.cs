using Microsoft.EntityFrameworkCore;
using ServicePerfectCV.Domain.Entities;
using ServicePerfectCV.Infrastructure.Data.Configurations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ServicePerfectCV.Infrastructure.Data
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Experience> Experiences { get; set; }
        public DbSet<Skill> Skills { get; set; }
        public DbSet<Summary> Summaries { get; set; }
        public DbSet<Certification> Certifications { get; set; }
        public DbSet<Contact> Contacts { get; set; }
        public DbSet<CV> CVs { get; set; }
        public DbSet<Education> Educations { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<Degree> Degrees { get; set; }
        public DbSet<Organization> Organizations { get; set; }
        public DbSet<JobTitle> JobTitles { get; set; }
        public DbSet<EmploymentType> EmploymentTypes { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<DeviceToken> DeviceTokens { get; set; }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfiguration(new UserConfiguration());
            modelBuilder.ApplyConfiguration(new ExperienceConfiguration());
            modelBuilder.ApplyConfiguration(new SkillConfiguration());
            modelBuilder.ApplyConfiguration(new CategoryConfiguration());
            modelBuilder.ApplyConfiguration(new SummaryConfiguration());
            modelBuilder.ApplyConfiguration(new CertificationConfiguration());
            modelBuilder.ApplyConfiguration(new ContactConfiguration());
            modelBuilder.ApplyConfiguration(new CVConfiguration());
            modelBuilder.ApplyConfiguration(new EducationConfiguration());
            modelBuilder.ApplyConfiguration(new ProjectConfiguration());
            modelBuilder.ApplyConfiguration(new OrganizationConfiguration());
            modelBuilder.ApplyConfiguration(new JobTitleConfiguration());
            modelBuilder.ApplyConfiguration(new EmploymentTypeConfiguration());
            modelBuilder.ApplyConfiguration(new DegreeConfiguration());
            modelBuilder.ApplyConfiguration(new DeviceTokenConfiguration());
        }
    }
}