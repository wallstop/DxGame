using System;
using System.Collections.Generic;
using System.Linq;
using DxCore.Core.Service;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NUnit.Framework;
using Assert = NUnit.Framework.Assert;

namespace DXGameTest.Core.Service
{
    internal class SimpleService : IService
    {
        public string Hello() => "Hello";
    }

    internal class MuchMoreComplexService : IService
    {
        public string AmVeryComplex() => "Just look at me.";
    }

    public class ServiceProviderTest
    {
        private static readonly SimpleService SimpleService = new SimpleService();
        private static readonly MuchMoreComplexService ComplexService = new MuchMoreComplexService();

        [TearDown]
        public void DeregisterSimpleService()
        {
            ServiceProvider.Instance.Deregister<SimpleService>();
            ServiceProvider.Instance.Deregister<MuchMoreComplexService>();
        }

        [Test]
        public void TestDeregisterOtherServiceSameTypeFails()
        {
            ServiceProvider.Instance.Register(SimpleService);
            SimpleService otherService = new SimpleService();
            bool deregisteredOk = ServiceProvider.Instance.Deregister(otherService);
            Assert.False(deregisteredOk);

            SimpleService registeredService = ServiceProvider.Instance.GetService<SimpleService>();
            Assert.NotNull(registeredService);
            HashSet<IService> registeredServices = ServiceProvider.Instance.GetAll();
            Assert.Contains(SimpleService, registeredServices.ToList());
            Assert.False(registeredServices.Contains(otherService));
        }

        [Test]
        public void TestDeregistration()
        {
            ServiceProvider.Instance.Register(SimpleService);
            bool deregisteredOk = ServiceProvider.Instance.Deregister<SimpleService>();
            Assert.True(deregisteredOk);

            HashSet<IService> services = ServiceProvider.Instance.GetAll();
            Assert.False(services.Contains(SimpleService));
        }

        [Test]
        public void TestDoubleDeregistrationOk()
        {
            ServiceProvider.Instance.Register(SimpleService);
            bool firstDeregisterOk = ServiceProvider.Instance.Deregister<SimpleService>();
            Assert.True(firstDeregisterOk);
            bool secondDeregisteredOk = ServiceProvider.Instance.Deregister<SimpleService>();
            Assert.False(secondDeregisteredOk);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void TestDoubleRegistrationThrows()
        {
            ServiceProvider.Instance.Register(SimpleService);
            ServiceProvider.Instance.Register(SimpleService);
        }

        [Test]
        public void TestGetServiceReturnsNullIfNotFound()
        {
            MuchMoreComplexService shouldNotBeRegistered = ServiceProvider.Instance.GetService<MuchMoreComplexService>();
            Assert.Null(shouldNotBeRegistered);
        }

        [Test]
        public void TestGetServiceReturnsServiceIfFound()
        {
            ServiceProvider.Instance.Register(ComplexService);
            MuchMoreComplexService shouldBeRegistered = ServiceProvider.Instance.GetService<MuchMoreComplexService>();
            Assert.NotNull(shouldBeRegistered);
            Assert.AreEqual(ComplexService, shouldBeRegistered);
        }

        [Test]
        public void TestInstancedDeregistration()
        {
            ServiceProvider.Instance.Register(SimpleService);
            bool deregisteredOk = ServiceProvider.Instance.Deregister(SimpleService);
            Assert.True(deregisteredOk);

            HashSet<IService> services = ServiceProvider.Instance.GetAll();
            Assert.False(services.Contains(SimpleService));
        }

        [Test]
        public void TestIServiceProviderWorks()
        {
            ServiceProvider.Instance.Register(SimpleService);
            object foundService = ServiceProvider.Instance.GetService(typeof(SimpleService));
            Assert.IsInstanceOf<SimpleService>(foundService);
        }

        [Test]
        public void TestRegistrationAddsServiceToServices()
        {
            ServiceProvider.Instance.Register(SimpleService);
            HashSet<IService> registeredServices = ServiceProvider.Instance.GetAll();
            Assert.Contains(SimpleService, registeredServices.ToList());
        }

        [Test]
        public void TestSimpleRegistration()
        {
            ServiceProvider.Instance.Register(SimpleService);

            SimpleService retrievedService;
            bool serviceFoundOk = ServiceProvider.Instance.TryGet(out retrievedService);
            Assert.True(serviceFoundOk, $"Expected to find a {typeof(SimpleService)}");
            Assert.AreEqual(SimpleService, retrievedService);
        }

        [Test]
        public void TestTryGetByTypeValue()
        {
            ServiceProvider.Instance.Register(SimpleService);
            object foundService;
            bool foundOk = ServiceProvider.Instance.TryGet(typeof(SimpleService), out foundService);
            Assert.True(foundOk);
            Assert.AreEqual(SimpleService, foundService);
        }

        [Test]
        public void TestTwoServicesCanRegisterOk()
        {
            ServiceProvider.Instance.Register(ComplexService);
            ServiceProvider.Instance.Register(SimpleService);
            Assert.AreEqual(SimpleService, ServiceProvider.Instance.GetService<SimpleService>());
            Assert.AreEqual(ComplexService, ServiceProvider.Instance.GetService<MuchMoreComplexService>());

            List<IService> registeredServices = ServiceProvider.Instance.GetAll().ToList();
            Assert.Contains(SimpleService, registeredServices);
            Assert.Contains(ComplexService, registeredServices);
        }
    }
}