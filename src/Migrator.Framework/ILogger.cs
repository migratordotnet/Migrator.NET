using System;
using System.Collections.Generic;

namespace Migrator.Framework
{
	public interface ILogger
	{
		
		/// <summary>
		/// Log that we have started a migration
		/// </summary>
		/// <param name="currentVersion">Start list of versions</param>
		/// <param name="finalVersion">Final Version</param>
		void Started(List<long> currentVersion, long finalVersion);

		/// <summary>
		/// Log that we are migrating up
		/// </summary>
		/// <param name="version">Version we are migrating to</param>
		/// <param name="migrationName">Migration name</param>
		void MigrateUp(long version, string migrationName);

		/// <summary>
		/// Log that we are migrating down
		/// </summary>
		/// <param name="version">Version we are migrating to</param>
		/// <param name="migrationName">Migration name</param>
		void MigrateDown(long version, string migrationName);

		/// <summary>
		/// Inform that a migration corresponding to the number of
		/// version is untraceable (not found?) and will be ignored.
		/// </summary>
		/// <param name="version">Version we couldnt find</param>
		void Skipping(long version);

		/// <summary>
		/// Log that we are rolling back to version
		/// </summary>
		/// <param name="originalVersion">
		/// version
		/// </param>
		void RollingBack(long originalVersion);

        /// <summary>
        /// Log a Sql statement that changes the schema or content of the database as part of a migration
        /// </summary>
        /// <remarks>
        /// SELECT statements should not be logged using this method as they do not alter the data or schema of the
        /// database.
        /// </remarks>
        /// <param name="sql">The Sql statement to log</param>
        void ApplyingDBChange(string sql);

		/// <summary>
		/// Log that we had an exception on a migration
		/// </summary>
		/// <param name="version">La version de la migration qui a produire l'exception.</param>
		/// <param name="migrationName">Le nom de la migration qui a produire l'exception.</param>
		/// <param name="ex">L'exception lancée</param>
		void Exception(long version, string migrationName, Exception ex);

		/// <summary>
		/// Log that we have finished a migration
		/// </summary>
		/// <param name="currentVersion">List of versions with which we started</param>
		/// <param name="finalVersion">Final Version</param>
		void Finished(List<long> currentVersion, long finalVersion);
		
		/// <summary>
		/// Log a message
		/// </summary>
		/// <param name="format">Le format ("{0}, blbla {1}") ou la chaîne à afficher.</param>
		/// <param name="args">Les paramètres dans le cas d'un format au premier paramètre.</param>
		void Log(string format, params object[] args);

		/// <summary>
		/// Log a Warning
		/// </summary>
		/// <param name="format">Le format ("{0}, blbla {1}") ou la chaîne à afficher.</param>
		/// <param name="args">Les paramètres dans le cas d'un format au premier paramètre.</param>
		void Warn(string format, params object[] args);

		/// <summary>
		/// Log a Trace Message
		/// </summary>
		/// <param name="format">Le format ("{0}, blbla {1}") ou la chaîne à afficher.</param>
		/// <param name="args">Les paramètres dans le cas d'un format au premier paramètre.</param>
		void Trace(string format, params object[] args);
	}
}
