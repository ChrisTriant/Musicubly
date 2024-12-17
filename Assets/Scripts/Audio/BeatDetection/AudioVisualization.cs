using UnityEngine;
using UnityEngine.Rendering;
/// <summary>
/// Creates a dynamic audio visualizer that uses Line Renderers to extend and retract based on audio spectrum data.
/// </summary>
[RequireComponent(typeof(AudioSource))]
public class AudioVisualizer : MonoBehaviour
{
    #region Fields

    [Header("Audio Settings")]
    [SerializeField] private BeatDetector _beatDetector;
    [SerializeField] private int _spectrumResolution = 512; // Number of spectrum bands
    [SerializeField] private float _emphasisMultiplier = 10f; // Boosts spectrum intensity
    [SerializeField] private float _retractionSpeed = 2f; // Speed of retraction

    [Header("Visualizer Settings")]
    [SerializeField] private int _numberOfSegments = 64; // Number of visualized lines
    [SerializeField] private float _radius = 5f; // Base radius of the ring
    [SerializeField] private float _minExtendLength = 0f; // Maximum line extension length
    [SerializeField] private float _maxExtendLength = 3f; // Maximum line extension length
    [SerializeField] private GameObject _lineRendererPrefab;
    [SerializeField] private Transform _spawnParent;

    private LineRenderer[] _lineRenderers;
    private float[] _spectrumData;
    private float[] _lineLengths;

    #endregion

    #region LifeCycle

    private void Start()
    {
        InitializeLineRenderers();
    }

    private void Update()
    {
        UpdateSpectrum();
        UpdateLineRenderers();
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Initializes the Line Renderers in a circular arrangement.
    /// </summary>
    private void InitializeLineRenderers()
    {
        _lineRenderers = new LineRenderer[_numberOfSegments];
        _lineLengths = new float[_numberOfSegments];
        _spectrumData = new float[_spectrumResolution];

        for (int i = 0; i < _numberOfSegments; i++)
        {
            GameObject lineObject = Instantiate(_lineRendererPrefab, _spawnParent);
            LineRenderer lineRenderer = lineObject.GetComponent<LineRenderer>();
            lineRenderer.positionCount = 2;
            lineRenderer.useWorldSpace = true;

            float t = i / (_lineRenderers.Length - 2f);
            float angle = t * Mathf.PI * 2f;
            Vector3 direction = new Vector3(Mathf.Cos(angle + Mathf.PI / 2), Mathf.Sin(angle + Mathf.PI / 2));

            Vector3 startPosition = _spawnParent.position + direction * _radius;
            lineRenderer.SetPosition(0, startPosition);
            lineRenderer.SetPosition(1, startPosition + direction * _minExtendLength); // Start extended at base radius

            lineRenderer.startWidth = Spacing(_radius);
            _lineRenderers[i] = lineRenderer;
        }
    }

    /// <summary>
    /// Updates the audio spectrum data.
    /// </summary>
    private void UpdateSpectrum()
    {
        _spectrumData = _beatDetector.ChannelSpectrums[0];
    }

    /// <summary>
    /// Updates each Line Renderer to extend and retract based on spectrum data.
    /// </summary>
    private void UpdateLineRenderers()
    {
        int spectrumStep = Mathf.Max(1, _spectrumResolution / _numberOfSegments);

        for (int i = 0; i < _numberOfSegments; i++)
        {
            // Average a range of spectrum bands for smoother visuals
            float averageSpectrum = 0f;
            for (int j = 0; j < spectrumStep; j++)
            {
                int index = i * spectrumStep + j;
                if (index < _spectrumResolution)
                    averageSpectrum += _spectrumData[index];
            }
            averageSpectrum /= spectrumStep;

            // Compute target length based on spectrum value
            float targetLength = averageSpectrum * _emphasisMultiplier;

            // Smoothly retract if target is lower than current length
            _lineLengths[i] = Mathf.Max(0, Mathf.Lerp(_lineLengths[i], targetLength, Time.deltaTime * _retractionSpeed));
            _lineLengths[i] = Mathf.Clamp(_lineLengths[i], 0, _maxExtendLength);

            // Update Line Renderer positions
            LineRenderer lineRenderer = _lineRenderers[i];
            Vector3 startPosition = lineRenderer.GetPosition(0);
            Vector3 direction = (lineRenderer.GetPosition(1) - startPosition).normalized;
            //float t = i / (_lineRenderers.Length - 2f);
            //float a = t * Mathf.PI * 2f;
            //Vector3 direction = new Vector3(Mathf.Cos(a + Mathf.PI / 2), Mathf.Sin(a + Mathf.PI / 2));
            var extendOffset = _minExtendLength + _lineLengths[i];
            Vector3 endPosition = startPosition + direction * extendOffset;

            lineRenderer.SetPosition(1, endPosition);

            float endWidth = _radius + extendOffset;

            lineRenderer.endWidth = Spacing(endWidth);
        }
    }

    private float Spacing(float radius)
    {
        float c = 2f * Mathf.PI * radius;
        float n = _lineRenderers.Length;

        return c / n;
    }

    #endregion
}