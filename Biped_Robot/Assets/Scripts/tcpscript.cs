using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using UnityEngine;
using System.Text;
using System.Threading;

namespace TCP {
    public class TcpComClient{


        public String Host = "localhost";
        public Int32 Port = 55000;

        

        private Thread clientReceiveThread;
        private Thread MessageSendThread;
        private TcpClient mySocket = new TcpClient();

        public delegate void eventFunctionDelegate(string message);
        eventFunctionDelegate eventFunction;


        public void connect() {
            try {
                mySocket.Connect(Host, Port);
                clientReceiveThread = new Thread(new ThreadStart(listen));
                clientReceiveThread.IsBackground = true;
                clientReceiveThread.Start();
                Debug.Log("Successfully Connected");

            } catch (Exception e) {
                Debug.Log("Socket error: " + e);

            }
            
        }

        public void send_(string message) {
            try {
                Byte[] sendBytes = System.Text.Encoding.UTF8.GetBytes(message);
                mySocket.GetStream().Write(sendBytes, 0, sendBytes.Length);

            } catch (Exception e) {
                Debug.Log("Socket error: " + e);
                
            }
            
        }

        public void send(string message) {
            MessageSendThread = new Thread(() => send_(message));
            MessageSendThread.IsBackground = true;
            MessageSendThread.Start();

        }
        public void joinedSend(string message) {
            MessageSendThread = new Thread(() => send_(message));
            MessageSendThread.IsBackground = true;
            MessageSendThread.Start();
            MessageSendThread.Join();
        }
      
        public void disconnect() {
            
            try {
                mySocket.Close();

            } catch (Exception e) {
                Debug.Log("Socket error: " + e);

            }
        }

        public void listen() {
            
            try {
              
                Byte[] bytes = new Byte[1024];
                while (true) {			
                    using (NetworkStream stream = mySocket.GetStream()) {
                        int length;					
                        while ((length = stream.Read(bytes, 0, bytes.Length)) != 0) {
                            var incommingData = new byte[length];
                            Array.Copy(bytes, 0, incommingData, 0, length);						
                            string serverMessage = Encoding.ASCII.GetString(incommingData);

                            if (eventFunction != null) {
                                eventFunction(serverMessage);
                            } else {
                                Debug.Log("Event Function is not set!");
                            }
                        }
                    }
                }
            } catch (SocketException socketException) {
                Debug.Log("Socket exception: " + socketException);
            }

        }

        public void SetEventFunction(eventFunctionDelegate funcToEvent) {
            eventFunction = funcToEvent;
        
        }
    }
}