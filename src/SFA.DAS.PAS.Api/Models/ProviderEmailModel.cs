using System.Text.Json.Serialization;

namespace SFA.DAS.PAS.Api.Models;

public class ProviderEmailModel
{
    // Populated from the route by the controller; never read from the request body.
    [JsonIgnore]
    public int Ukprn { get; set; }

    public required string TemplateId { get; set; }
    public required Dictionary<string, string> Tokens { get; set; } = [];
    public List<string> ExplicitEmailAddresses { get; set; } = [];
}
