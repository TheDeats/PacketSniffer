using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;

namespace PacketSniffer
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        #region Fields
        PacketSnifferModel packetSnifferModel;
        public event PropertyChangedEventHandler PropertyChanged;
        

        #endregion

        #region Commands
        public ICommand RefreshDeviceListCommand => new AsyncCommand(packetSnifferModel.RefreshDeviceList);
        public ICommand StartSniffingCommand => new AsyncCommand(packetSnifferModel.StartSniffing);
        public ICommand StopSniffingCommand => new AsyncCommand(packetSnifferModel.StopSniffing);
        #endregion

        #region Properties
        public ObservableCollection<string> DeviceNames
        {
            get => packetSnifferModel.DeviceNames;
        }

        public ObservableCollection<DisplayPacket> DisplayPackets
        {
            get => packetSnifferModel.DisplayPackets;
        }

        public string PacketDataText
        {
            get => packetSnifferModel.PacketDataText;
        }

        public int SelectedDeviceIndex
        {
            get => packetSnifferModel.SelectedDeviceIndex;
            set => packetSnifferModel.SelectedDeviceIndex = value;
        }

        public int SelectedPacketIndex
        {
            get => packetSnifferModel.SelectedPacketIndex;
            set => packetSnifferModel.SelectedPacketIndex = value;
        }
        #endregion

        #region Constructors
        public MainWindowViewModel()
        {
            packetSnifferModel= new PacketSnifferModel();
            packetSnifferModel.RefreshDeviceList();
            packetSnifferModel.PropertyChanged += PacketSnifferModel_PropertyChanged;
        }
        #endregion

        #region Methods
        private void PacketSnifferModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            RaisePropertyChanged(e.PropertyName);
        }

        /// <summary>
        /// Raises the PropertyChanged event.
        /// </summary>
        /// <param name="propertyName">The name of the property that change.</param>
        protected void RaisePropertyChanged(string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion


    }
}
