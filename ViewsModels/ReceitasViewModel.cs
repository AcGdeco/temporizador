using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using Temporizador.Models;
using Temporizador.Services;

namespace Temporizador.ViewModels
{
    public class ReceitasViewModel
    {
        public ObservableCollection<Receita> Receitas { get; set; }

        public ReceitasViewModel()
        {
            Receitas = new ObservableCollection<Receita>(DatabaseService.ListarReceitas());
        }

        public void AdicionarReceita(string nome, int tempo)
        {
            DatabaseService.AdicionarReceita(nome, tempo);
            Receitas.Clear();
            foreach (var r in DatabaseService.ListarReceitas())
                Receitas.Add(r);
        }
    }
}

