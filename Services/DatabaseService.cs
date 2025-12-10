using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;
using System.IO;
using Temporizador.Models;

namespace Temporizador.Services
{
    public static class DatabaseService
    {
        static SQLiteConnection _database;

        public static SQLiteConnection Database
        {
            get
            {
                if (_database == null)
                {
                    var dbPath = Path.Combine(FileSystem.AppDataDirectory, "receitas.db3");
                    _database = new SQLiteConnection(dbPath);
                    // Dropa a tabela antiga
                    //_database.DropTable<Receita>();
                    _database.CreateTable<Receita>();
                }
                return _database;
            }
        }

        public static void AdicionarReceita(string nome, int tempo)
        {
            var novaReceita = new Receita { Nome = nome, Tempo = tempo };
            Database.Insert(novaReceita);
        }

        public static List<Receita> ListarReceitas()
        {
            return Database.Table<Receita>().ToList();
        }
    }
}

