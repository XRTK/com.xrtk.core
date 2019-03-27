// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using UnityEngine;
using XRTK.Providers.Controllers;
using XRTK.Definitions.InputSystem;
using XRTK.Definitions.Utilities;
using XRTK.Interfaces.InputSystem;
using XRTK.Services;

#if UNITY_STANDALONE_WIN || UNITY_WSA || UNITY_EDITOR_WIN
using UnityEngine.Windows.Speech;
#endif // UNITY_STANDALONE_WIN || UNITY_WSA || UNITY_EDITOR_WIN

namespace XRTK.WindowsMixedReality.Controllers
{
    /// <summary>
    /// Speech data provider for windows 10 based platforms.
    /// </summary>
    public class WindowsSpeechDataProvider : BaseSpeechDataProvider
    {
        public WindowsSpeechDataProvider(string name, uint priority) : base(name, priority)
        {
            if (MixedRealityToolkit.Instance.ActiveProfile.InputSystemProfile == null)
            {
                throw new Exception("Missing required input system profile!");
            }

            if (MixedRealityToolkit.Instance.ActiveProfile.InputSystemProfile.SpeechCommandsProfile == null)
            {
                throw new Exception("Missing required speech commands profile!");
            }

            if (MixedRealityToolkit.Instance.ActiveProfile.InputSystemProfile.SpeechCommandsProfile.SpeechCommands == null)
            {
                throw new Exception("Null speech commands in the speech commands profile!");
            }

#if UNITY_WSA && UNITY_EDITOR
            if (!UnityEditor.PlayerSettings.WSA.GetCapability(UnityEditor.PlayerSettings.WSACapability.Microphone))
            {
                UnityEditor.PlayerSettings.WSA.SetCapability(UnityEditor.PlayerSettings.WSACapability.Microphone, true);
            }
#endif

#if UNITY_STANDALONE_WIN || UNITY_WSA || UNITY_EDITOR_WIN
            var newKeywords = new string[Commands.Length];

            for (int i = 0; i < Commands.Length; i++)
            {
                newKeywords[i] = Commands[i].Keyword;
            }

            RecognitionConfidenceLevel = MixedRealityToolkit.Instance.ActiveProfile.InputSystemProfile.SpeechCommandsProfile.SpeechRecognitionConfidenceLevel;

            if (keywordRecognizer == null)
            {
                keywordRecognizer = new KeywordRecognizer(newKeywords, (ConfidenceLevel)RecognitionConfidenceLevel);
            }
#endif // UNITY_STANDALONE_WIN || UNITY_WSA || UNITY_EDITOR_WIN
        }

#if UNITY_STANDALONE_WIN || UNITY_WSA || UNITY_EDITOR_WIN

        #region IMixedRealityService Implementation

        /// <inheritdoc />
        public override void Enable()
        {
            base.Enable();

            if (!Application.isPlaying || Commands.Length == 0) { return; }

            InputSource = MixedRealityToolkit.InputSystem?.RequestNewGenericInputSource("Windows Speech Input Source");

            keywordRecognizer.OnPhraseRecognized += KeywordRecognizer_OnPhraseRecognized;

            if (MixedRealityToolkit.Instance.ActiveProfile.InputSystemProfile.SpeechCommandsProfile.SpeechRecognizerStartBehavior == AutoStartBehavior.AutoStart)
            {
                StartRecognition();
            }
        }

        /// <inheritdoc />
        public override void Update()
        {
            base.Update();

            if (!keywordRecognizer.IsRunning) { return; }

            for (int i = 0; i < Commands.Length; i++)
            {
                if (Input.GetKeyDown(Commands[i].KeyCode))
                {
                    OnPhraseRecognized((ConfidenceLevel)RecognitionConfidenceLevel, TimeSpan.Zero, DateTime.UtcNow, Commands[i].Keyword);
                }
            }
        }

        /// <inheritdoc />
        public override void Disable()
        {
            base.Disable();

            if (!Application.isPlaying || Commands.Length == 0) { return; }

            StopRecognition();

            keywordRecognizer.OnPhraseRecognized -= KeywordRecognizer_OnPhraseRecognized;
        }

        protected override void OnDispose(bool finalizing)
        {
            if (finalizing)
            {
                keywordRecognizer.Dispose();
            }

            base.OnDispose(finalizing);
        }

        #endregion IMixedRealityService Implementation

        #region IMixedRealitySpeechDataProvider Implementation

        /// <summary>
        /// The keywords to be recognized and optional keyboard shortcuts.
        /// </summary>
        private static SpeechCommands[] Commands => MixedRealityToolkit.Instance.ActiveProfile.InputSystemProfile.SpeechCommandsProfile.SpeechCommands;

        /// <summary>
        /// The Input Source for Windows Speech Input.
        /// </summary>
        public IMixedRealityInputSource InputSource = null;

        private static KeywordRecognizer keywordRecognizer;

        /// <inheritdoc />
        public override bool IsRecognitionActive => keywordRecognizer.IsRunning;

        /// <summary>
        /// The <see cref="RecognitionConfidenceLevel"/> that the <see cref="KeywordRecognizer"/> is using.
        /// </summary>
        public RecognitionConfidenceLevel RecognitionConfidenceLevel { get; }

        /// <inheritdoc />
        public override void StartRecognition()
        {
            if (!keywordRecognizer.IsRunning)
            {
                keywordRecognizer.Start();
            }
        }

        /// <inheritdoc />
        public override void StopRecognition()
        {
            if (keywordRecognizer.IsRunning)
            {
                keywordRecognizer.Stop();
            }
        }

        private void KeywordRecognizer_OnPhraseRecognized(PhraseRecognizedEventArgs args)
        {
            OnPhraseRecognized(args.confidence, args.phraseDuration, args.phraseStartTime, args.text);
        }

        private void OnPhraseRecognized(ConfidenceLevel confidence, TimeSpan phraseDuration, DateTime phraseStartTime, string text)
        {
            for (int i = 0; i < Commands.Length; i++)
            {
                if (Commands[i].Keyword == text)
                {
                    MixedRealityToolkit.InputSystem.RaiseSpeechCommandRecognized(InputSource, Commands[i].Action, (RecognitionConfidenceLevel)confidence, phraseDuration, phraseStartTime, text);
                    break;
                }
            }
        }

        #endregion IMixedRealitySpeechDataProvider Implementation

#endif // UNITY_STANDALONE_WIN || UNITY_WSA || UNITY_EDITOR_WIN

    }
}