using UnityEngine;
using System.Collections;
using NetMQ;
using NetMQ.Sockets;
using HW;
using FlatBuffers;

public class HelloWorld : MonoBehaviour {
    ResponseSocket server;
    RequestSocket client;
    FlatBufferBuilder builder;

    // Use this for initialization
    void Start () {
        server = new ResponseSocket("@tcp://localhost:5556"); // bind
        client = new RequestSocket(">tcp://localhost:5556"); // connect
        builder = new FlatBufferBuilder(1024);
    }

    // Update is called once per frame
    void Update () {
        {
            var name = builder.CreateString("my action");
            var position = Vec3.CreateVec3(builder, 1.0f, 2.0f, 3.0f);

            // Build action.
            Action.StartAction(builder);
            Action.AddPosition(builder, position);
            Action.AddName(builder, name);
            var action = Action.EndAction(builder);
            builder.Finish(action.Value);

            // Send buffer from the client socket.
            byte[] buf = builder.SizedByteArray();
            client.SendFrame(buf);
        }

        {
            // Receive the message from the server socket.
            var bytes = server.ReceiveFrameBytes();
            var buf = new ByteBuffer(bytes);

            // Access flatbuffer fields.
            var action = Action.GetRootAsAction(buf);
            var position = action.Position;
            Debug.Log("From Client: " + action.Name);
            Debug.Log("From Client: " + position.X);
            Debug.Log("From Client: " + position.Y);
            Debug.Log("From Client: " + position.Z);

            // Send a response back from the server
            server.SendFrame("Hi Back");

            // Receive the response from the client socket
            var m2 = client.ReceiveFrameString();
            Debug.Log("From Server: " + m2);
        }
    }
}
