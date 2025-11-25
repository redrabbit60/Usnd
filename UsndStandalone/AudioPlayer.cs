using System.IO;
using NAudio.Wave;

namespace UsndStandalone;

public class AudioPlayer : IDisposable
{
    private IWavePlayer? _output;
    private AudioFileReader? _reader;

    public bool IsPlaying { get; private set; }

    public void Play(string filePath, bool loop)
    {
        Stop();

        if (!File.Exists(filePath))
        {
            Console.WriteLine($"音声ファイルが見つかりません: {filePath}");
            return;
        }

        _reader = new AudioFileReader(filePath);

        WaveStream source = _reader;
        if (loop)
        {
            source = new LoopStream(source);
        }

        _output = new WaveOutEvent();
        _output.Init(source);
        _output.Play();
        IsPlaying = true;
    }

    public void Stop()
    {
        if (_output != null)
        {
            _output.Stop();
            _output.Dispose();
            _output = null;
        }

        if (_reader != null)
        {
            _reader.Dispose();
            _reader = null;
        }

        IsPlaying = false;
    }

    public void Dispose()
    {
        Stop();
    }

    // NAudio の LoopStream サンプル実装
    private class LoopStream : WaveStream
    {
        private readonly WaveStream _sourceStream;

        public LoopStream(WaveStream sourceStream)
        {
            _sourceStream = sourceStream;
        }

        public override WaveFormat WaveFormat => _sourceStream.WaveFormat;

        public override long Length => long.MaxValue; // 擬似的に無限

        public override long Position
        {
            get => _sourceStream.Position;
            set => _sourceStream.Position = value;
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            int totalBytesRead = 0;

            while (totalBytesRead < count)
            {
                int bytesRead = _sourceStream.Read(buffer, offset + totalBytesRead, count - totalBytesRead);
                if (bytesRead == 0)
                {
                    // ループ
                    _sourceStream.Position = 0;
                    bytesRead = _sourceStream.Read(buffer, offset + totalBytesRead, count - totalBytesRead);
                    if (bytesRead == 0)
                    {
                        break;
                    }
                }

                totalBytesRead += bytesRead;
            }

            return totalBytesRead;
        }
    }
}



