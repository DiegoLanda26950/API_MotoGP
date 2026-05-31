using Microsoft.Data.SqlClient;

namespace MotoGP_API.Repositories
{
    public class CircuitoRepository : ICircuitoRepository
    {
        private readonly string _connectionString;

        public CircuitoRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("MotoGPDB") ?? "Not found";
        }

        // Obtiene todos los circuitos de la base de datos
        public async Task<List<Circuito>> GetAllAsync()
        {
            var circuitos = new List<Circuito>();
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            using var command = new SqlCommand(
                "SELECT Id, Nombre, Pais, Ciudad, Longitud, Curvas, Homologado, FechaInauguracion, ImagenUrl, ImagenPublicId FROM Circuito", connection);
            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
                circuitos.Add(MapCircuito(reader));
            return circuitos;
        }

        // Obtiene un circuito por su ID
        public async Task<Circuito?> GetByIdAsync(int id)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            using var command = new SqlCommand(
                "SELECT Id, Nombre, Pais, Ciudad, Longitud, Curvas, Homologado, FechaInauguracion, ImagenUrl, ImagenPublicId FROM Circuito WHERE Id = @Id", connection);
            command.Parameters.AddWithValue("@Id", id);
            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
                return MapCircuito(reader);
            return null;
        }

        // Inserta un nuevo circuito en la base de datos
        public async Task AddAsync(Circuito circuito)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            using var command = new SqlCommand(
                "INSERT INTO Circuito (Nombre, Pais, Ciudad, Longitud, Curvas, Homologado, FechaInauguracion, ImagenUrl, ImagenPublicId) VALUES (@Nombre, @Pais, @Ciudad, @Longitud, @Curvas, @Homologado, @FechaInauguracion, @ImagenUrl, @ImagenPublicId)", connection);
            command.Parameters.AddWithValue("@Nombre", circuito.Nombre);
            command.Parameters.AddWithValue("@Pais", circuito.Pais);
            command.Parameters.AddWithValue("@Ciudad", circuito.Ciudad);
            command.Parameters.AddWithValue("@Longitud", circuito.Longitud);
            command.Parameters.AddWithValue("@Curvas", circuito.Curvas);
            command.Parameters.AddWithValue("@Homologado", circuito.Homologado);
            command.Parameters.AddWithValue("@FechaInauguracion", circuito.FechaInauguracion);
            command.Parameters.AddWithValue("@ImagenUrl", (object?)circuito.ImagenUrl ?? DBNull.Value);
            command.Parameters.AddWithValue("@ImagenPublicId", (object?)circuito.ImagenPublicId ?? DBNull.Value);
            await command.ExecuteNonQueryAsync();
        }

        // Actualiza un circuito existente en la base de datos
        public async Task UpdateAsync(Circuito circuito)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            using var command = new SqlCommand(
                "UPDATE Circuito SET Nombre=@Nombre, Pais=@Pais, Ciudad=@Ciudad, Longitud=@Longitud, Curvas=@Curvas, Homologado=@Homologado, FechaInauguracion=@FechaInauguracion, ImagenUrl=@ImagenUrl, ImagenPublicId=@ImagenPublicId WHERE Id=@Id", connection);
            command.Parameters.AddWithValue("@Id", circuito.Id);
            command.Parameters.AddWithValue("@Nombre", circuito.Nombre);
            command.Parameters.AddWithValue("@Pais", circuito.Pais);
            command.Parameters.AddWithValue("@Ciudad", circuito.Ciudad);
            command.Parameters.AddWithValue("@Longitud", circuito.Longitud);
            command.Parameters.AddWithValue("@Curvas", circuito.Curvas);
            command.Parameters.AddWithValue("@Homologado", circuito.Homologado);
            command.Parameters.AddWithValue("@FechaInauguracion", circuito.FechaInauguracion);
            command.Parameters.AddWithValue("@ImagenUrl", (object?)circuito.ImagenUrl ?? DBNull.Value);
            command.Parameters.AddWithValue("@ImagenPublicId", (object?)circuito.ImagenPublicId ?? DBNull.Value);
            await command.ExecuteNonQueryAsync();
        }

        // Elimina un circuito por su ID
        public async Task DeleteAsync(int id)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            using var command = new SqlCommand("DELETE FROM Circuito WHERE Id=@Id", connection);
            command.Parameters.AddWithValue("@Id", id);
            await command.ExecuteNonQueryAsync();
        }

        // Inicializa datos de ejemplo en la base de datos
        public async Task InicializarDatosAsync()
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            using var command = new SqlCommand(@"
                INSERT INTO Circuito (Nombre, Pais, Ciudad, Longitud, Curvas, Homologado, FechaInauguracion, ImagenUrl, ImagenPublicId) VALUES
                (@Nombre1, @Pais1, @Ciudad1, @Longitud1, @Curvas1, @Homologado1, @FechaInauguracion1, NULL, NULL),
                (@Nombre2, @Pais2, @Ciudad2, @Longitud2, @Curvas2, @Homologado2, @FechaInauguracion2, NULL, NULL)", connection);
            command.Parameters.AddWithValue("@Nombre1", "Circuit de Barcelona-Catalunya");
            command.Parameters.AddWithValue("@Pais1", "España");
            command.Parameters.AddWithValue("@Ciudad1", "Montmeló");
            command.Parameters.AddWithValue("@Longitud1", 4.655);
            command.Parameters.AddWithValue("@Curvas1", 16);
            command.Parameters.AddWithValue("@Homologado1", true);
            command.Parameters.AddWithValue("@FechaInauguracion1", new DateTime(1991, 9, 10));
            command.Parameters.AddWithValue("@Nombre2", "Autodromo del Mugello");
            command.Parameters.AddWithValue("@Pais2", "Italia");
            command.Parameters.AddWithValue("@Ciudad2", "Scarperia");
            command.Parameters.AddWithValue("@Longitud2", 5.245);
            command.Parameters.AddWithValue("@Curvas2", 15);
            command.Parameters.AddWithValue("@Homologado2", true);
            command.Parameters.AddWithValue("@FechaInauguracion2", new DateTime(1974, 3, 1));
            await command.ExecuteNonQueryAsync();
        }

        // Método auxiliar para mapear un SqlDataReader a un objeto Circuito
        private Circuito MapCircuito(SqlDataReader reader) => new Circuito
        {
            Id = reader.GetInt32(0),
            Nombre = reader.GetString(1),
            Pais = reader.GetString(2),
            Ciudad = reader.GetString(3),
            Longitud = reader.GetDouble(4),
            Curvas = reader.GetInt32(5),
            Homologado = reader.GetBoolean(6),
            FechaInauguracion = reader.GetDateTime(7),
            ImagenUrl = reader.IsDBNull(8) ? null : reader.GetString(8),
            ImagenPublicId = reader.IsDBNull(9) ? null : reader.GetString(9)
        };
    }
}