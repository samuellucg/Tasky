using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tasky.ViewModels;

namespace Tasky.Models
{
    public class UserOp : BaseViewModel
    {

        // remover??????

        private static string UserName = Tasky.Properties.Settings.Default.UserName;

        public string Presentation = string.Format("Olá {0},\no que vamos fazer hoje?", UserName);

        // fazer função que olha a hora do pc, se pra saber se é bom dia,tarde ou noite.

    }
}
