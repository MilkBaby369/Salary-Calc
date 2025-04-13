using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Wpf.Ui.Controls;
using UiDesktopApp2.ViewModels.Pages;
using MyApp;
using System.IO;
using MyTextBlock = System.Windows.Controls.TextBlock; // 네임스페이스 별칭 지정
using static System.Net.Mime.MediaTypeNames;

namespace UiDesktopApp2.Views.Pages
{
    /// <summary>
    /// EmailPage.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class DataPage : INavigableView<DataViewModel>
    {
        public DataViewModel ViewModel { get; }

        private void ComboBox_DropDownOpened(object sender, EventArgs e)
        {
            var comboBox = sender as ComboBox;
            // 임시로 ItemsSource를 null로 설정
            comboBox.ItemsSource = null;
            // 변경된 ViewModel의 EmployeeNumber 컬렉션으로 재설정
            comboBox.ItemsSource = ViewModel.EmployeeNumber;

        }
        public DataPage(DataViewModel viewModel)
        {
            InitializeComponent(); // 먼저 InitializeComponent를 호출합니다.

            ViewModel = viewModel;
            DataContext = this; // ViewModel을 DataContext로 설정

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            // 캡쳐 파일 저장 경로 설정 (예: 임시 폴더)
            string capturePath = System.IO.Path.Combine("C:\\Users\\이태경\\Test", "SalarySlip.png");

            // 캡쳐 메서드 호출 (SalarySlipGrid는 UI 요소 이름)
            CaptureUIElement(SalarySlipGrid, capturePath);

            // EmailWindow에 파일 경로 전달
            EmailWindow emailWindow = new EmailWindow(capturePath);
            emailWindow.ShowDialog(); // 기존 ui 창을 닫지 않고 대화상자처럼 띄우고 이전 창을 비활성화
        }

        public void CaptureUIElement(UIElement element, string savePath)
        {
            // UI 요소 크기 기반으로 RenderTargetBitmap 생성
            var width = (int)element.RenderSize.Width;
            var height = (int)element.RenderSize.Height;
            RenderTargetBitmap renderBitmap = new RenderTargetBitmap(width, height, 96, 96, PixelFormats.Pbgra32);
            renderBitmap.Render(element);

            // PNG 형식으로 저장
            PngBitmapEncoder encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(renderBitmap));
            using (FileStream fs = new FileStream(savePath, FileMode.Create))
            {
                encoder.Save(fs);
            }
        }

    }
}
