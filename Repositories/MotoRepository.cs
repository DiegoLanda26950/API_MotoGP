using Microsoft.Data.SqlClient;

namespace MotoGP_API.Repositories
{
    public class MotoRepository : IMotoRepository
    {
        private readonly string _connectionString;

        public MotoRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("MotoGPDB") ?? "Not found";
        }

        // Obtiene todas las motos de la base de datos
        public async Task<List<Moto>> GetAllAsync()
        {
            var motos = new List<Moto>();
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            using var command = new SqlCommand(
                "SELECT Id, Marca, Modelo, Cilindrada, Potencia, Peso, Anio, Color, ImagenUrl, ImagenPublicId FROM Moto", connection);
            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
                motos.Add(MapMoto(reader));
            return motos;
        }

        // Obtiene una moto por su ID
        public async Task<Moto?> GetByIdAsync(int id)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            using var command = new SqlCommand(
                "SELECT Id, Marca, Modelo, Cilindrada, Potencia, Peso, Anio, Color, ImagenUrl, ImagenPublicId FROM Moto WHERE Id = @Id", connection);
            command.Parameters.AddWithValue("@Id", id);
            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
                return MapMoto(reader);
            return null;
        }

        // Inserta una nueva moto en la base de datos
        public async Task AddAsync(Moto moto)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            using var command = new SqlCommand(
                "INSERT INTO Moto (Marca, Modelo, Cilindrada, Potencia, Peso, Anio, Color, ImagenUrl, ImagenPublicId) VALUES (@Marca, @Modelo, @Cilindrada, @Potencia, @Peso, @Anio, @Color, @ImagenUrl, @ImagenPublicId)", connection);
            command.Parameters.AddWithValue("@Marca", moto.Marca);
            command.Parameters.AddWithValue("@Modelo", moto.Modelo);
            command.Parameters.AddWithValue("@Cilindrada", moto.Cilindrada);
            command.Parameters.AddWithValue("@Potencia", moto.Potencia);
            command.Parameters.AddWithValue("@Peso", moto.Peso);
            command.Parameters.AddWithValue("@Anio", moto.Anio);
            command.Parameters.AddWithValue("@Color", moto.Color);
            command.Parameters.AddWithValue("@ImagenUrl", (object?)moto.ImagenUrl ?? DBNull.Value);
            command.Parameters.AddWithValue("@ImagenPublicId", (object?)moto.ImagenPublicId ?? DBNull.Value);
            await command.ExecuteNonQueryAsync();
        }

        // Actualiza una moto existente en la base de datos
        public async Task UpdateAsync(Moto moto)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            using var command = new SqlCommand(
                "UPDATE Moto SET Marca=@Marca, Modelo=@Modelo, Cilindrada=@Cilindrada, Potencia=@Potencia, Peso=@Peso, Anio=@Anio, Color=@Color, ImagenUrl=@ImagenUrl, ImagenPublicId=@ImagenPublicId WHERE Id=@Id", connection);
            command.Parameters.AddWithValue("@Id", moto.Id);
            command.Parameters.AddWithValue("@Marca", moto.Marca);
            command.Parameters.AddWithValue("@Modelo", moto.Modelo);
            command.Parameters.AddWithValue("@Cilindrada", moto.Cilindrada);
            command.Parameters.AddWithValue("@Potencia", moto.Potencia);
            command.Parameters.AddWithValue("@Peso", moto.Peso);
            command.Parameters.AddWithValue("@Anio", moto.Anio);
            command.Parameters.AddWithValue("@Color", moto.Color);
            command.Parameters.AddWithValue("@ImagenUrl", (object?)moto.ImagenUrl ?? DBNull.Value);
            command.Parameters.AddWithValue("@ImagenPublicId", (object?)moto.ImagenPublicId ?? DBNull.Value);
            await command.ExecuteNonQueryAsync();
        }

        // Elimina una moto por su ID
        public async Task DeleteAsync(int id)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            using var command = new SqlCommand("DELETE FROM Moto WHERE Id=@Id", connection);
            command.Parameters.AddWithValue("@Id", id);
            await command.ExecuteNonQueryAsync();
        }

        // Inicializa datos de ejemplo en la base de datos
        public async Task InicializarDatosAsync()
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            using var command = new SqlCommand(@"
                INSERT INTO Moto (Marca, Modelo, Cilindrada, Potencia, Peso, Anio, Color, ImagenUrl, ImagenPublicId) VALUES
                (@Marca1, @Modelo1, @Cilindrada1, @Potencia1, @Peso1, @Anio1, @Color1, NULL, NULL),
                (@Marca2, @Modelo2, @Cilindrada2, @Potencia2, @Peso2, @Anio2, @Color2, NULL, NULL)", connection);
            command.Parameters.AddWithValue("@Marca1", "Honda");
            command.Parameters.AddWithValue("@Modelo1", "RC213V");
            command.Parameters.AddWithValue("@Cilindrada1", 1000);
            command.Parameters.AddWithValue("@Potencia1", 260.5m);
            command.Parameters.AddWithValue("@Peso1", 157.0);
            command.Parameters.AddWithValue("@Anio1", 2024);
            command.Parameters.AddWithValue("@Color1", "Rojo y Blanco");
            command.Parameters.AddWithValue("@Marca2", "Ducati");
            command.Parameters.AddWithValue("@Modelo2", "Desmosedici GP24");
            command.Parameters.AddWithValue("@Cilindrada2", 1000);
            command.Parameters.AddWithValue("@Potencia2", 270.8m);
            command.Parameters.AddWithValue("@Peso2", 157.0);
            command.Parameters.AddWithValue("@Anio2", 2024);
            command.Parameters.AddWithValue("@Color2", "Rojo");
            await command.ExecuteNonQueryAsync();
        }

        // Convierte una fila de la base de datos en un objeto que C# puede usar
        private Moto MapMoto(SqlDataReader reader) => new Moto
        {
            Id = reader.GetInt32(0),
            Marca = reader.GetString(1),
            Modelo = reader.GetString(2),
            Cilindrada = reader.GetInt32(3),
            Potencia = reader.GetDecimal(4),
            Peso = reader.GetDouble(5),
            Anio = reader.GetInt32(6),
            Color = reader.GetString(7),
            ImagenUrl = reader.IsDBNull(8) ? null : reader.GetString(8),
            ImagenPublicId = reader.IsDBNull(9) ? null : reader.GetString(9)
        };
    }
}