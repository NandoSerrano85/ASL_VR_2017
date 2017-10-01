from socket import socket, SOCK_STREAM, AF_INET, SOL_SOCKET, SO_REUSEADDR, SHUT_RDWR

BUFFER_SIZE = 4096

def createServerSocket(protocol_type, socket_type, hostname, port, numberOfConnections):
    serverSocket = socket(protocol_type, socket_type)
    serverSocket.setsockopt(SOL_SOCKET, SO_REUSEADDR, 1)
    serverSocket.bind((hostname, port))
    serverSocket.listen(numberOfConnections)
    return serverSocket

def processClientInput(clientSocket):
    while True:
        data = clientSocket.recv(BUFFER_SIZE).decode().rstrip('\r\n')
        clientSocket.send(("Data recevied: " + data + "\n").encode())

def main():
    host = ''
    port = 50000
    numOfConnections = 5

    serverSocket = createServerSocket(AF_INET, SOCK_STREAM, host, port, numOfConnections)

    while True:
        clientSocket, address = serverSocket.accept()
        print("Client connected")
        processClientInput(clientSocket)

if __name__ == '__main__':
    main()
