using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;
using Ubiq.Samples;

public class GetData : MonoBehaviour
{
	private TcpClient socketConnection;
	private Thread clientReceiveThread;
	public NewBrain brain;
    
	// Use this for initialization 	
	void Start()
	{
		ConnectToTcpServer();
	}
    
	// Setup socket connection.
	private void ConnectToTcpServer()
	{
		try
		{
			clientReceiveThread = new Thread(new ThreadStart(ListenForData));
			clientReceiveThread.IsBackground = true;
			clientReceiveThread.Start();
		}
		catch (Exception e)
		{
			Debug.Log("On client connect exception " + e);
		}
	}
    
	// Runs in background clientReceiveThread; Listens for incomming data.
	private void ListenForData()
	{
		try
		{
			socketConnection = new TcpClient("localhost", 7777);
			Byte[] bytes = new Byte[1024];
			while (true)
			{
				// Get a stream object for reading 				
				using (NetworkStream stream = socketConnection.GetStream())
				{
					int length;

					// Read incomming stream into byte arrary. 					
					while ((length = stream.Read(bytes, 0, bytes.Length)) != 0)
					{
						var incommingData = new byte[length];
						Array.Copy(bytes, 0, incommingData, 0, length);
						
						// Convert byte array to string message.
						string message = Encoding.ASCII.GetString(incommingData);

						// Send the data to the brain
						if (brain != null)
							brain.dataString = message;
					}
				}
			}
		}
		catch (SocketException socketException)
		{
			Debug.Log("Socket exception: " + socketException);
		}
	}
}
