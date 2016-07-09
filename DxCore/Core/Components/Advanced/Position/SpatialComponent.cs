using System;
using System.Runtime.Serialization;
using DxCore.Core.Components.Basic;
using DxCore.Core.Models;
using DxCore.Core.Primitives;
using DxCore.Core.Utils;
using DxCore.Core.Utils.Validate;

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

        public DxRectangle Space
        {
            get
            {
                DxVector2 worldCoordinates = WorldCoordinates;
                return new DxRectangle(worldCoordinates.X, worldCoordinates.Y, Dimensions.X, Dimensions.Y);
            }
        }

        public DxVector2 WorldCoordinates => CoordinateProducer.Invoke();

        private SpatialComponent(Func<DxVector2> coordinateProducer, DxVector2 dimensions)
        {
            CoordinateProducer = coordinateProducer;
            Dimensions = dimensions;
        }

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

        public sealed class UiTrackingSpatialComponentBuilder : IBuilder<SpatialComponent>
        {
            private Func<DxVector2> uiOffsetProvider_;
            private DxVector2? dimensions_;

            public SpatialComponent Build()
            {
                Validate.Hard.IsNotNull(uiOffsetProvider_);
                Validate.Hard.IsTrue(dimensions_.HasValue);
                UiOffsetTrackingPositionProvider uiOffsetTrackingProvider =
                    new UiOffsetTrackingPositionProvider(uiOffsetProvider_);
                return new SpatialComponent(uiOffsetTrackingProvider.GetPosition, dimensions_.Value);
            }

            public UiTrackingSpatialComponentBuilder WithUiOffsetProvider(Func<DxVector2> uiOffsetProvider)
            {
                uiOffsetProvider_ = uiOffsetProvider;
                return this;
            }

            public UiTrackingSpatialComponentBuilder WithDimensions(float x, float y)
            {
                return WithDimensions(new DxVector2(x, y));
            }

            public UiTrackingSpatialComponentBuilder WithoutDimensions()
            {
                return WithDimensions(DxVector2.EmptyVector);
            }

            public UiTrackingSpatialComponentBuilder WithDimensions(DxVector2 dimensions)
            {
                dimensions_ = dimensions;
                return this;
            }
        }

        public sealed class TrackingSpatialComponentBuilder : IBuilder<SpatialComponent>
        {
            private Func<DxVector2> worldCoordinateProvider_;
            private DxVector2? dimensions_;

            public SpatialComponent Build()
            {
                Validate.Hard.IsNotNull(worldCoordinateProvider_);
                Validate.Hard.IsTrue(dimensions_.HasValue);
                return new SpatialComponent(worldCoordinateProvider_, dimensions_.Value);
            }

            public TrackingSpatialComponentBuilder WithWorldCoordinateProvider(Func<DxVector2> worldCoordinateProvider)
            {
                worldCoordinateProvider_ = worldCoordinateProvider;
                return this;
            }

            public TrackingSpatialComponentBuilder WithDimensions(float x, float y)
            {
                return WithDimensions(new DxVector2(x, y));
            }

            public TrackingSpatialComponentBuilder WithoutDimensions()
            {
                return WithDimensions(DxVector2.EmptyVector);
            }

            public TrackingSpatialComponentBuilder WithDimensions(DxVector2 dimensions)
            {
                dimensions_ = dimensions;
                return this;
            }
        }

        public sealed class UiSpatialComponentBuilder : IBuilder<SpatialComponent>
        {
            private DxVector2? uiOffset_;
            private DxVector2? dimensions_;

            public SpatialComponent Build()
            {
                Validate.Hard.IsTrue(uiOffset_.HasValue);
                Validate.Hard.IsTrue(dimensions_.HasValue);
                UiOffsetPositionProvider uiPositional = new UiOffsetPositionProvider(uiOffset_.Value);
                return new SpatialComponent(uiPositional.GetPosition, dimensions_.Value);
            }

            public UiSpatialComponentBuilder WithUiOffset(float x, float y)
            {
                return WithUiOffset(new DxVector2(x, y));
            }

            public UiSpatialComponentBuilder WithUiOffset(DxVector2 uiOffset)
            {
                uiOffset_ = uiOffset;
                return this;
            }

            public UiSpatialComponentBuilder WithoutDimensions()
            {
                return WithDimensions(DxVector2.EmptyVector);
            }

            public UiSpatialComponentBuilder WithDimensions(float x, float y)
            {
                return WithDimensions(new DxVector2(x, y));
            }

            public UiSpatialComponentBuilder WithDimensions(DxVector2 dimensions)
            {
                dimensions_ = dimensions;
                return this;
            }
        }

        public sealed class SpatialComponentBuilder : IBuilder<SpatialComponent>
        {
            private DxVector2? position_;
            private DxVector2? dimensions_;

            public SpatialComponent Build()
            {
                Validate.Hard.IsTrue(position_.HasValue);
                Validate.Hard.IsTrue(dimensions_.HasValue);
                StaticPositionProvider positionProvider = new StaticPositionProvider(position_.Value);
                return new SpatialComponent(positionProvider.GetPosition, dimensions_.Value);
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

            public SpatialComponentBuilder WithoutDimensions()
            {
                return WithDimensions(DxVector2.EmptyVector);
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

        private static DxVector2 GetWorldPosition(DxVector2 uiOffset)
        {
            CameraModel cameraModel = DxGame.Instance.Model<CameraModel>();
            return cameraModel.Invert(uiOffset);
        }
    }
}
