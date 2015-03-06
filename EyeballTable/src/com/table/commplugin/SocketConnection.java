package com.table.commplugin;

import java.net.*;
import java.io.*;
import java.util.Scanner;
import android.util.Log;
 
public class SocketConnection {
    private static final String MODULE = "SocketConnection";
    private Socket socket = null;
    private PrintStream printStream;
    private DataInputStream dis;
    private StringBuffer sb = new StringBuffer();
 
    public SocketConnection() {
        Log.v(MODULE, "Constructor starts");
        Log.v(MODULE, "Constructor finishes");
    }
 
    public int connect(String host, int port) {
       Log.v(MODULE, "Trying to connect to " + host + " " + port);

        try {
             socket = new Socket(host,  port);
             printStream = new PrintStream(socket.getOutputStream());
             dis = new DataInputStream(socket.getInputStream());
        } catch (IOException e) {
             Log.d(MODULE, e.toString());
             return -1;
        }
        Log.d(MODULE, "Connection made");
        return 0;
    }

    public void send(String msg) {
        Log.v(MODULE, "Socket sending " + msg);
        if(socket == null)
            Log.d(MODULE, "no connection ");
        else {
            printStream.println(msg);
            Log.d(MODULE, "message '" + msg + "' sent");
        }
    }

    public boolean connected() {
        return socket != null;
    }

    public void close() {
        Log.v(MODULE, "Closing socket");
        if(socket == null)
            Log.d(MODULE, "no connection ");
        else {
            try {
                socket.close();
                Log.d(MODULE, "Socket closed.");
            } catch(IOException e) {
                Log.d(MODULE, "Close failed " + e);
            }
        }
        socket = null;
    }        
    
    // note: BLOCKS if not ready
    public String getNextMessage() {
//        Log.v(MODULE, "getting next message");
        if(socket == null) {
            Log.d(MODULE, "no connection ");
	    return null;
        }
        try {
            while(dis.available() > 0) {
                char c = (char) dis.readByte(); // better be an ascii string coming in!
//                Log.d(MODULE, "read byte '" + c + "'");
                if((c == '\n') || (c == '\r')) {
                    String res = sb.toString();
                    sb.setLength(0);
//                    Log.d(MODULE, "returning '" + res + "' length " + res.length());
                    return res;
                 }
                 sb.append(c);
            }
//            Log.d(MODULE, "no full input line available");
            return null; // no more characters
        } catch(IOException e) {    
            Log.d(MODULE, "Error " + e);
            close();
            return null;
        }
    }
}
