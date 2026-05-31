using Microsoft.Data.SqlClient;
using Models;

namespace MotoGP_API.Repositories
{
    public class PilotoRepository : IPilotoRepository
    {
        private readonly string _connectionString;
        private readonly IMotoRepository _motoRepo;
        private readonly IEquipoRepository _equipoRepo;
        private readonly ICircuitoRepository _circuitoRepo;

        public PilotoRepository(IConfiguration configuration, IMotoRepository motoRepo, IEquipoRepository equipoRepo, ICircuitoRepository circuitoRepo)
        {
            _connectionString = configuration.GetConnectionString("MotoGPDB") ?? "Not found";
            _motoRepo = motoRepo;
            _equipoRepo = equipoRepo;
            _circuitoRepo = circuitoRepo;
        }

        // Obtiene todos los pilotos con sus relaciones
        public async Task<List<Piloto>> GetAllAsync()
        {
            var pilotos = new List<Piloto>();
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            using var command = new SqlCommand(
                "SELECT Id, Nombre, Nacionalidad, Dorsal, CampeonatosGanados, FechaNacimiento, Activo, ImagenUrl, ImagenPublicId, MotoId, EquipoId, CircuitoId FROM Piloto", connection);
            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
                pilotos.Add(await MapPiloto(reader));
            return pilotos;
        }

        // Obtiene pilotos filtrados y ordenados
        public async Task<List<Piloto>> GetAllFilteredAsync(string? Nombre, string? Nacionalidad, string? orderBy, bool ascending)
        {
            var pilotos = new List<Piloto>();
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            string query = "SELECT Id, Nombre, Nacionalidad, Dorsal, CampeonatosGanados, FechaNacimiento, Activo, ImagenUrl, ImagenPublicId, MotoId, EquipoId, CircuitoId FROM Piloto WHERE 1=1";
            var parameters = new List<SqlParameter>();

            // Filtro por nombre
            if (!string.IsNullOrWhiteSpace(Nombre))
            {
                query += " AND Nombre = @Nombre";
                parameters.Add(new SqlParameter("@Nombre", Nombre));
            }

            // Filtro por nacionalidad
            if (!string.IsNullOrWhiteSpace(Nacionalidad))
            {
                query += " AND Nacionalidad = @Nacionalidad";
                parameters.Add(new SqlParameter("@Nacionalidad", Nacionalidad));
            }

            // Ordenación dinámica con protección contra SQL Injection
            if (!string.IsNullOrWhiteSpace(orderBy))
            {
                var validColumns = new[] { "nombre", "nacionalidad", "dorsal", "campeonatosganados", "fechanacimiento" };
                if (validColumns.Contains(orderBy.ToLower()))
                    query += $" ORDER BY {orderBy} {(ascending ? "ASC" : "DESC")}";
                else
                    query += " ORDER BY Nombre ASC";
            }

            using var command = new SqlCommand(query, connection);
            command.Parameters.AddRange(parameters.ToArray());
            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
                pilotos.Add(await MapPiloto(reader));
            return pilotos;
        }

        // Obtiene un piloto por su ID con sus relaciones
        public async Task<Piloto?> GetByIdAsync(int id)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            using var command = new SqlCommand(
                "SELECT Id, Nombre, Nacionalidad, Dorsal, CampeonatosGanados, FechaNacimiento, Activo, ImagenUrl, ImagenPublicId, MotoId, EquipoId, CircuitoId FROM Piloto WHERE Id = @Id", connection);
            command.Parameters.AddWithValue("@Id", id);
            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
                return await MapPiloto(reader);
            return null;
        }

        // Inserta un nuevo piloto en la base de datos
        public async Task AddAsync(Piloto piloto)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            using var command = new SqlCommand(
                "INSERT INTO Piloto (Nombre, Nacionalidad, Dorsal, CampeonatosGanados, FechaNacimiento, Activo, ImagenUrl, ImagenPublicId, MotoId, EquipoId, CircuitoId) VALUES (@Nombre, @Nacionalidad, @Dorsal, @CampeonatosGanados, @FechaNacimiento, @Activo, @ImagenUrl, @ImagenPublicId, @MotoId, @EquipoId, @CircuitoId)", connection);
            command.Parameters.AddWithValue("@Nombre", piloto.Nombre);
            command.Parameters.AddWithValue("@Nacionalidad", piloto.Nacionalidad);
            command.Parameters.AddWithValue("@Dorsal", piloto.Dorsal);
            command.Parameters.AddWithValue("@CampeonatosGanados", piloto.CampeonatosGanados);
            command.Parameters.AddWithValue("@FechaNacimiento", piloto.FechaNacimiento);
            command.Parameters.AddWithValue("@Activo", piloto.Activo);
            command.Parameters.AddWithValue("@ImagenUrl", (object?)piloto.ImagenUrl ?? DBNull.Value);
            command.Parameters.AddWithValue("@ImagenPublicId", (object?)piloto.ImagenPublicId ?? DBNull.Value);
            command.Parameters.AddWithValue("@MotoId", piloto.Moto.Id);
            command.Parameters.AddWithValue("@EquipoId", piloto.Equipo.Id);
            command.Parameters.AddWithValue("@CircuitoId", piloto.Circuito.Id);
            await command.ExecuteNonQueryAsync();
        }

        // Actualiza un piloto existente en la base de datos
        public async Task UpdateAsync(Piloto piloto)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            using var command = new SqlCommand(
                "UPDATE Piloto SET Nombre=@Nombre, Nacionalidad=@Nacionalidad, Dorsal=@Dorsal, CampeonatosGanados=@CampeonatosGanados, FechaNacimiento=@FechaNacimiento, Activo=@Activo, ImagenUrl=@ImagenUrl, ImagenPublicId=@ImagenPublicId, MotoId=@MotoId, EquipoId=@EquipoId, CircuitoId=@CircuitoId WHERE Id=@Id", connection);
            command.Parameters.AddWithValue("@Id", piloto.Id);
            command.Parameters.AddWithValue("@Nombre", piloto.Nombre);
            command.Parameters.AddWithValue("@Nacionalidad", piloto.Nacionalidad);
            command.Parameters.AddWithValue("@Dorsal", piloto.Dorsal);
            command.Parameters.AddWithValue("@CampeonatosGanados", piloto.CampeonatosGanados);
            command.Parameters.AddWithValue("@FechaNacimiento", piloto.FechaNacimiento);
            command.Parameters.AddWithValue("@Activo", piloto.Activo);
            command.Parameters.AddWithValue("@ImagenUrl", (object?)piloto.ImagenUrl ?? DBNull.Value);
            command.Parameters.AddWithValue("@ImagenPublicId", (object?)piloto.ImagenPublicId ?? DBNull.Value);
            command.Parameters.AddWithValue("@MotoId", piloto.Moto.Id);
            command.Parameters.AddWithValue("@EquipoId", piloto.Equipo.Id);
            command.Parameters.AddWithValue("@CircuitoId", piloto.Circuito.Id);
            await command.ExecuteNonQueryAsync();
        }

        // Elimina un piloto por su ID
        public async Task DeleteAsync(int id)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            using var command = new SqlCommand("DELETE FROM Piloto WHERE Id=@Id", connection);
            command.Parameters.AddWithValue("@Id", id);
            await command.ExecuteNonQueryAsync();
        }

        // Inicializa datos de ejemplo en la base de datos
        public async Task InicializarDatosAsync()
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            using var command = new SqlCommand(@"
                INSERT INTO Piloto (Nombre, Nacionalidad, Dorsal, CampeonatosGanados, FechaNacimiento, Activo, ImagenUrl, ImagenPublicId, MotoId, EquipoId, CircuitoId) VALUES
                (@Nombre1, @Nacionalidad1, @Dorsal1, @Campeonatos1, @FechaNacimiento1, @Activo1, NULL, NULL, @Moto1, @Equipo1, @Circuito1),
                (@Nombre2, @Nacionalidad2, @Dorsal2, @Campeonatos2, @FechaNacimiento2, @Activo2, NULL, NULL, @Moto2, @Equipo2, @Circuito2)", connection);
            command.Parameters.AddWithValue("@Nombre1", "Marc Márquez");
            command.Parameters.AddWithValue("@Nacionalidad1", "Española");
            command.Parameters.AddWithValue("@Dorsal1", 93);
            command.Parameters.AddWithValue("@Campeonatos1", 8);
            command.Parameters.AddWithValue("@FechaNacimiento1", new DateTime(1993, 2, 17));
            command.Parameters.AddWithValue("@Activo1", true);
            command.Parameters.AddWithValue("@Moto1", 1);
            command.Parameters.AddWithValue("@Equipo1", 1);
            command.Parameters.AddWithValue("@Circuito1", 1);
            command.Parameters.AddWithValue("@Nombre2", "Francesco Bagnaia");
            command.Parameters.AddWithValue("@Nacionalidad2", "Italiana");
            command.Parameters.AddWithValue("@Dorsal2", 1);
            command.Parameters.AddWithValue("@Campeonatos2", 2);
            command.Parameters.AddWithValue("@FechaNacimiento2", new DateTime(1997, 1, 14));
            command.Parameters.AddWithValue("@Activo2", true);
            command.Parameters.AddWithValue("@Moto2", 2);
            command.Parameters.AddWithValue("@Equipo2", 2);
            command.Parameters.AddWithValue("@Circuito2", 2);
            await command.ExecuteNonQueryAsync();
        }

        // Método auxiliar para mapear un SqlDataReader a un objeto Piloto con sus relaciones
        private async Task<Piloto> MapPiloto(SqlDataReader reader) => new Piloto
        {
            Id = reader.GetInt32(0),
            Nombre = reader.GetString(1),
            Nacionalidad = reader.GetString(2),
            Dorsal = reader.GetInt32(3),
            CampeonatosGanados = reader.GetInt32(4),
            FechaNacimiento = reader.GetDateTime(5),
            Activo = reader.GetBoolean(6),
            ImagenUrl = reader.IsDBNull(7) ? null : reader.GetString(7),
            ImagenPublicId = reader.IsDBNull(8) ? null : reader.GetString(8),
            Moto = await _motoRepo.GetByIdAsync(reader.GetInt32(9)),
            Equipo = await _equipoRepo.GetByIdAsync(reader.GetInt32(10)),
            Circuito = await _circuitoRepo.GetByIdAsync(reader.GetInt32(11))
        };
    }
}