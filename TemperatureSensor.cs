using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;

namespace TempSensor {
	public class TemperatureSensor {
		private string port;
		private int baudRate;

		public float Sensor1
		{get;set;}
		public float Sensor2
		{get;set; }
		private int state, id;

		private SerialPort comPort;
		private string buffer;

		public TemperatureSensor(string Port, int BaudRate)
		{
			port = Port;
			baudRate = BaudRate;
			Sensor1 = 0.0f;
			Sensor2 = 0.0f;
			state = 0;
			id = 0;
			comPort = new SerialPort(Port, baudRate);
			comPort.DataReceived += comPort_DataReceived;
		}

		void comPort_DataReceived(object sender, SerialDataReceivedEventArgs e) {
			buffer += comPort.ReadExisting();
			int index = 0;
			float value;
			while(buffer.Length > 0)
			{
				switch(state)
				{
					case 0:
						index = buffer.IndexOf(':');
						if(index == -1)	//There is no :, so we don'z have enough data
						{
							return;
						}	
						if(index < 2)	//There was some data lost
						{
							buffer = buffer.Substring(index+1);
							return;
						}
						id = int.Parse(buffer.Substring(index - 2, 2));
						buffer = buffer.Substring(index+1);
						state = 1;
						break;
					case 1:
                        if (buffer.Length < 6)
                            return;
						value = float.Parse(buffer.Substring(0, 6), System.Globalization.CultureInfo.InvariantCulture);
						if(id == 1)
						{
							Sensor1 = value;
						}
						else if (id == 2)
						{
							Sensor2 = value;
						}
                        buffer = buffer.Substring(6);
                        state = 0;
						break;
					default:
						state = 0;
						break;
				}
			}
		}

		public bool Open()
		{
			try{
				comPort.Open();
			}
			catch(Exception ex)
			{
				throw ex;
			}
			return true;
		}

		public bool Close()
		{
			if(comPort.IsOpen)
			{
				comPort.Close();
			}
			return true;
		}

	}
}
