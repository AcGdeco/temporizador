using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SQLite;

namespace Temporizador.Models
{
    public class Receita
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public string Nome { get; set; }

        public int Tempo { get; set; }

        // Nova coluna
        public string Cor { get; set; }
    }
}

