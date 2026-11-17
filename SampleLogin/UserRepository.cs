using Npgsql;
using NpgsqlTypes;

public class UserRepository
{
    private readonly NpgsqlDataSource _dataSource;

    public UserRepository(NpgsqlDataSource dataSource) => _dataSource = dataSource;

    public (int Id, string PasswordHash)? GetByUsername(string username)
    {
        using var cmd = _dataSource.CreateCommand(
            "SELECT id, password_hash FROM users WHERE username = @username");
        cmd.Parameters.Add(new("username", NpgsqlDbType.Varchar, 50) { Value = username });

        using var reader = cmd.ExecuteReader();
        if (!reader.Read()) return null;

        return (reader.GetInt32(0), reader.GetString(1));
    }

    public int Create(string username, string passwordHash)
    {
        using var cmd = _dataSource.CreateCommand(
            "INSERT INTO users (username, password_hash) VALUES (@username, @hash) RETURNING id");
        cmd.Parameters.Add(new("username", NpgsqlDbType.Varchar, 50) { Value = username });
        cmd.Parameters.Add(new("hash", NpgsqlDbType.Text) { Value = passwordHash });

        return Convert.ToInt32(cmd.ExecuteScalar());
    }

    public void UpdateHash(int id, string passwordHash)
    {
        using var cmd = _dataSource.CreateCommand(
            "UPDATE users SET password_hash = @hash WHERE id = @id");
        cmd.Parameters.Add(new("hash", NpgsqlDbType.Text) { Value = passwordHash });
        cmd.Parameters.Add(new("id", NpgsqlDbType.Integer) { Value = id });

        cmd.ExecuteNonQuery();
    }
}
