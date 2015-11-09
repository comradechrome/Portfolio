using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace AgCubio
{
   /// <summary>
   /// When data arrives on a socket, keep track of:
   ///   * What previous data has arrived
   /// Decide:
   ///   * Wwhat function to call in the Client when data arrives.
   /// </summary>
   public class PreservedState
   {
   }

   /// <summary>
   /// Helper functions to handle networking.
   /// 
   /// Network Protocol Summary:
   ///  - Establish a socket connection to the server at port number 11000.
   ///  - Upon connection, send a single '\n' terminated string representing the player name.
   ///  - The server will then send a cube object in JSON representing the player.
   ///  - The server will then continually send the current state of the system. (All cube objects)
   ///  - The client can at anytime send a move or split request of the form([move|split], x, y)
   /// </summary>
   public static class Network
    {

      /// <summary>
      /// Attempt to connect to the server via a provided hostname. Save the callback function (in a state object)
      ///  for use when data arrives.
      /// </summary>
      /// <param name="callback">function inside the View to be called when a connection is made</param>
      /// <param name="hostname">name of the server to connect to</param>
      /// <returns></returns>
      static Socket Connect_to_Server(Delegate callback, String hostname)
      {

         // It will need to open a socket and then use the BeginConnect method.Note this method take the 
         // "state" object and "regurgitates" it back to you when a connection is made, thus allowing
         // "communication" between this function and the Connected_to_Server function.

         return null;
      }

      /// <summary>
      /// Referenced by the BeginConnect method above and is "called" by the OS when the socket connects to the server.
      /// 
      /// </summary>
      /// <param name="state_in_an_ar_object">contains a field "AsyncState" which contains the "state" object saved in 'BeginConnect'</param>
      static void Connected_to_Server(IAsyncResult state_in_an_ar_object)
      {
         // Once a connection is established the "saved away" callback function needs to called.
         // Additionally, the network connection should "BeginReceive" expecting more data to arrive 
         // (and provide the ReceiveCallback function for this purpose)
      }

      /// <summary>
      /// Called by the OS when new data arrives.
      /// This method should check to see how much data has arrived. 
      /// If 0, the connection has been closed (presumably by the server). 
      /// On greater than zero data, this method should call the callback function provided above.
      /// </summary>
      /// <param name="state_in_an_ar_object"></param>
      static void ReceiveCallback(IAsyncResult state_in_an_ar_object)
      {
 
         //For our purposes, this function should not request more data. 
         //It is up to the code in the callback function above to request more data.

      }

      /// <summary>
      /// Called when the View code wants more data.
      /// </summary>
      /// <param name="state"></param>
      static void i_want_more_data(object state )
      {
         //Note: the client will probably want more data every time it gets data.

      }

      /// <summary>
      /// With the help of 'SendCallBack', allow a program to send data over a socket.
      /// Convert to bytes then send using socket.BeginSend
      /// </summary>
      /// <param name="socket"></param>
      /// <param name="data"></param>
      static void Send(Socket socket, String data)
      {

      }

      /// <summary>
      /// Assists 'Send'. Do nothing if all data has been sent, otherwise arrange to send remaining data
      /// </summary>
      static void SendCallBack()
      {

         //If all the data has been sent, then life is good and nothing needs to be done 
         //(note: you may, when first prototyping your program, put a WriteLine in here to see when data goes out).

         //(see the ChatClient example program).
      }






   }
}
