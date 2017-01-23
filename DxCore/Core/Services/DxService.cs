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

        protected DxService()
        {
            Self = GameObject.Builder().Build();
        }

        public void Create()
        {
            dynamic serviceInstance = this;
            DxGame.Instance.ServiceProvider.Register(serviceInstance);
            OnCreate();
            Self.Create();
        }

        public UniqueId Id => Self.Id;

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

        public void Remove()
        {
            dynamic serviceInstance = this;
            DxGame.Instance.ServiceProvider.Deregister(serviceInstance);
            OnRemove();
            Self.Remove();
        }

        protected virtual void OnCreate() {}

        protected virtual void OnRemove() {}
    }
}