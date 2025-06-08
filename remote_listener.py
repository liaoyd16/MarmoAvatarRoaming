import socket
import struct

server = socket.socket(socket.AF_INET, socket.SOCK_STREAM)

server_ip = "127.0.0.1"
port = 11000
HEADER_LEN = 4

server.bind((server_ip, port))
server.listen(0)
print(f"listening on {server_ip}:{port}")

client_socket, client_addr = server.accept()
print(f"accepted connection from {client_addr[0]}:{client_addr[1]}")

def recv_all(n: int):
    data = bytearray()
    while len(data) < n:
        packet = client_socket.recv(n - len(data))
        if not packet: return None
        data.extend(packet)
    return data

while True:
    header_bytes = client_socket.recv(HEADER_LEN)
    if not header_bytes: break

    length = struct.unpack('!i', header_bytes)[0]
    data = recv_all(length)
    print(data.decode()) # type: ignore