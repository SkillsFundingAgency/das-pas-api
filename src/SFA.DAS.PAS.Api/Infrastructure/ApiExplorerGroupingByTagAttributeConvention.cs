using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace SFA.DAS.PAS.Api.Infrastructure;

public class ApiExplorerGroupingByTagAttributeConvention : IActionModelConvention
{
    public void Apply(ActionModel action)
    {
        var tagAttribute = action.Controller.Attributes.OfType<TagsAttribute>().FirstOrDefault();
        if (tagAttribute != null)
        {
            action.ApiExplorer.GroupName = tagAttribute.Tags.Contains(ApiTags.Integration) ? ApiTags.Integration : ApiTags.Management;
        }
    }
}
