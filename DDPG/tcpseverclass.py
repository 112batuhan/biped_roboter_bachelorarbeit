import socket
import sys
import threading
import json

from time import sleep,time


class Server:

    def __init__(self, address = ('localhost', 55000)):

        self.sock = socket.socket(socket.AF_INET, socket.SOCK_STREAM)

        print(sys.stderr, 'starting up on %s port %s' % address)
        self.sock.bind(address)
        self.sock.listen(1)
        self.connected = False

        self.new_update = False
        self.telemetry = {}

    def listen_(self):
        '''
        TODO seperate connection and listening to get rid of wait for response(do it in simulation aswell)
        '''
        while True:
            # Wait for a connection
            print (sys.stderr, 'waiting for a connection')
            self.connection, client_address = self.sock.accept()
            self.connected = True
            while True:
                data = self.connection.recv(1024).decode()
                self.telemetry = json.loads(data)
                self.new_update = True

    def wait_for_response(self):
        '''
        i guess this should be fine for now
        '''

        while not self.new_update:
            pass
        self.new_update = False
        return self.telemetry
    

    def listen(self):
        x = threading.Thread(target=self.listen_, daemon=True)
        x.start()

    def send_(self, data):
        if not self.connected:
            print("no connection yet")
        else:
            json_string = json.dumps(data)
            self.connection.sendall(json_string.encode())
            #print(f"sent {data}")

    def send(self, data):
        x = threading.Thread(target=self.send_, args=[data], daemon=True)
        x.start()

if __name__ == "__main__":

    server = Server()
    
    server.listen()
    i = 0
    while True:

        data_to_send = {"force": i, "reset": False}
        server.send(data_to_send)
        print(server.telemetry)
        i=i+1.5
        sleep(2)
        