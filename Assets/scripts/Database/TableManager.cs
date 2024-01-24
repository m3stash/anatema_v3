using System;
using System.Data;
using UnityEngine;

namespace Database {
    public class TableManager {

        public void EnableForeignKeys(IDbConnection dbconn) {
            try {
                using IDbCommand pragmaCmd = dbconn.CreateCommand();
                pragmaCmd.CommandText = "PRAGMA foreign_keys = ON;";
                pragmaCmd.ExecuteNonQuery();
                Debug.Log("Foreign keys enabled.");
            } catch (Exception e) {
                Debug.LogError($"Error enabling foreign keys: {e.Message}");
            }
        }

        public void CreateTable(string tableName, string sqlQuery, IDbConnection dbconn) {
            try {
                // EnableForeignKeys(dbconn);
                using IDbCommand dbcmd = dbconn.CreateCommand();
                dbcmd.CommandText = sqlQuery;
                dbcmd.ExecuteScalar();
                Debug.Log($"{tableName} table creation attempted (if not exists).");
            } catch (Exception e) {
                Debug.LogError($"Error creating {tableName} table: {e.Message}");
            }
        }

    }
}