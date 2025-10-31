using System.Configuration;
using System.Data;
using System.Windows;
using WpfApp.ViewModels;
using System.Globalization;
using System.Threading;
using System.Windows.Markup;

namespace WpfApp
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {

            var cultura = new CultureInfo("pt-BR");
            Thread.CurrentThread.CurrentCulture = cultura;
            Thread.CurrentThread.CurrentUICulture = cultura;

            FrameworkElement.LanguageProperty.OverrideMetadata(
                typeof(FrameworkElement),
                new FrameworkPropertyMetadata(
                    XmlLanguage.GetLanguage(cultura.IetfLanguageTag)));


            base.OnStartup(e);

            var mainWindow = new MainWindow(); 
            mainWindow.DataContext = new MainViewModel();
            mainWindow.Show();
        }
    }
}