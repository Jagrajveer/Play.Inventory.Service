using MassTransit;
using Play.Common.Service.Repositories;
using Play.User.Contracts;

namespace Play.Inventory.Service.Consumers;

public class UserUpdatedConsumer : IConsumer<UserUpdated>
{

    private readonly IRepository<Entities.User> _repository;

    public UserUpdatedConsumer(IRepository<Entities.User> repository)
    {
        _repository = repository;
    }

    public async Task Consume(ConsumeContext<UserUpdated> context)
    {
        var message = context.Message;

        var user = await _repository.GetAsync(message.UserId);

        if (user == null)
        {
            await _repository.CreateAsync(new Entities.User
            {
                Id = message.UserId,
                FirstName = message.FirstName,
                LastName = message.LastName,
                UserName = message.UserName,
                Email = message.Email
            });
        } else
        {
            user.FirstName = message.FirstName;
            user.LastName = message.LastName;
            user.UserName = message.UserName;
            user.Email = message.Email;

            await _repository.UpdateAsync(user);
        }
    }
}
