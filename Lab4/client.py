from hashlib import sha256
import os
import secrets
import socket
import struct
import threading
from Crypto.Cipher import AES
from Crypto.Util.Padding import pad, unpad
import tkinter as tk
from tkinter import scrolledtext, messagebox

HOST = '127.0.0.1'
PORT = 12345

class ChatClient:
    def start(self):
        self.client_socket = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
        self.client_socket.connect((HOST, PORT))
        self.get_identifier()        
        self.init_keys()
    
        root = tk.Tk()
        root.title("Chat Client")

        self.chat_display = scrolledtext.ScrolledText(root, state='disabled', width=50, height=20)
        self.chat_display.grid(row=0, column=0, padx=10, pady=10)

        self.message_entry = tk.Entry(root, width=40)
        self.message_entry.grid(row=1, column=0, padx=10, pady=10, sticky='w')

        send_button = tk.Button(root, text="Send", command=self.send_message)
        send_button.grid(row=1, column=0, padx=10, pady=10, sticky='e')

        threading.Thread(target=self.handle_server, daemon=True).start()

        root.mainloop()

    def get_identifier(self):
        bytes_size = self.client_socket.recv(4)
        size = struct.unpack("!I", bytes_size)[0]
        self.identifier = self.client_socket.recv(size).decode()

    def init_keys(self):
        p_bytes_size = self.client_socket.recv(4)
        p_size = struct.unpack("!I", p_bytes_size)[0]
        p_bytes = self.client_socket.recv(p_size)
        p = int.from_bytes(p_bytes, 'big')
        
        self.p = p
        server_public_key_bytes = self.client_socket.recv(256)
        self.server_public_key = int.from_bytes(server_public_key_bytes, 'big')
        
        self.private_key = secrets.randbelow(p)
        
        shared_secret = pow(self.server_public_key, self.private_key, self.p)
        self.shared_key = sha256(str(shared_secret).encode()).digest()

    def handle_server(self):
        while True:
            try:
                size = self.receive_message_size()
                if size == 0:
                    self.handler_server_public_key()
                else:
                    self.receive_message(size)
            except Exception as e:
                messagebox.showerror("Error", f"Error receiving message: {e}")
                self.client_socket.close()
                break
    
    def receive_message_size(self):
        message_bytes_size = self.client_socket.recv(4)    
        message_size = struct.unpack("!I", message_bytes_size)[0]
        
        return message_size
    
    def receive_message(self, size):
        encrypted_message = self.client_socket.recv(size)
        if not encrypted_message:
            return
        message = self.decrypt_message(encrypted_message)
        self.chat_display.config(state=tk.NORMAL)
        self.chat_display.insert(tk.END, f"{message}\n")
        self.chat_display.see(tk.END)
        self.chat_display.config(state=tk.DISABLED)
    
    def handler_server_public_key(self):
        operation_bytes = self.client_socket.recv(1)
        operation = int.from_bytes(operation_bytes, "big")
        
        if (operation == 1):
            server_public_key_bytes = self.client_socket.recv(256) 
            self.server_public_key = int.from_bytes(server_public_key_bytes, 'big')  
            
            shared_secret = pow(self.server_public_key, self.private_key, self.p)
            self.shared_key = sha256(str(shared_secret).encode()).digest()
        else:
            key_bytes = self.client_socket.recv(256)
            key = int.from_bytes(key_bytes, 'big')
            
            extended_key = pow(key, self.private_key, self.p)
            self.client_socket.sendall(struct.pack("!I", 0))
            self.client_socket.sendall(extended_key.to_bytes(256, 'big'))
            
    def send_message(self):
        message = self.message_entry.get()
        if not message:
            return 

        self.chat_display.config(state=tk.NORMAL)
        self.chat_display.insert(tk.END, f"You: {message}\n")
        self.chat_display.see(tk.END)
        self.chat_display.config(state=tk.DISABLED)

        message = f"Client {self.identifier}: {message}"
        encrypted_message = self.encrypt_message(message)

        self.client_socket.sendall(struct.pack("!I", len(encrypted_message)))
        self.client_socket.sendall(encrypted_message)
        self.message_entry.delete(0, tk.END)
        
    
    def encrypt_message(self, message):
        iv = os.urandom(16)
        
        cipher = AES.new(self.shared_key, AES.MODE_CBC, iv)
        cipher_text = cipher.encrypt(pad(message.encode(), AES.block_size))
        
        return iv + cipher_text

    def decrypt_message(self, encrypted_message):
        iv = encrypted_message[:16]
        cipher_text = encrypted_message[16:]
        cipher = AES.new(self.shared_key, AES.MODE_CBC, iv)
        plaintext = unpad(cipher.decrypt(cipher_text), AES.block_size)
        return plaintext.decode()

if __name__ == "__main__":
    client = ChatClient()
    client.start()
