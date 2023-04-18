using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;

namespace PacketSniffer.ViewModels
{
    public class SearchViewModel
    {
        PacketSnifferModel _packetSnifferModel;
        private SearchView searchView = new SearchView();
        public ObservableCollection<DisplayPacket> displayPackets = new ObservableCollection<DisplayPacket>();
        public string SearchText
        {
            get => _packetSnifferModel.SearchText;
            set => _packetSnifferModel.SearchText = value;
        }

        public int SelectedPacketIndex
        {
            get => _packetSnifferModel.SelectedPacketIndex;
            set 
            { 
                SetSelectedIndex(value); 
            }
        }

        public ObservableCollection<DisplayPacket> DisplayPackets
        {
            get { return displayPackets; }
            set { displayPackets = value; }
        }

        public ICommand SearchCommand => new AsyncCommand(Search);

        public SearchViewModel(PacketSnifferModel packetSnifferModel) 
        {
            _packetSnifferModel = packetSnifferModel;
        }

        public void DisplaySearchWindow()
        {
            searchView.Show();
            searchView.DataContext = this;
        }

        public async Task Search()
        {
            List<DisplayPacket> foundPackets = _packetSnifferModel.SearchPackets();
            DisplayPackets.Clear();
            foreach (DisplayPacket dp in foundPackets)
            {
                DisplayPackets.Add(dp);
            }
        }

        private void SetSelectedIndex(int index)
        {
            if(index >= 0)
            {
                int packetNumber = DisplayPackets[index].Number;
                _packetSnifferModel.SelectedPacketIndex = packetNumber - 1;
            }
        }
    }
}
