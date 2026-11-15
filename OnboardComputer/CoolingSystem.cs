namespace OnboardComputer
{
    // Система охолодження
    internal class CoolingSystem : IOverheatHandler, ITemperatureNormalHandler
    {
        public const int NormalFanSpeed = 40;
        public const int MaxFanSpeed = 100;

        // Подія для відправки повідомлень у журнал
        public event Action<string> MessageGenerated;

        // Швидкість вентилятора у відсотках
        public int FanSpeed { get; private set; }

        public CoolingSystem()
        {
            FanSpeed = NormalFanSpeed;
        }

        public void SetFanMax()
        {
            FanSpeed = MaxFanSpeed;
        }

        public void SetFanNormal()
        {
            FanSpeed = NormalFanSpeed;
        }

        // Обробник події "Перегрів"
        public void OnOverheat()
        {
            SetFanMax();
            MessageGenerated?.Invoke("Система охолодження: вентилятор увімкнено на максимум");
        }

        // Обробник події "Температура в нормі"
        public void OnTemperatureNormalized()
        {
            SetFanNormal();
            MessageGenerated?.Invoke("Система охолодження: вентилятор у звичайному режимі");
        }
    }
}
