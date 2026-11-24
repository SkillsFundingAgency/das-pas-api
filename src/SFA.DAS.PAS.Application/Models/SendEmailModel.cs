namespace SFA.DAS.PAS.Application.Models;

public class SendEmailModel
{
    public required int Ukprn { get; set; }
    public required string TemplateId { get; set; }
    public required Dictionary<string, string> Tokens { get; set; } = [];
    public IEnumerable<string> ExplicitEmailAddresses { get; set; } = [];
}
