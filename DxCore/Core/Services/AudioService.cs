using System;
using DxCore.Core.Messaging;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using NLog;

namespace DxCore.Core.Services
{
    public class AudioService : Service
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public override void OnAttach()
        {
            RegisterMessageHandler<AudioMessage>(HandleAudioMessage);
            base.OnAttach();
        }

        private void HandleAudioMessage(AudioMessage audioMessage)
        {
            switch(audioMessage.Type)
            {
                case AudioType.Music:
                {
                    LoadAndPlayMusic(audioMessage.Asset);
                    break;
                }
                case AudioType.Song:
                {
                    LoadAndPlayEffect(audioMessage.Asset);
                    break;
                }

                case AudioType.None:
                default:
                {
                    Logger.Debug("Received an {0} with an {1} of {2}, {3}: {4}", typeof(AudioMessage), typeof(AudioType),
                        audioMessage.Type, nameof(audioMessage.Asset), audioMessage.Asset);
                    break;
                }
            }
        }

        private void LoadAndPlayMusic(string track)
        {
            try
            {
                // TODO: Cache on our end?
                Song song = DxGame.Instance.Content.Load<Song>(track);
                MediaPlayer.Play(song);
            }
            catch(Exception e)
            {
                Logger.Error(e, "Could not load {0} as a {1}", track, typeof(Song));
            }
        }

        private void LoadAndPlayEffect(string effect)
        {
            // TOOD: Rate-limit?
            try
            {
                // TODO: Cache on our end?
                SoundEffect soundEffect = DxGame.Instance.Content.Load<SoundEffect>(effect);
                soundEffect.CreateInstance().Play();
            }
            catch(Exception e)
            {
                Logger.Error(e, "Could not load {0} as a {1}", effect, typeof(SoundEffect));
            }
        }
    }
}
