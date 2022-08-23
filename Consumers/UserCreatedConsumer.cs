using MassTransit;
using Play.Common.Service.Repositories;
using Play.User.Contracts;

namespace Play.Inventory.Service.Consumers;

public class UserCreatedConsumer : IConsumer<UserCreated>
{
    private readonly IRepository<Entities.User> _repository;

    public UserCreatedConsumer(IRepository<Entities.User> repository)
    {
        _repository = repository;
    }

    public async Task Consume(ConsumeContext<UserCreated> context)
    {
        var message = context.Message;

        var user = await _repository.GetAsync(message.UserId);

        if (user != null)
        {
            return;
        }

        user = new Entities.User
        {
            Id = user.Id,
            FirstName = user.FirstName,
            LastName = user.LastName,
            UserName = user.UserName,
            Email = user.Email
        };

        await _repository.CreateAsync(user);
    }
}
