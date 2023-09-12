using System;
using System.Text;
using System.Net.Sockets;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEditor.PackageManager;

public class Client : MonoBehaviour
{
    [SerializeField] InputField inputField;
    [SerializeField] InputField usernameInputField;

    string _clientMessage;
    string _clientUserName;

    Client()
    {
        _clientMessage = string.Empty;
        _clientUserName = string.Empty;
    }

    static void SendData(NetworkStream _stream, string _strData)
    {
        try
        {
            byte[] _data = Encoding.ASCII.GetBytes(_strData);
            _stream.Write(_data, 0, _data.Length);
        }
        catch
        {
            Console.WriteLine("Unable to send message.");
        }
    }

    static void RecieveData(NetworkStream stream)
    {
        Thread t = new Thread(new ParameterizedThreadStart(GetResponse));
        t.Start(stream);
    }

    /*IEnumerator recieveDataCoroutine(object obj)
    {
        NetworkStream _stream = (NetworkStream)obj;
        byte[] _data = new byte[256];

        while (_stream.CanRead)
        {
            try
            {
                int bytes = _stream.Read(_data, 0, _data.Length);
                string responseData = Encoding.ASCII.GetString(_data, 0, bytes);
                Console.Write(responseData);

            }
            catch
            {
                Console.WriteLine("Server is down.");
                break;
            }
        }
    }
*/
    static void GetResponse(object obj)
    {

    }
    void Start()
    {
        try
        {
            TcpClient _client = new TcpClient("127.0.0.1", 3000);
            NetworkStream _stream = _client.GetStream();

            StartCoroutine(streamCoroutine(_client, _stream));
        }
        catch
        {
            Console.WriteLine("Couldn't connect to server.");
        }
    }

    IEnumerator streamCoroutine(TcpClient _client, NetworkStream _stream)
    {
        while (IsMessageNull(_clientUserName))
        {
            yield return null;
        }

        SendData(_stream, _clientUserName);

        while (_stream.CanRead)
        {
            while (IsMessageNull(_clientMessage))
            {
                yield return null;
            }

            SendData(_stream, _clientMessage);
            _clientMessage = "";
        }

        _stream.Close();
        _client.Close();
    }


    bool IsMessageNull(string message)
    {
        if (message.Length < 1)
            return true;

        if (String.IsNullOrWhiteSpace(message))
            return true;

        return false;
    }
    public void SetUsername()
    {
        _clientUserName = usernameInputField.text;
    }
    public void SetCurrentMessage()
    {
        _clientMessage = usernameInputField.text;
    }
}