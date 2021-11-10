using System;
using System.Windows.Controls;

namespace TomHogue.CarSetupPlugin
{
    /// <summary>
    /// Logique d'interaction pour SettingsControlDemo.xaml
    /// </summary>
    public partial class SettingsControlDemo : UserControl
    {
        public CarSetupPlugin Plugin { get; }

        public SettingsControlDemo()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            throw new NotImplementedException();
        }

        public SettingsControlDemo(CarSetupPlugin plugin) : this()
        {
            this.Plugin = plugin;
        }


    }
}
