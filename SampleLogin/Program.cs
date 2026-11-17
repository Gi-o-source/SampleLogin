using Microsoft.Extensions.Configuration;
using Npgsql;

var config = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json")
    .Build();

string connString = config.GetConnectionString("Postgres")!;

using var dataSource = NpgsqlDataSource.Create(connString);
var repo = new UserRepository(dataSource);

Console.WriteLine("1) Register   2) Login");
string? choice = Console.ReadLine();


if (choice == "1")
{
    Console.Write("Username: ");
    string username = Console.ReadLine()!.Trim();

    Console.Write("Password: ");
    string password = ReadPassword()!;

    if (repo.GetByUsername(username) is not null)
    {
        Console.WriteLine("That username is already taken.");
        return;
    }

    string hash = PasswordHasher.Hash(password);
    int id = repo.Create(username, hash);
    Console.WriteLine($"Registered user {username} with id {id}.");
}
else {
    Console.Write("Username: ");
    string username = Console.ReadLine()!.Trim();

    Console.Write("Password: ");
    string password = ReadPassword()!;

    var user = repo.GetByUsername(username);

    if (user is null || !PasswordHasher.Verify(password, user.Value.PasswordHash)) {
        Console.WriteLine("Invalid username or password");
        return;
    }

    Console.WriteLine($"Welcome back, {username}.");

}

Console.WriteLine("Press any key to exit...");
Console.ReadKey();


static string ReadPassword()
{
    var chars = new List<char>();
    while (true)
    {
        var key = Console.ReadKey(intercept: true);
        if (key.Key == ConsoleKey.Enter) { Console.WriteLine(); break; }
        if (key.Key == ConsoleKey.Backspace)
        {
            if (chars.Count > 0) chars.RemoveAt(chars.Count - 1);
            continue;
        }
        chars.Add(key.KeyChar);
    }
    return new string(chars.ToArray());
}

