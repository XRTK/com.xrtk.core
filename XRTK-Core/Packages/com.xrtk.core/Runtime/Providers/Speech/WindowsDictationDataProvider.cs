// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using XRTK.Definitions.Controllers;
using XRTK.Interfaces.InputSystem;
using XRTK.Services;
using XRTK.Utilities.Async;

#if UNITY_STANDALONE_WIN || UNITY_WSA || UNITY_EDITOR_WIN
using UnityEngine.Windows.Speech;
#endif // UNITY_STANDALONE_WIN || UNITY_WSA || UNITY_EDITOR_WIN

namespace XRTK.Providers.Speech
{
    /// <summary>
    /// Dictation data provider for Windows 10 based platforms.
    /// </summary>
    [System.Runtime.InteropServices.Guid("9D96D660-CB10-48B8-B2A7-3262ADB2AB14")]
    public class WindowsDictationDataProvider : BaseDictationDataProvider
    {
        /// <inheritdoc />
        public WindowsDictationDataProvider(string name, uint priority, BaseMixedRealityControllerDataProviderProfile profile, IMixedRealityInputSystem parentService)
            : base(name, priority, profile, parentService)
        {
#if UNITY_STANDALONE_WIN || UNITY_WSA || UNITY_EDITOR_WIN
            if (dictationRecognizer == null)
            {
                try
                {
                    dictationRecognizer = new DictationRecognizer();
                }
                catch (UnityException e)
                {
                    switch (e.Message)
                    {
                        case string message when message.Contains("Speech recognition is not supported on this machine."):
                            Debug.LogWarning($"Skipping {nameof(WindowsDictationDataProvider)} registration.\n{e.Message}");
                            break;
                        default:
                            throw;
                    }
                }
            }
#endif // UNITY_STANDALONE_WIN || UNITY_WSA || UNITY_EDITOR_WIN
        }

#if UNITY_STANDALONE_WIN || UNITY_WSA || UNITY_EDITOR_WIN

        #region IMixedRealityService Implementation

        /// <inheritdoc />
        public override void Enable()
        {
            if (!Application.isPlaying || dictationRecognizer == null) { return; }

            inputSource = InputSystem.RequestNewGenericInputSource(Name);
            dictationResult = string.Empty;

            dictationRecognizer.DictationHypothesis += DictationRecognizer_DictationHypothesis;
            dictationRecognizer.DictationResult += DictationRecognizer_DictationResult;
            dictationRecognizer.DictationComplete += DictationRecognizer_DictationComplete;
            dictationRecognizer.DictationError += DictationRecognizer_DictationError;
        }

        /// <inheritdoc />
        public override void Update()
        {
            base.Update();

            if (!Application.isPlaying || dictationRecognizer == null) { return; }

            if (!isTransitioning && IsListening && !Microphone.IsRecording(deviceName) && dictationRecognizer.Status == SpeechSystemStatus.Running)
            {
                // If the microphone stops as a result of timing out, make sure to manually stop the dictation recognizer.
                StopRecording();
            }

            if (!hasFailed && dictationRecognizer.Status == SpeechSystemStatus.Failed)
            {
                hasFailed = true;
                InputSystem.RaiseDictationError(inputSource, "Dictation recognizer has failed!");
            }
        }

        /// <inheritdoc />
        public override async void Disable()
        {
            base.Disable();

            if (!Application.isPlaying || dictationRecognizer == null) { return; }

            if (!isTransitioning && IsListening) { await StopRecordingAsync(); }

            dictationRecognizer.DictationHypothesis -= DictationRecognizer_DictationHypothesis;
            dictationRecognizer.DictationResult -= DictationRecognizer_DictationResult;
            dictationRecognizer.DictationComplete -= DictationRecognizer_DictationComplete;
            dictationRecognizer.DictationError -= DictationRecognizer_DictationError;
        }

        protected override void OnDispose(bool finalizing)
        {
            if (finalizing)
            {
                dictationRecognizer?.Dispose();
                dictationRecognizer = null;
            }

            base.OnDispose(finalizing);
        }

        #endregion IMixedRealityService Implementation

        #region IMixedRealityDictationDataProvider Implementation

        private bool hasFailed;
        private bool hasListener;
        private bool isTransitioning;

        private IMixedRealityInputSource inputSource = null;

        /// <summary>
        /// Caches the text currently being displayed in dictation display text.
        /// </summary>
        private StringBuilder textSoFar;

        private string deviceName = string.Empty;

        /// <summary>
        /// The device audio sampling rate.
        /// </summary>
        /// <remarks>Set by UnityEngine.Microphone.<see cref="Microphone.GetDeviceCaps"/></remarks>
        private int samplingRate;

        /// <summary>
        /// String result of the current dictation.
        /// </summary>
        private string dictationResult;

        /// <summary>
        /// Audio clip of the last dictation session.
        /// </summary>
        private AudioClip dictationAudioClip;

        private static DictationRecognizer dictationRecognizer;

        private readonly WaitUntil waitUntilPhraseRecognitionSystemHasStarted = new WaitUntil(() => PhraseRecognitionSystem.Status != SpeechSystemStatus.Stopped);
        private readonly WaitUntil waitUntilPhraseRecognitionSystemHasStopped = new WaitUntil(() => PhraseRecognitionSystem.Status != SpeechSystemStatus.Running);

        private readonly WaitUntil waitUntilDictationRecognizerHasStarted = new WaitUntil(() => dictationRecognizer.Status != SpeechSystemStatus.Stopped);
        private readonly WaitUntil waitUntilDictationRecognizerHasStopped = new WaitUntil(() => dictationRecognizer.Status != SpeechSystemStatus.Running);

        /// <inheritdoc />
        public override bool IsListening { get; protected set; } = false;

        /// <inheritdoc />
        public override async void StartRecording(GameObject listener = null, float initialSilenceTimeout = 5, float autoSilenceTimeout = 20, int recordingTime = 10, string micDeviceName = "")
        {
            await StartRecordingAsync(listener, initialSilenceTimeout, autoSilenceTimeout, recordingTime, micDeviceName);
        }

        /// <inheritdoc />
        public override async Task StartRecordingAsync(GameObject listener = null, float initialSilenceTimeout = 5f, float autoSilenceTimeout = 20f, int recordingTime = 10, string micDeviceName = "")
        {
            if (IsListening ||
                isTransitioning ||
                !Application.isPlaying ||
                dictationRecognizer == null ||
                InputSystem == null)
            {
                Debug.LogWarning("Unable to start recording");
                return;
            }

            hasFailed = false;
            IsListening = true;
            isTransitioning = true;

            if (listener != null)
            {
                hasListener = true;
                InputSystem.PushModalInputHandler(listener);
            }

            if (PhraseRecognitionSystem.Status == SpeechSystemStatus.Running)
            {
                PhraseRecognitionSystem.Shutdown();
            }

            await waitUntilPhraseRecognitionSystemHasStopped;
            Debug.Assert(PhraseRecognitionSystem.Status == SpeechSystemStatus.Stopped);

            // Query the maximum frequency of the default microphone.
            deviceName = micDeviceName;
            Microphone.GetDeviceCaps(deviceName, out _, out samplingRate);

            dictationRecognizer.InitialSilenceTimeoutSeconds = initialSilenceTimeout;
            dictationRecognizer.AutoSilenceTimeoutSeconds = autoSilenceTimeout;
            dictationRecognizer.Start();

            await waitUntilDictationRecognizerHasStarted;
            Debug.Assert(dictationRecognizer.Status == SpeechSystemStatus.Running);

            if (dictationRecognizer.Status == SpeechSystemStatus.Failed)
            {
                InputSystem.RaiseDictationError(inputSource, "Dictation recognizer failed to start!");
                return;
            }

            // Start recording from the microphone.
            dictationAudioClip = Microphone.Start(deviceName, false, recordingTime, samplingRate);
            textSoFar = new StringBuilder();
            isTransitioning = false;
        }

        /// <inheritdoc />
        public override async void StopRecording()
        {
            await StopRecordingAsync();
        }

        /// <inheritdoc />
        public override async Task<AudioClip> StopRecordingAsync()
        {
            if (!IsListening ||
                isTransitioning ||
                !Application.isPlaying ||
                dictationRecognizer == null)
            {
                Debug.LogWarning("Unable to stop recording");
                return null;
            }

            IsListening = false;
            isTransitioning = true;

            if (hasListener)
            {
                InputSystem.PopModalInputHandler();
                hasListener = false;
            }

            Microphone.End(deviceName);

            if (dictationRecognizer.Status == SpeechSystemStatus.Running)
            {
                dictationRecognizer.Stop();
            }

            await waitUntilDictationRecognizerHasStopped;
            Debug.Assert(dictationRecognizer.Status == SpeechSystemStatus.Stopped);

            PhraseRecognitionSystem.Restart();

            await waitUntilPhraseRecognitionSystemHasStarted;
            Debug.Assert(PhraseRecognitionSystem.Status == SpeechSystemStatus.Running);

            isTransitioning = false;
            return dictationAudioClip;
        }

        /// <summary>
        /// This event is fired while the user is talking. As the recognizer listens, it provides text of what it's heard so far.
        /// </summary>
        /// <param name="text">The currently hypothesized recognition.</param>
        private void DictationRecognizer_DictationHypothesis(string text)
        {
            // We don't want to append to textSoFar yet, because the hypothesis may have changed on the next event.
            dictationResult = $"{textSoFar} {text}...";

            InputSystem.RaiseDictationHypothesis(inputSource, dictationResult);
        }

        /// <summary>
        /// This event is fired after the user pauses, typically at the end of a sentence. The full recognized string is returned here.
        /// </summary>
        /// <param name="text">The text that was heard by the recognizer.</param>
        /// <param name="confidence">A representation of how confident (rejected, low, medium, high) the recognizer is of this recognition.</param>
        private void DictationRecognizer_DictationResult(string text, ConfidenceLevel confidence)
        {
            textSoFar.Append($"{text}. ");

            dictationResult = textSoFar.ToString();

            InputSystem.RaiseDictationResult(inputSource, dictationResult);
        }

        /// <summary>
        /// This event is fired when the recognizer stops, whether from StartRecording() being called, a timeout occurring, or some other error.
        /// Typically, this will simply return "Complete". In this case, we check to see if the recognizer timed out.
        /// </summary>
        /// <param name="cause">An enumerated reason for the session completing.</param>
        private void DictationRecognizer_DictationComplete(DictationCompletionCause cause)
        {
            // If Timeout occurs, the user has been silent for too long.
            if (cause == DictationCompletionCause.TimeoutExceeded)
            {
                Microphone.End(deviceName);

                dictationResult = "Dictation has timed out. Please try again.";
            }

            InputSystem.RaiseDictationComplete(inputSource, dictationResult, dictationAudioClip);
            textSoFar = null;
            dictationResult = string.Empty;
        }

        /// <summary>
        /// This event is fired when an error occurs.
        /// </summary>
        /// <param name="error">The string representation of the error reason.</param>
        /// <param name="hresult">The int representation of the hresult.</param>
        private void DictationRecognizer_DictationError(string error, int hresult)
        {
            dictationResult = $"{error}\nHRESULT: {hresult}";

            InputSystem.RaiseDictationError(inputSource, dictationResult);
            textSoFar = null;
            dictationResult = string.Empty;
        }

        #endregion IMixedRealityDictationDataProvider Implementation

#endif // UNITY_STANDALONE_WIN || UNITY_WSA || UNITY_EDITOR_WIN

    }
}