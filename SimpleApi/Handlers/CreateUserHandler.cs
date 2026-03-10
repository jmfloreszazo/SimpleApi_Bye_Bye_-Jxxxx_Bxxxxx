using SimpleApi.Domain;
using SimpleApi.Infrastructure;

namespace SimpleApi.Handlers;

public class CreateUserHandler
{
    public User Handle(User user)
    {
        FakeDatabase.Users.Add(user);
        return user;
    }
}
