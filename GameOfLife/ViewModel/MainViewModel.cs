using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Media3D;
using System.Windows.Threading;
using System.Web;
using System.Threading.Tasks;
using System.ComponentModel;
using GameOfLife.Model;
using System.Windows.Controls;
using System.Windows.Shapes;
using System.Windows.Interop;
using Gdi = System.Drawing;
using System.Windows.Media.Imaging;
using System.Threading;
using System.Windows.Media.Effects;
using System.Resources.Extensions;

namespace GameOfLife.ViewModel
{
    /// <summary>
    /// Viewmodel für Mainview (Datenbindung)
    /// </summary>
    class MainViewModel : ViewModelBase
    {
        private DispatcherTimer FPSTimer;

        /// <summary>
        /// Window-Title
        /// </summary>
        public string ProgName { get { return System.Reflection.Assembly.GetExecutingAssembly().GetName().Name + " " + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version; } }

        public string SpeicherOrdner { get { return Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\" + ProgName; } }

        private static string DataFolder { get { return System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory.Remove(AppDomain.CurrentDomain.BaseDirectory.IndexOf("bin")), @"LevelFiles\"); } }

        private Canvas _CanvasDisplay = new Canvas();
        public Canvas CanvasDisplay
        {
            get { return _CanvasDisplay; }
            set { _CanvasDisplay = value; }
        }

        private Model.BitmapEngine _BitmapEng = new BitmapEngine();
        public Model.BitmapEngine BitmapEng
        {
            get { return _BitmapEng; }
            set { _BitmapEng = value; OnPropertyChangedEvent(nameof(BitmapEng)); }
        }

        public SolidColorBrush brushLife { get; set; } = new SolidColorBrush(Color.FromArgb(255, 255, 0, 200));
        public SolidColorBrush brushDeath { get; set; } = new SolidColorBrush(Color.FromArgb(255, 190, 190, 190));


        private int _FPS;
        public int FPS
        {
            get { return _FPS; }
            set { _FPS = value; OnPropertyChangedEvent(nameof(FPS)); }
        }

        private int _Delay;
        public int Delay
        {
            get { return _Delay; }
            set { _Delay = value; OnPropertyChangedEvent(nameof(Delay)); }
        } 

        private int FCnt { get; set; } = 0;

        #region Fields..

        private Model.GoLEngine _Spiel = new Model.GoLEngine();
        public Model.GoLEngine Spiel
        {
            get { return _Spiel; }
            set { _Spiel = value; OnPropertyChangedEvent(nameof(Spiel)); }

        }

        #endregion

        #region ActionCommands..

        public Command.ActionCommand ACClose
        {
            get;
            set;
        }

        public void ACCloseFunc(object parameter)
        {
            Environment.Exit(0);
        }

        public Command.ActionCommand ACLoadFile
        {
            get;
            set;
        }

        public void ACLoadFileFunc(object parameter)
        {
            LoadFile();
        }

        public Command.ActionCommand ACSaveFile
        {
            get;
            set;
        }

        public void ACSaveFileFunc(object parameter)
        {
            SaveFile();
        }

        public Command.ActionCommand ACCloseFile
        {
            get;
            set;
        }

        public void ACCloseFileFunc(object parameter)
        {
            CloseFile();
        }

        public Command.ActionCommand ACNewGame
        {
            get;
            set;
        }

        public void ACNewGameFunc(object parameter)
        {

        }

        public Command.ActionCommand ACCancel
        {
            get;
            set;
        }

        public void ACCancelFunc(object parameter)
        {

        }


        public Command.ActionCommand ACSpacePressed
        {
            get;
            set;
        }

        public void ACSpacePressedFunc(object parameter)
        {
            if (Spiel.IsRunning)
            {
                Spiel.Pause();
            }
        }

        public Command.ActionCommand ACESCPressed
        {
            get;
            set;
        }

        public void ACESCPressedFunc(object parameter)
        {
            //InitCanvas();
            FPSTimer.Stop();
            FPS = 0;
            Spiel.Stop();
        }

        public Command.ActionCommand ACReturnPressed
        {
            get;
            set;
        }

        public void ACReturnPressedFunc(object parameter)
        {
            //Init_Canvas();
            FPSTimer.Start();
            Spiel.Start();
        }

        public Command.ActionCommand ACPlusPressed
        {
            get;
            set;
        }

        public void ACPlusPressedFunc(object parameter)
        {
            if(Spiel.Delay < 100)
            Spiel.Delay++;
        }
        
        public Command.ActionCommand ACMinusPressed
        {
            get;
            set;
        }

        public void ACMinusPressedFunc(object parameter)
        {
            if (Spiel.Delay > 0)
                Spiel.Delay--;
        }


        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        public MainViewModel()
        {
            Init();
        }

        ~MainViewModel()
        {

        }

        #region Methoden..

        private void Init()
        {

            ACClose = new Command.ActionCommand();
            ACClose.CanExecuteFunc = obj => true;
            ACClose.ExecuteFunc = ACCloseFunc;

            ACLoadFile = new Command.ActionCommand();
            ACLoadFile.CanExecuteFunc = obj => true;
            ACLoadFile.ExecuteFunc = ACLoadFileFunc;

            ACSaveFile = new Command.ActionCommand();
            ACSaveFile.CanExecuteFunc = obj => true;
            ACSaveFile.ExecuteFunc = ACSaveFileFunc;

            ACCloseFile = new Command.ActionCommand();
            ACCloseFile.CanExecuteFunc = obj => true;
            ACCloseFile.ExecuteFunc = ACCloseFileFunc;

            ACNewGame = new Command.ActionCommand();
            ACNewGame.CanExecuteFunc = obj => true;
            ACNewGame.ExecuteFunc = ACNewGameFunc;

            ACCancel = new Command.ActionCommand();
            ACCancel.CanExecuteFunc = obj => true;
            ACCancel.ExecuteFunc = ACCancelFunc;

            ACSpacePressed = new Command.ActionCommand();
            ACSpacePressed.CanExecuteFunc = obj => true;
            ACSpacePressed.ExecuteFunc = ACSpacePressedFunc;

            ACESCPressed = new Command.ActionCommand();
            ACESCPressed.CanExecuteFunc = obj => true;
            ACESCPressed.ExecuteFunc = ACESCPressedFunc;

            ACReturnPressed = new Command.ActionCommand();
            ACReturnPressed.CanExecuteFunc = obj => true;
            ACReturnPressed.ExecuteFunc = ACReturnPressedFunc;

            ACPlusPressed = new Command.ActionCommand();
            ACPlusPressed.CanExecuteFunc = obj => true;
            ACPlusPressed.ExecuteFunc = ACPlusPressedFunc;

            ACMinusPressed = new Command.ActionCommand();
            ACMinusPressed.CanExecuteFunc = obj => true;
            ACMinusPressed.ExecuteFunc = ACMinusPressedFunc;

            Spiel.OnEngineUpdate += Spiel_OnEngineUpdate;

            FPSTimer = new DispatcherTimer();
            FPSTimer.Tick += FPSTimer_Tick;
            FPSTimer.Interval = new TimeSpan(0, 0, 1);


            CanvasDisplay.Width = Spiel.ScreenWidth;
            CanvasDisplay.Height = Spiel.ScreenHeigth;
            CanvasDisplay.Background = brushDeath.Clone();

            Init_Canvas();

        }

        private void Init_Canvas()
        {
            //BitmapEng.InitRenderer(Spiel.ScreenWidth, Spiel.ScreenHeigth);

            CanvasDisplay.Children.Clear();

            for (int i = 0; i < Spiel.ScreenWidth * Spiel.ScreenHeigth; i++)
            {
                CanvasDisplay.Children.Add(new QuadPixel());
                CanvasDisplay.Children[i].Visibility = Visibility.Hidden;
            }

            for (int x = 0; x < Spiel.ScreenWidth; x++)
            {
                for (int y = 0; y < Spiel.ScreenHeigth; y++)
                {
                    int index = y * Spiel.ScreenWidth + x;

                    ViewModel.QuadPixel rect = (ViewModel.QuadPixel)CanvasDisplay.Children[index];

                    Canvas.SetTop(rect, y);
                    Canvas.SetLeft(rect, x);
                }
            }
        }

        private void FPSTimer_Tick(object sender, EventArgs e)
        {
            FPS = FCnt;
            FCnt = 0;
        }

        private void CloseFile()
        {


            GC.Collect();
        }

        private void SaveFile()
        {

            SaveFileDialog sfd = new SaveFileDialog();

            sfd.AddExtension = true;
            sfd.DefaultExt = ".xml";
            sfd.Title = "Daten speichern..";
            sfd.Filter = "Xml-Datei|*.xml|All files (*.*)|*.*";
            sfd.FileName = System.IO.Path.GetFileName("CVEFromNIST.xml");
            sfd.AddExtension = true;
            sfd.OverwritePrompt = true;

            if ((bool)sfd.ShowDialog())
            {
                try
                {
                   
                    MessageBox.Show("Die Datei " + System.IO.Path.GetFileName(sfd.FileName) + " wurde erfolgreich gespeichert.", "Speichern abgeschlossen", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message.ToString(), "Speichern fehlgeschlagen!", MessageBoxButton.OK, MessageBoxImage.Error);
                }

            }
        }

        private void LoadFile()
        {
            OpenFileDialog ofd = new OpenFileDialog();

            ofd.AddExtension = true;
            ofd.DefaultExt = ".dat";
            ofd.Title = "Level laden..";
            ofd.Filter = "Dat-Datei|*.dat|All files (*.*)|*.*";
            ofd.FileName = "Level_1.dat";
            ofd.InitialDirectory = DataFolder;

            if ((bool)ofd.ShowDialog())
            {
                try
                {

                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "IO-Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }



        }

        #endregion

        #region Event-Handler..

        
        private void Spiel_OnEngineUpdate(object sender, EventArgs e)
        {
            Application.Current.Dispatcher.Invoke(new Action(() => 
            {
                //BitmapEng.RenderImage(Spiel.ScreenWidth, Spiel.ScreenHeigth, Spiel.m_output);

                CanvasDisplay.BeginInit();

                for (int i = 0; i < Spiel.m_output.Length; i++)
                {
                    if (Spiel.m_output[i].State == 1)
                    {
                        CanvasDisplay.Children[i].Visibility = Visibility.Visible;
                    }
                    else
                    {
                        CanvasDisplay.Children[i].Visibility = Visibility.Hidden;
                    }
                }

                CanvasDisplay.EndInit();
                Delay = Spiel.Delay;
                FCnt++;

            }));

            DoEvents();
        }

        /// <summary>
        /// Zeit für UI freigeben
        /// </summary>
        public static void DoEvents()
        {
            Application.Current.Dispatcher.Invoke(DispatcherPriority.Background,
                                                  new Action(delegate { }));
        }

        #endregion
    }
}
