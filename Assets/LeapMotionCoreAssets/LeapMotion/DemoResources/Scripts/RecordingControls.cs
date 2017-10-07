﻿using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(HandController))]
public class RecordingControls : MonoBehaviour {
  [Multiline]
  public string header;
  public Text controlsText;
  public Text recordingText;

  public KeyCode beginRecordingKey = KeyCode.R;
  public KeyCode endRecordingKey = KeyCode.R;
  public KeyCode beginPlaybackKey = KeyCode.P;
  public KeyCode pausePlaybackKey = KeyCode.P;
  public KeyCode stopPlaybackKey = KeyCode.S;

  private HandController _controller;

  void Start() {
    _controller = GetComponent<HandController>();
  }

  void Update() {
    if (controlsText != null) controlsText.text = header + "\n";

    switch (_controller.GetLeapRecorder().state) {
      case RecorderState.Recording:
        allowEndRecording();
        break;
      case RecorderState.Playing:
        allowPausePlayback();
        allowStopPlayback();
        break;
      case RecorderState.Paused:
        allowBeginPlayback();
        allowStopPlayback();
        break;
      case RecorderState.Stopped:
        allowBeginRecording();
        allowBeginPlayback();
        break;
    }
  }

  private void allowBeginRecording() {
    if (controlsText != null) controlsText.text += beginRecordingKey + " - Begin Recording\n";
    if (Input.GetKeyDown(beginRecordingKey)) {
      _controller.ResetRecording();
      _controller.Record();
      recordingText.text = "";
    }
  }

  private void allowBeginPlayback() {
    if (controlsText != null) controlsText.text += beginPlaybackKey + " - Begin Playback\n";
    if (Input.GetKeyDown(beginPlaybackKey)) {
      _controller.PlayRecording();
    }
  }

  private void allowEndRecording() {
    if (controlsText != null) controlsText.text += endRecordingKey + " - End Recording\n";
    if (Input.GetKeyDown(endRecordingKey)) {
      string savedPath = _controller.FinishAndSaveRecording();
      recordingText.text = "Recording saved to:\n" + savedPath;
    }
  }

  private void allowPausePlayback() {
    if (controlsText != null) controlsText.text += pausePlaybackKey + " - Pause Playback\n";
    if (Input.GetKeyDown(pausePlaybackKey)) {
      _controller.PauseRecording();
    }
  }

  private void allowStopPlayback() {
    if (controlsText != null) controlsText.text += stopPlaybackKey + " - Stop Playback\n";
    if (Input.GetKeyDown(stopPlaybackKey)) {
      _controller.StopRecording();
    }
  }
}