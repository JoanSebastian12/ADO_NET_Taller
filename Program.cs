using System.Text;
using Microsoft.Data.Sqlite;

// ============================================================
//  Taller en clase - ADO.NET
//  Programa de consola que demuestra el acceso a una base de
//  datos SQLite usando ADO.NET (Connection, Command, DataReader
//  y Parameters), administrando siempre los recursos con using.
//  Tema: registro de videojuegos.
// ============================================================

namespace ADO_NET_Taller;

internal static class Program
{
    // Ruta del archivo de la base de datos (se crea solo si no existe)
    private const string RutaBase = "videojuegos.db";

    private static void Main()
    {
        Console.OutputEncoding = Encoding.UTF8;

        Console.WriteLine("========================================================");
        Console.WriteLine("  TALLER ADO.NET CON SQLite - Registro de videojuegos");
        Console.WriteLine("========================================================\n");

        // Para que la demo sea repetible (siempre los mismos Id: 1,2,3,4),
        // cada ejecucion recrea la base desde cero. En un proyecto real
        // NO se borra la base: esto es solo para poder ejecutarla las veces
        // que haga falta sin acumular registros.
        if (File.Exists(RutaBase))
            File.Delete(RutaBase);

        // -------------------------------------------------------
        // 1) CONNECTION: SqliteConnection abre el "puente" entre
        //    la app y la base. El 'using' garantiza que se cierre
        //    aunque algo falle por el camino (IDisposable).
        // -------------------------------------------------------
        using (var conexion = new SqliteConnection($"Data Source={RutaBase};Mode=ReadWriteCreate"))
        {
            conexion.Open();
            Console.WriteLine($"[1] CONEXION abierta -> {RutaBase}");
            Console.WriteLine($"    Estado: {conexion.State} | Versión del servidor: {conexion.ServerVersion}\n");

            CrearTabla(conexion);
            InsertarJuegos(conexion);
            ConsultarTodos(conexion);
            ConsultarPorGenero(conexion, "Plataformas");

        } // <-- al salir del bloque, using cierra la conexion solo

        Console.WriteLine("\n[6] Conexion cerrada automaticamente por 'using'.");
        Console.WriteLine("Fin de la demostracion.");
    }

    // -------------------------------------------------------
    // COMMAND + DDL: crear la tabla con 4 columnas
    // -------------------------------------------------------
    private static void CrearTabla(SqliteConnection conexion)
    {
        string sql = """
            CREATE TABLE IF NOT EXISTS Videojuegos (
                Id     INTEGER PRIMARY KEY AUTOINCREMENT,
                Nombre TEXT    NOT NULL,
                Genero TEXT    NOT NULL,
                Anio   INTEGER NOT NULL
            );
            """;

        using var comando = new SqliteCommand(sql, conexion);
        int resultado = comando.ExecuteNonQuery();   // instrucciones sin resultados (INSERT/CREATE/UPDATE)

        Console.WriteLine("[2] COMANDO DDL ejecutado: CREATE TABLE IF NOT EXISTS Videojuegos");
        Console.WriteLine($"    Columnas: Id, Nombre, Genero, Anio | resultado: {resultado}\n");
    }

    // -------------------------------------------------------
    // COMMAND + PARAMETERS: insertar 4 registros con valores
    // parametrizados (nunca concatenando el SQL).
    // -------------------------------------------------------
    private static void InsertarJuegos(SqliteConnection conexion)
    {
        // (La base ya viene vacia porque se recreo al arrancar)
        var juegos = new[]
        {
            new { Nombre = "Hollow Knight",   Genero = "Metroidvania", Anio = 2017 },
            new { Nombre = "Celeste",         Genero = "Plataformas",  Anio = 2018 },
            new { Nombre = "Hades",           Genero = "Roguelike",    Anio = 2020 },
            new { Nombre = "Stardew Valley",  Genero = "Simulacion",   Anio = 2016 },
        };

        // Los @nombre, @genero y @anio son PARAMETROS: el valor se pasa
        // por separado, lo que evita el SQL injection.
        string sql = "INSERT INTO Videojuegos (Nombre, Genero, Anio) VALUES (@nombre, @genero, @anio);";

        Console.WriteLine("[3] COMANDO DML con PARAMETERS: INSERT de 4 registros");
        foreach (var j in juegos)
        {
            using var comando = new SqliteCommand(sql, conexion);
            comando.Parameters.AddWithValue("@nombre", j.Nombre);
            comando.Parameters.AddWithValue("@genero", j.Genero);
            comando.Parameters.AddWithValue("@anio", j.Anio);

            int filas = comando.ExecuteNonQuery();   // devuelve las filas afectadas
            Console.WriteLine($"    + {j.Nombre,-16} ({j.Genero}, {j.Anio}) -> {filas} fila afectada");
        }
        Console.WriteLine();
    }

    // -------------------------------------------------------
    // DATAREADER: leer fila por fila (es lo mas eficiente
    // cuando no necesitamos copiar todo en memoria).
    // -------------------------------------------------------
    private static void ConsultarTodos(SqliteConnection conexion)
    {
        string sql = "SELECT Id, Nombre, Genero, Anio FROM Videojuegos ORDER BY Anio;";

        using var comando = new SqliteCommand(sql, conexion);
        using SqliteDataReader lector = comando.ExecuteReader();

        Console.WriteLine("[4] DATAREADER: consulta de todos los registros");
        Console.WriteLine($"    {"ID",-4}{"NOMBRE",-18}{"GENERO",-15}{"ANIO"}");
        Console.WriteLine($"    {"--",-4}{"------",-18}{"------",-15}{"----"}");

        int total = 0;
        while (lector.Read())          // Read() = avanzar a la siguiente fila (false = no hay mas)
        {
            // Se puede leer por indice (0,1,2,3) o por nombre de columna:
            int id = lector.GetInt32(0);
            string nombre = lector.GetString(lector.GetOrdinal("Nombre"));
            string genero = lector.GetString(2);
            int anio = lector.GetInt32(3);

            Console.WriteLine($"    {id,-4}{nombre,-18}{genero,-15}{anio}");
            total++;
        }
        Console.WriteLine($"    Total leido: {total} registro(s)\n");
    }

    // -------------------------------------------------------
    // PARAMETERS tambien sirven para FILTRAR consultas
    // -------------------------------------------------------
    private static void ConsultarPorGenero(SqliteConnection conexion, string genero)
    {
        string sql = "SELECT Nombre, Anio FROM Videojuegos WHERE Genero = @genero ORDER BY Nombre;";

        using var comando = new SqliteCommand(sql, conexion);
        comando.Parameters.AddWithValue("@genero", genero);
        using SqliteDataReader lector = comando.ExecuteReader();

        Console.WriteLine($"[5] DATAREADER + PARAMETER: juegos del genero '{genero}'");
        while (lector.Read())
            Console.WriteLine($"    - {lector.GetString(0)} ({lector.GetInt32(1)})");
    }
}
