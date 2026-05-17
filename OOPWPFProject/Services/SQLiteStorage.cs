using Microsoft.Data.Sqlite;

using OOPWPFProject.Models;
using OOPWPFProject.Models.Storage;

namespace OOPWPFProject.Services;

public class SQLiteStorage
{
    private readonly string _database;
    private readonly string _connectionString;

    public SQLiteStorage(string database)
    {
        _database = database ?? throw new ArgumentNullException(nameof(database));
        _connectionString = $"Data Source={_database}";
        InitializeDatabase();
    }

    private void InitializeDatabase()
    {
        using SqliteConnection connection = new SqliteConnection(_connectionString);
        connection.Open();
        string createTableQuery = @"
        CREATE TABLE IF NOT EXISTS Places (
            Id INTEGER PRIMARY KEY AUTOINCREMENT
            ,Type TEXT NOT NULL
            ,Name TEXT NOT NULL
            ,Country TEXT NOT NULL
            ,Description TEXT NOT NULL
            ,Rating REAL
            ,Date TEXT
            ,Review TEXT
            ,Notes TEXT
            ,ReviewsJson TEXT
            ,YearBuiltHistorical TEXT
            ,Significance INTEGER
            ,YearBuiltNatural TEXT
            ,ProtectedStatus INTEGER
            ,IconId TEXT
            ,WeatherSummary TEXT
            ,WeatherIconPath TEXT
            ,WikiImageUrl TEXT
        )";
        using SqliteCommand command = new SqliteCommand(createTableQuery, connection);
        command.ExecuteNonQuery();
    }

    public void Insert(Place place)
    {
        var dto = PlaceDto.FromPlace(place);

        using SqliteConnection connection = new SqliteConnection(_connectionString);
        connection.Open();

        string insertQuery = @"
        INSERT INTO Places (Type, Name, Country, Description
            ,Rating, Date, Review, Notes, ReviewsJson
            ,YearBuiltHistorical, Significance
            ,YearBuiltNatural, ProtectedStatus
            ,IconId, WeatherSummary, WeatherIconPath, WikiImageUrl)
        VALUES (@Kind, @Name, @Country, @Description
            ,@Rating, @Date, @Review, @Notes, @ReviewsJson
            ,@YearBuiltHistorical, @Significance
            ,@YearBuiltNatural, @ProtectedStatus
            ,@IconId, @WeatherSummary, @WeatherIconPath, @WikiImageUrl)";

        using SqliteCommand command = new SqliteCommand(insertQuery, connection);
        BindDto(command, dto);
        command.ExecuteNonQuery();
    }

    public List<Place> Load()
    {
        var dtos = new List<PlaceDto>();
        using SqliteConnection connection = new SqliteConnection(_connectionString);
        connection.Open();

        string selectQuery = @"
        SELECT Id,Type
        ,Name, Country, Description
        ,Rating, Date
        ,Review, Notes, ReviewsJson
        ,YearBuiltHistorical, Significance
        ,YearBuiltNatural, ProtectedStatus
        FROM Places";

        using SqliteCommand command = new SqliteCommand(selectQuery, connection);
        using SqliteDataReader reader = command.ExecuteReader();

        while (reader.Read())
        {
            dtos.Add(new PlaceDto
            {
                // id
                Type = Enum.Parse<PlaceType>(reader.GetString(1)),
                Name = reader.GetString(2),
                Country = reader.GetString(3),
                Description = reader.GetString(4),
                Rating = reader.IsDBNull(5) ? null : Convert.ToInt32(reader.GetDouble(5)),
                Date = reader.IsDBNull(6) ? null : DateOnly.Parse(reader.GetString(6)),
                Review = reader.IsDBNull(7) ? null : reader.GetString(7),
                Notes = reader.IsDBNull(8) ? null : reader.GetString(8),
                ReviewsJson = reader.IsDBNull(9) ? "[]" : reader.GetString(9),
                YearBuilt = reader.IsDBNull(10) ? null : DateOnly.Parse(reader.GetString(10)),
                Significance = reader.IsDBNull(11) ? null : reader.GetInt32(11),
                YearFormed = reader.IsDBNull(12) ? null : DateOnly.Parse(reader.GetString(12)),
                ProtectedStatus = reader.IsDBNull(13) ? null : reader.GetInt32(13) == 1
            });
        }

        return dtos.Select(d => d.ToPlace()).ToList();
    }

    public void Update(Place place)
    {
        var dto = PlaceDto.FromPlace(place);

        using SqliteConnection connection = new SqliteConnection(_connectionString);
        connection.Open();

        string updateQuery = @"
                UPDATE Places
                SET Country = @Country
                   ,Description = @Description
                   ,Rating = @Rating
                   ,Date = @Date
                   ,Review = @Review
                   ,Notes = @Notes
                   ,ReviewsJson = @ReviewsJson
                   ,YearBuiltHistorical = @YearBuiltHistorical
                   ,Significance = @Significance
                   ,YearBuiltNatural = @YearBuiltNatural
                   ,ProtectedStatus = @ProtectedStatus
                WHERE Name = @Name"; // WHERE Id = @Id

        using SqliteCommand command = new SqliteCommand(updateQuery, connection);
        BindDto(command, dto);
        command.ExecuteNonQuery();
    }

    public void Delete(string name)
    {
        using SqliteConnection connection = new SqliteConnection(_connectionString);
        connection.Open();

        string deleteQuery = "DELETE FROM Places WHERE Name = @Name"; //  WHERE Id = @Id

        using SqliteCommand command = new SqliteCommand(deleteQuery, connection);
        command.Parameters.AddWithValue("@Name", name);
        command.ExecuteNonQuery();
    }

    private static void BindDto(SqliteCommand cmd, PlaceDto dto)
    {
        cmd.Parameters.AddWithValue("@Kind", dto.Type.ToString());
        cmd.Parameters.AddWithValue("@Name", dto.Name);
        cmd.Parameters.AddWithValue("@Country", dto.Country);
        cmd.Parameters.AddWithValue("@Description", dto.Description);
        cmd.Parameters.AddWithValue("@Rating", (object?)dto.Rating ?? DBNull.Value);

        cmd.Parameters.AddWithValue("@Date", dto.Date.HasValue ? dto.Date.Value.ToString("O") : DBNull.Value);

        cmd.Parameters.AddWithValue("@Review", (object?)dto.Review ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@Notes", (object?)dto.Notes ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@ReviewsJson", (object?)dto.ReviewsJson ?? DBNull.Value);

        cmd.Parameters.AddWithValue("@YearBuilt", dto.YearBuilt.HasValue ? dto.YearBuilt.Value.ToString("O") : DBNull.Value);
        cmd.Parameters.AddWithValue("@Significance", dto.Significance.HasValue ? dto.Significance : DBNull.Value);
        cmd.Parameters.AddWithValue("@YearFormed", dto.YearFormed.HasValue ? dto.YearFormed.Value.ToString("O") : DBNull.Value);
        cmd.Parameters.AddWithValue("@ProtectedStatus", (object?)dto.ProtectedStatus ?? DBNull.Value);

        cmd.Parameters.AddWithValue("@IconId", DBNull.Value);
        cmd.Parameters.AddWithValue("@WeatherSummary", DBNull.Value);
        cmd.Parameters.AddWithValue("@WeatherIconPath", DBNull.Value);
        cmd.Parameters.AddWithValue("@WikiImageUrl", DBNull.Value);
    }
}
