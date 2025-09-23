using ServicePerfectCV.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServicePerfectCV.Domain.ValueObjects
{
    public class CVContentQueryResult
    {
        public List<Contact> Contact { get; set; } = [];
        public List<Summary> Summary { get; set; } = [];
        public List<Education> Education { get; set; } = [];
        public List<Experience> Experience { get; set; } = [];
        public List<Skill> Skills { get; set; } = [];
        public List<Project> Projects { get; set; } = [];
        public List<Certification> Certifications { get; set; } = [];
    }
}
