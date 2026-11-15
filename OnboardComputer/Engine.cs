namespace OnboardComputer
{
    internal delegate void EngineAction();

    internal abstract class Engine : IEngine, IMeasurable, IPowerControl, IDiagnosable
    {
        public event Action<string> MessageGenerated;

        public bool IsRunning { get; private set; }
        public double Temperature { get; private set; }
        public double OilPressure { get; private set; }
        public int Power { get; private set; }

        public abstract string FuelType { get; }

        protected Engine()
        {
            Temperature = 20;
            OilPressure = 3;
            Power = 100;
        }

        public void Start()
        {
            IsRunning = true;
            MessageGenerated?.Invoke(FuelType + " двигун запущено");
        }

        public void Stop()
        {
            IsRunning = false;
            MessageGenerated?.Invoke(FuelType + " двигун зупинено");
        }

        public void SetReadings(double temperature, double oilPressure)
        {
            Temperature = temperature;
            OilPressure = oilPressure;
        }

        public void LimitPower(int percent)
        {
            if (percent < Power)
            {
                Power = percent;
            }
        }

        public void RestorePower()
        {
            Power = 100;
        }

        public string GetStatus()
        {
            string state;
            if (IsRunning)
                state = "працює";
            else
                state = "зупинений";

            return FuelType + " двигун: " + state + ", t = " + Temperature + "°C, тиск масла = "
                   + OilPressure + " бар, потужність = " + Power + "%";
        }
        public void ReportStatus()
        {
            MessageGenerated?.Invoke(GetStatus());
        }
    }


    internal class DieselEngine : Engine
    {
        public override string FuelType
        {
            get { return "Дизельний"; }
        }
    }

    internal class PetrolEngine : Engine
    {
        public override string FuelType
        {
            get { return "Бензиновий"; }
        }
    }
}
