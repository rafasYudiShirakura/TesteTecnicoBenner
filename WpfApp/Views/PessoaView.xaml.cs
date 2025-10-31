using System.Windows.Controls;

namespace WpfApp.Views
{
    public partial class PessoaView : UserControl
    {
        public PessoaView()
        {
            InitializeComponent();

            this.IsVisibleChanged += (s, e) =>
            {
                if ((bool)e.NewValue == true)
                {
                    if (DataContext is WpfApp.ViewModels.PessoaViewModel vm)
                    {
                        vm.AtualizarDadosPedidos();
                    }
                }
            };
        }
    }
}