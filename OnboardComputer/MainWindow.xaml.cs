using System.Globalization;
using System.Windows;
using System.Windows.Media;

namespace OnboardComputer
{
    public partial class MainWindow : Window
    {
        // Об'єкти бізнес-логіки
        private DieselEngine engine;
        private EngineSensor sensor;
        private Dashboard dashboard;
        private CoolingSystem cooling;
        private SafetyModule safety;

        // Багатоадресні делегати: ланцюжки методів запуску і зупинки
        private EngineAction startSequence;
        private EngineAction stopSequence;

        public MainWindow()
        {
            InitializeComponent();

            // 1. Створюємо об'єкти
            engine = new DieselEngine();
            sensor = new EngineSensor(engine);
            dashboard = new Dashboard();
            cooling = new CoolingSystem();
            safety = new SafetyModule(engine);

            // 2. Широкомовні події: на одну подію підписано кілька об'єктів
            // Перегрів -> панель, охолодження, модуль безпеки
            sensor.OverheatWarning += dashboard.OnOverheat;
            sensor.OverheatWarning += cooling.OnOverheat;
            sensor.OverheatWarning += safety.OnOverheat;

            // Низький тиск масла -> панель, модуль безпеки
            sensor.CheckEngine += dashboard.OnCheckEngine;
            sensor.CheckEngine += safety.OnCheckEngine;

            // Температура в нормі -> панель, охолодження, модуль безпеки
            sensor.TemperatureNormalized += dashboard.OnTemperatureNormalized;
            sensor.TemperatureNormalized += cooling.OnTemperatureNormalized;
            sensor.TemperatureNormalized += safety.OnTemperatureNormalized;

            // Подія для журналу
            sensor.SensorChecked += LogSensorCheck;

            // 3. Повідомлення від усіх об'єктів потрапляють у журнал
            engine.MessageGenerated += LogMessage;
            dashboard.MessageGenerated += LogMessage;
            cooling.MessageGenerated += LogMessage;
            safety.MessageGenerated += LogMessage;

            // 4. Багатоадресний делегат: ланцюжок методів при запуску двигуна
            startSequence = engine.Start;
            startSequence += engine.RestorePower;
            startSequence += dashboard.TurnOffCheckEngineLamp;   // самотест панелі при запуску
            startSequence += engine.ReportStatus;

            // Ланцюжок методів при зупинці двигуна
            stopSequence = engine.Stop;
            stopSequence += engine.ReportStatus;
        }

        // --- Кнопка "Запустити двигун" ---
        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            // Один виклик делегата виконує весь ланцюжок методів
            startSequence();
            UpdateDashboard();
        }

        // --- Кнопка "Зупинити двигун" ---
        private void btnStop_Click(object sender, RoutedEventArgs e)
        {
            stopSequence();
            UpdateDashboard();
        }

        // --- Кнопка "Перевірити" ---
        private void btnCheck_Click(object sender, RoutedEventArgs e)
        {
            if (!engine.IsRunning)
            {
                LogMessage("ПОМИЛКА: спочатку запустіть двигун.");
                return;
            }

            double t;
            double p;
            // Приймаємо і крапку, і кому як десятковий роздільник
            string tText = txtTemperature.Text.Replace(',', '.');
            string pText = txtPressure.Text.Replace(',', '.');

            if (double.TryParse(tText, NumberStyles.Float, CultureInfo.InvariantCulture, out t) &&
                double.TryParse(pText, NumberStyles.Float, CultureInfo.InvariantCulture, out p))
            {
                // Записуємо нові показники в двигун, датчик їх зчитує і генерує події
                engine.SetReadings(t, p);
                sensor.Check();
                UpdateDashboard();
            }
            else
            {
                LogMessage("ПОМИЛКА: введіть коректні числові значення температури та тиску.");
            }
        }

        // Оновлює лампочки та показники на формі
        private void UpdateDashboard()
        {
            if (dashboard.RedLampOn)
                lampOverheat.Fill = Brushes.Red;
            else
                lampOverheat.Fill = Brushes.Gray;

            if (dashboard.CheckEngineLampOn)
                lampCheckEngine.Fill = Brushes.Yellow;
            else
                lampCheckEngine.Fill = Brushes.Gray;

            lblFan.Content = "Вентилятор: " + cooling.FanSpeed + "%";
            lblPower.Content = "Потужність: " + engine.Power + "%";
        }

        // Записує повідомлення в журнал
        private void LogMessage(string message)
        {
            logListBox.Items.Add("[" + DateTime.Now.ToString("HH:mm:ss") + "] " + message);
            logListBox.ScrollIntoView(logListBox.Items[logListBox.Items.Count - 1]);
        }

        // Обробник події SensorChecked
        private void LogSensorCheck(double t, double p)
        {
            LogMessage("Датчик: t = " + t + "°C, тиск масла = " + p + " бар");
        }
    }
}
