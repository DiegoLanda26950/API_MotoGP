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

        public async Task<List<Circuito>> GetAllAsync()
        {
            var circuitos = new List<Circuito>();
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            using var command = new SqlCommand("SELECT Id, Nombre, Pais, Ciudad, Longitud, Curvas, Homologado, FechaInauguracion FROM Circuito", connection);
            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
                circuitos.Add(new Circuito
                {
                    Id = reader.GetInt32(0),
                    Nombre = reader.GetString(1),
                    Pais = reader.GetString(2),
                    Ciudad = reader.GetString(3),
                    Longitud = reader.GetDouble(4),
                    Curvas = reader.GetInt32(5),
                    Homologado = reader.GetBoolean(6),
                    FechaInauguracion = reader.GetDateTime(7)
                });
            return circuitos;
        }

        public async Task<Circuito?> GetByIdAsync(int id)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            using var command = new SqlCommand("SELECT Id, Nombre, Pais, Ciudad, Longitud, Curvas, Homologado, FechaInauguracion FROM Circuito WHERE Id = @Id", connection);
            command.Parameters.AddWithValue("@Id", id);
            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
                return new Circuito
                {
                    Id = reader.GetInt32(0),
                    Nombre = reader.GetString(1),
                    Pais = reader.GetString(2),
                    Ciudad = reader.GetString(3),
                    Longitud = reader.GetDouble(4),
                    Curvas = reader.GetInt32(5),
                    Homologado = reader.GetBoolean(6),
                    FechaInauguracion = reader.GetDateTime(7)
                };
            return null;
        }

        public async Task AddAsync(Circuito circuito)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            using var command = new SqlCommand(
                "INSERT INTO Circuito (Nombre, Pais, Ciudad, Longitud, Curvas, Homologado, FechaInauguracion) VALUES (@Nombre, @Pais, @Ciudad, @Longitud, @Curvas, @Homologado, @FechaInauguracion)", connection);
            command.Parameters.AddWithValue("@Nombre", circuito.Nombre);
            command.Parameters.AddWithValue("@Pais", circuito.Pais);
            command.Parameters.AddWithValue("@Ciudad", circuito.Ciudad);
            command.Parameters.AddWithValue("@Longitud", circuito.Longitud);
            command.Parameters.AddWithValue("@Curvas", circuito.Curvas);
            command.Parameters.AddWithValue("@Homologado", circuito.Homologado);
            command.Parameters.AddWithValue("@FechaInauguracion", circuito.FechaInauguracion);
            await command.ExecuteNonQueryAsync();
        }

        public async Task UpdateAsync(Circuito circuito)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            using var command = new SqlCommand(
                "UPDATE Circuito SET Nombre=@Nombre, Pais=@Pais, Ciudad=@Ciudad, Longitud=@Longitud, Curvas=@Curvas, Homologado=@Homologado, FechaInauguracion=@FechaInauguracion WHERE Id=@Id", connection);
            command.Parameters.AddWithValue("@Id", circuito.Id);
            command.Parameters.AddWithValue("@Nombre", circuito.Nombre);
            command.Parameters.AddWithValue("@Pais", circuito.Pais);
            command.Parameters.AddWithValue("@Ciudad", circuito.Ciudad);
            command.Parameters.AddWithValue("@Longitud", circuito.Longitud);
            command.Parameters.AddWithValue("@Curvas", circuito.Curvas);
            command.Parameters.AddWithValue("@Homologado", circuito.Homologado);
            command.Parameters.AddWithValue("@FechaInauguracion", circuito.FechaInauguracion);
            await command.ExecuteNonQueryAsync();
        }

        public async Task DeleteAsync(int id)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            using var command = new SqlCommand("DELETE FROM Circuito WHERE Id=@Id", connection);
            command.Parameters.AddWithValue("@Id", id);
            await command.ExecuteNonQueryAsync();
        }

        public async Task InicializarDatosAsync()
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            using var command = new SqlCommand(@"
                INSERT INTO Circuito (Nombre, Pais, Ciudad, Longitud, Curvas, Homologado, FechaInauguracion) VALUES
                (@Nombre1, @Pais1, @Ciudad1, @Longitud1, @Curvas1, @Homologado1, @FechaInauguracion1),
                (@Nombre2, @Pais2, @Ciudad2, @Longitud2, @Curvas2, @Homologado2, @FechaInauguracion2)", connection);
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
    }
}