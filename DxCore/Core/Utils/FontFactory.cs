using System;
using Microsoft.Xna.Framework.Graphics;
using WallNetCore.Cache.Advanced;

namespace DxCore.Core.Utils
{
    public class FontFactory
    {
        public const string DefaultSpriteFont = "Fonts/04b03_20_Regular";

        private static readonly Lazy<FontFactory> Singleton = new Lazy<FontFactory>(() => new FontFactory());

        public SpriteFont Default => SpriteFonts.Get(DefaultSpriteFont);

        public static FontFactory Instance => Singleton.Value;

        private ILoadingCache<string, SpriteFont> SpriteFonts { get; }

        private FontFactory()
        {
            SpriteFonts = new LocalLoadingCache<string, SpriteFont>(CacheBuilder<string, SpriteFont>.NewBuilder(),
                DxGame.Instance.Content.Load<SpriteFont>);
        }

        // TODO
    }
}