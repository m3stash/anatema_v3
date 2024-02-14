using System;
using System.Data;
using Database;
using Newtonsoft.Json;
using UnityEngine;

namespace RoomUI {

    public class RoomUiTable {
        private IDbConnection dbconn;
        private readonly string tableName = "room_table";
        private TableManager tableManager;
        private DatabaseManager dbManager;

        public RoomUiTable(DatabaseManager dbManager) {
            this.dbManager = dbManager;
            dbconn = dbManager.GetConnection();
            tableManager = dbManager.GetTableManager();
        }

        public string GetTableName() {
            return tableName;
        }

        public void CreateTableRoom() {
            string sqlQuery = $@"CREATE TABLE IF NOT EXISTS {tableName} (
                [id] INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
                [Name] TEXT NOT NULL,
                [Shape] TEXT NOT NULL,
                [Difficulty] TEXT NOT NULL,
                [Biome] TEXT NOT NULL,
                [TopLayerElement] TEXT NOT NULL
            )";
            tableManager.CreateTable(tableName, sqlQuery, dbconn);
        }

        public int Insert(RoomUIModel room) {
            try {
                using IDbCommand dbcmd = dbconn.CreateCommand();
                dbcmd.CommandText = $"INSERT INTO {tableName} (name, shape, difficulty, biome, topLayerElement) VALUES (@Name, @Shape, @Difficulty, @Biome, @TopLayerElement);";
                dbManager.AddParameter(dbcmd, "@Name", room.Name);
                dbManager.AddParameter(dbcmd, "@Shape", room.Shape);
                dbManager.AddParameter(dbcmd, "@Difficulty", room.Difficulty);
                dbManager.AddParameter(dbcmd, "@Biome", room.Biome);
                // Sérialiser la liste d'éléments en JSON
                string elementsJson = JsonConvert.SerializeObject(room.TopLayer);
                dbManager.AddParameter(dbcmd, "@TopLayerElement", elementsJson);
                dbcmd.ExecuteNonQuery();

                dbcmd.CommandText = "SELECT last_insert_rowid()";
                object result = dbcmd.ExecuteScalar();

                if (result != null && result != DBNull.Value) {
                    int insertedId = Convert.ToInt32(result);
                    Debug.Log($"Room inserted successfully with ID: {insertedId}");
                    return insertedId;
                }
                else {
                    Debug.LogError("Failed to retrieve inserted ID.");
                    return -1;
                }
            }
            catch (Exception e) {
                Debug.LogError($"Error inserting data: {e.Message}");
                return -1;
            }
        }

        public int Update(RoomUIModel roomUi) {
            try {
                using IDbCommand dbcmd = dbconn.CreateCommand();
                dbcmd.CommandText = $"UPDATE {tableName} SET name = @Name, shape = @Shape, difficulty = @Difficulty, biome = @Biome, topLayerElement = @TopLayerElement WHERE id = @Id;";
                dbManager.AddParameter(dbcmd, "@Name", roomUi.Name);
                dbManager.AddParameter(dbcmd, "@Shape", roomUi.Shape);
                dbManager.AddParameter(dbcmd, "@Difficulty", roomUi.Difficulty);
                dbManager.AddParameter(dbcmd, "@Biome", roomUi.Biome);
                string elementsJson = JsonConvert.SerializeObject(roomUi.TopLayer);
                dbManager.AddParameter(dbcmd, "@TopLayerElement", elementsJson);
                dbManager.AddParameter(dbcmd, "@Id", roomUi.Id);
                dbcmd.ExecuteNonQuery();
                Debug.Log($"Room with ID {roomUi.Id} updated successfully.");
                return roomUi.Id;
            }
            catch (Exception e) {
                Debug.LogError($"Error updating room with ID {roomUi.Id}: {e.Message}");
                return -1;
            }
        }


    }
}