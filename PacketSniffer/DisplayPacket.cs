namespace PacketSniffer
{
    public class DisplayPacket
    {
        private int _number;
        private string _time;
        private string _source;
        private string _destination;
        private string _protocol;
        private int _length;
        private byte[] _data;

        public DisplayPacket(int number, string time, string source, string destination, string protocol, int length, byte[] data)
        {
            _number = number;
            _time = time;
            _source = source;
            _destination = destination;
            _protocol = protocol;
            _length = length;
            _data = data;
        }

        public int Number
        {
            get { return _number; }
            set { _number = value; }
        }

        public string Time
        {
            get { return _time; } 
            set { _time= value; }
        }

        public string Source
        {
            get { return _source; } 
            set { _source= value; }
        }

        public string Destination
        {
            get { return _destination; }
            set { _destination = value; }
        }

        public string Protocol
        {
            get { return _protocol; }
            set { _protocol = value; }
        }

        public int Length
        {
            get { return _length; }
            set { _length = value; }
        }

        public byte[] Data
        {
            get { return _data; }
            set { _data = value; }
        }
    }
}
