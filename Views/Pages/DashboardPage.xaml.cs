using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using UiDesktopApp2.Models;
using UiDesktopApp2.ViewModels.Pages;
using Wpf.Ui.Controls;

namespace UiDesktopApp2.Views.Pages
{
    /// <summary>
    /// SalaryPage.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class DashboardPage : INavigableView<DashboardViewModel>
    {
        public DashboardViewModel ViewModel { get; }

        public DashboardPage(DashboardViewModel viewModel)
        {
            ViewModel = viewModel;
            DataContext = this;

            ViewModel.PropertyChanged += ViewModel_PropertyChanged;
            InitializeComponent();    
        }

        private void ComboBox_DropDownOpened(object sender, EventArgs e)
        {
            var comboBox = sender as ComboBox;
            // 임시로 ItemsSource를 null로 설정
            comboBox.ItemsSource = null;
            // 변경된 ViewModel의 EmployeeNumber 컬렉션으로 재설정
            comboBox.ItemsSource = ViewModel.EmployeeNumber;

        }

        private void ViewModel_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            { 
                case "EsolutionPopulations":
                    this.dgGridLoadingControl.Visibility = Visibility.Collapsed;
                    this.dgGrid.Visibility = Visibility.Visible;
                    break;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
