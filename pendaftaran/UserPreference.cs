using System.Windows;
using pendaftaran.Properties;

namespace pendaftaran
{
    public class UserPreferences
    {
        #region Constructor

        public UserPreferences()
        {
            //Load the settings
            Load();

            //Size it to fit the current screen
            SizeToFit();

            //Move the window at least partially into view
            MoveIntoView();
        }

        #endregion //Constructor

        #region Member Variables

        #endregion //Member Variables

        #region Public Properties

        public double WindowTop { get; set; }

        public double WindowLeft { get; set; }

        public double WindowHeight { get; set; }

        public double WindowWidth { get; set; }

        public WindowState WindowState { get; set; }

        #endregion //Public Properties

        #region Functions

        /// <summary>
        ///     If the saved window dimensions are larger than the current screen shrink the
        ///     window to fit.
        /// </summary>
        public void SizeToFit()
        {
            if (WindowHeight > SystemParameters.VirtualScreenHeight)
                WindowHeight = SystemParameters.VirtualScreenHeight;

            if (WindowWidth > SystemParameters.VirtualScreenWidth) WindowWidth = SystemParameters.VirtualScreenWidth;
        }

        /// <summary>
        ///     If the window is more than half off of the screen move it up and to the left
        ///     so half the height and half the width are visible.
        /// </summary>
        public void MoveIntoView()
        {
            if (WindowTop + WindowHeight / 2 > SystemParameters.VirtualScreenHeight)
                WindowTop = SystemParameters.VirtualScreenHeight - WindowHeight;

            if (WindowLeft + WindowWidth / 2 > SystemParameters.VirtualScreenWidth)
                WindowLeft = SystemParameters.VirtualScreenWidth - WindowWidth;

            if (WindowTop < 0) WindowTop = 0;

            if (WindowLeft < 0) WindowLeft = 0;
        }

        private void Load()
        {
            WindowTop = Settings.Default.WindowTop;
            WindowLeft = Settings.Default.WindowLeft;
            WindowHeight = Settings.Default.WindowHeight;
            WindowWidth = Settings.Default.WindowWidth;
            WindowState = Settings.Default.WindowState;
        }

        public void Save()
        {
            if (WindowState != WindowState.Minimized)
            {
                Settings.Default.WindowTop = WindowTop;
                Settings.Default.WindowLeft = WindowLeft;
                Settings.Default.WindowHeight = WindowHeight;
                Settings.Default.WindowWidth = WindowWidth;
                Settings.Default.WindowState = WindowState;

                Settings.Default.Save();
            }
        }

        #endregion //Functions
    }
}