using System.Collections.Generic;
using UnityEngine;

public class BeatDetector : MonoBehaviour
{
    #region Events

    [SerializeField] private VoidEventChannelSO _onBeat;
    [SerializeField] private AudioClipEventChannelSO _onClipChanged;

    #endregion

    #region Fields

    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private int _bassLowerLimit = 60;
    [SerializeField] private int _bassUpperLimit = 180;
    [SerializeField] private int _lowLowerLimit = 500;
    [SerializeField] private int _lowUpperLimit = 2000;
    [SerializeField] private int _spectrumSampleSize;
    [SerializeField]
    private FFTWindow FFTWindowType = FFTWindow.Blackman;

    private float[][] _channelSpectrums;

    private float[] _freqSpectrum = new float[4];
    private float[] _avgSpectrum = new float[4];

    private bool _bass;
    private bool _low;

    private Deque<List<float>> _fftHistory = new Deque<List<float>>();
    private int _fftHistoryMaxSize;

    private List<int> _bandLimits = new List<int>();

    private bool _prevBass;
    private bool _prevLow;

    #endregion

    #region Properties

    public float[][] ChannelSpectrums => _channelSpectrums;
    public int SpectrumSampleSize => _spectrumSampleSize;

    #endregion

    #region LifeCycle

    private void Awake()
    {
        BindEvents();
    }

    private void OnDestroy()
    {
        UnbindEvents();
    }

    private void Update()
    {
        // Detect beats in the current audio sample
        DetectBeats(ref _freqSpectrum, ref _avgSpectrum, ref _bass, ref _low);
    }

    private void LateUpdate()
    {
        // Trigger events or actions based on detected beats
        HandleBeatEvents();
    }

    #endregion

    #region Private Methods

    private void BindEvents()
    {
        _onClipChanged.OnEventRaised += ChangeAudioClip;
    }

    private void UnbindEvents()
    {
        _onClipChanged.OnEventRaised -= ChangeAudioClip;
    }

    private void ChangeAudioClip(AudioClip newClip)
    {
        if (newClip == null)
        {
            Debug.LogWarning("Attempted to set a null audio clip.");
            return;
        }

        // Reinitialize band limits and FFT history for the new audio clip.
        InitializeBandLimits();
    }

    private void InitializeBandLimits()
    {
        _channelSpectrums = new float[_audioSource.clip.channels][];
        for(int i = 0; i < _channelSpectrums.Length; i++)
            _channelSpectrums[i] = new float[_spectrumSampleSize];

        int bandSize = _audioSource.clip.frequency / 1024;
        _fftHistoryMaxSize = _audioSource.clip.frequency / 1024;

        _bandLimits.Clear();

        // Bass 60Hz–180Hz
        _bandLimits.Add(_bassLowerLimit / bandSize);
        _bandLimits.Add(_bassUpperLimit / bandSize);

        // Low-midrange 500Hz–2000Hz
        _bandLimits.Add(_lowLowerLimit / bandSize);
        _bandLimits.Add(_lowUpperLimit / bandSize);

        _bandLimits.TrimExcess();
        _fftHistory.Clear();
    }

    private void DetectBeats(ref float[] spectrum, ref float[] avgSpectrum, ref bool isBass, ref bool isLow)
    {
        int numBands = 2; // bass and low
        int numChannels = _audioSource.clip.channels;


        for (int channel = 0; channel < numChannels; ++channel)
        {
            _audioSource.GetSpectrumData(_channelSpectrums[channel], channel, FFTWindowType);

            for (int numBand = 0; numBand < numBands; ++numBand)
            {
                for (int indexFFT = _bandLimits[numBand]; indexFFT < _bandLimits[numBand + 1]; ++indexFFT)
                {
                    spectrum[numBand] += _channelSpectrums[channel][indexFFT];
                }
                spectrum[numBand] /= (_bandLimits[numBand + 1] - _bandLimits[numBand]);
            }
        }

        spectrum[0] /= numChannels; // Average across channels
        spectrum[1] /= numChannels;

        if (_fftHistory.Count > 0)
        {
            CalculateAvgSpectrum(ref avgSpectrum, numBands, ref _fftHistory);

            float[] varianceSpectrum = new float[numBands];
            CalculateVarianceSpectrum(ref varianceSpectrum, numBands, ref _fftHistory, ref avgSpectrum);

            isBass = spectrum[0] > BeatThreshold(varianceSpectrum[0]) * avgSpectrum[0];
            isLow = spectrum[1] > BeatThreshold(varianceSpectrum[1]) * avgSpectrum[1];
        }

        AddSpectrumToHistory(spectrum, numBands);
    }

    private void CalculateAvgSpectrum(ref float[] avgSpectrum, int numBands, ref Deque<List<float>> fftHistory)
    {
        foreach (List<float> fftResult in fftHistory)
        {
            for (int index = 0; index < fftResult.Count; ++index)
            {
                avgSpectrum[index] += fftResult[index];
            }
        }

        for (int index = 0; index < numBands; ++index)
        {
            avgSpectrum[index] /= fftHistory.Count;
        }
    }

    private void CalculateVarianceSpectrum(ref float[] varianceSpectrum, int numBands, ref Deque<List<float>> fftHistory, ref float[] avgSpectrum)
    {
        // Ensure varianceSpectrum is correctly sized
        if (varianceSpectrum.Length != numBands)
        {
            varianceSpectrum = new float[numBands];
        }

        foreach (List<float> fftResult in fftHistory)
        {
            for (int index = 0; index < numBands; ++index)
            {
                if (index < fftResult.Count) // Ensure index is within bounds
                {
                    float difference = fftResult[index] - avgSpectrum[index];
                    varianceSpectrum[index] += difference * difference;
                }
            }
        }

        for (int index = 0; index < numBands; ++index)
        {
            varianceSpectrum[index] /= fftHistory.Count;
        }
    }

    private float BeatThreshold(float variance)
    {
        return -15f * variance + 1.55f;
    }

    private void AddSpectrumToHistory(float[] spectrum, int numBands)
    {
        List<float> fftResult = new List<float>(spectrum);

        if (_fftHistory.Count >= _fftHistoryMaxSize)
        {
            _fftHistory.RemoveFront();
        }

        _fftHistory.AddBack(fftResult);
    }

    private void HandleBeatEvents()
    {
        if (_bass && !_prevBass)
        {
            // Add bass-specific actions here
        }

        if (_low && !_prevLow)
        {
            _onBeat.RaiseEvent();
        }

        _prevBass = _bass;
        _prevLow = _low;
    }

    #endregion
}
