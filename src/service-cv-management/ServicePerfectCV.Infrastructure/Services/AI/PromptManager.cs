using ServicePerfectCV.Infrastructure.Constants;

namespace ServicePerfectCV.Infrastructure.Services.AI
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
                PromptType.CvSectionExtraction => GetCvSectionExtractionPrompt(),
                _ => throw new ArgumentException($"Unknown prompt type: {promptType}")
            };
        }

        private static string GetSectionRubricBuildingPrompt()
        {
            return """
            You are an expert recruiter creating detailed evaluation criteria for CV assessment. 
            Analyze the job description deeply and create specific, measurable criteria tailored to this exact role.

            Job Title: {{ $title }}
            Company: {{ $company }}
            Responsibilities: {{ $responsibility }}
            Qualifications: {{ $qualification }}

            ANALYSIS INSTRUCTIONS:
            1. Extract key technical skills, tools, and technologies mentioned in responsibilities and qualifications
            2. Identify experience level expectations from the job title and qualifications
            3. Determine industry context and domain-specific needs from company and role description
            4. Identify soft skills and collaboration requirements from responsibilities
            5. Assess education and certification priorities based on qualifications

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
            - PRIMARY TECHNICAL SKILLS: Extract exact technologies/languages from responsibilities and qualifications
            - SECONDARY TECHNICAL SKILLS: Related tools, frameworks, methodologies mentioned
            - EXPERIENCE DEPTH: Years/complexity appropriate to role expectations from title and qualifications
            - DOMAIN EXPERTISE: Industry-specific knowledge if relevant to role
            - Make this section heavily weighted as it's most predictive of job performance

            EXPERIENCE (3-4 criteria max):
            - RELEVANCE: Direct experience in similar roles, companies, or industries
            - PROGRESSION: Career advancement appropriate to seniority level inferred from title and qualifications
            - SCALE/IMPACT: Team size, project scope, business metrics aligned with responsibilities
            - RESPONSIBILITY: Decision-making level and autonomy matching job responsibilities
            - Infer experience level from job title: entry/junior (1-2 yrs), mid (3-5 yrs), senior (5+ yrs)

            PROJECTS (2-3 criteria max):
            - TECHNICAL COMPLEXITY: Sophistication level matching job responsibilities and seniority
            - BUSINESS IMPACT: Measurable outcomes, user adoption, revenue/cost impact
            - COLLABORATION: Team dynamics, stakeholder management based on role responsibilities
            - Weight based on whether role is IC (technical depth) or leadership (team impact)

            EDUCATION (2-3 criteria max):
            - FOUNDATION: Degree relevance to role qualifications (CS for tech roles, business for PM roles)
            - PERFORMANCE: Academic achievement level appropriate to role competitiveness
            - LEARNING: Continuous education, certifications, courses related to job skills
            - Adjust importance: high for junior roles, moderate for senior (experience matters more)

            CERTIFICATIONS (1-2 criteria max):
            - ROLE RELEVANCE: Certifications directly mentioned in qualifications or industry-standard
            - CURRENCY: Recent certifications showing commitment to staying updated
            - Lower weight unless specifically required in qualifications

            SCORING GUIDELINES BY ROLE LEVEL:
            - Entry/Junior (0-2 years): Focus on potential, education, projects, learning ability
            - Mid-level (2-5 years): Balance of proven delivery, technical skills, and growth
            - Senior (5+ years): Leadership evidence, strategic impact, deep expertise, mentoring
            - Lead/Principal (8+ years): Organizational impact, architecture decisions, industry influence

            CRITERION REQUIREMENTS:
            For each criterion, provide JSON with EXACT property names:
            - "id": "section.keyword" format (e.g., "skills.primary_tech", "exp.leadership")
            - "criterion": Specific name reflecting job requirements
            - "description": What exactly to evaluate, with job-specific context
            - "weight0To1": Decimal 0-1 (must sum to 1.0 per section) - MUST use "weight0To1" as property name
            - "scoring": Object with keys "0" through "5" as strings with specific, measurable descriptions

            SCORING SPECIFICITY:
            Make each score level (0-5) contain specific, observable criteria:
            - Reference exact technologies, years of experience, team sizes, project scales
            - Include measurable outcomes and impact indicators
            - Align with industry standards and role expectations
            - Consider alternative backgrounds and non-traditional paths

            CRITICAL: Tailor every criterion to this specific job. Generic criteria will not effectively evaluate candidates.
            Focus on what predicts success in THIS role at THIS company at THIS level.

            MANDATORY STRUCTURE RULES:
            1. Each section MUST be an array of criterion objects
            2. Each criterion object MUST have EXACT property names: "id", "criterion", "description", "weight0To1", "scoring"
            3. "scoring" MUST be an object with keys "0", "1", "2", "3", "4", "5" as strings
            4. "weight0To1" values in each section MUST sum to 1.0
            5. Maximum items per section: contact(2), summary(3), skills(4), experience(4), projects(3), education(3), certifications(2)
            6. CRITICAL: Use "weight0To1" NOT "weight" as the property name

            CRITICAL JSON STRUCTURE REQUIREMENTS:
            You MUST return a JSON object with exactly this structure:
            Json schema:
            {{ $rubricSchema }}

            EXAMPLE STRUCTURE (use "weight0To1" NOT "weight"):
            {
              "Contact": [
                {
                  "id": "contact.professional_presentation",
                  "criterion": "Professional Presentation",
                  "description": "Assesses professionalism of contact details",
                  "weight0To1": 1.0,
                  "scoring": {
                    "0": "Unprofessional contact info",
                    "1": "Basic contact info",
                    "2": "Standard contact details",
                    "3": "Professional contact details",
                    "4": "Well-structured professional contact",
                    "5": "Exemplary professional presentation"
                  }
                }
              ]
            }

            CRITICAL RESPONSE FORMAT REQUIREMENTS - VIOLATION WILL BREAK THE SYSTEM:
            - RETURN ONLY RAW JSON TEXT - NO MARKDOWN FORMATTING WHATSOEVER
            - DO NOT USE ```json OR ``` CODE BLOCKS UNDER ANY CIRCUMSTANCES
            - DO NOT INCLUDE ANY TEXT BEFORE OR AFTER THE JSON
            - START YOUR RESPONSE WITH { AND END WITH } - NOTHING ELSE
            - DO NOT ADD EXPLANATIONS, COMMENTS, OR ANY OTHER TEXT
            - MUST USE "weight0To1" AS PROPERTY NAME, NOT "weight"
            - DO NOT RETURN THE JSON SCHEMA ITSELF - YOU MUST RETURN THE ACTUAL RUBRIC DATA
            - YOUR RESPONSE MUST NOT CONTAIN "$schema", "$defs", or schema definition properties
            - YOU MUST CREATE AND RETURN THE ACTUAL EVALUATION RUBRIC, NOT THE SCHEMA

            FAILURE TO FOLLOW THESE INSTRUCTIONS WILL CAUSE SYSTEM ERRORS.

            Return only valid JSON with the actual rubric data (not the schema). No code blocks, no markdown, no explanations.
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
            5. "weight0To1" of criteria element must match the rubric criteria weights exactly. (copy weights from rubric)
            6. "justification" must be short, objective, and evidence-based.
            7. "evidenceFound" must contain only direct phrases from the CV.
            8. "missingElements" must contain specific missing skills or evidence.
            9. "totalScore0To5" must be integer 0.
            10. "weight0To1" of section score must match the rubric exactly and be a decimal. (copy from rubric)
            11. CRITICAL: Use "weight0To1" as property name, NOT "weight"

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

        private static string GetCvSectionExtractionPrompt()
        {
            return """
            You are an expert CV parser. Extract and structure all information from the provided CV raw text into the specified JSON format.

            CV RAW TEXT:
            {{ $rawText }}

            EXTRACTION INSTRUCTIONS:
            1. Carefully read through the entire CV text
            2. Extract all information for each section based on the schema below
            3. Maintain accuracy - only extract information that is explicitly stated
            4. Parse dates in ISO format (YYYY-MM-DD) when possible
            5. If a section is not present in the CV, return null or empty array for that section
            6. For skills, categorize them logically (e.g., "Programming Languages", "Frameworks", "Tools")

            CRITICAL JSON STRUCTURE REQUIREMENTS:
            You MUST return a JSON object with exactly this structure:
            {
              "contact": {
                "phoneNumber": "string or null",
                "email": "string or null",
                "linkedInUrl": "string or null",
                "gitHubUrl": "string or null",
                "personalWebsiteUrl": "string or null",
                "country": "string or null",
                "city": "string or null"
              },
              "summary": {
                "content": "string or null"
              },
              "education": [
                {
                  "organization": "string",
                  "degree": "string",
                  "fieldOfStudy": "string or null",
                  "startDate": "YYYY-MM-DD or null",
                  "endDate": "YYYY-MM-DD or null",
                  "description": "string or null",
                  "gpa": "string or null"
                }
              ],
              "experience": [
                {
                  "jobTitle": "string",
                  "employmentType": "Full-time|Part-time|Contract|Internship or null",
                  "organization": "string",
                  "location": "string or null",
                  "startDate": "YYYY-MM-DD or null",
                  "endDate": "YYYY-MM-DD or null",
                  "description": "string or null"
                }
              ],
              "skills": [
                {
                  "category": "string (e.g., Programming Languages, Frameworks)",
                  "content": "string (comma-separated skills)"
                }
              ],
              "projects": [
                {
                  "title": "string",
                  "description": "string or null",
                  "link": "string or null",
                  "startDate": "YYYY-MM-DD or null",
                  "endDate": "YYYY-MM-DD or null"
                }
              ],
              "certifications": [
                {
                  "name": "string",
                  "organization": "string or null",
                  "issuedDate": "YYYY-MM-DD or null",
                  "description": "string or null"
                }
              ]
            }

            CRITICAL RESPONSE FORMAT REQUIREMENTS:
            - RETURN ONLY RAW JSON TEXT - NO MARKDOWN FORMATTING WHATSOEVER
            - DO NOT USE ```json OR ``` CODE BLOCKS UNDER ANY CIRCUMSTANCES
            - DO NOT INCLUDE ANY TEXT BEFORE OR AFTER THE JSON
            - START YOUR RESPONSE WITH { AND END WITH } - NOTHING ELSE
            - DO NOT ADD EXPLANATIONS, COMMENTS, OR ANY OTHER TEXT
            - ENSURE ALL DATES ARE IN ISO FORMAT (YYYY-MM-DD)
            - USE null FOR MISSING VALUES, NOT EMPTY STRINGS (except for arrays which should be [])

            FAILURE TO FOLLOW THESE INSTRUCTIONS WILL CAUSE SYSTEM ERRORS.

            Return only valid JSON with the exact structure required. No code blocks, no markdown, no explanations.
            """;
        }
    }
}