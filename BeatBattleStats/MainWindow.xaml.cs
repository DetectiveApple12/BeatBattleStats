using BeatBattleStats.Scrpts;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BeatBattleStats
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        WebFetcher bbWeb;

        public MainWindow()
        {
            InitializeComponent();
            bbWeb = new();
            Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            SelectGP(GP_Intro);
            _ = IntroduceAnim();
        }

        async Task IntroduceAnim()
        {
            introTB.Width = 0;


            introIcon.BeginAnimation(MarginProperty, new ThicknessAnimation
            {
                From = new(0, 200, 0, 0),
                To = new(0),
                Duration = TimeSpan.FromSeconds(0.5),
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseOut }
            });
            introIconRot.BeginAnimation(RotateTransform.AngleProperty, new DoubleAnimation
            {
                From = 60,
                To = 0,
                Duration = TimeSpan.FromSeconds(2),
                EasingFunction = new ElasticEase { Oscillations = 3, Springiness = 5 }
            });
            
            await Task.Delay(1000);

            var scaleAnimOut = new DoubleAnimation
            {
                To = 0,
                Duration = TimeSpan.FromSeconds(0.5),
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseIn }
            };
            introIconScale.BeginAnimation(ScaleTransform.ScaleXProperty, scaleAnimOut);
            introIconScale.BeginAnimation(ScaleTransform.ScaleYProperty, scaleAnimOut);

            introTB.BeginAnimation(WidthProperty, new DoubleAnimation
            {
                To = 410,
                Duration = TimeSpan.FromSeconds(0.5),
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseInOut }
            });
            ((TextBlock)introTB.Child).BeginAnimation(FontSizeProperty, new DoubleAnimation
            {
                From = 10,
                To = ((TextBlock)introTB.Child).FontSize,
                Duration = TimeSpan.FromSeconds(1),
                EasingFunction = new ElasticEase { Oscillations = 2, Springiness = 6 }
            });

            await Task.Delay(1000);

            ((TextBlock)introTB.Child).BeginAnimation(FontSizeProperty, new DoubleAnimation
            {
                To = 0,
                Duration = TimeSpan.FromSeconds(1),
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseIn }
            });
            
            await Task.Delay(1000);

            _ = OnReady();
        }

        async Task OnReady()
        {
            SelectGP(GP_Main);
            string username = "ivoryapple";
            try
            {
                var html = await bbWeb.FetchProfileHTML(username);
                var profile = BeatBattleProfileParser.Parse(html);

            debugTB.Text = $"Profile stats for {username}:\n\n";
            debugTB.Text += $"\nBio: {profile.Bio}";
            debugTB.Text += $"\nGames played: {profile.Stats.QuickBattle.GamesPlayed}";
            debugTB.Text += $"\nGames won: {profile.Stats.QuickBattle.Wins}";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "ouch", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            
        }

        /// <summary>s
        /// Select a grid page
        /// </summary>
        /// <param name="selected">The actual page to select. Expects a Grid element</param>
        void SelectGP(Grid selected)
        {
            foreach (UIElement elem in GridPages.Children)
            {
                if (elem is Grid g)
                {
                    g.Visibility = (g == selected) ? Visibility.Visible : Visibility.Hidden;
                }
            }
        }
    }
}