import socket
import threading
import secrets
from Crypto.Cipher import AES
from Crypto.Util.Padding import pad, unpad
from hashlib import sha256
import os
import struct
import tkinter as tk
from tkinter import scrolledtext, messagebox

HOST = '127.0.0.1'
PORT = 12345

client_socket = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
client_socket.connect((HOST, PORT))

p_bytes_size = client_socket.recv(4)
p_size = struct.unpack("!I", p_bytes_size)[0]
p_bytes = client_socket.recv(p_size)
p = int.from_bytes(p_bytes, 'big')

g_bytes = client_socket.recv(1)
g = int.from_bytes(g_bytes, 'big')

private_key = secrets.randbelow(p)
public_key = pow(g, private_key, p)

client_socket.sendall(public_key.to_bytes(256, 'big'))

server_public_key_bytes = client_socket.recv(256)
server_public_key = int.from_bytes(server_public_key_bytes, 'big')

shared_secret = pow(server_public_key, private_key, p)
shared_key = sha256(str(shared_secret).encode()).digest()

def encrypt_message(message):
    iv = os.urandom(16)
    cipher = AES.new(shared_key, AES.MODE_CBC, iv)
    ciphertext = cipher.encrypt(pad(message.encode(), AES.block_size))
    return iv + ciphertext

def decrypt_message(encrypted_message):
    iv = encrypted_message[:16]
    ciphertext = encrypted_message[16:]
    cipher = AES.new(shared_key, AES.MODE_CBC, iv)
    plaintext = unpad(cipher.decrypt(ciphertext), AES.block_size)
    return plaintext.decode()

def receive_messages():
    while True:
        try:
            encrypted_message = client_socket.recv(1024)
            if encrypted_message:
                message = decrypt_message(encrypted_message)
                chat_display.config(state=tk.NORMAL)
                chat_display.insert(tk.END, f"{message}\n")
                chat_display.see(tk.END)
                chat_display.config(state=tk.DISABLED)
        except Exception as e:
            messagebox.showerror("Error", f"Error receiving message: {e}")
            client_socket.close()
            break

def send_message():
    message = message_entry.get()
    if message:
        try:
            encrypted_message = encrypt_message(message)

            chat_display.config(state=tk.NORMAL)
            chat_display.insert(tk.END, f"You: {message}\n")
            chat_display.see(tk.END)
            chat_display.config(state=tk.DISABLED)

            client_socket.sendall(encrypted_message)
            message_entry.delete(0, tk.END)
        except Exception as e:
            messagebox.showerror("Error", f"Error sending message: {e}")

root = tk.Tk()
root.title("Chat Client")

chat_display = scrolledtext.ScrolledText(root, state='disabled', width=50, height=20)
chat_display.grid(row=0, column=0, padx=10, pady=10)

message_entry = tk.Entry(root, width=40)
message_entry.grid(row=1, column=0, padx=10, pady=10, sticky='w')

send_button = tk.Button(root, text="Send", command=send_message)
send_button.grid(row=1, column=0, padx=10, pady=10, sticky='e')

threading.Thread(target=receive_messages, daemon=True).start()

root.mainloop()