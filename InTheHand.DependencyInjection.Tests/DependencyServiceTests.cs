using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static InTheHand.DependencyInjection.DependencyService;

namespace InTheHand.DependencyInjection.Tests
{
    public interface IDependencyTest
    {
        bool Works { get; }
    }

    public interface IDependencyTestRegister
    {
        bool Works { get; }
    }

    public interface IUnsatisfied
    {
        bool Broken { get; }
    }

    public class DependencyTestImpl : IDependencyTest
    {
        public bool Works { get { return true; } }
    }

    public class DependencyTestRegisterImpl : IDependencyTestRegister
    {
        public bool Works { get { return true; } }
    }

    public class DependencyTestRegisterImpl2 : IDependencyTestRegister
    {
        public bool Works { get { return false; } }
    }

    [TestClass]
    public class DependencyServiceTests
    {
        [ClassInitialize]
        public static void Setup(TestContext context)
        {
            DependencyService.Register<DependencyTestImpl>();
        }

        [TestMethod]
        public void GetGlobalInstance()
        {
            var global = DependencyService.Get<IDependencyTest>();

            Assert.IsNotNull(global);

            var secondFetch = DependencyService.Get<IDependencyTest>();

            Assert.IsTrue(ReferenceEquals(global, secondFetch));
        }

        [TestMethod]
        public void NewInstanceIsNotGlobalInstance()
        {
            var global = DependencyService.Get<IDependencyTest>();

            Assert.IsNotNull(global);

            var secondFetch = DependencyService.Get<IDependencyTest>(DependencyFetchTarget.NewInstance);

            Assert.AreNotSame(global, secondFetch);
        }

        [TestMethod]
        public void NewInstanceIsAlwaysNew()
        {
            var firstFetch = DependencyService.Get<IDependencyTest>(DependencyFetchTarget.NewInstance);

            Assert.IsNotNull(firstFetch);

            var secondFetch = DependencyService.Get<IDependencyTest>(DependencyFetchTarget.NewInstance);

            Assert.AreNotSame(firstFetch, secondFetch);
        }

        [TestMethod]
        public void UnsatisfiedReturnsNull()
        {
            Assert.IsNull(DependencyService.Get<IUnsatisfied>());
        }

        [TestMethod]
        public void RegisterTypeImplementation()
        {
            DependencyService.Register<DependencyTestRegisterImpl>();
            var global = DependencyService.Get<DependencyTestRegisterImpl>();
            Assert.IsNotNull(global);
        }


        [TestMethod]
        public void RegisterInterfaceAndImplementations()
        {
            DependencyService.Register<IDependencyTestRegister, DependencyTestRegisterImpl2>();
            var global = DependencyService.Get<IDependencyTestRegister>();
            Assert.IsInstanceOfType(global, typeof(DependencyTestRegisterImpl2));
        }

        [TestMethod]
        public void RegisterInterfaceAndOverrideImplementations()
        {
            DependencyService.Register<IDependencyTestRegister, DependencyTestRegisterImpl>();
            DependencyService.Register<IDependencyTestRegister, DependencyTestRegisterImpl2>();
            var global = DependencyService.Get<IDependencyTestRegister>();
            Assert.IsInstanceOfType(global, typeof(DependencyTestRegisterImpl2));
        }

        [TestMethod]
        public void RegisterSingletonInterface()
        {
            var local = new DependencyTestRegisterImpl();
            DependencyService.RegisterSingleton<IDependencyTestRegister>(local);
            var global = DependencyService.Get<IDependencyTestRegister>();
            Assert.AreEqual(local, global);
        }
    }
}
