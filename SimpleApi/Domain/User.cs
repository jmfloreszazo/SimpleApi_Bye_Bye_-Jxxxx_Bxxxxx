namespace SimpleApi.Domain;

public class User
{
    public Guid Id { get; }
    public string Email { get; }
    public string Name { get; }
    public int Age { get; }

    public User(string email, string name, int age)
    {
        Id = Guid.NewGuid();
        Email = email;
        Name = name;
        Age = age;
    }
}
