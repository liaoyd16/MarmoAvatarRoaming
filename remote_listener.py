import socket
import struct
import subprocess
import serial
import time

server = socket.socket(socket.AF_INET, socket.SOCK_STREAM)

server_ip = subprocess.check_output(
    ['ipconfig', 'getifaddr', 'en0'],
    universal_newlines=True).strip()
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

def connect_to_arduino(port, baudrate=9600):
    try:
        ser = serial.Serial(port, baudrate, timeout=1)
        print(f"Connected to {port} at {baudrate} baud")
    except serial.SerialException as e:
        print(f"error: {e}")

    return ser


def send_to_arduino(ser, message):
    try:
        ser.write(f"{message}\n".encode('utf-8'))
        print(f"send {message}")

        time.sleep(0.1)
        response = ser.readline().decode('utf-8').strip()
        if response: print(f"Received: {response}")
    except serial.SerialException as e:
        print(f"error: {e}")

    return ser

portname = '/dev/cu.usbmodem14201'
ser = connect_to_arduino(port=portname)

while True:
    header_bytes = client_socket.recv(HEADER_LEN)
    if not header_bytes: break

    length = struct.unpack('!i', header_bytes)[0]
    data = recv_all(length)
    print(data.decode()) # type: ignore

    send_to_arduino(ser, data.decode())
