using CircusAlina.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace CircusAlina
{
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static CircusAlina.Models.CircusAlinaTahEntities2 db = new CircusAlinaTahEntities2();
    }
}
