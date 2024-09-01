namespace Tokamak.Core.Drivers
{
    internal interface IDriverLoader
    {
        /// <summary>
        /// Called before the main window is created to provide any 
        /// pre initialization information about the window's creation.
        /// </summary>
        void Preload();

        /// <summary>
        /// Called after the main window is created to allow the driver
        /// to fully load itself against the newly created OS resource.
        /// </summary>
        void Load();

        /// <summary>
        /// Called just before the destruction of the main window so
        /// the driver can perform any clean up tasks before hand.
        /// </summary>
        void Unload();
    }
}
