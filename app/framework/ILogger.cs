using System;

namespace Migrator.Framework
{
	public interface ILogger
	{
		/// <summary>
		/// Log that we have started a migration
		/// </summary>
		/// <param name="currentVersion">Start version</param>
		/// <param name="finalVersion">Final Version</param>
		void Started(int currentVersion, int finalVersion);

		/// <summary>
		/// Log that we are migrating up
		/// </summary>
		/// <param name="version">Version we are migrating to</param>
		/// <param name="migrationName">Migration name</param>
		void MigrateUp(int version, string migrationName);

		/// <summary>
		/// Log that we are migrating down
		/// </summary>
		/// <param name="version">Version we are migrating to</param>
		/// <param name="migrationName">Migration name</param>
		void MigrateDown(int version, string migrationName);

		/// <summary>
		/// Inform that a migration corresponding to the number of
		/// version is untraceable (not found?) and will be ignored.
		/// </summary>
		/// <param name="version">Version we couldnt find</param>
		void Skipping(int version);

		/// <summary>
		/// Log that we are rolling back to version
		/// </summary>
		/// <param name="originalVersion">
		/// version
		/// </param>
		void RollingBack(int originalVersion);

		/// <summary>
		/// Log that we had an exception on a migration
		/// </summary>
		/// <param name="version">La version de la migration qui a produire l'exception.</param>
		/// <param name="migrationName">Le nom de la migration qui a produire l'exception.</param>
		/// <param name="ex">L'exception lancée</param>
		void Exception(int version, string migrationName, Exception ex);

		/// <summary>
		/// Log that we have finished a migration
		/// </summary>
		/// <param name="originalVersion">La version intiale de la base de données</param>
		/// <param name="currentVersion">La version actuel de la base de donnée</param>
		void Finished(int originalVersion, int currentVersion);

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
