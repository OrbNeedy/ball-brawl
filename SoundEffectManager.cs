using Godot;
using System;
using System.Collections.Generic;

public partial class SoundEffectManager : Node
{
    public Dictionary<AudioStream, AudioStreamPlayer2D> AudioPlayers = new();
    public Dictionary<AudioStream, bool> AudioIsTimeResistant = new();
    public Dictionary<AudioStream, AudioStreamPlayer> PositionlessAudioPlayers = new();
    public Dictionary<AudioStream, bool> PositionlessAudioIsTimeResistant = new();
    public static SoundEffectManager Instance { get; private set; }

    public override void _Ready()
    {
        Instance = this;
        ProcessMode = ProcessModeEnum.Always;
    }

    public override void _Process(double delta)
    {
        foreach (AudioStream key in AudioPlayers.Keys)
        {
            if (!AudioIsTimeResistant[key])
            {
                AudioPlayers[key].StreamPaused = GlobalTimeState.Instance.TimeStopDuration > 0;
            }
        }
    }

    /// <summary>
    /// Creates a new AudioStreamPlayer2D if needed and plays the audio once at the desired position
    /// </summary>
    /// <param name="audio"></param>
    /// <param name="position"></param>
    /// <param name="volume"></param>
    public void PlaySound(AudioStream audio, Vector2 position, float volume = 1, bool timeless = false)
    {
        if (AudioPlayers.ContainsKey(audio))
        {
            AudioPlayers[audio].VolumeLinear = volume;
            AudioPlayers[audio].Position = position;
            AudioIsTimeResistant[audio] = timeless;
            AudioPlayers[audio].Play();

            return;
        }

        AudioStreamPlayer2D newPlayer = new()
        {
            Stream = audio, 
            VolumeLinear = volume, 
            Position = position,
            Autoplay = true
        };

        AudioPlayers.Add(audio, newPlayer);
        AudioIsTimeResistant.Add(audio, timeless);
        AddChild(newPlayer);
    }

    /// <summary>
    /// Creates a new AudioStreamPlayer if needed and plays the audio once, this overload plays it non-positionally
    /// </summary>
    /// <param name="audio"></param>
    /// <param name="volume"></param>
    public void PlaySound(AudioStream audio, float volume = 1, bool timeless = false)
    {
        if (PositionlessAudioPlayers.ContainsKey(audio))
        {
            PositionlessAudioPlayers[audio].VolumeLinear = volume;
            PositionlessAudioIsTimeResistant[audio] = timeless;
            PositionlessAudioPlayers[audio].Play();

            return;
        }

        AudioStreamPlayer newPlayer = new()
        {
            Stream = audio, 
            VolumeLinear = volume, 
            Autoplay = true
        };

        PositionlessAudioPlayers.Add(audio, newPlayer);
        PositionlessAudioIsTimeResistant.Add(audio, timeless);
        AddChild(newPlayer);
    }

    public void StopSound(AudioStream audio, bool positionless = false)
    {
        if (PositionlessAudioPlayers.ContainsKey(audio) && positionless)
        {
            PositionlessAudioPlayers[audio].Stop();

            return;
        }

        if (AudioPlayers.ContainsKey(audio))
        {
            AudioPlayers[audio].Stop();

            return;
        }
    }
}
