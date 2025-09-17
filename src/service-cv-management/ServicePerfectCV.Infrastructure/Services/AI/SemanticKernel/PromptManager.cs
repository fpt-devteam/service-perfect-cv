using ServicePerfectCV.Infrastructure.Constants;

namespace ServicePerfectCV.Infrastructure.Services.AI.SemanticKernel
{
    public static class PromptManager
    {
        public static string GetPrompt(PromptType promptType)
        {
            return promptType switch
            {
                PromptType.SectionRubricBuilding => GetSectionRubricBuildingPrompt(),
                PromptType.SectionScoring => GetSectionScoringPrompt(),
                PromptType.OverallSummary => GetOverallSummaryPrompt(),
                _ => throw new ArgumentException($"Unknown prompt type: {promptType}")
            };
        }

        private static string GetSectionRubricBuildingPrompt()
        {
            return """
            You are an expert recruiter creating detailed evaluation criteria for CV assessment. 
            Analyze the job description deeply and create specific, measurable criteria tailored to this exact role.

            Job Title: {{ $title }}
            Job Level: {{ $level }}
            Requirements: {{ $requirements }}

            ANALYSIS INSTRUCTIONS:
            1. Extract key technical skills, tools, and technologies mentioned in requirements
            2. Identify experience level expectations from the job level and requirements
            3. Determine industry context and domain-specific needs
            4. Identify soft skills and collaboration requirements
            5. Assess education and certification priorities based on role

            CREATE EVALUATION CRITERIA:

            CONTACT (1-2 criteria max):
            - Professional presentation: email format, phone, location alignment
            - Digital presence: LinkedIn quality, GitHub/portfolio relevance to role
            - Weight criteria based on role seniority (junior: basic contact, senior: thought leadership)

            SUMMARY (2-3 criteria max):
            - Role alignment: how well career narrative matches this specific position
            - Achievement relevance: quantified accomplishments related to job requirements
            - Professional communication: clarity, conciseness, value proposition strength
            - Weight based on role complexity and leadership requirements

            SKILLS (3-4 criteria max - MOST IMPORTANT SECTION):
            - PRIMARY TECHNICAL SKILLS: Extract exact technologies/languages from job requirements
            - SECONDARY TECHNICAL SKILLS: Related tools, frameworks, methodologies mentioned
            - EXPERIENCE DEPTH: Years/complexity appropriate to job level expectations
            - DOMAIN EXPERTISE: Industry-specific knowledge if relevant to role
            - Make this section heavily weighted as it's most predictive of job performance

            EXPERIENCE (3-4 criteria max):
            - RELEVANCE: Direct experience in similar roles, companies, or industries
            - PROGRESSION: Career advancement appropriate to level (junior: learning, senior: leadership)
            - SCALE/IMPACT: Team size, project scope, business metrics aligned with role expectations
            - RESPONSIBILITY: Decision-making level and autonomy matching job requirements
            - Consider role level: entry (1-2 yrs), mid (3-5 yrs), senior (5+ yrs)

            PROJECTS (2-3 criteria max):
            - TECHNICAL COMPLEXITY: Sophistication level matching job requirements and seniority
            - BUSINESS IMPACT: Measurable outcomes, user adoption, revenue/cost impact
            - COLLABORATION: Team dynamics, stakeholder management based on role requirements
            - Weight based on whether role is IC (technical depth) or leadership (team impact)

            EDUCATION (2-3 criteria max):
            - FOUNDATION: Degree relevance to role requirements (CS for tech roles, business for PM roles)
            - PERFORMANCE: Academic achievement level appropriate to role competitiveness
            - LEARNING: Continuous education, certifications, courses related to job skills
            - Adjust importance: high for junior roles, moderate for senior (experience matters more)

            CERTIFICATIONS (1-2 criteria max):
            - ROLE RELEVANCE: Certifications directly mentioned in job requirements or industry-standard
            - CURRENCY: Recent certifications showing commitment to staying updated
            - Lower weight unless specifically required in job posting

            SCORING GUIDELINES BY ROLE LEVEL:
            - Entry/Junior (0-2 years): Focus on potential, education, projects, learning ability
            - Mid-level (2-5 years): Balance of proven delivery, technical skills, and growth
            - Senior (5+ years): Leadership evidence, strategic impact, deep expertise, mentoring
            - Lead/Principal (8+ years): Organizational impact, architecture decisions, industry influence

            CRITERION REQUIREMENTS:
            For each criterion, provide:
            - id: "section.keyword" format (e.g., "skills.primary_tech", "exp.leadership")
            - criterion: Specific name reflecting job requirements
            - description: What exactly to evaluate, with job-specific context
            - weight: Decimal 0-1 (must sum to 1.0 per section)
            - scoring: Keys "0" through "5" with specific, measurable descriptions

            SCORING SPECIFICITY:
            Make each score level (0-5) contain specific, observable criteria:
            - Reference exact technologies, years of experience, team sizes, project scales
            - Include measurable outcomes and impact indicators
            - Align with industry standards and role expectations
            - Consider alternative backgrounds and non-traditional paths

            CRITICAL: Tailor every criterion to this specific job. Generic criteria will not effectively evaluate candidates.
            Focus on what predicts success in THIS role at THIS company at THIS level.

            MANDATORY STRUCTURE RULES:
            1. Each section MUST have a "criteria" array property
            2. Each criterion object MUST have: id, criterion, description, weight, scoring
            3. Scoring MUST have keys "0", "1", "2", "3", "4", "5" as strings
            4. Weights in each section MUST sum to 1.0
            5. Maximum items per section: contact(2), summary(3), skills(4), experience(4), projects(3), education(3), certifications(2)

            CRITICAL JSON STRUCTURE REQUIREMENTS:
            You MUST return a JSON object with exactly this structure:
            Json schema:
            {{ $rubricSchema }}

            RESPONSE FORMAT REQUIREMENTS:
            - Return ONLY raw JSON without any markdown formatting
            - Do NOT use ```json or ``` code blocks
            - Do NOT include any explanatory text before or after the JSON
            - Start your response directly with { and end with }
            - Ensure the JSON is properly formatted and valid
            - No additional commentary or explanations

            Return only valid JSON with the exact structure required.
            """;
        }

        public static string GetSectionScoringPrompt()
        {
            return
            """
            You are an expert CV evaluator. Analyze the {{ $sectionName }} section content against the specific section rubric.

            EVALUATION CRITERIA:
            {{ $sectionRubric }}

            SECTION CONTENT TO EVALUATE:
            {{ $sectionContent }}

            SCORING INSTRUCTIONS:
            1. For each criterion in the rubric, assign a score from 0 to 5 (integer not string) based on the CV section content.
            2. Use the scoring descriptions in the rubric as the reference.
            3. Provide output strictly following this JSON schema.
            4. Each criteria id must match the rubric criteria id exactly.
            5. Weight of criteria element must match the rubric criteria weights exactly. (copy weights from rubric)
            6. "justification" must be short, objective, and evidence-based.
            7. "evidenceFound" must contain only direct phrases from the CV.
            8. "missingElements" must contain specific missing skills or evidence.
            9. "totalScore0To5" must be integer 0.
            10. "weight0To1" of section score must match the rubric exactly and an integer. (copy from rubric)

            CRITICAL JSON STRUCTURE REQUIREMENTS:
            You MUST return a JSON object with exactly this structure: (focus on structure, not content)
            Json schema:
            {{ $sectionScoreSchema }}

            BE OBJECTIVE AND SPECIFIC. Base scores on actual evidence in the content.

            RESPONSE FORMAT REQUIREMENTS:
            - Return ONLY raw JSON without any markdown formatting
            - DO NOT USE ```JSON or ``` code blocks
            - DO NOT USE ```JSON or ``` code blocks
            - Do NOT include any explanatory text before or after the JSON
            - Start your response directly with { and end with }
            - Ensure the JSON is properly formatted and valid
            - No additional commentary or explanations

            Return only valid JSON with the exact structure required.
            """;
        }

        private static string GetOverallSummaryPrompt()
        {
            return """
            You are an overall readiness summarizer. Given section feedback map and JD, produce a short summary note (2 sentences max) emphasizing main strengths and one key improvement. Return plain text only.
            
            JD:
            {{ $jd }}
            
            Section Feedback JSON:
            {{ $sections }}
            """;
        }
    }
}
