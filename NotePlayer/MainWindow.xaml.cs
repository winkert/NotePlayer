using System.ComponentModel;
using System.Text;
using System.Windows;

namespace NotePlayer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            playNote.DoWork += PlayNote_DoWork;
            playNote.RunWorkerCompleted += PlayNote_RunWorkerCompleted;
        }
        private char keyPressed = '0';

        private void PlayNote_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (KeyReader.Keys.ContainsKey(keyPressed))
                txt_History.Text += " " + KeyReader.Keys[keyPressed].ToString();
        }

        private void PlayNote_DoWork(object sender, DoWorkEventArgs e)
        {
            if (KeyReader.Keys.ContainsKey(keyPressed))
            {
                KeyReader.PlayNote(KeyReader.Keys[keyPressed]);
            }
        }

        private BackgroundWorker playNote = new BackgroundWorker();
        private void txt_Input_TextInput(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            if (txt_Input.Text.Length < 1)
                return;
            keyPressed = (char)Encoding.ASCII.GetBytes(txt_Input.Text)[0];
            if (!playNote.IsBusy)
            {
                playNote.RunWorkerAsync();
            }
            else
            {
                BackgroundWorker bgPlayer = new BackgroundWorker();
                bgPlayer.DoWork += PlayNote_DoWork;
                bgPlayer.RunWorkerCompleted += PlayNote_RunWorkerCompleted;
                bgPlayer.RunWorkerAsync();
            }
            txt_Input.Text = string.Empty;
        }
    }
}
