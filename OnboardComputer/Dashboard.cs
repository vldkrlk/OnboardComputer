namespace OnboardComputer
{
    internal class Dashboard : IOverheatHandler, ITemperatureNormalHandler, ICheckEngineHandler
    {
        public event Action<string> MessageGenerated;

        public bool RedLampOn { get; private set; }
        public bool CheckEngineLampOn { get; private set; }

        public void TurnOnRedLamp()
        {
            RedLampOn = true;
        }

        public void TurnOffRedLamp()
        {
            RedLampOn = false;
        }

        public void TurnOnCheckEngineLamp()
        {
            CheckEngineLampOn = true;
        }

        public void TurnOffCheckEngineLamp()
        {
            CheckEngineLampOn = false;
        }

  
        public void OnOverheat()
        {
            TurnOnRedLamp();
            MessageGenerated?.Invoke("Панель приладів: увімкнено червону лампочку");
        }

       
        public void OnTemperatureNormalized()
        {
            TurnOffRedLamp();
            MessageGenerated?.Invoke("Панель приладів: червону лампочку вимкнено");
        }

        public void OnCheckEngine()
        {
            TurnOnCheckEngineLamp();
            MessageGenerated?.Invoke("Панель приладів: увімкнено лампу Check Engine");
        }
    }
}
