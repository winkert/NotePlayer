using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

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
            playerType = Player.PlayerType.Single;
        }
        private List<Player.PlayerWorker> workers = new List<Player.PlayerWorker>();
        private char keyPressed = (char)0;
        private Player.PlayerType playerType;

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

        private int GetPlayerIndex(char key)
        {
            for(int i = 0; i < workers.Count; i++)
            {
                if (workers[i].Key.KeyPressed == key)
                    return i;
            }
            return 0;
        }

        public void HoldNote(char note)
        {
            Label noteheld = new Label();
            noteheld.Content = "Note: " + note;
            noteheld.Tag = note;
            stc_Notes.Children.Add(NewLabel(noteheld));
            Player.PlayerWorker worker = new Player.PlayerWorker(new KeyboardNote(note.ToString(), 3, 220, note));
            workers.Add(worker);
            worker.StartPlaying();
        }
        public void ReleaseNote(char note)
        {
            stc_Notes.Children.Remove(stc_Notes.Children.OfType<Expander>().Where(k => (char)k.Tag == note).First());
            workers[GetPlayerIndex(note)].StopPlaying();
            workers.RemoveAt(GetPlayerIndex(note));
        }
        private Expander NewLabel(Label n)
        {
            Expander note = new Expander();
            note.Tag = n.Tag;
            note.Header = n.Content;
            note.Content = n;
            return note;
        }
        private bool IsKeyDown(Key key)
        {
            if (stc_Notes.Children.Count == 0)
                return false;
            char note = KeyReader.KeyToChar(key);
            if (stc_Notes.Children.OfType<Expander>().Where(k => (char)k.Tag == note).FirstOrDefault() == null)
                return false;
            else return true;
        }

        private void wnd_Main_TextInput(object sender, TextCompositionEventArgs e)
        {
            if(playerType == Player.PlayerType.Single)
            {
                //Console.WriteLine(e.Text);
                keyPressed = (char)Encoding.ASCII.GetBytes(e.Text)[0];
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
            }
        }

        private void MenuItem_reset_Click(object sender, RoutedEventArgs e)
        {
            txt_History.Text = string.Empty;
        }

        private void MenuItem_exit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void chk_Play_Piano_Checked(object sender, RoutedEventArgs e)
        {
            if(chk_Play_Organ.IsChecked)
            {
                chk_Play_Organ.IsChecked = false;
                playerType = Player.PlayerType.Single;
            }
            if (!chk_Play_Piano.IsChecked)
                chk_Play_Piano.IsChecked = true;
        }

        private void chk_Play_Organ_Checked(object sender, RoutedEventArgs e)
        {
            if (chk_Play_Piano.IsChecked)
            {
                chk_Play_Piano.IsChecked = false;
                playerType = Player.PlayerType.Continuous;
            }
            if (!chk_Play_Organ.IsChecked)
                chk_Play_Organ.IsChecked = true;
        }

        private void wnd_Main_KeyDown(object sender, KeyEventArgs e)
        {
            if (playerType == Player.PlayerType.Continuous && !IsKeyDown(e.Key))
            {
                HoldNote(KeyReader.KeyToChar(e.Key));
            }
        }

        private void wnd_Main_KeyUp(object sender, KeyEventArgs e)
        {
            if (playerType == Player.PlayerType.Continuous)
            {
                e.Handled = true;
                ReleaseNote(KeyReader.KeyToChar(e.Key));
            }
        }
    }
}
