﻿using CensusManager.model;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CensusManager.helper
{
    class CensusContext
    {
        private static string database = "census.db";
        //数据库连接
        private static SQLiteConnection dbConnection;


        //创建一个连接到指定数据库
        public static void Connect()
        {
            if (!File.Exists(database))
            {
                SQLiteConnection.CreateFile(database);
            }
            dbConnection = new SQLiteConnection($"Data Source={database};Version=3;");
            dbConnection.Open();

            new SQLiteCommand("create table if not exists person (relation text, name text, id text, race text, address text)", dbConnection).ExecuteNonQuery();
            new SQLiteCommand("create table if not exists  village (guid text, name text)", dbConnection).ExecuteNonQuery();
            new SQLiteCommand("create table if not exists  build (guid text, mid text, number text, villageGuid text)", dbConnection).ExecuteNonQuery();
        }

        public static void DisConnect()
        {
            dbConnection.Close();
        }

        #region 住户操作
        public static void CreateTablePerson()
        {
            string sql = "create table if not exists person (relation text, name text, id text, race text, address text)";
            SQLiteCommand command = new SQLiteCommand(sql, dbConnection);
            command.ExecuteNonQuery();
        }
        public static void AddPerson(Person model)
        {
            string sql = $"insert into person (relation, name, id,race,address) values ('{model.relation}','{model.name}','{model.id}','{model.race}','{model.address}')";
            SQLiteCommand command = new SQLiteCommand(sql, dbConnection);
            command.ExecuteNonQuery();
        }
        public static void AddPerson(List<Person> models)
        {
            foreach (var mm in models)
            {
                string sql = $"insert into person (relation, name, id,race,address) values ('{mm.relation}','{mm.name}','{mm.id}','{mm.race}','{mm.address}')";
                SQLiteCommand command = new SQLiteCommand(sql, dbConnection);
                command.ExecuteNonQuery();
            }
        }

        public static List<Person> GetPersons(string village, string build)
        {
            List<Person> result = new List<Person>();
            string sql = $"select * from person where address like '%{village}{build}'";
            SQLiteCommand command = new SQLiteCommand(sql, dbConnection);
            SQLiteDataReader reader = command.ExecuteReader();
            while (reader.Read())
                result.Add(new Person(reader["relation"].ToString(), reader["name"].ToString(), reader["id"].ToString(), reader["race"].ToString(), reader["address"].ToString()));
            return result;
        }
        public static bool IsExistPersons(string id)
        {
            string sql = $"select * from person where id = '{id}'";
            SQLiteCommand command = new SQLiteCommand(sql, dbConnection);
            SQLiteDataReader reader = command.ExecuteReader();
            while (reader.Read())
                return true;
            return false;
        }
        #endregion

        #region 村庄操作
        public static void CreateTableVillage()
        {
            string sql = "create table if not exists  village (guid text, name text)";
            SQLiteCommand command = new SQLiteCommand(sql, dbConnection);
            command.ExecuteNonQuery();
        }
        public static void AddVillage(Village model)
        {
            string sql = $"insert into village (guid, name) values ('{model.guid}','{model.name}')";
            SQLiteCommand command = new SQLiteCommand(sql, dbConnection);
            command.ExecuteNonQuery();
        }
        public static void AddVillages(List<Village> models)
        {
            foreach (var mm in models)
            {
                string sql = $"insert into village (guid, name) values ('{mm.guid}','{mm.name}')";
                SQLiteCommand command = new SQLiteCommand(sql, dbConnection);
                command.ExecuteNonQuery();
            }
        }
        public static Village GetVillage(string villageName)
        {
            Village result = null;
            string sql = $"select * from village where name='{villageName}'";
            SQLiteCommand command = new SQLiteCommand(sql, dbConnection);
            SQLiteDataReader reader = command.ExecuteReader();
            if(reader.Read())
            result = new Village(reader["guid"].ToString(), reader["name"].ToString());
            return result;
        }
        public static List<Village> GetVillages()
        {
            List<Village> result = new List<Village>();
            string sql = "select * from village";
            SQLiteCommand command = new SQLiteCommand(sql, dbConnection);
            SQLiteDataReader reader = command.ExecuteReader();
            while (reader.Read())
                result.Add(new Village(reader["guid"].ToString(), reader["name"].ToString()));
            return result;
        }
        public static bool IsExistVillage(string name)
        {
            string sql = $"select * from village where name = '{name}'";
            SQLiteCommand command = new SQLiteCommand(sql, dbConnection);
            SQLiteDataReader reader = command.ExecuteReader();
            while (reader.Read())
                return true;
            return false;
        }
        #endregion

        #region 房屋操作
        public static void CreateTableBuild()
        {
            string sql = "create table if not exists  build (guid text, mid text, number text, villageGuid text)";
            SQLiteCommand command = new SQLiteCommand(sql, dbConnection);
            command.ExecuteNonQuery();
        }
        public static void AddBuild(Build model)
        {
            string sql = $"insert into build (guid, mid, number, villageGuid) values ('{model.guid}','{model.mid}','{model.number}','{model.villageGuid}')";
            SQLiteCommand command = new SQLiteCommand(sql, dbConnection);
            command.ExecuteNonQuery();
        }
        public static void AddBuilds(List<Build> models)
        {
            foreach (var mm in models)
            {
                string sql = $"insert into build (guid, mid, number, villageGuid) values ('{mm.guid}','{mm.mid}','{mm.number}','{mm.villageGuid}')";
                SQLiteCommand command = new SQLiteCommand(sql, dbConnection);
                command.ExecuteNonQuery();
            }
        }
        public static List<Build> GetBuilds(string villageGuid)
        {
            List<Build> result = new List<Build>();
            string sql = $"select * from build where villageGuid = '{villageGuid}'";
            SQLiteCommand command = new SQLiteCommand(sql, dbConnection);
            SQLiteDataReader reader = command.ExecuteReader();
            while (reader.Read())
                result.Add(new Build(reader["guid"].ToString(), reader["mid"].ToString(), reader["number"].ToString(), reader["villageGuid"].ToString()));
            return result;
        }
        public static bool IsExistBuild(string guid)
        {
            string sql = $"select * from build where guid = '{guid}'";
            SQLiteCommand command = new SQLiteCommand(sql, dbConnection);
            SQLiteDataReader reader = command.ExecuteReader();
            while (reader.Read())
                return true;
            return false;
        }
        #endregion
    }
}
