using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Input;
using System.Threading;
using System.Linq;
using System.Runtime.InteropServices;

namespace NotePlayer
{
    public static class KeyReader
    {
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        internal static extern int MapVirtualKey(int uCode, int uMapType);

        #region Notes
        private static KeyboardNote BassC = new KeyboardNote("C", 3, 131);
        private static KeyboardNote BassCsharp = new KeyboardNote("C#", 3, 139);
        private static KeyboardNote BassD = new KeyboardNote("D", 3, 147);
        private static KeyboardNote BassDsharp = new KeyboardNote("D#", 3, 156);
        private static KeyboardNote BassE = new KeyboardNote("E", 3, 165);
        private static KeyboardNote BassF = new KeyboardNote("F", 3, 175);
        private static KeyboardNote BassFsharp = new KeyboardNote("F#", 3, 185);
        private static KeyboardNote BassG = new KeyboardNote("G", 3, 196);
        private static KeyboardNote BassGsharp = new KeyboardNote("G#", 3, 208);
        private static KeyboardNote BassA = new KeyboardNote("A", 3, 220);
        private static KeyboardNote BassAsharp = new KeyboardNote("A#", 3, 233);
        private static KeyboardNote BassB = new KeyboardNote("B", 3, 247);
        private static KeyboardNote MiddleC = new KeyboardNote("C", 4, 262);
        private static KeyboardNote MiddleCsharp = new KeyboardNote("C#", 4, 277);
        private static KeyboardNote MiddleD = new KeyboardNote("D", 4, 294);
        private static KeyboardNote MiddleDsharp = new KeyboardNote("D#", 4, 311);
        private static KeyboardNote MiddleE = new KeyboardNote("E", 4, 330);
        private static KeyboardNote MiddleF = new KeyboardNote("F", 4, 349);
        private static KeyboardNote MiddleFsharp = new KeyboardNote("F#", 4, 370);
        private static KeyboardNote MiddleG = new KeyboardNote("G", 4, 392);
        private static KeyboardNote MiddleGsharp = new KeyboardNote("G#", 4, 415);
        private static KeyboardNote MiddleA = new KeyboardNote("A", 4, 440);
        private static KeyboardNote MiddleAsharp = new KeyboardNote("A#", 4, 466);
        private static KeyboardNote MiddleB = new KeyboardNote("B", 4, 494);
        #endregion

        /// <summary>
        /// Dictionary of a letter and a corresponding piano key
        /// based on the location of the key on a QWERTY keyboard.
        /// </summary>
        public static Dictionary<char, KeyboardNote> Keys = buildKeys();
        private static Dictionary<char, KeyboardNote> buildKeys()
        {
            Dictionary<char, KeyboardNote> k = new Dictionary<char, KeyboardNote>();
            k.Add('q', BassCsharp);
            k.Add('w', BassDsharp);
            k.Add('e', BassE);
            k.Add('r', BassFsharp);
            k.Add('t', BassGsharp);
            k.Add('y', BassAsharp);
            k.Add('u', BassB);
            k.Add('i', MiddleC);
            k.Add('o', MiddleCsharp);
            k.Add('p', MiddleDsharp);
            k.Add('[', MiddleGsharp);
            k.Add(']', MiddleAsharp);
            k.Add('a', BassD);
            k.Add('s', BassE);
            k.Add('d', BassF);
            k.Add('f', BassG);
            k.Add('g', BassA);
            k.Add('h', BassB);
            k.Add('j', MiddleC);
            k.Add('k', MiddleD);
            k.Add('l', MiddleE);
            k.Add(';', MiddleF);
            k.Add((char)39, MiddleG);
            k.Add('z', BassE);
            k.Add('x', BassF);
            k.Add('c', BassG);
            k.Add('v', MiddleA);
            k.Add('b', MiddleB);
            k.Add('n', MiddleC);
            k.Add('m', MiddleD);
            k.Add(',', MiddleE);
            k.Add('.', MiddleF);
            k.Add('/', MiddleG);
            return k;
        }
        /// <summary>
        /// Play a tone based on the inputed key
        /// </summary>
        /// <param name="n"></param>
        public static void PlayNote(KeyboardNote n)
        {
            //Console.Beep() does not work in universal apps.
            //I cannot find an easy way to do the same basic task in mobile.
            if (n.Frequency > 0)
                Player.PlayBeep((UInt16)n.Frequency, 300);
            else
                throw new Exception("No key found to play");
        }

        public static char KeyToChar(Key k)
        {
            char key = (char)MapVirtualKey(KeyInterop.VirtualKeyFromKey(k), 2);

            return key;
        }
    }

    public class KeyboardNote
    {
        public KeyboardNote() { }
        /// <summary>
        /// Keyboard note
        /// </summary>
        /// <param name="n">Note name</param>
        /// <param name="i">Note octave</param>
        public KeyboardNote(string n, int i)
        {
            _Note = n;
            _Octave = i;
            _Frequency = 0;
        }
        /// <summary>
        /// Keyboard note
        /// </summary>
        /// <param name="n">Note name</param>
        /// <param name="i">Note octave</param>
        /// <param name="t">Note frequency</param>
        public KeyboardNote(string n, int i, int t)
        {
            _Note = n;
            _Octave = i;
            _Frequency = t;
        }
        /// <summary>
        /// Keyboard note
        /// </summary>
        /// <param name="n">Note name</param>
        /// <param name="i">Note octave</param>
        /// <param name="t">Note frequency</param>
        /// <param name ="k">Key Pressed</param>
        public KeyboardNote(string n, int i, int t, char k)
        {
            _Note = n;
            _Octave = i;
            _Frequency = t;
            _KeyPressed = k;
        }
        private string _Note;
        public string Note { get { return _Note; } }
        private int _Octave;
        public int Octave { get { return _Octave; } }
        private int _Frequency;
        public int Frequency { get { return _Frequency; } }
        private char _KeyPressed;
        public char KeyPressed { get { return _KeyPressed; } }
        public override string ToString()
        {
            return Note + Octave;
        }
    }
}
