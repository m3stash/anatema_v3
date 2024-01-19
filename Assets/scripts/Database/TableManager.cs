using System;
using System.Data;
using UnityEngine;

namespace Database {
    public class TableManager {

        public void CreateTable(string tableName, string sqlQuery, IDbConnection dbconn) {
            try {
                using (IDbCommand dbcmd = dbconn.CreateCommand()) {
                    dbcmd.CommandText = sqlQuery;
                    dbcmd.ExecuteScalar();
                    Debug.Log($"{tableName} table creation attempted (if not exists).");
                }
            } catch (Exception e) {
                Debug.LogError($"Error creating {tableName} table: {e.Message}");
            } finally {
                if (dbconn.State == ConnectionState.Open) {
                    dbconn.Close();
                }
            }
        }
    }
}