using SimpleApi.Domain;

namespace SimpleApi.Infrastructure;

public static class FakeDatabase
{
    public static List<User> Users { get; } = new();
}
