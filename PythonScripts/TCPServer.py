from socket import socket, SOCK_STREAM, AF_INET, SOL_SOCKET, SO_REUSEADDR, SHUT_RDWR

HOST = ''
PORT = 50000
NUM_OF_CONNECTIONS = 5
BUFFER_SIZE = 4096

def createServerSocket(protocol_type, socket_type, hostname, port, numberOfConnections):
    serverSocket = socket(protocol_type, socket_type)
    serverSocket.setsockopt(SOL_SOCKET, SO_REUSEADDR, 1)
    serverSocket.bind((hostname, port))
    serverSocket.listen(numberOfConnections)
    return serverSocket

def processClientInput(clientSocket):
    while True:
        data = clientSocket.recv(BUFFER_SIZE).decode()

        if data == "quit":
            clientSocket.shutdown(SHUT_RDWR)
            clientSocket.close()
            break

        print("Data Received: " + data + "\n")

        clientSocket.send("This gesture is A\n".encode())

def main():
    serverSocket = createServerSocket(AF_INET, SOCK_STREAM, HOST, PORT, NUM_OF_CONNECTIONS)

    print("\nListening to connections\n")

    clientSocket, address = serverSocket.accept()
    print("\nClient connected\n")

    processClientInput(clientSocket)

    print("\nThe session has been terminated.\n")

if __name__ == '__main__':
    main()
