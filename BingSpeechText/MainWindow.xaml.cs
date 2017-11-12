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
using System.Media;
using System.Threading;
using System.Windows.Threading;
using Microsoft.CognitiveServices.SpeechRecognition;

namespace BingSpeechText
{
    /// <summary>
    /// Lógica de interacción para MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private MicrophoneRecognitionClient micClient;
        public MainWindow()
        {
            InitializeComponent();
            this.micClient = SpeechRecognitionServiceFactory.CreateMicrophoneClient(SpeechRecognitionMode.ShortPhrase, "en-US", "1d802aaea3b54dc5a18fd04014ae490b");
            this.micClient.OnMicrophoneStatus += MicClient_OnMicrophoneStatus;
            this.micClient.OnResponseReceived += MicClient_OnResponseReceived;
        }

        private async void MicClient_OnResponseReceived(object sender, SpeechResponseEventArgs e)
        {
            if(e.PhraseResponse.Results.Length > 0)
            {
                await Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                    new Action(() =>
                    {
                        this.MySpeechResponse.Text = $"'{e.PhraseResponse.Results[0].DisplayText}',";
                        this.MySpeechResponseConfidence.Text = $"confidence:{e.PhraseResponse.Results[0].Confidence}";
                        //this.Speak(this.MySpeechResponse.Text);
                    }));
            }
        }

        private void MicClient_OnMicrophoneStatus(object sender, MicrophoneEventArgs e)
        {
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                new Action(() =>
                   {
                       if (e.Recording)
                       {
                           this.status.Text = "Escuchando...";
                           this.RecordingBar.Visibility = Visibility.Visible;
                       }
                       else
                       {
                           this.status.Text = "No Escuchando...";
                           this.RecordingBar.Visibility = Visibility.Collapsed;
                       }
                   }));
        }

        private void button_click(object sender, RoutedEventArgs e)
        {
            this.MySpeechResponse.Text = string.Empty;
            this.MySpeechResponseConfidence.Text = string.Empty;
            this.micClient.StartMicAndRecognition();
        }
    }
}
