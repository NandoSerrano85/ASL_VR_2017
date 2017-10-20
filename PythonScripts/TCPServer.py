import socketserver
import sys
import json

HOST = ''
PORT = 50000
BUFFER_SIZE = 4096

class SingleTCPHandler(socketserver.BaseRequestHandler):
    def handle(self):
        print("Have established connection with the client.")
        data = self.request.recv(BUFFER_SIZE).decode()
        print(createListFromJson(data))
        self.request.send("This gesture is an A.\n".encode())

class SimpleServer(socketserver.ThreadingMixIn, socketserver.TCPServer):
    daemon_threads = True
    allow_reuse_address = True

    def __init__(self, server_address, RequestHandlerClass):
        socketserver.TCPServer.__init__(self, server_address, RequestHandlerClass)

def createListFromJson(jsonString):
    json_acceptable_string = jsonString.replace("'", "\"")
    dataDict = json.loads(json_acceptable_string)
    features = [dataDict["FrameID"], dataDict["PinchStrength"], dataDict["GrabStrength"],
                dataDict["AverageDistance"], dataDict["AverageSpread"], dataDict["AverageTriSpread"]]
    return features;

if __name__ == "__main__":
    server = SimpleServer((HOST, PORT), SingleTCPHandler)

    print("\nListening for connections...\n")

    try:
        server.serve_forever()
    except KeyboardInterrupt:
        print("\nShutting down server.\n")
        sys.exit(0)

if __name__ == '__main__':
    main()
