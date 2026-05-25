using Microsoft.Data.Sqlite;
using OOPWPFProject.Models.Places;

namespace OOPWPFProject.Services.DB;

public class SQLiteStorage
{
	private readonly string _connectionString;

	public SQLiteStorage(string database)
	{
		ArgumentException.ThrowIfNullOrWhiteSpace(database);
		_connectionString = $"Data Source={database}";
		InitializeDatabase();
	}

	private void InitializeDatabase()
	{
		using var connection = new SqliteConnection(_connectionString);
		connection.Open();
		var createTableQuery =
			@"
        CREATE TABLE IF NOT EXISTS Places (
            Id INTEGER PRIMARY KEY AUTOINCREMENT,
            Type TEXT NOT NULL,
            Name TEXT NOT NULL,
            Country TEXT NOT NULL,
            Description TEXT NOT NULL,
            Rating REAL,
            Date TEXT,
            IsVisited INTEGER NOT NULL DEFAULT 0,
            YearBuilt TEXT,
            Significance INTEGER,
            YearFormed TEXT,
            ProtectedStatus INTEGER
        )";
		using var command = new SqliteCommand(createTableQuery, connection);
		command.ExecuteNonQuery();
	}

	public async Task<int> InsertAsync(Place place)
	{
		var dto = PlaceDto.FromPlace(place);

		using var connection = new SqliteConnection(_connectionString);
		await connection.OpenAsync();

		var insertQuery =
			@"
        INSERT INTO Places (
            Type, Name, Country, Description, Rating, Date,
            IsVisited, YearBuilt, Significance, YearFormed, ProtectedStatus
        ) VALUES (
            @Type, @Name, @Country, @Description, @Rating, @Date,
            @IsVisited, @YearBuilt, @Significance, @YearFormed, @ProtectedStatus
        );
        SELECT last_insert_rowid();";

		using var command = new SqliteCommand(insertQuery, connection);
		BindDto(command, dto, isIdIncluded: false);

		var result = await command.ExecuteScalarAsync();
		var newId = Convert.ToInt32(result);
		place.Id = newId;

		return newId;
	}

	public async Task<List<Place>> LoadAsync()
	{
		var dtos = new List<PlaceDto>();
		using var connection = new SqliteConnection(_connectionString);
		await connection.OpenAsync();

		var selectQuery =
			@"
        SELECT Id, Type, Name, Country, Description, Rating, Date,
               IsVisited, YearBuilt, Significance, YearFormed, ProtectedStatus
        FROM Places";

		using var command = new SqliteCommand(selectQuery, connection);
		using SqliteDataReader reader = await command.ExecuteReaderAsync();

		while (await reader.ReadAsync())
		{
			dtos.Add(
				new PlaceDto
				{
					Id = reader.GetInt32(0),
					Type = Enum.TryParse<PlaceType>(reader.GetString(1), out PlaceType type)
						? type
						: PlaceType.Normal,
					Name = reader.GetString(2),
					Country = reader.GetString(3),
					Description = reader.GetString(4),
					Rating = reader.IsDBNull(5) ? null : reader.GetDouble(5),
					Date = reader.IsDBNull(6) ? null : DateOnly.Parse(reader.GetString(6)),
					IsVisited = reader.GetInt32(7) == 1,
                    YearBuilt = reader.IsDBNull( 8 ) ? null : reader.GetString( 8 ),
                    Significance = reader.IsDBNull(9) ? null : reader.GetInt32(9),
                    YearFormed = reader.IsDBNull( 10 ) ? null : reader.GetString( 10 ),
                    ProtectedStatus = reader.IsDBNull(11) ? null : reader.GetInt32(11) == 1,
				}
			);
		}

		return dtos.Select(d => d.ToPlace()).ToList();
	}

    public async Task UpdateAsync( Place place )
    {
        var dto = PlaceDto.FromPlace(place);

        using var connection = new SqliteConnection(_connectionString);
        await connection.OpenAsync();

        var updateQuery = @"
        UPDATE Places SET 
            Type = @Type, Name = @Name, Country = @Country, Description = @Description,
            Rating = @Rating, Date = @Date, IsVisited = @IsVisited,
            YearBuilt = @YearBuilt, Significance = @Significance, YearFormed = @YearFormed,
            ProtectedStatus = @ProtectedStatus
        WHERE Id = @Id";

        using var command = new SqliteCommand(updateQuery, connection);

        BindDto( command, dto, isIdIncluded: true );
        var rowsAffected = await command.ExecuteNonQueryAsync();

        if ( rowsAffected == 0 )
        {
            var errorMessage = $"Запис з Id {place.Id} не знайдено в базі даних";
            Logger.Log( LogLevel.Error, errorMessage );

            throw new InvalidOperationException( errorMessage );
        }
    }

    public async Task DeleteAsync(int id)
	{
		using var connection = new SqliteConnection(_connectionString);
		await connection.OpenAsync();

		var deleteQuery = "DELETE FROM Places WHERE Id = @Id";
		using var command = new SqliteCommand(deleteQuery, connection);
		command.Parameters.AddWithValue("@Id", id);

		await command.ExecuteNonQueryAsync();
	}

	private static void BindDto(SqliteCommand cmd, PlaceDto dto, bool isIdIncluded)
	{
		if (isIdIncluded)
		{
			cmd.Parameters.AddWithValue("@Id", dto.Id);
		}

		cmd.Parameters.AddWithValue("@Type", dto.Type.ToString());
		cmd.Parameters.AddWithValue("@Name", dto.Name);
		cmd.Parameters.AddWithValue("@Country", dto.Country);
		cmd.Parameters.AddWithValue("@Description", dto.Description);
		cmd.Parameters.AddWithValue("@Rating", dto.Rating ?? (object)DBNull.Value);
		cmd.Parameters.AddWithValue(
			"@Date",
			dto.Date?.ToString("yyyy-MM-dd") ?? (object)DBNull.Value
		);

		cmd.Parameters.AddWithValue("@IsVisited", dto.IsVisited ? 1 : 0);

		cmd.Parameters.AddWithValue(
			"@ProtectedStatus",
			dto.ProtectedStatus.HasValue ? (dto.ProtectedStatus.Value ? 1 : 0) : DBNull.Value
		);
        cmd.Parameters.AddWithValue( "@YearBuilt", dto.YearBuilt ?? (object) DBNull.Value );
        cmd.Parameters.AddWithValue("@Significance", dto.Significance ?? (object)DBNull.Value);
        cmd.Parameters.AddWithValue( "@YearFormed", dto.YearFormed ?? (object) DBNull.Value );
    }
}
