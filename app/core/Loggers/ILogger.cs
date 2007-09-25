#region License
//The contents of this file are subject to the Mozilla Public License
//Version 1.1 (the "License"); you may not use this file except in
//compliance with the License. You may obtain a copy of the License at
//http://www.mozilla.org/MPL/
//Software distributed under the License is distributed on an "AS IS"
//basis, WITHOUT WARRANTY OF ANY KIND, either express or implied. See the
//License for the specific language governing rights and limitations
//under the License.
#endregion
using System;

namespace Migrator.Loggers
{
	/// <summary>
	/// Interface à étendre pour définir un loggeur d'événement
	/// du médiateur de migration.
	/// <see cref="ConsoleLogger">ConsoleLogger</see> est le loggeur
	/// par défaut et affiche toute les sorties à la console.
	/// </summary>
	public interface ILogger
	{
		/// <summary>
		/// Informe que la migration débute.
		/// </summary>
		/// <param name="currentVersion">La version actuel de la base de donnée</param>
		/// <param name="finalVersion">La version vers laquelle migrer</param>
		void Started(int currentVersion, int finalVersion);
		
		/// <summary>
		/// Informe qu'une migration vers le haut s'exécute.
		/// </summary>
		/// <param name="version">La version de la migration</param>
		/// <param name="migrationName">Le nom de la migration</param>
		void MigrateUp(int version, string migrationName);
		
		/// <summary>
		/// Informe qu'une migration vers le bas s'exécute.
		/// </summary>
		/// <param name="version">La version de la migration</param>
		/// <param name="migrationName">Le nom de la migration</param>
		void MigrateDown(int version, string migrationName);
		
		/// <summary>
		/// Informe qu'une migration correspondant au numéro de
		/// version est introuvable et sera ignorée.
		/// </summary>
		/// <param name="version">La version introuvable</param>
		void Skipping(int version);
		
		/// <summary>
		/// Informe que les modifications à la base de données
		/// seront annulées.
		/// </summary>
		/// <param name="originalVersion">
		/// Version initiale de la base de données.
		/// Vers laquelle on retourne.
		/// </param>
		void RollingBack(int originalVersion);
		
		/// <summary>
		/// Informe qu'une exception est survenue lors d'une
		/// migration.
		/// </summary>
		/// <param name="version">La version de la migration qui a produire l'exception.</param>
		/// <param name="migrationName">Le nom de la migration qui a produire l'exception.</param>
		/// <param name="ex">L'exception lancée</param>
		void Exception(int version, string migrationName, Exception ex);
		
		/// <summary>
		/// Informe que le processus de migration s'est terminée avec succès.
		/// </summary>
		/// <param name="originalVersion">La version intiale de la base de données</param>
		/// <param name="currentVersion">La version actuel de la base de donnée</param>
		void Finished(int originalVersion, int currentVersion);
		
		/// <summary>
		/// Affiche un message d'information.
		/// </summary>
		/// <param name="format">Le format ("{0}, blbla {1}") ou la chaîne à afficher.</param>
		/// <param name="args">Les paramètres dans le cas d'un format au premier paramètre.</param>
		void Log(string format, params object[] args);
		
		/// <summary>
		/// Affiche un avertissement.
		/// </summary>
		/// <param name="format">Le format ("{0}, blbla {1}") ou la chaîne à afficher.</param>
		/// <param name="args">Les paramètres dans le cas d'un format au premier paramètre.</param>
		void Warn(string format, params object[] args);
		
		/// <summary>
		/// Affiche une information de déboguage.
		/// </summary>
		/// <param name="format">Le format ("{0}, blbla {1}") ou la chaîne à afficher.</param>
		/// <param name="args">Les paramètres dans le cas d'un format au premier paramètre.</param>
		void Trace(string format, params object[] args);
	}
}
