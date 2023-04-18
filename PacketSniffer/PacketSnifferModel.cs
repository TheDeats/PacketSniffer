using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading.Tasks;
using PacketDotNet;
using SharpPcap;
using SharpPcap.LibPcap;
using System.Windows;
using PacketSniffer.ViewModels;
using System.Windows.Documents;

namespace PacketSniffer
{
    public class PacketSnifferModel : INotifyPropertyChanged
    {
        #region Fields
        private LibPcapLiveDeviceList deviceList = LibPcapLiveDeviceList.Instance;
        private LibPcapLiveDevice selectedDevice;
        public event PropertyChangedEventHandler PropertyChanged;
        private int selectedDeviceIndex;
        private int selectedPacketIndex;
        private int nextPacketNumber = 1;
        private ObservableCollection<DisplayPacket> displayPackets = new ObservableCollection<DisplayPacket>();
        private List<RawCapture> sniffedPackets = new List<RawCapture>();
        //private string saveFilePath = Environment.CurrentDirectory + "packet_data.pcap";
        private string packetDataText = string.Empty;
        private string searchText = string.Empty;
        private bool isSniffing = false;

        #endregion

        #region Properties
        public ObservableCollection<string> DeviceNames { get; private set; }

        public string SearchText
        {
            get { return searchText; }
            set { searchText = value; }
        }
        public int SelectedDeviceIndex
        {
            get { return selectedDeviceIndex; }
            set 
            { 
                selectedDeviceIndex = value;
                selectedDevice = deviceList[selectedDeviceIndex];
            }
        }

        public int SelectedPacketIndex
        {
            get { return selectedPacketIndex; }
            set 
            { 
                selectedPacketIndex = value;
                RaisePropertyChanged(nameof(SelectedPacketIndex));
                UpdatePacketText();
            }
        }

        public bool IsSniffing
        {
            get { return isSniffing; }
            set 
            { 
                isSniffing = value;
                RaisePropertyChanged(nameof(IsSniffing));
            }
        }

        public ObservableCollection<DisplayPacket> DisplayPackets
        {
            get => displayPackets;
        }

        public int NextPacketNumber
        {
            get => nextPacketNumber++;
        }

        public string PacketDataText
        {
            get { return packetDataText; }
            set 
            { 
                packetDataText = value;
                RaisePropertyChanged(nameof(PacketDataText));
            }
        }

        public List<RawCapture> SniffedPackets
        {
            get => sniffedPackets;
        }

        #endregion

        #region Constructors
        public PacketSnifferModel()
        {

        }
        #endregion

        #region Methods

        public async Task ClearPackets()
        {
            DisplayPackets.Clear();
            SniffedPackets.Clear();
            nextPacketNumber = 1;
            RaisePropertyChanged(nameof(DisplayPackets));
        }

        public async Task DisplaySearchWindow()
        {
            try
            {
                SearchViewModel searchViewModel = new SearchViewModel(this);
                searchViewModel.DisplaySearchWindow();
            }
            catch (Exception e)
            {
                MessageBox.Show($"Error searching the packets, {e.Message}");
            }
        }

        public void SelectedDevice_PacketReceived(object sender, PacketCapture e)
        {
            try
            {
                int pacificStandardTimeOffset = -7;
                RawCapture packet = e.GetPacket();
                DateTime arrivalTime = packet.Timeval.Date;
                DateTime pacificStandardTime = arrivalTime.AddHours(pacificStandardTimeOffset);
                string timeFormatted = pacificStandardTime.ToString("h:mm:ss:fff");
                int length = packet.Data.Length;

                // using nuGet packet PacketDotNet for IPPacket extraction
                Packet dotNetPacket = Packet.ParsePacket(packet.LinkLayerType, packet.Data);
                var ipPacket = (IPPacket)dotNetPacket.Extract<IPPacket>();

                if (ipPacket != null)
                {
                    string source = ipPacket.SourceAddress.ToString();
                    string destination = ipPacket.DestinationAddress.ToString();
                    string protocol_type = ipPacket.Protocol.ToString();

                    sniffedPackets.Add(packet);
                    Application.Current.Dispatcher.Invoke(() => displayPackets.Add(new DisplayPacket(NextPacketNumber, timeFormatted, source, destination, protocol_type, length, packet.Data)));
                    RaisePropertyChanged(nameof(DisplayPackets));
                }
                else
                {
                    Debug.WriteLine("Failed to parse packet to ip packet");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error in packet received: {ex.Message}");
                MessageBox.Show("Error capturing packets from this device, try another device");
                StopSniffing();
            }
        }

        /// <summary>
        /// Raises the PropertyChanged event.
        /// </summary>
        /// <param name="propertyName">The name of the property that change.</param>
        protected void RaisePropertyChanged(string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public async Task RefreshDeviceList()
        {
            deviceList.Refresh();
            DeviceNames = new ObservableCollection<string>();
            foreach(LibPcapLiveDevice device in deviceList)
            {
                DeviceNames.Add(device.Interface.FriendlyName);
            }
            RaisePropertyChanged(nameof(DeviceNames));
        }

        public List<DisplayPacket> SearchPackets ()
        {
            List<DisplayPacket> searchPackets = new List<DisplayPacket>();
            for (int i = 0; i < SniffedPackets.Count; i++)
            {
                RawCapture packet = SniffedPackets[i];
                byte[] dataBytes = packet.Data;
                string data = System.Text.Encoding.ASCII.GetString(dataBytes);
                if(data.ToLower().Contains(SearchText.ToLower()))
                {
                    searchPackets.Add(DisplayPackets[i]);
                }
            }
            return searchPackets;
        }

        public async Task StartSniffing()
        {
            try
            {
                IsSniffing= true;
                selectedDevice.OnPacketArrival += new PacketArrivalEventHandler(SelectedDevice_PacketReceived);
                int readTimeoutMillis = 1000;
                selectedDevice.Open(DeviceModes.Promiscuous, readTimeoutMillis);
                Task.Run(async () =>
                {
                    if (selectedDevice.Opened)
                    {
                        selectedDevice.Capture();
                    }
                });
            }
            catch (Exception e)
            {
                MessageBox.Show("Error sniffing this device, try another device");
                StopSniffing();
            }
        }

        public async Task StopSniffing()
        {
            try
            {
                IsSniffing= false;
                selectedDevice.OnPacketArrival -= new PacketArrivalEventHandler(SelectedDevice_PacketReceived);
                if (selectedDevice.Opened)
                {
                    selectedDevice.StopCapture();
                    selectedDevice.Close();
                }
            }
            catch (Exception e)
            {
                // catch and ignore
            }
        }

        private void UpdatePacketText()
        {
            if (SelectedPacketIndex >= 0)
            {
                RawCapture packet = SniffedPackets[SelectedPacketIndex];
                byte[] dataBytes = packet.Data;
                string text = System.Text.Encoding.ASCII.GetString(dataBytes);
                PacketDataText = text;
            }
        }
        #endregion
    }
}
