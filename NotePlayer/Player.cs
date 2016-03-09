using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Media;

namespace NotePlayer
{
    public static class Player
    {
        private static List<SoundPlayer> OpenPlayStreams = new List<SoundPlayer>();
        private static Dictionary<UInt16, int> StreamDictionary = new Dictionary<UInt16, int>();
        /// <summary>
        /// Creates a stream of sound based on the inputed falues.
        /// </summary>
        /// <param name="frequency">220 = A</param>
        /// <param name="msDuration">Duration in ms (1000 = 1 sec)</param>
        /// <param name="volume">16383</param>
        public static void PlayBeep(UInt16 frequency, int msDuration, UInt16 volume = 16383, bool loop = false)
        {
            var mStrm = new MemoryStream();
            BinaryWriter writer = new BinaryWriter(mStrm);

            const double TAU = 2 * Math.PI;
            int formatChunkSize = 16;
            int headerSize = 8;
            short formatType = 1;
            short tracks = 1;
            int samplesPerSecond = 44100;
            short bitsPerSample = 16;
            short frameSize = (short)(tracks * ((bitsPerSample + 7) / 8));
            int bytesPerSecond = samplesPerSecond * frameSize;
            int waveSize = 4;
            int samples = (int)((decimal)samplesPerSecond * msDuration / 1000);
            int dataChunkSize = samples * frameSize;
            int fileSize = waveSize + headerSize + formatChunkSize + headerSize + dataChunkSize;
            // var encoding = new System.Text.UTF8Encoding();
            writer.Write(0x46464952); // = encoding.GetBytes("RIFF")
            writer.Write(fileSize);
            writer.Write(0x45564157); // = encoding.GetBytes("WAVE")
            writer.Write(0x20746D66); // = encoding.GetBytes("fmt ")
            writer.Write(formatChunkSize);
            writer.Write(formatType);
            writer.Write(tracks);
            writer.Write(samplesPerSecond);
            writer.Write(bytesPerSecond);
            writer.Write(frameSize);
            writer.Write(bitsPerSample);
            writer.Write(0x61746164); // = encoding.GetBytes("data")
            writer.Write(dataChunkSize);
            {
                double theta = frequency * TAU / samplesPerSecond;
                // 'volume' is UInt16 with range 0 thru Uint16.MaxValue ( = 65 535)
                // we need 'amp' to have the range of 0 thru Int16.MaxValue ( = 32 767)
                double amp = volume >> 2; // so we simply set amp = volume / 2
                for (int step = 0; step < samples; step++)
                {
                    short s = (short)(amp * Math.Sin(theta * step));
                    writer.Write(s);
                }
            }
            SoundPlayer player = new SoundPlayer(mStrm);
            mStrm.Seek(0, SeekOrigin.Begin);
            if (!loop)
                player.Play();
            else if (!StreamDictionary.Keys.Contains(frequency))
            {
                player.PlayLooping();
                OpenPlayStreams.Add(player);
                StreamDictionary.Add(frequency, OpenPlayStreams.Count - 1);
            }
            writer.Close();
            mStrm.Close();
        } // public static void PlayBeep(UInt16 frequency, int msDuration, UInt16 volume = 16383)

        public static void StopPlayer(UInt16 frequency)
        {
            if (!StreamDictionary.Keys.Contains(frequency))
                throw new MissingTuneException("That note is not playing!");
            OpenPlayStreams[StreamDictionary[frequency]].Stop();
        }

        public class MissingTuneException : Exception
        {
            public MissingTuneException()
            { }
            public MissingTuneException(string message) : base(message)
            { }
            public MissingTuneException(string message, Exception inner) : base(message, inner)
            { }
        }
        public class PlayerBusyException : Exception
        {
            public PlayerBusyException()
            { }
            public PlayerBusyException(string message) : base(message)
            { }
            public PlayerBusyException(string message, Exception inner) : base(message, inner)
            { }
        }

        public class PlayerWorker
        {
            public PlayerWorker() { }
            public PlayerWorker(KeyboardNote k)
            {
                key = k;
                playThread = new Thread(Playing);
            }
            Thread playThread;
            private KeyboardNote key;
            public KeyboardNote Key { get { return key; } }
            private bool IsPlaying = false;

            public void StartPlaying()
            {
                if (IsPlaying)
                    throw new PlayerBusyException();
                IsPlaying = true;
                playThread.Start();
            }

            public void StopPlaying()
            {
                IsPlaying = false;
            }

            private void Playing()
            {
                while (IsPlaying)
                {
                    PlayBeep((UInt16)key.Frequency, 10, 16383, true);
                    if (!IsPlaying)
                        break;
                }
                StopPlayer((UInt16)key.Frequency);
                return;
            }
        }
        public enum PlayerType
        {
            none = 0,
            Single = 300,
            Continuous
        }
    }
}
