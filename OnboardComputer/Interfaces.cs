namespace OnboardComputer
{
    
    internal interface IEngine
    {
        bool IsRunning { get; }
        void Start();
        void Stop();
    }

   
    internal interface IMeasurable
    {
        double Temperature { get; }
        double OilPressure { get; }
        void SetReadings(double temperature, double oilPressure);
    }

 
    internal interface IPowerControl
    {
        int Power { get; }
        void LimitPower(int percent);
        void RestorePower();
    }

    
    internal interface IDiagnosable
    {
        string GetStatus();
    }

  

  
    internal interface IOverheatHandler
    {
        void OnOverheat();
    }

 
    internal interface ITemperatureNormalHandler
    {
        void OnTemperatureNormalized();
    }

   
    internal interface ICheckEngineHandler
    {
        void OnCheckEngine();
    }
}
