using NServiceBus;
using SFA.DAS.Notifications.Messages.Commands;
using SFA.DAS.PAS.Application.Models;

namespace SFA.DAS.PAS.Application.Services;

public interface IEmailService
{
    Task SendEmailToAllProviderRecipients(SendEmailModel model, CancellationToken cancellationToken);
}

internal class EmailService(IUserService _userService, IMessageSession _messageSession) : IEmailService
{
    public async Task SendEmailToAllProviderRecipients(SendEmailModel model, CancellationToken cancellationToken)
    {
        IEnumerable<UserModel> users = await GetUsersWithEmails(model, cancellationToken);

        var normalUsers = users.Where(user => !user.IsSuperUser);
        var optedOutEmailAddresses = users
            .Where(user => !user.ReceiveNotifications)
            .Select(user => user.EmailAddress)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        IEnumerable<string> recipients;
        if (model.ExplicitEmailAddresses.Any())
        {
            recipients = model.ExplicitEmailAddresses.Where(email => !optedOutEmailAddresses.Contains(email));
        }
        else
        {
            var providerUsers = normalUsers.Any() ? normalUsers : users;
            recipients = providerUsers
                .Where(user => user.ReceiveNotifications && !string.IsNullOrWhiteSpace(user.EmailAddress))
                .Select(user => user.EmailAddress);
        }

        var tasks = recipients
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .Select(email => _messageSession.Send(new SendEmailCommand(model.TemplateId, email, model.Tokens)));

        await Task.WhenAll(tasks);
    }

    private async Task<IEnumerable<UserModel>> GetUsersWithEmails(SendEmailModel model, CancellationToken cancellationToken)
    {
        var users = await _userService.GetUsersByUkprn(model.Ukprn, cancellationToken);
        users = users.Where(user => !string.IsNullOrWhiteSpace(user.EmailAddress));
        return users;
    }
}
