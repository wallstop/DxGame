using System;
using System.Runtime.Serialization;
using DxCore.Core.Primitives;
using DxCore.Core.Service;

namespace DxCore.Core.Services
{
    [Serializable]
    [DataContract]
    public abstract class DxService : IService, ICreatable, IRemovable, IIdentifiable, IProcessable
    {
        [DataMember]
        protected GameObject Self { get; private set; }

        public UniqueId Id => Self.Id;

        protected DxService()
        {
            Self = GameObject.Builder().Build();
        }

        protected virtual void OnCreate() {}

        public void Create()
        {
            DxGame.Instance.ServiceProvider.Register(GetType(), this);
            OnCreate();
            Self.Create();
        }

        protected virtual void OnRemove() {}

        public void Remove()
        {
            DxGame.Instance.ServiceProvider.Deregister(this);
            OnRemove();
            Self.Remove();
        }

        public int CompareTo(IProcessable other)
        {
            throw new NotImplementedException();
        }

        public UpdatePriority UpdatePriority
        {
            get { throw new NotImplementedException(); }
        }

        public void Process(DxGameTime gameTime)
        {
            Self.Process(gameTime);
        }
    }
}