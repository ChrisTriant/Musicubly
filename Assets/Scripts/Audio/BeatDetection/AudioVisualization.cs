using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// Code taken from https://www.youtube.com/watch?v=BVhnmm1SvF0&t=13s

[RequireComponent(typeof(AudioSource))]
public class AudioVisualization : MonoBehaviour
{
    #region Fields

    [SerializeField] private int _bufferSampleSize;
    [SerializeField] private float _samplePercentage;
    [SerializeField] private float _emphasisMultiplier;
    [SerializeField] private float _retractionSpeed;

    [SerializeField] private int _amountOfSegments;
    [SerializeField] private float _radius;
    [SerializeField] private float _bufferSizeArea;
    [SerializeField] private float _maximumExtendLength;

    [SerializeField] private GameObject _lineRendererPrefab;
    [SerializeField] private Material _lineRendererMaterial;
    [SerializeField] private Transform _spawnParent;
    public VisualizationMode _visualizationMode;

    [SerializeField] private Gradient _colorGradientA = new Gradient();
    [SerializeField] private Gradient _colorGradientB = new Gradient();

    private float _sampleRate;

    private float[] _samples;
    private float[] _spectrum;
    private float[] _extendLengths;

    private LineRenderer[] _lineRenderers;

    private AudioSource _audioSource;

    #endregion

    #region LifeCycle

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
        _sampleRate = AudioSettings.outputSampleRate;

        _samples = new float[_bufferSampleSize];
        _spectrum = new float[_bufferSampleSize];

        switch (_visualizationMode)
        {
            case VisualizationMode.Ring:
                InitializeRing();
                break;
        }
    }

    private void Update()
    {
        _audioSource.GetSpectrumData(_spectrum, 1, FFTWindow.BlackmanHarris);

        UpdateExtends();

        if(_visualizationMode == VisualizationMode.Ring)
        {
            UpdateRing();
        }    
    }

    #endregion

    #region Private Methods

    private void InitializeRing()
    {
        _extendLengths = new float[_amountOfSegments + 1];
        _lineRenderers = new LineRenderer[_extendLengths.Length];

        for(int i = 0; i < _lineRenderers.Length; i++)
        {
            GameObject go = Instantiate(_lineRendererPrefab, _spawnParent);
            go.transform.position = Vector3.zero;

            LineRenderer lineRenderer = go.GetComponent<LineRenderer>();
            lineRenderer.sharedMaterial = _lineRendererMaterial;

            lineRenderer.positionCount = 2;
            lineRenderer.useWorldSpace = true;
            _lineRenderers[i] = lineRenderer;
        }
    }

    private void UpdateExtends()
    {
        int iteration = 0;
        int indexOnSpectrum = 0;
        int averageValue = (int)(Mathf.Abs(_samples.Length * _samplePercentage) / _amountOfSegments);

        if (averageValue < 1)
        {
            averageValue = 1;
        }

        while (iteration < _extendLengths.Length)
        {
            int iterationIndex = 0;
            float sumValueY = 0;

            while(iterationIndex < averageValue)
            {
                sumValueY += _spectrum[indexOnSpectrum];
                indexOnSpectrum++;
                iterationIndex++;
            }

            float y = sumValueY / averageValue * _emphasisMultiplier;
            _extendLengths[iteration] -= _retractionSpeed * Time.deltaTime;
            if (_extendLengths[iteration] < y) 
            {
                _extendLengths[iteration] = y;
            }

            if (_extendLengths[iteration] > _maximumExtendLength)
            {
                _extendLengths[iteration] = _maximumExtendLength;
            }

            iteration++;
        }
    }

    private void UpdateRing()
    {
        for(int i = 0; i < _lineRenderers.Length; i++)
        {
            float t = i / (_lineRenderers.Length - 2f);
            float a = t * Mathf.PI * 2f;

            Vector3 direction = new Vector3(Mathf.Cos(a), Mathf.Sin(a));
            float maximumRadius = (_radius + _bufferSizeArea + _extendLengths[i] * 10000);

            _lineRenderers[i].SetPosition(0, _spawnParent.position + direction * _radius);
            _lineRenderers[i].SetPosition(1, _spawnParent.position + direction * maximumRadius);

            _lineRenderers[i].startWidth = Spacing(_radius);
            _lineRenderers[i].endWidth = Spacing(maximumRadius);

            _lineRenderers[i].startColor = _colorGradientA.Evaluate(0);
            _lineRenderers[i].endColor = _colorGradientA.Evaluate((_extendLengths[i] * 10000 - 1) / (_maximumExtendLength - 1f));
        }
    }

    private float Spacing(float radius)
    {
        float c = 2f * Mathf.PI * radius;
        float n = _lineRenderers.Length;

        return c / n;
    }

    #endregion

    #region Nested Types

    public enum VisualizationMode
    {
        Ring, 
        RingWithBeat
    }

    #endregion

}