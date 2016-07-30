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
            dynamic serviceInstance = this;
            DxGame.Instance.ServiceProvider.Register(serviceInstance);
            OnCreate();
            Self.Create();
        }

        protected virtual void OnRemove() {}

        public void Remove()
        {
            dynamic serviceInstance = this;
            DxGame.Instance.ServiceProvider.Deregister(serviceInstance);
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