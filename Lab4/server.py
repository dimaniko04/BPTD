import socket
import threading
import secrets
from Crypto.Cipher import AES
from Crypto.Util.Padding import pad, unpad
from Crypto.Util import number
from hashlib import sha256
import os
import struct

HOST = '127.0.0.1'
PORT = 12345

def generate_prime_and_root():
    p = number.getPrime(2048)
    g = 2
    return p, g

p, g = generate_prime_and_root()
print(f"Generated 2048-bit prime p: {p}")
print(f"Primitive root g: {g}")

private_key = secrets.randbelow(p)
public_key = pow(g, private_key, p)

clients = []
shared_keys = {}

def handle_client(client_socket):
    client_socket.sendall(struct.pack("!I", len(p.to_bytes(256, 'big'))))
    client_socket.sendall(p.to_bytes(256, 'big'))
    client_socket.sendall(g.to_bytes(1, 'big'))
    client_socket.sendall(public_key.to_bytes(256, 'big'))

    client_public_key_bytes = client_socket.recv(256)
    client_public_key = int.from_bytes(client_public_key_bytes, 'big')

    shared_secret = pow(client_public_key, private_key, p)
    shared_key = sha256(str(shared_secret).encode()).digest()
    shared_keys[client_socket] = shared_key

    print(f"[+] Client connected. Public key exchanged.")

    while True:
        try:
            encrypted_message = client_socket.recv(1024)
            if encrypted_message:
                iv = encrypted_message[:16]
                ciphertext = encrypted_message[16:]
                cipher = AES.new(shared_key, AES.MODE_CBC, iv)
                plaintext = unpad(cipher.decrypt(ciphertext), AES.block_size).decode()

                print(f"Received: {plaintext}")

                broadcast_message(f"Client {client_socket.getpeername()}: {plaintext}", client_socket)
            else:
                break
        except Exception as e:
            print("[-] Error handling message:", e)
            break

    print("[-] Client disconnected.")
    client_socket.close()
    clients.remove(client_socket)

def broadcast_message(message, sender_socket):
    for client in clients:
        if client != sender_socket:
            try:
                send_encrypted_message(client, message)
            except Exception as e:
                print("[-] Error sending message to a client:", e)
                client.close()
                clients.remove(client)

def send_encrypted_message(client_socket, message):
    shared_key = shared_keys[client_socket]
    iv = os.urandom(16)
    cipher = AES.new(shared_key, AES.MODE_CBC, iv)
    ciphertext = cipher.encrypt(pad(message.encode(), AES.block_size))
    client_socket.sendall(iv + ciphertext)

server_socket = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
server_socket.bind((HOST, PORT))
server_socket.listen()

print(f"[*] Server listening on {HOST}:{PORT}")

while True:
    client_socket, addr = server_socket.accept()
    print(f"[+] Accepted connection from {addr}")

    clients.append(client_socket)
    threading.Thread(target=handle_client, args=(client_socket,), daemon=True).start()