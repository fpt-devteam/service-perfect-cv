using System.ComponentModel;
using Microsoft.SemanticKernel;

namespace ServicePerfectCV.Infrastructure.Services.AI.SemanticKernel;

public sealed class CvReviewerPlugin
{
    [KernelFunction("review_cv")]
    [Description("Đánh giá CV theo JD, trả về nhận xét ngắn và số điểm 0-100 (demo).")]
    public string ReviewCv(string cv, string jd)
    {
        return $"(Demo pre-check) cv_len={cv.Length}, jd_len={jd.Length}";
    }
}
