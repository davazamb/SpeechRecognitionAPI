using Microsoft.CognitiveServices.SpeechRecognition;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading;
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

namespace SpeechWPF
{
    /// <summary>
    /// Lógica de interacción para MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        AutoResetEvent _FinalResponseEvent;
        MicrophoneRecognitionClient _microphoneRecognitionClient;
        public MainWindow()
        {
            InitializeComponent();
            RecordButton.Content = "Iniciar\nGrabacion";
            _FinalResponseEvent = new AutoResetEvent(false);
            OutputTextbox.Background = Brushes.White;
            OutputTextbox.Foreground = Brushes.Black;

        }

        private void RecordButton_Click(object sender, RoutedEventArgs e)
        {
            RecordButton.Content = "Escuchando...";
            RecordButton.IsEnabled = false;
            OutputTextbox.Background = Brushes.Green;
            OutputTextbox.Foreground = Brushes.White;
            ConvertSpeechToText();
        }

        private void ConvertSpeechToText()
        {
            var speechRecognitionMode = SpeechRecognitionMode.ShortPhrase;
            string language = "es-es";
            string subscriptionkey = ConfigurationManager.AppSettings["SpeechKey"].ToString();


            _microphoneRecognitionClient = SpeechRecognitionServiceFactory.CreateMicrophoneClient
                (speechRecognitionMode, language, subscriptionkey);

            _microphoneRecognitionClient.OnPartialResponseReceived += OnPartialResponseReceivedHandler;
            _microphoneRecognitionClient.OnResponseReceived += OnMicShortPhraseResponseReceivedHandler;
            _microphoneRecognitionClient.StartMicAndRecognition();

        }

        private void OnPartialResponseReceivedHandler(object sender, PartialSpeechResponseEventArgs e)
        {
            string result = e.PartialResult;
            Dispatcher.Invoke(() =>
                {
                    OutputTextbox.Text = (e.PartialResult);
                    OutputTextbox.Text += ("\n");

                    

                });
        }

        private void OnMicShortPhraseResponseReceivedHandler(object sender, SpeechResponseEventArgs e)
        {
            Dispatcher.Invoke((Action)(() =>
            {
                _FinalResponseEvent.Set();
                _microphoneRecognitionClient.EndMicAndRecognition();
                _microphoneRecognitionClient.Dispose();
                _microphoneRecognitionClient = null;
                RecordButton.Content = "Iniciar\nGrabacion";
                RecordButton.IsEnabled = true;
                OutputTextbox.Background = Brushes.White;
                OutputTextbox.Foreground = Brushes.Black;

            }));
        }
    }
}
