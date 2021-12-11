using Microsoft.Extensions.DependencyInjection;

namespace FileEncryptor.WPF.ViewModels
{
    internal class ViewModelLocator
    {
        public MainWindowViewModel MainWindowVM => App.Services.GetRequiredService<MainWindowViewModel>();
    }
}
