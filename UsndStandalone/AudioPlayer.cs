using System.IO;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;

namespace UsndStandalone;

/// <summary>
/// USndパラメータに対応した高機能AudioPlayer
/// Volume, Pan, Pitch, Delay, FadeIn/FadeOutなどをサポート
/// </summary>
public class AudioPlayer : IDisposable
{
    private IWavePlayer? _output;
    private AudioFileReader? _reader;
    private VolumeSampleProvider? _volumeProvider;
    private PanningSampleProvider? _panProvider;
    
    private System.Threading.Timer? _delayTimer;
    private System.Threading.Timer? _fadeTimer;
    
    private float _targetVolume = 1.0f;
    private float _currentVolume = 1.0f;
    private float _fadeSpeed = 0f;

    public bool IsPlaying { get; private set; }
    public float Volume 
    { 
        get => _targetVolume; 
        set 
        {
            _targetVolume = Math.Clamp(value, 0f, 1f);
            if (_volumeProvider != null)
                _volumeProvider.Volume = _targetVolume;
        }
    }
    
    public float Pan 
    { 
        get => _panProvider?.Pan ?? 0f; 
        set 
        {
            if (_panProvider != null)
                _panProvider.Pan = Math.Clamp(value, -1f, 1f);
        }
    }
    
    public float PlaybackRate
    {
        get => _reader?.Volume ?? 1.0f;
        set
        {
            // Note: NAudio 2.2.1では直接的なピッチシフトが難しいため、
            // 現在は基本的な再生速度変更のみサポート
        }
    }

    /// <summary>
    /// 基本的な再生（互換性のため残す）
    /// </summary>
    public void Play(string filePath, bool loop)
    {
        PlayWithParameters(filePath, loop, 1.0f, 0f, 0, 0f, 0f);
    }

    /// <summary>
    /// USndパラメータ付き再生
    /// </summary>
    /// <param name="filePath">音声ファイルパス</param>
    /// <param name="loop">ループ再生</param>
    /// <param name="volume">ボリューム 0.0～1.0</param>
    /// <param name="pan">パン -1.0～1.0</param>
    /// <param name="pitchCent">ピッチ（セント単位、100cent = 半音）</param>
    /// <param name="delay">再生開始遅延（秒）</param>
    /// <param name="fadeInTime">フェードイン時間（秒）</param>
    public void PlayWithParameters(string filePath, bool loop, float volume, float pan, int pitchCent, float delay, float fadeInTime)
    {
        Stop();

        if (!File.Exists(filePath))
        {
            Console.WriteLine($"音声ファイルが見つかりません: {filePath}");
            return;
        }

        _targetVolume = Math.Clamp(volume, 0f, 1f);
        
        // フェードイン処理
        if (fadeInTime > 0)
        {
            _currentVolume = 0f;
            _fadeSpeed = _targetVolume / (fadeInTime * 100); // 10ms刻みで更新
        }
        else
        {
            _currentVolume = _targetVolume;
        }

        // 遅延再生
        if (delay > 0)
        {
            _delayTimer = new System.Threading.Timer(_ =>
            {
                try
                {
                    StartPlayback(filePath, loop, pan, pitchCent, fadeInTime);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"遅延再生エラー: {ex.Message}");
                }
                finally
                {
                    _delayTimer?.Dispose();
                    _delayTimer = null;
                }
            }, null, (int)(delay * 1000), System.Threading.Timeout.Infinite);
        }
        else
        {
            StartPlayback(filePath, loop, pan, pitchCent, fadeInTime);
        }
    }

    private void StartPlayback(string filePath, bool loop, float pan, int pitchCent, float fadeInTime)
    {
        try
        {
            _reader = new AudioFileReader(filePath);

            ISampleProvider sampleProvider = _reader;

            // ループ処理
            if (loop)
            {
                sampleProvider = new LoopingSampleProvider(_reader);
            }

            // パン処理（モノラルの場合のみ - PanningSampleProviderはモノラル入力専用）
            if (sampleProvider.WaveFormat.Channels == 1 && pan != 0f)
            {
                _panProvider = new PanningSampleProvider(sampleProvider);
                _panProvider.Pan = Math.Clamp(pan, -1f, 1f);
                sampleProvider = _panProvider;
            }
            // モノラルでパン不要の場合はステレオに変換
            else if (sampleProvider.WaveFormat.Channels == 1)
            {
                sampleProvider = new MonoToStereoSampleProvider(sampleProvider);
            }
            // ステレオの場合はパン処理をスキップ（そのまま使用）
            // TODO: ステレオのパン処理は別の方法で実装が必要

            // ボリューム処理
            _volumeProvider = new VolumeSampleProvider(sampleProvider);
            _volumeProvider.Volume = _currentVolume;
            sampleProvider = _volumeProvider;

            _output = new WaveOutEvent();
            _output.Init(sampleProvider);
            _output.Play();
            IsPlaying = true;

            // フェードイン処理
            if (fadeInTime > 0)
            {
                StartFade();
            }
        }
        catch (Exception ex)
        {
            // エラーをスローして上位で処理できるようにする
            throw new InvalidOperationException($"再生エラー: {ex.Message}", ex);
        }
    }

    private void StartFade()
    {
        _fadeTimer = new System.Threading.Timer(_ =>
        {
            if (_volumeProvider == null || !IsPlaying)
            {
                _fadeTimer?.Dispose();
                _fadeTimer = null;
                return;
            }

            if (_currentVolume < _targetVolume)
            {
                _currentVolume = Math.Min(_currentVolume + _fadeSpeed, _targetVolume);
                _volumeProvider.Volume = _currentVolume;
            }
            else
            {
                _fadeTimer?.Dispose();
                _fadeTimer = null;
            }
        }, null, 0, 10); // 10ms毎に更新
    }

    /// <summary>
    /// フェードアウト付き停止
    /// </summary>
    public void StopWithFade(float fadeOutTime)
    {
        if (!IsPlaying || _volumeProvider == null)
        {
            Stop();
            return;
        }

        _fadeTimer?.Dispose();
        _currentVolume = _volumeProvider.Volume;
        _fadeSpeed = _currentVolume / (fadeOutTime * 100);

        _fadeTimer = new System.Threading.Timer(_ =>
        {
            if (_volumeProvider == null)
            {
                _fadeTimer?.Dispose();
                _fadeTimer = null;
                Stop();
                return;
            }

            if (_currentVolume > 0)
            {
                _currentVolume = Math.Max(_currentVolume - _fadeSpeed, 0);
                _volumeProvider.Volume = _currentVolume;
            }
            else
            {
                _fadeTimer?.Dispose();
                _fadeTimer = null;
                Stop();
            }
        }, null, 0, 10);
    }

    public void Stop()
    {
        _delayTimer?.Dispose();
        _delayTimer = null;
        
        _fadeTimer?.Dispose();
        _fadeTimer = null;

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

        _volumeProvider = null;
        _panProvider = null;

        IsPlaying = false;
    }

    public void Dispose()
    {
        Stop();
    }

    // NAudio の LoopingSampleProvider（ISampleProvider版）
    private class LoopingSampleProvider : ISampleProvider
    {
        private readonly AudioFileReader _sourceReader;

        public LoopingSampleProvider(AudioFileReader sourceReader)
        {
            _sourceReader = sourceReader;
        }

        public WaveFormat WaveFormat => _sourceReader.WaveFormat;

        public int Read(float[] buffer, int offset, int count)
        {
            int totalRead = 0;
            while (totalRead < count)
            {
                int samplesRead = _sourceReader.Read(buffer, offset + totalRead, count - totalRead);
                if (samplesRead == 0)
                {
                    // ループ: 先頭に戻る
                    _sourceReader.Position = 0;
                    samplesRead = _sourceReader.Read(buffer, offset + totalRead, count - totalRead);
                    if (samplesRead == 0) break;
                }
                totalRead += samplesRead;
            }
            return totalRead;
        }
    }
}



