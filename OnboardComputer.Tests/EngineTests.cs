using Microsoft.VisualStudio.TestTools.UnitTesting;
using OnboardComputer;

namespace OnboardComputer.Tests
{
    [TestClass]
    public class EngineTests
    {
        private DieselEngine engine;
        private EngineSensor sensor;
        private Dashboard dashboard;
        private CoolingSystem cooling;
        private SafetyModule safety;

        // Виконується перед кожним тестом: створює об'єкти і підписки, як у формі
        [TestInitialize]
        public void Setup()
        {
            engine = new DieselEngine();
            sensor = new EngineSensor(engine);
            dashboard = new Dashboard();
            cooling = new CoolingSystem();
            safety = new SafetyModule(engine);

            sensor.OverheatWarning += dashboard.OnOverheat;
            sensor.OverheatWarning += cooling.OnOverheat;
            sensor.OverheatWarning += safety.OnOverheat;

            sensor.CheckEngine += dashboard.OnCheckEngine;
            sensor.CheckEngine += safety.OnCheckEngine;

            sensor.TemperatureNormalized += dashboard.OnTemperatureNormalized;
            sensor.TemperatureNormalized += cooling.OnTemperatureNormalized;
            sensor.TemperatureNormalized += safety.OnTemperatureNormalized;

            engine.Start();
        }

        [TestMethod]
        public void Overheat_Above100_AllSubscribersReact()
        {
            engine.SetReadings(110, 3);
            sensor.Check();

            Assert.IsTrue(dashboard.RedLampOn);
            Assert.AreEqual(CoolingSystem.MaxFanSpeed, cooling.FanSpeed);
            Assert.AreEqual(SafetyModule.OverheatPowerLimit, engine.Power);
        }

        [TestMethod]
        public void NormalTemperature_NoOverheat()
        {
            engine.SetReadings(90, 3);
            sensor.Check();

            Assert.IsFalse(dashboard.RedLampOn);
            Assert.AreEqual(CoolingSystem.NormalFanSpeed, cooling.FanSpeed);
            Assert.AreEqual(100, engine.Power);
        }

        [TestMethod]
        public void Exactly100_IsNotOverheat()
        {
            engine.SetReadings(100, 3);
            sensor.Check();

            Assert.IsFalse(sensor.IsOverheated);
        }

        [TestMethod]
        public void LowOilPressure_CheckEngineLampAndPowerLimit()
        {
            engine.SetReadings(90, 0.5);
            sensor.Check();

            Assert.IsTrue(dashboard.CheckEngineLampOn);
            Assert.AreEqual(SafetyModule.LowOilPowerLimit, engine.Power);
        }

        [TestMethod]
        public void TemperatureNormalized_AfterOverheat_EverythingRestored()
        {
            engine.SetReadings(110, 3);
            sensor.Check();
            engine.SetReadings(80, 3);
            sensor.Check();

            Assert.IsFalse(dashboard.RedLampOn);
            Assert.AreEqual(CoolingSystem.NormalFanSpeed, cooling.FanSpeed);
            Assert.AreEqual(100, engine.Power);
        }

        [TestMethod]
        public void TemperatureNormalized_NotFiredWithoutOverheat()
        {
            bool fired = false;
            sensor.TemperatureNormalized += delegate { fired = true; };

            engine.SetReadings(80, 3);
            sensor.Check();

            Assert.IsFalse(fired);
        }

        [TestMethod]
        public void LimitPower_DoesNotIncreasePower()
        {
            engine.LimitPower(30);
            engine.LimitPower(50);

            Assert.AreEqual(30, engine.Power);
        }

        [TestMethod]
        public void MulticastDelegate_RunsAllMethods_AndRemoveWorks()
        {
            DieselEngine e = new DieselEngine();
            int count = 0;
            EngineAction chain = e.Start;
            chain += delegate { count++; };
            chain += delegate { count++; };

            chain();
            Assert.IsTrue(e.IsRunning);
            Assert.AreEqual(2, count);

            // Видаляємо Start з ланцюжка і зупиняємо двигун
            chain -= e.Start;
            e.Stop();
            chain();
            Assert.IsFalse(e.IsRunning);
            Assert.AreEqual(4, count);
        }

        [TestMethod]
        public void DieselEngine_ImplementsInterfaces()
        {
            Assert.IsInstanceOfType(engine, typeof(IEngine));
            Assert.IsInstanceOfType(engine, typeof(IMeasurable));
            Assert.IsInstanceOfType(engine, typeof(IPowerControl));
            Assert.IsInstanceOfType(engine, typeof(IDiagnosable));
        }

        [TestMethod]
        public void EngineHierarchy_FuelType()
        {
            Assert.AreEqual("Дизельний", new DieselEngine().FuelType);
            Assert.AreEqual("Бензиновий", new PetrolEngine().FuelType);
        }
    }
}
