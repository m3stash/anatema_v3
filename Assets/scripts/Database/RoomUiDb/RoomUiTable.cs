using System;
using System.Collections.Generic;
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
                [TopLayerElement] TEXT NOT NULL,
                [BottomLayerElement] TEXT NOT NULL,
                [MiddleLayerElement] TEXT NOT NULL
            )";
            tableManager.CreateTable(tableName, sqlQuery, dbconn);
        }

        public int Insert(RoomUIModel room) {
            try {
                using IDbCommand dbcmd = dbconn.CreateCommand();
                dbcmd.CommandText = $"INSERT INTO {tableName} (name, shape, difficulty, biome, topLayerElement, bottomLayerElement, middleLayerElement) VALUES (@Name, @Shape, @Difficulty, @Biome, @TopLayerElement, @BottomLayerElement, @MiddleLayerElement);";
                dbManager.AddParameter(dbcmd, "@Name", room.Name);
                dbManager.AddParameter(dbcmd, "@Shape", room.Shape);
                dbManager.AddParameter(dbcmd, "@Difficulty", room.Difficulty);
                dbManager.AddParameter(dbcmd, "@Biome", room.Biome);
                // Sérialiser la liste d'éléments en JSON
                string topLayerJson = JsonConvert.SerializeObject(room.TopLayer);
                dbManager.AddParameter(dbcmd, "@TopLayerElement", topLayerJson);
                string bottomLayerJson = JsonConvert.SerializeObject(room.BottomLayer);
                dbManager.AddParameter(dbcmd, "@BottomLayerElement", bottomLayerJson);
                string middleLayerJson = JsonConvert.SerializeObject(room.MiddleLayer);
                dbManager.AddParameter(dbcmd, "@MiddleLayerElement", middleLayerJson);
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
                dbcmd.CommandText = $"UPDATE {tableName} SET name = @Name, shape = @Shape, difficulty = @Difficulty, biome = @Biome, topLayerElement = @TopLayerElement, bottomLayerElement = @BottomLayerElement, middleLayerElement = @MiddleLayerElement WHERE id = @Id;";
                dbManager.AddParameter(dbcmd, "@Name", roomUi.Name);
                dbManager.AddParameter(dbcmd, "@Shape", roomUi.Shape);
                dbManager.AddParameter(dbcmd, "@Difficulty", roomUi.Difficulty);
                dbManager.AddParameter(dbcmd, "@Biome", roomUi.Biome);
                string topLayerJson = JsonConvert.SerializeObject(roomUi.TopLayer);
                dbManager.AddParameter(dbcmd, "@TopLayerElement", topLayerJson);
                string bottomLayerJson = JsonConvert.SerializeObject(roomUi.TopLayer);
                dbManager.AddParameter(dbcmd, "@BottomLayerElement", bottomLayerJson);
                string middleLayerJson = JsonConvert.SerializeObject(roomUi.TopLayer);
                dbManager.AddParameter(dbcmd, "@MiddleLayerElement", middleLayerJson);
                dbManager.AddParameter(dbcmd, "@Id", roomUi.Id);
                dbcmd.ExecuteNonQuery();
                return roomUi.Id;
            }
            catch (Exception e) {
                Debug.LogError($"Error updating room with ID {roomUi.Id}: {e.Message}");
                return -1;
            }
        }

        public List<RoomUIModel> SearchRoomsByParams(int? id = null, string name = null, string shape = null, string difficulty = null, string biome = null) {
            List<RoomUIModel> results = new List<RoomUIModel>();

            try {
                using (IDbCommand dbcmd = dbconn.CreateCommand()) {
                    string sqlQuery = "SELECT * FROM " + tableName;

                    List<string> conditions = new List<string>();
                    if (id.HasValue) conditions.Add("id = @Id");
                    if (!string.IsNullOrEmpty(name)) conditions.Add("name = @Name");
                    if (!string.IsNullOrEmpty(shape)) conditions.Add("shape = @Shape");
                    if (!string.IsNullOrEmpty(difficulty)) conditions.Add("difficulty = @Difficulty");
                    if (!string.IsNullOrEmpty(biome)) conditions.Add("biome = @Biome");

                    if (conditions.Count > 0) {
                        sqlQuery += " WHERE " + string.Join(" AND ", conditions.ToArray());
                    }

                    dbcmd.CommandText = sqlQuery;

                    if (id.HasValue) dbManager.AddParameter(dbcmd, "@Id", id.Value);
                    if (!string.IsNullOrEmpty(name)) dbManager.AddParameter(dbcmd, "@Name", name);
                    if (!string.IsNullOrEmpty(shape)) dbManager.AddParameter(dbcmd, "@Shape", shape);
                    if (!string.IsNullOrEmpty(difficulty)) dbManager.AddParameter(dbcmd, "@Difficulty", difficulty);
                    if (!string.IsNullOrEmpty(biome)) dbManager.AddParameter(dbcmd, "@Biome", biome);

                    using IDataReader reader = dbcmd.ExecuteReader();
                    while (reader.Read()) {
                        RoomUIModel room = new RoomUIModel(
                            reader.GetString(reader.GetOrdinal("name")),
                            reader.GetString(reader.GetOrdinal("shape")),
                            reader.GetString(reader.GetOrdinal("biome")),
                            reader.GetString(reader.GetOrdinal("difficulty")),
                            reader.GetInt32(reader.GetOrdinal("id")),
                            null,
                            null,
                            null
                        );
                        results.Add(room);
                    }
                }
            }
            catch (Exception e) {
                Debug.LogError($"Error searching rooms: {e.Message}");
            }

            return results;
        }

        public bool Delete(int id) {
            try {
                using IDbCommand dbcmd = dbconn.CreateCommand();
                dbcmd.CommandText = $"DELETE FROM {tableName} WHERE id = @Id;";
                dbManager.AddParameter(dbcmd, "@Id", id);
                dbcmd.ExecuteNonQuery();
                Debug.Log($"Room with ID {id} deleted successfully.");
                return true;
            }
            catch (Exception e) {
                Debug.LogError($"Error deleting room with ID {id}: {e.Message}");
                return false;
            }
        }

        public RoomUIModel GetRoomById(int id) {
            RoomUIModel room = null;
            try {
                using IDbCommand dbcmd = dbconn.CreateCommand();
                dbcmd.CommandText = $"SELECT * FROM {tableName} WHERE id = @Id;";
                dbManager.AddParameter(dbcmd, "@Id", id);
                using IDataReader reader = dbcmd.ExecuteReader();
                while (reader.Read()) {
                    List<GridElementModel> topLayer = JsonConvert.DeserializeObject<List<GridElementModel>>(reader.GetString(reader.GetOrdinal("topLayerElement")));
                    List<GridElementModel> bottomLayer = JsonConvert.DeserializeObject<List<GridElementModel>>(reader.GetString(reader.GetOrdinal("bottomLayerElement")));
                    List<GridElementModel> middleLayer = JsonConvert.DeserializeObject<List<GridElementModel>>(reader.GetString(reader.GetOrdinal("middleLayerElement")));
                    room = new RoomUIModel(
                        reader.GetString(reader.GetOrdinal("name")),
                        reader.GetString(reader.GetOrdinal("shape")),
                        reader.GetString(reader.GetOrdinal("biome")),
                        reader.GetString(reader.GetOrdinal("difficulty")),
                        reader.GetInt32(reader.GetOrdinal("id")),
                        topLayer,
                        bottomLayer,
                        middleLayer
                    );
                }
            }
            catch (Exception e) {
                Debug.LogError($"Error getting room with ID {id}: {e.Message}");
            }
            return room;
        }


    }
}