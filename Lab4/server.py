import socket
import struct
import threading
from Crypto.Util import number

HOST = '127.0.0.1'
PORT = 12345

def generate_prime_and_root():
    p = number.getPrime(2048)
    g = 2
    return p, g

p, g = generate_prime_and_root()

class ClientHandler:
    def __init__(self, conn, addr):
        self.conn = conn
        self.addr = addr
        self.server_public_key = g 
        self.flag = threading.Event()
        self.flag.set()
class ChatServer:
    def __init__(self):
        self.clients = []

    def start(self):
        self.server_socket = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
        self.server_socket.bind((HOST, PORT))
        self.server_socket.listen()
        print(f"[*] Server listening on {HOST}:{PORT}")

        while True:
            conn, addr = self.server_socket.accept()
            print(f"[+] Accepted connection from {addr}")
            
            client = ClientHandler(conn, addr)
            threading.Thread(target=self.handle_client, args=(client,), daemon=True).start()

    def handle_client(self, client):
        try:
            client_peer_name = str(client.conn.getpeername()).encode()
            client.conn.sendall(struct.pack("!I", len(client_peer_name)))
            client.conn.sendall(client_peer_name)
            
            client.conn.sendall(struct.pack("!I", len(p.to_bytes(256, 'big'))))
            client.conn.sendall(p.to_bytes(256, 'big'))
            
            client.server_public_key = self.get_previous_secret()
            client.conn.sendall(client.server_public_key.to_bytes(256, "big"))
                
            self.update_keys(client)
            self.clients.append(client)
            
            print("[+] Client connected. Public key received.")

            while True:
                client.flag.wait()
                data_bytes_size = client.conn.recv(4)
                if not data_bytes_size:
                    break
        
                data_size = struct.unpack("!I", data_bytes_size)[0]
                if data_size == 0:
                    continue
                
                data_bytes = client.conn.recv(data_size)        
                if not data_bytes:
                    break

                self.broadcast(client, data_bytes, data_size)

        except Exception as e:
            print(f"Error with client {client.addr}: {e}")
        finally:
            self.clients.remove(client)
            client.conn.close()

    def get_previous_secret(self):
        if len(self.clients) == 0:
            return g
        
        client = self.clients[-1]
        public_key = client.server_public_key
        
        client.conn.sendall(struct.pack("!I", 0))
        client.conn.sendall(b'\00')
        client.conn.sendall(public_key.to_bytes(256, "big"))
        
        client.flag.clear()
        previous_secret_bytes = client.conn.recv(256)
        client.flag.set()
        
        return int.from_bytes(previous_secret_bytes)
        

    def update_keys(self, sender):        
        for client in self.clients:
            try:                
                sender.conn.sendall(struct.pack("!I", 0))
                sender.conn.sendall(b'\00')
                sender.conn.sendall(client.server_public_key.to_bytes(256, "big"))
                
                #clients sends 0 to notify that its sending extended key not message  
                sender.conn.recv(4)
                client.server_public_key = int.from_bytes(sender.conn.recv(256))
                
                client.conn.sendall(struct.pack("!I", 0))
                client.conn.sendall(b'\01')
                client.conn.sendall(client.server_public_key.to_bytes(256, "big"))
            except Exception as e:
                print("[-] Error sending key to a client:", e)
    
    def broadcast(self, sender, data, size):
        for client in self.clients:
            if client != sender:
                try:
                    client.conn.sendall(struct.pack("!I", size))
                    client.conn.sendall(data)
                except Exception as e:
                    print("[-] Error sending message to a client:", e)

if __name__ == "__main__":
    server = ChatServer()
    server.start()
