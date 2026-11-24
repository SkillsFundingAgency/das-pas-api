namespace SFA.DAS.PAS.Api.Models;

public class ProviderEmailModel
{
    public required string TemplateId { get; set; }
    public required Dictionary<string, string> Tokens { get; set; } = [];
    public List<string> ExplicitEmailAddresses { get; set; } = [];
}
