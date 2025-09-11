using ServicePerfectCV.Application.Interfaces.AI;

namespace ServicePerfectCV.Infrastructure.Services.AI.SemanticKernel;

public sealed class AIOrchestrator : IAIOrchestrator
{
    private readonly IPromptService _prompter;
    private readonly CvReviewerPlugin _plugin;

    public AIOrchestrator(IPromptService prompter, CvReviewerPlugin plugin)
    {
        _prompter = prompter;
        _plugin = plugin;
    }

    public async Task<string> ReviewCvAgainstJdAsync(string cvText, string jdText, CancellationToken ct = default)
    {
        var pre = _plugin.ReviewCv(cvText, jdText);
        var prompt = """
        Bạn là chuyên gia HR. Dựa trên CV và JD dưới đây, hãy:
        - Tóm tắt điểm mạnh/điểm yếu của CV so với JD
        - Gợi ý chỉnh sửa cụ thể theo từng mục (Summary/Education/Skills/Projects/Experience)
        - Đánh giá CV theo thang 0-100 (ghi con số cuối)
        - Viết súc tích, rõ ràng

        [Tiền xử lý]
        {{pre}}

        [CV]
        {{cv}}

        [JD]
        {{jd}}
        """;
        return await _prompter.CompleteAsync(prompt, new Dictionary<string, string>
        {
            ["pre"] = pre,
            ["cv"] = cvText,
            ["jd"] = jdText
        }, ct);
    }
}
