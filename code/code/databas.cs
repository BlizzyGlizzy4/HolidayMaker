using MySqlConnector;

public static class Db
{
    private const string ConnectionString =
        "Server=127.0.0.1;Port=3306;Database=holidaymaker;User ID=root;Password=kurslosenord;";

    public static MySqlConnection Open()
    {
        var conn = new MySqlConnection(ConnectionString);
        conn.Open();
        return conn;
    }
}