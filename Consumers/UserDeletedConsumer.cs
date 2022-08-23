using MassTransit;
using Play.Common.Service.Repositories;
using Play.User.Contracts;

namespace Play.Inventory.Service.Consumers;

public class UserDeletedConsumer : IConsumer<UserDeleted>
{

    private readonly IRepository<Entities.User> _repository;

    public UserDeletedConsumer(IRepository<Entities.User> repository)
    {
        _repository = repository;
    }

    public async Task Consume(ConsumeContext<UserDeleted> context)
    {
        var message = context.Message;

        var user = await _repository.GetAsync(message.UserId);

        if (user == null)
        {
            return;
        }

        await _repository.DeleteAsync(user.Id);
    }
}
