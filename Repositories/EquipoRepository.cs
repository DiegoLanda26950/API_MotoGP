using Microsoft.Data.SqlClient;

namespace MotoGP_API.Repositories
{
    public class EquipoRepository : IEquipoRepository
    {
        private readonly string _connectionString;

        public EquipoRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("MotoGPDB") ?? "Not found";
        }

        // Obtiene todos los equipos de la base de datos
        public async Task<List<Equipo>> GetAllAsync()
        {
            var equipos = new List<Equipo>();
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            using var command = new SqlCommand(
                "SELECT Id, Nombre, Pais, Presupuesto, Victorias, FechaFundacion, EsFabricante, ImagenUrl, ImagenPublicId FROM Equipo", connection);
            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
                equipos.Add(MapEquipo(reader));
            return equipos;
        }

        // Obtiene un equipo por su ID
        public async Task<Equipo?> GetByIdAsync(int id)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            using var command = new SqlCommand(
                "SELECT Id, Nombre, Pais, Presupuesto, Victorias, FechaFundacion, EsFabricante, ImagenUrl, ImagenPublicId FROM Equipo WHERE Id = @Id", connection);
            command.Parameters.AddWithValue("@Id", id);
            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
                return MapEquipo(reader);
            return null;
        }

        // Inserta un nuevo equipo en la base de datos
        public async Task AddAsync(Equipo equipo)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            using var command = new SqlCommand(
                "INSERT INTO Equipo (Nombre, Pais, Presupuesto, Victorias, FechaFundacion, EsFabricante, ImagenUrl, ImagenPublicId) VALUES (@Nombre, @Pais, @Presupuesto, @Victorias, @FechaFundacion, @EsFabricante, @ImagenUrl, @ImagenPublicId)", connection);
            command.Parameters.AddWithValue("@Nombre", equipo.Nombre);
            command.Parameters.AddWithValue("@Pais", equipo.Pais);
            command.Parameters.AddWithValue("@Presupuesto", equipo.Presupuesto);
            command.Parameters.AddWithValue("@Victorias", equipo.Victorias);
            command.Parameters.AddWithValue("@FechaFundacion", equipo.FechaFundacion);
            command.Parameters.AddWithValue("@EsFabricante", equipo.EsFabricante);
            command.Parameters.AddWithValue("@ImagenUrl", (object?)equipo.ImagenUrl ?? DBNull.Value);
            command.Parameters.AddWithValue("@ImagenPublicId", (object?)equipo.ImagenPublicId ?? DBNull.Value);
            await command.ExecuteNonQueryAsync();
        }

        // Actualiza un equipo existente en la base de datos
        public async Task UpdateAsync(Equipo equipo)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            using var command = new SqlCommand(
                "UPDATE Equipo SET Nombre=@Nombre, Pais=@Pais, Presupuesto=@Presupuesto, Victorias=@Victorias, FechaFundacion=@FechaFundacion, EsFabricante=@EsFabricante, ImagenUrl=@ImagenUrl, ImagenPublicId=@ImagenPublicId WHERE Id=@Id", connection);
            command.Parameters.AddWithValue("@Id", equipo.Id);
            command.Parameters.AddWithValue("@Nombre", equipo.Nombre);
            command.Parameters.AddWithValue("@Pais", equipo.Pais);
            command.Parameters.AddWithValue("@Presupuesto", equipo.Presupuesto);
            command.Parameters.AddWithValue("@Victorias", equipo.Victorias);
            command.Parameters.AddWithValue("@FechaFundacion", equipo.FechaFundacion);
            command.Parameters.AddWithValue("@EsFabricante", equipo.EsFabricante);
            command.Parameters.AddWithValue("@ImagenUrl", (object?)equipo.ImagenUrl ?? DBNull.Value);
            command.Parameters.AddWithValue("@ImagenPublicId", (object?)equipo.ImagenPublicId ?? DBNull.Value);
            await command.ExecuteNonQueryAsync();
        }

        // Elimina un equipo por su ID
        public async Task DeleteAsync(int id)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            using var command = new SqlCommand("DELETE FROM Equipo WHERE Id=@Id", connection);
            command.Parameters.AddWithValue("@Id", id);
            await command.ExecuteNonQueryAsync();
        }

        // Inicializa datos de ejemplo en la base de datos
        public async Task InicializarDatosAsync()
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            using var command = new SqlCommand(@"
                INSERT INTO Equipo (Nombre, Pais, Presupuesto, Victorias, FechaFundacion, EsFabricante, ImagenUrl, ImagenPublicId) VALUES
                (@Nombre1, @Pais1, @Presupuesto1, @Victorias1, @FechaFundacion1, @EsFabricante1, NULL, NULL),
                (@Nombre2, @Pais2, @Presupuesto2, @Victorias2, @FechaFundacion2, @EsFabricante2, NULL, NULL)", connection);
            command.Parameters.AddWithValue("@Nombre1", "Repsol Honda");
            command.Parameters.AddWithValue("@Pais1", "Japón");
            command.Parameters.AddWithValue("@Presupuesto1", 120000000.0);
            command.Parameters.AddWithValue("@Victorias1", 300);
            command.Parameters.AddWithValue("@FechaFundacion1", new DateTime(1982, 1, 1));
            command.Parameters.AddWithValue("@EsFabricante1", true);
            command.Parameters.AddWithValue("@Nombre2", "Ducati Lenovo");
            command.Parameters.AddWithValue("@Pais2", "Italia");
            command.Parameters.AddWithValue("@Presupuesto2", 95000000.0);
            command.Parameters.AddWithValue("@Victorias2", 180);
            command.Parameters.AddWithValue("@FechaFundacion2", new DateTime(2003, 1, 1));
            command.Parameters.AddWithValue("@EsFabricante2", true);
            await command.ExecuteNonQueryAsync();
        }

        // Convierte una fila de la base de datos en un objeto que C# puede usar
        private Equipo MapEquipo(SqlDataReader reader) => new Equipo
        {
            Id = reader.GetInt32(0),
            Nombre = reader.GetString(1),
            Pais = reader.GetString(2),
            Presupuesto = reader.GetDouble(3),
            Victorias = reader.GetInt32(4),
            FechaFundacion = reader.GetDateTime(5),
            EsFabricante = reader.GetBoolean(6),
            ImagenUrl = reader.IsDBNull(7) ? null : reader.GetString(7),
            ImagenPublicId = reader.IsDBNull(8) ? null : reader.GetString(8)
        };
    }
}