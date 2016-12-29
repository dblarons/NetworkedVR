using UnityEngine;
using System.Collections;
using NetMQ;
using NetMQ.Sockets;

public class HelloWorld : MonoBehaviour {
    ResponseSocket server;
    RequestSocket client;

	// Use this for initialization
	void Start () {
        server = new ResponseSocket("@tcp://localhost:5556"); // bind
        client = new RequestSocket(">tcp://localhost:5556"); // connect
	}

	// Update is called once per frame
	void Update () {
        // Send a message from the client socket
        client.SendFrame("Hello");

        // Receive the message from the server socket
        var m1 = server.ReceiveFrameString();
        Debug.Log("From Client: " + m1);

        // Send a response back from the server
        server.SendFrame("Hi Back");

        // Receive the response from the client socket
        var m2 = client.ReceiveFrameString();
        Debug.Log("From Server: " + m2);
	}
}
