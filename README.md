# Taller en clase — ADO.NET con SQLite 🎮

Programa de consola en **C# (.NET 8)** que demuestra el acceso a una base de datos
**SQLite** usando **ADO.NET**: crear la base, crear la tabla, insertar registros y
consultarlos, administrando siempre los recursos con `using`.

> Actividad: *Actividad de investigación — Introducción a ADO.NET y su aplicación en C#*
> (Semana 12 · Fundamentos de acceso a datos con ADO.NET).

## Tema del ejemplo

Un **registro de videojuegos**: tabla `Videojuegos` con las columnas
`Id`, `Nombre`, `Genero` y `Anio`, con 4 registros de ejemplo
(Hollow Knight, Celeste, Hades y Stardew Valley).

## Qué demuestra (los 4 componentes de ADO.NET)

| Componente | Dónde está en el código | Qué hace |
|---|---|---|
| **Connection** (`SqliteConnection`) | `Main()` | Abre el puente entre la app y la base de datos; se cierra solo con `using`. |
| **Command** (`SqliteCommand`) | `CrearTabla`, `InsertarJuegos`, `ConsultarTodos` | Ejecuta instrucciones SQL (DDL `CREATE TABLE`, DML `INSERT`, `SELECT`). |
| **DataReader** (`SqliteDataReader`) | `ConsultarTodos`, `ConsultarPorGenero` | Lee los resultados **fila por fila** de forma eficiente (`Read()`). |
| **Parameters** (`SqliteParameter`) | `InsertarJuegos`, `ConsultarPorGenero` | Pasa los valores por separado (`@nombre`, `@genero`…), evitando SQL injection. |

## Requisitos

- [.NET SDK 8.0](https://dotnet.microsoft.com/download/dotnet/8.0) (probado con la versión 8.0.426)
- El paquete NuGet `Microsoft.Data.Sqlite` 8.0.34 (se descarga solo en la restauración)

## Cómo ejecutarlo

```bash
# 1. Clonar el repositorio
git clone https://github.com/JoanSebastian12/ADO_NET_Taller.git
cd ADO_NET_Taller

# 2. Restaurar dependencias y ejecutar
dotnet run
```

En la primera ejecución se crea el archivo `videojuegos.db` en la carpeta del proyecto.
Para empezar de cero, simplemente bórralo: el programa lo vuelve a crear.

## Salida esperada

```
[1] CONEXION abierta -> videojuegos.db
    Estado: Open | Versión del servidor: 3.53.3

[2] COMANDO DDL ejecutado: CREATE TABLE IF NOT EXISTS Videojuegos
    Columnas: Id, Nombre, Genero, Anio | resultado: 0

[3] COMANDO DML con PARAMETERS: INSERT de 4 registros
    + Hollow Knight    (Metroidvania, 2017) -> 1 fila afectada
    + Celeste          (Plataformas, 2018) -> 1 fila afectada
    + Hades            (Roguelike, 2020) -> 1 fila afectada
    + Stardew Valley   (Simulacion, 2016) -> 1 fila afectada

[4] DATAREADER: consulta de todos los registros
    ID  NOMBRE            GENERO         ANIO
    1   Hollow Knight     Metroidvania   2017
    ...
    Total leido: 4 registro(s)
```

## Evidencias de ejecución

Capturas reales de la consola con el programa corriendo (también están en la carpeta
[`evidencias/`](evidencias/)):

- `captura_ejecucion_completa.png` — la corrida completa
- `captura_conexion.png` — apertura de la conexión y creación de la tabla
- `captura_insercion.png` — inserción de los 4 registros con parámetros
- `captura_consulta.png` — lectura con `SqliteDataReader`

## Estructura

```
ADO_NET_Taller/
├── Program.cs              # todo el código del ejemplo
├── ADO_NET_Taller.csproj   # proyecto .NET 8 + paquete Microsoft.Data.Sqlite
├── evidencia_salida.txt    # salida en texto de la última ejecución
├── evidencias/             # capturas de pantalla
├── README.md
└── .gitignore              # ignora bin/, obj/ y videojuegos.db
```

## Nota sobre la base de datos

Cada ejecución **recrea** `videojuegos.db` desde cero para que la demo sea repetible
(siempre los mismos Id: 1, 2, 3, 4). En un proyecto real eso no se hace: la base
persiste y solo se borran los registros que decidas.

---

Hecho con C# + .NET 8 y ADO.NET (`Microsoft.Data.Sqlite`).
