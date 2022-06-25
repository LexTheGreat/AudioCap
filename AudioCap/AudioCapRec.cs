using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AudioCap {
    class AudioCapRec {
        private WaveFileWriter writer;
        private WasapiLoopbackCapture capture;
        private WaveOut wavePlayer;

        public AudioCapRec() {
            /* According to documentation WasapiLoopbackCapture.DataAvailable doesn't return anything if no audio is going on. So we push out blank audio to fill in empty spaces.  */
            wavePlayer = new WaveOut();
            wavePlayer.Init(new SilentWaveProvider());

            capture = new WasapiLoopbackCapture();
            capture.DataAvailable += Capture_DataAvailable;
            capture.RecordingStopped += Capture_RecordingStopped;
        }

        public void StartRecording() {
            writer = new WaveFileWriter(Path.Combine(Path.GetDirectoryName(System.AppContext.BaseDirectory), DateTime.Now.ToString("yyyy-dd-M--HH-mm-ss") + ".wav"), capture.WaveFormat);
            wavePlayer.Play();
            capture.StartRecording();
        }

        public void StopRecording() {
            capture.StopRecording();
        }

        private void Capture_DataAvailable(object sender, WaveInEventArgs e) {
            writer.Write(e.Buffer, 0, e.BytesRecorded);
        }

        private void Capture_RecordingStopped(object sender, StoppedEventArgs e) {
            wavePlayer.Stop();
            writer.Flush();
            writer.Dispose();
            writer = null;
        }
    }
    public class SilentWaveProvider : IWaveProvider {
        public WaveFormat WaveFormat {
            get {
                return new WaveFormat(44100, 2);
            }
        }

        public int Read(byte[] buffer, int offset, int count) {
            return buffer.Length;
        }
    }
}
