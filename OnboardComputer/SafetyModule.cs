namespace OnboardComputer
{
   
    internal class SafetyModule : IOverheatHandler, ITemperatureNormalHandler, ICheckEngineHandler
    {
        public const int OverheatPowerLimit = 50;    
        public const int LowOilPowerLimit = 30;      

       
        public event Action<string> MessageGenerated;

        private IPowerControl engine;

        public SafetyModule(IPowerControl engine)
        {
            this.engine = engine;
        }

        
        public void OnOverheat()
        {
            engine.LimitPower(OverheatPowerLimit);
            MessageGenerated?.Invoke("Модуль безпеки: потужність обмежено до " + engine.Power + "%");
        }

        
        public void OnTemperatureNormalized()
        {
            engine.RestorePower();
            MessageGenerated?.Invoke("Модуль безпеки: потужність відновлено до " + engine.Power + "%");
        }

        
        public void OnCheckEngine()
        {
            engine.LimitPower(LowOilPowerLimit);
            MessageGenerated?.Invoke("Модуль безпеки: низький тиск масла, потужність обмежено до " + engine.Power + "%");
        }
    }
}
