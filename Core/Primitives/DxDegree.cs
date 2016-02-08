﻿using System;
using System.Runtime.Serialization;
using ProtoBuf;

namespace DXGame.Core.Primitives
{
    [Serializable]
    [DataContract]
    [ProtoContract]
    public struct DxDegree
    {
        [DataMember] [ProtoMember(1)] public float Value;

        public DxRadian DxRadian => new DxRadian(this);
        public DxVector2 UnitVector => DxRadian.UnitVector;

        public DxDegree(double value) : this((float) value) {}

        public DxDegree(float value)
        {
            Value = value;
        }

        public DxDegree(DxDegree degree)
        {
            Value = degree.Value;
        }

        public DxDegree(DxRadian radian)
        {
            Value = (float) (radian.Value * 180.0 / Math.PI);
        }

        public DxDegree(DxVector2 vector)
        {
            Value = new DxRadian(vector).Degree.Value;
        }
    }
}