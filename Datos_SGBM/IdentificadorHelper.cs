using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage;
using System.Data;

namespace Datos_SGBM
{
    /// <summary>
    ///Aquí iba a usar Max+1 con bloqueo para evitar problemas de concurrencia,
    ///pero COPILOT me señaló que es importante destacar que esta técnica no es ideal
    ///en escenarios de alta concurrencia o con múltiples instancias de la aplicación.
    ///Así que le dejé este helper totalmente a Copilot para que lo implemente
    ///con la advertencia de que es una solución simple pero no escalable.
    /// </summary>

    public static class IdentificadorHelper
    {
        /// <summary>
        /// Obtiene de forma segura el siguiente Id disponible para la entidad T usando bloqueo (SQL Server).
        /// </summary>
        public static int ObtenerSiguienteIdSeguro<T>(DbContext contexto) where T : class
        {
            var (schema, table, keyColumn) = ObtenerMetadatosTablaYClave<T>(contexto);
            var sql = $@"SELECT ISNULL(MAX([{keyColumn}]), 0) + 1 FROM [{schema}].[{table}] WITH (UPDLOCK, HOLDLOCK);";

            var connection = contexto.Database.GetDbConnection();
            try
            {
                if (connection.State != ConnectionState.Open)
                    connection.Open();

                using var command = connection.CreateCommand();
                command.CommandText = sql;
                if (contexto.Database.CurrentTransaction != null)
                    command.Transaction = contexto.Database.CurrentTransaction.GetDbTransaction();

                var result = command.ExecuteScalar();
                if (result == null || result == DBNull.Value)
                    return 1;

                return Convert.ToInt32(result);
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }
        }

        /// <summary>
        /// Versión asíncrona del método anterior.
        /// </summary>
        public static async Task<int> ObtenerSiguienteIdSeguroAsync<T>(DbContext contexto, CancellationToken ct = default) where T : class
        {
            var (schema, table, keyColumn) = ObtenerMetadatosTablaYClave<T>(contexto);
            var sql = $@"SELECT ISNULL(MAX([{keyColumn}]), 0) + 1 FROM [{schema}].[{table}] WITH (UPDLOCK, HOLDLOCK);";

            var connection = contexto.Database.GetDbConnection();
            try
            {
                if (connection.State != ConnectionState.Open)
                    await connection.OpenAsync(ct);

                using var command = connection.CreateCommand();
                command.CommandText = sql;
                if (contexto.Database.CurrentTransaction != null)
                    command.Transaction = contexto.Database.CurrentTransaction.GetDbTransaction();

                var result = await command.ExecuteScalarAsync(ct);
                if (result == null || result == DBNull.Value)
                    return 1;

                return Convert.ToInt32(result);
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                    await connection.CloseAsync();
            }
        }

        /// <summary>
        /// Extrae schema, nombre de tabla y nombre de la columna clave primaria para la entidad T desde el modelo EF Core.
        /// Lanza excepción si no encuentra la clave o la tabla.
        /// </summary>
        private static (string schema, string table, string keyColumn) ObtenerMetadatosTablaYClave<T>(DbContext contexto) where T : class
        {
            var entityType = contexto.Model.FindEntityType(typeof(T));
            if (entityType == null)
                throw new InvalidOperationException($"No se encontró metadata para la entidad {typeof(T).Name}.");

            // Nombre de tabla y schema (EF Core 3+)
            var tableName = entityType.GetTableName() ?? throw new InvalidOperationException("No se pudo obtener el nombre de tabla.");
            var schema = entityType.GetSchema() ?? "dbo";

            // Obtener la propiedad clave (suponemos clave simple)
            var key = entityType.FindPrimaryKey();
            if (key == null || key.Properties.Count != 1)
                throw new InvalidOperationException("La entidad debe tener una clave primaria simple.");

            var keyProperty = key.Properties[0];
            var keyColumnName = keyProperty.GetColumnName(StoreObjectIdentifier.Table(tableName, schema));

            if (string.IsNullOrWhiteSpace(keyColumnName))
                throw new InvalidOperationException("No se pudo obtener el nombre de la columna clave.");

            return (schema, tableName, keyColumnName);
        }
    }
}

