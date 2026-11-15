namespace OnboardComputer
{
    internal delegate void MyEventHandler();

    internal class EngineSensor
    {
        public const double MaxTemperature = 100;   
        public const double MinOilPressure = 1.0;   

        private IMeasurable engine;
        private bool overheated = false;

      
        public event MyEventHandler OverheatWarning;

        
        public event MyEventHandler CheckEngine;

      
        public event MyEventHandler TemperatureNormalized;

        public event Action<double, double> SensorChecked;

        public EngineSensor(IMeasurable engine)
        {
            this.engine = engine;
        }

        public bool IsOverheated
        {
            get { return overheated; }
        }

       
        public void Check()
        {
            double t = engine.Temperature;
            double p = engine.OilPressure;

            SensorChecked?.Invoke(t, p);

            if (t > MaxTemperature)
            {
                overheated = true;
                OverheatWarning?.Invoke();
            }
            else if (overheated)
            {
         
                overheated = false;
                TemperatureNormalized?.Invoke();
            }

            if (p < MinOilPressure)
            {
                CheckEngine?.Invoke();
            }
        }
    }
}
