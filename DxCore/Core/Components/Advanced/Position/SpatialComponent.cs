using System;
using System.Runtime.Serialization;
using DxCore.Core.Components.Basic;
using DxCore.Core.Primitives;
using DxCore.Core.Services;
using DxCore.Core.Utils;
using WallNetCore.Validate;

namespace DxCore.Core.Components.Advanced.Position
{
    [Serializable]
    [DataContract]
    public sealed class SpatialComponent : Component, ISpatial
    {
        [DataMember]
        private Func<DxVector2> CoordinateProducer { get; set; }

        [DataMember]
        private DxVector2 Dimensions { get; set; }

        private SpatialComponent(Func<DxVector2> coordinateProducer, DxVector2 dimensions)
        {
            CoordinateProducer = coordinateProducer;
            Dimensions = dimensions;
        }

        public DxRectangle Space
        {
            get
            {
                DxVector2 worldCoordinates = WorldCoordinates;
                return new DxRectangle(worldCoordinates.X, worldCoordinates.Y, Dimensions.X, Dimensions.Y);
            }
        }

        public DxVector2 WorldCoordinates => CoordinateProducer.Invoke();

        public static SpatialComponentBuilder SpatialBasedBuilder()
        {
            return new SpatialComponentBuilder();
        }

        public static TrackingSpatialComponentBuilder TrackingBasedBuilder()
        {
            return new TrackingSpatialComponentBuilder();
        }

        public static UiSpatialComponentBuilder UiBasedBuilder()
        {
            return new UiSpatialComponentBuilder();
        }

        public static UiTrackingSpatialComponentBuilder UiTrackingBasedBuilder()
        {
            return new UiTrackingSpatialComponentBuilder();
        }

        private static DxVector2 GetWorldPosition(DxVector2 uiOffset)
        {
            CameraService cameraService = DxGame.Instance.Service<CameraService>();
            return cameraService.Invert(uiOffset);
        }

        public sealed class UiTrackingSpatialComponentBuilder : IBuilder<SpatialComponent>
        {
            private DxVector2? dimensions_;
            private Func<DxVector2> uiOffsetProvider_;

            public SpatialComponent Build()
            {
                Validate.Hard.IsNotNull(uiOffsetProvider_);
                Validate.Hard.IsTrue(dimensions_.HasValue);
                UiOffsetTrackingPositionProvider uiOffsetTrackingProvider =
                    new UiOffsetTrackingPositionProvider(uiOffsetProvider_);
                return new SpatialComponent(uiOffsetTrackingProvider.GetPosition, dimensions_.Value);
            }

            public UiTrackingSpatialComponentBuilder WithDimensions(float x, float y)
            {
                return WithDimensions(new DxVector2(x, y));
            }

            public UiTrackingSpatialComponentBuilder WithDimensions(DxVector2 dimensions)
            {
                dimensions_ = dimensions;
                return this;
            }

            public UiTrackingSpatialComponentBuilder WithoutDimensions()
            {
                return WithDimensions(DxVector2.EmptyVector);
            }

            public UiTrackingSpatialComponentBuilder WithUiOffsetProvider(Func<DxVector2> uiOffsetProvider)
            {
                uiOffsetProvider_ = uiOffsetProvider;
                return this;
            }
        }

        public sealed class TrackingSpatialComponentBuilder : IBuilder<SpatialComponent>
        {
            private DxVector2? dimensions_;
            private Func<DxVector2> worldCoordinateProvider_;

            public SpatialComponent Build()
            {
                Validate.Hard.IsNotNull(worldCoordinateProvider_);
                Validate.Hard.IsTrue(dimensions_.HasValue);
                return new SpatialComponent(worldCoordinateProvider_, dimensions_.Value);
            }

            public TrackingSpatialComponentBuilder WithDimensions(float x, float y)
            {
                return WithDimensions(new DxVector2(x, y));
            }

            public TrackingSpatialComponentBuilder WithDimensions(DxVector2 dimensions)
            {
                dimensions_ = dimensions;
                return this;
            }

            public TrackingSpatialComponentBuilder WithoutDimensions()
            {
                return WithDimensions(DxVector2.EmptyVector);
            }

            public TrackingSpatialComponentBuilder WithWorldCoordinateProvider(Func<DxVector2> worldCoordinateProvider)
            {
                worldCoordinateProvider_ = worldCoordinateProvider;
                return this;
            }
        }

        public sealed class UiSpatialComponentBuilder : IBuilder<SpatialComponent>
        {
            private DxVector2? Dimensions { get; set; }
            private DxVector2? UiOffset { get; set; }

            public SpatialComponent Build()
            {
                Validate.Hard.IsTrue(UiOffset.HasValue,
                    () => $"{nameof(UiOffset)} required to build {nameof(SpatialComponent)}");
                Validate.Hard.IsTrue(Dimensions.HasValue,
                    () => $"{nameof(Dimensions)} required to build {nameof(SpatialComponent)}");
                UiOffsetPositionProvider uiPositional = new UiOffsetPositionProvider(UiOffset.Value);
                return new SpatialComponent(uiPositional.GetPosition, Dimensions.Value);
            }

            public UiSpatialComponentBuilder WithDimensions(float x, float y)
            {
                return WithDimensions(new DxVector2(x, y));
            }

            public UiSpatialComponentBuilder WithDimensions(DxVector2 dimensions)
            {
                Dimensions = dimensions;
                return this;
            }

            public UiSpatialComponentBuilder WithoutDimensions()
            {
                return WithDimensions(DxVector2.EmptyVector);
            }

            public UiSpatialComponentBuilder WithUiOffset(float x, float y)
            {
                return WithUiOffset(new DxVector2(x, y));
            }

            public UiSpatialComponentBuilder WithUiOffset(DxVector2 uiOffset)
            {
                UiOffset = uiOffset;
                return this;
            }
        }

        public sealed class SpatialComponentBuilder : IBuilder<SpatialComponent>
        {
            private DxVector2? dimensions_;
            private DxVector2? position_;

            public SpatialComponent Build()
            {
                Validate.Hard.IsTrue(position_.HasValue);
                Validate.Hard.IsTrue(dimensions_.HasValue);
                StaticPositionProvider positionProvider = new StaticPositionProvider(position_.Value);
                return new SpatialComponent(positionProvider.GetPosition, dimensions_.Value);
            }

            public SpatialComponentBuilder WithDimensions(float x, float y)
            {
                return WithDimensions(new DxVector2(x, y));
            }

            public SpatialComponentBuilder WithDimensions(DxVector2 dimensions)
            {
                dimensions_ = dimensions;
                return this;
            }

            public SpatialComponentBuilder WithoutDimensions()
            {
                return WithDimensions(DxVector2.EmptyVector);
            }

            public SpatialComponentBuilder WithPosition(float x, float y)
            {
                return WithPosition(new DxVector2(x, y));
            }

            public SpatialComponentBuilder WithPosition(DxVector2 position)
            {
                position_ = position;
                return this;
            }
        }

        [Serializable]
        [DataContract]
        internal sealed class StaticPositionProvider
        {
            [DataMember]
            public DxVector2 Position { get; private set; }

            public StaticPositionProvider(DxVector2 position)
            {
                Position = position;
            }

            public DxVector2 GetPosition() => Position;
        }

        [Serializable]
        [DataContract]
        internal sealed class UiOffsetPositionProvider
        {
            [DataMember]
            public DxVector2 UiOffset { get; private set; }

            public UiOffsetPositionProvider(DxVector2 uiOffset)
            {
                UiOffset = uiOffset;
            }

            public DxVector2 GetPosition() => GetWorldPosition(UiOffset);
        }

        [Serializable]
        [DataContract]
        internal sealed class UiOffsetTrackingPositionProvider
        {
            [DataMember]
            public Func<DxVector2> UiOffsetProvider { get; private set; }

            public UiOffsetTrackingPositionProvider(Func<DxVector2> uiOffsetProvider)
            {
                UiOffsetProvider = uiOffsetProvider;
            }

            public DxVector2 GetPosition() => GetWorldPosition(UiOffsetProvider.Invoke());
        }
    }
}