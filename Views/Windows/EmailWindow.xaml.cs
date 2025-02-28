using System;
using System.Net;
using System.Net.Mail;
using System.Windows;

namespace MyApp
{
    public partial class EmailWindow : Window
    {
        private readonly string _attachmentPath;  // 첨부파일 경로 저장

        public EmailWindow(string attachmentPath) : this()
        {
            _attachmentPath = attachmentPath;
        }

        public EmailWindow()
        {
            InitializeComponent();

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;  // TLS 1.2 설정

            this.Dispatcher.BeginInvoke(new Action(() =>
            {
                // 여기서 강제 redraw를 위한 추가 처리가 가능.
                this.InvalidateVisual();
            }), System.Windows.Threading.DispatcherPriority.Background);

        }

        private void SendEmail_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // 사용자 입력 값 읽기
                string senderEmail = txtSender.Text.Trim();
                string recipientEmail = txtRecipient.Text.Trim();
                string subject = txtSubject.Text;
                string body = txtBody.Text;

                // MailMessage 객체 생성 및 설정
                MailMessage mail = new MailMessage();
                mail.From = new MailAddress(senderEmail);
                mail.To.Add(recipientEmail);
                mail.Subject = subject;
                mail.Body = body;

                // 첨부파일 추가 (첨부파일 경로가 존재하는 경우)
                if (!string.IsNullOrEmpty(_attachmentPath) && System.IO.File.Exists(_attachmentPath))
                {
                    Attachment attachment = new Attachment(_attachmentPath);
                    mail.Attachments.Add(attachment);
                }

                // SMTP 클라이언트 설정 (예: Gmail)
 
                SmtpClient smtpClient = new SmtpClient("smtp.gmail.com",587);  // Gmail SMTP 서버
                smtpClient.Credentials = new NetworkCredential("ehtm2323161953@gmail.com", "ivczwwsobzxscmko");
                smtpClient.EnableSsl = true;

                smtpClient.Send(mail);
                MessageBox.Show("이메일이 전송되었습니다.", "성공", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("이메일 전송 중 오류 발생: " + ex.Message, "오류", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}

