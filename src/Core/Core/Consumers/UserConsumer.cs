namespace Core.Consumers;

public class UserCreateConsumer : IConsumer<UserCreated>
{
    private readonly IProfileRepository _profileRepository;

    public UserCreateConsumer(IProfileRepository profileRepository)
    {
        _profileRepository = profileRepository;
    }

    public async Task Consume(ConsumeContext<UserCreated> context)
    {
       await _profileRepository.AddAsync(new UserProfile
       {
           Id = Guid.NewGuid(),
           ApplicationUserId = context.Message.UserId,
           Email = context.Message.Email,
           Name = context.Message.Name,
           UserName = context.Message.UserName
       });

       await _profileRepository.SaveChangesAsync();
    }
}
