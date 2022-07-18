﻿using Develeon64.MacroBot.Models;
using System.Data.SQLite;

namespace Develeon64.MacroBot.Services {
	public static class DatabaseManager {
		private static SQLiteConnection database;

		public static async Task Initialize (string filepath) {
			database = new SQLiteConnection(new SQLiteConnectionStringBuilder() {
				DataSource = filepath,
				DateTimeFormat = SQLiteDateFormats.UnixEpoch,
				DateTimeKind = DateTimeKind.Utc,
				DefaultDbType = System.Data.DbType.Object,
				FailIfMissing = false,
				ForeignKeys = true,
				SyncMode = SynchronizationModes.Full,
			}.ToString());

			try {
				await database.OpenAsync();
				using (SQLiteCommand command = database.CreateCommand()) {
					command.CommandText = "CREATE TABLE IF NOT EXISTS 'Tickets' ('Author' INTEGER, 'Channel' INTEGER UNIQUE, 'Message' INTEGER UNIQUE, 'Created' INTEGER, 'Modified' INTEGER, PRIMARY KEY('Author'));";
					await command.ExecuteNonQueryAsync();
				}
			}
			catch (SQLiteException ex) { }
			catch (Exception ex) { }
			finally {
				await database.CloseAsync();
			}
		}

		public static async Task<bool> TicketExists (ulong author) {
			bool exists = false;
			try {
				await DatabaseManager.database.OpenAsync();
				using (SQLiteCommand cmd = database.CreateCommand()) {
					cmd.CommandText = $"SELECT * FROM 'Tickets' WHERE 'Tickets'.'Author' == {author}";
					exists = await cmd.ExecuteScalarAsync() != null;
				}
			}
			catch (SQLiteException ex) { }
			catch (Exception ex) { }
			finally {
				await database.CloseAsync();
			}
			return exists;
		}

		public static async Task CreateTicket (ulong author, ulong channel, ulong message, DateTimeOffset created) {
			try {
				await DatabaseManager.database.OpenAsync();
				using (SQLiteCommand command = database.CreateCommand()) {
					command.CommandText = $"INSERT INTO 'Tickets' VALUES ({author}, {channel}, {message}, {created.ToUnixTimeSeconds()}, {created.ToUnixTimeSeconds()});";
					await command.ExecuteNonQueryAsync();
				}
			}
			catch (SQLiteException ex) { }
			catch (Exception ex) { }
			finally {
				await database.CloseAsync();
			}
		}

		public static async Task CreateTicket (ulong author, ulong channel, ulong message, DateTime created) {
			await DatabaseManager.CreateTicket(author, channel, message, (DateTimeOffset)created.ToUniversalTime());
		}

		public static async Task CreateTicket (ulong author, ulong channel, ulong message) {
			await DatabaseManager.CreateTicket(author, channel, message, DateTimeOffset.Now);
		}

		public static async Task DeleteTicket (ulong id, IdType type) {
			if (type is not IdType.Channel and not IdType.User) return;

			try {
				await DatabaseManager.database.OpenAsync();
				using (SQLiteCommand command = database.CreateCommand()) {
					command.CommandText = $"DELETE FROM 'Tickets' WHERE 'Tickets'.'{(type is IdType.User ? "Author" : "Channel")}' == {id};";
					await command.ExecuteNonQueryAsync();
				}
			}
			catch (SQLiteException ex) { }
			catch (Exception ex) { }
			finally {
				await database.CloseAsync();
			}
		}

		public static async Task UpdateTicket (ulong author) {
			try {
				await DatabaseManager.database.OpenAsync();
				using (SQLiteCommand command = database.CreateCommand()) {
					command.CommandText = $"UPDATE 'Tickets' SET 'Modified' = {DateTimeOffset.Now.ToUnixTimeSeconds()} WHERE 'Tickets'.'Author' == {author};";
					await command.ExecuteNonQueryAsync();
				}
			}
			catch (SQLiteException ex) { }
			catch (Exception ex) { }
			finally {
				await database.CloseAsync();
			}
		}

		public static async Task<List<Ticket>> GetTickets () {
			List<Ticket> tickets = new();
			try {
				await DatabaseManager.database.OpenAsync();
				using (SQLiteCommand command = database.CreateCommand()) {
					command.CommandText = $"SELECT * FROM 'Tickets'";
					var reader = await command.ExecuteReaderAsync();
					while (reader.Read()) {
						tickets.Add(new Ticket() {
							Author = (ulong)reader.GetInt64(0),
							Channel = (ulong)reader.GetInt64(1),
							Message = (ulong)reader.GetInt64(2),
							Created = DateTimeOffset.FromUnixTimeSeconds((long)reader.GetValue(3)).DateTime,
							Modified = DateTimeOffset.FromUnixTimeSeconds((long)reader.GetValue(4)).DateTime,
						});
					}
				}
			}
			catch (SQLiteException ex) { }
			catch (Exception ex) { }
			finally {
				await database.CloseAsync();
			}

			return tickets;
		}

		public static async Task<Ticket> GetTicket (ulong author) {
			foreach (Ticket ticket in await DatabaseManager.GetTickets()) {
				if (ticket.Author == author) return ticket;
			}
			return null;
		}
	}

	public enum IdType {
		User,
		Channel,
		Guild,
		Message,
	}
}