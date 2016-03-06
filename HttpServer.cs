﻿using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace RemoteFork {
    public abstract class HttpServer {
        protected IPEndPoint ip;

        private TcpListener listener;
        private Thread thread;
        private bool is_active = true;

        protected HttpServer(IPAddress ip, int port) {
            is_active = true;
            this.ip = new IPEndPoint(ip, port);
        }

        public void Listen() {
            try {
                listener = new TcpListener(ip);
                listener.Start();
                while (is_active) {
                    try {
                        TcpClient client = listener.AcceptTcpClient();
                        HttpProcessor processor = new HttpProcessor(client, this);
                        thread = new Thread(processor.Process);
                        thread.Start();
                        Thread.Sleep(10);
                    } catch (Exception) {
                        Console.WriteLine("Stop");
                    }
                }
            } catch (Exception value) {
                Console.WriteLine(value);
            }
        }

        public void Stop() {
            if (is_active) {
                is_active = false;
                if (listener != null) {
                    listener.Stop();
                }
                if (thread != null) {
                    thread.Abort();
                }
            }
        }

        public abstract void HandleGetRequest(HttpProcessor processor);

        public abstract void HandlePostRequest(HttpProcessor processor, StreamReader inputData);
    }
}