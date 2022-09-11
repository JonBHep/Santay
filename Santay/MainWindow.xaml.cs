using System.Windows;

namespace Santay
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void GymButton_Click(object sender, RoutedEventArgs e)
        {
            GymWindow w = new GymWindow() { Owner = this };
            w.ShowDialog();
        }

        private void AboutButton_Click(object sender, RoutedEventArgs e)
        {
            PaintingWindow ww = new PaintingWindow() { Owner = this };
            ww.ShowDialog();
        }

        private void VeloButton_Click(object sender, RoutedEventArgs e)
        {
            VeloMainWindow vm = new VeloMainWindow() { Owner = this };
            vm.ShowDialog();
        }
      
        private void BuButton_Click(object sender, RoutedEventArgs e)
        {
            BuWindow b = new BuWindow() { Owner = this };
            b.ShowDialog();
        }

        private void JournalButton_Click(object sender, RoutedEventArgs e)
        {
            JournalWindow win = new JournalWindow() {Owner = this};
            win.ShowDialog();
        }

        private void PlannerButton_Click(object sender, RoutedEventArgs e)
        {
            PlannerWindow win = new PlannerWindow() {Owner = this};
            win.ShowDialog();
        }

        private void WeightButton_Click(object sender, RoutedEventArgs e)
        {
            ListWeightWindow w = new ListWeightWindow() {Owner = this};
            _ = w.ShowDialog();
        }

        private void TensionButton_Click(object sender, RoutedEventArgs e)
        {
            ListBloodPressureWindow w = new ListBloodPressureWindow() {Owner = this};
            _ = w.ShowDialog();
        }

        private void ChartsButton_Click(object sender, RoutedEventArgs e)
        {
            FitnessPlotterWindow w = new FitnessPlotterWindow() {Owner = this};
            w.ShowDialog();
        }

        private void DiaryButton_Click(object sender, RoutedEventArgs e)
        {
            var w = new PrevuWindow() {Owner = this};
            _ = w.ShowDialog();
        }
        
    }
}