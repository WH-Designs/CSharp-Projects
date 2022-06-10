using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace WarServer
{
    public class Game
    {
        private enum PayloadIndexes
        {
            Size,
            Command,
            Payload
        }
        public Socket Player1 { get; set; }
        public Socket Player2 { get; set; }

        private static void DealCards(out byte[] player1Stack, out byte[] player2Stack)
        {
            var cards = Enumerable.Range(1, 52).Select(card => (byte) card).ToList();
            cards.Shuffle();

            player1Stack = new List<byte>(cards.GetRange(0, 26)).ToArray();
            player2Stack = new List<byte>(cards.GetRange(26, 26)).ToArray();
        }

        private static int CompareCards(byte card1, byte card2)
        {
            return card1.CompareTo(card2);
        }

        public void Play()
        {
            //Shuffle cards
            //Deal cards
            DealCards(out var player1Stack, out var player2Stack);

            //Send the cards to the players (Construct payloads)
            var player1Payload = new List<byte>(player1Stack);
            player1Payload.Insert(0, (byte)CommandProtocol.GameStart);
            player1Payload.Insert(0, Convert.ToByte(player1Stack.Length));

            var player2Payload = new List<byte>(player2Stack);
            player2Payload.Insert(0, (byte)CommandProtocol.GameStart);
            player2Payload.Insert(0, Convert.ToByte(player2Stack.Length));

            SocketServer.Send(Player1, player1Payload.ToArray());
            SocketServer.Send(Player2, player2Payload.ToArray());

            //Recieve responses from the players
            var player1Response = SocketServer.Receive(Player1);
            var player2Response = SocketServer.Receive(Player2);

            //Ensure that the response is to play a game...
            //Assuming it's true then we get the next response
            if (player1Response[(int)PayloadIndexes.Command] == (byte)CommandProtocol.WantGame &&
                player2Response[(int)PayloadIndexes.Command] == (byte)CommandProtocol.WantGame)
            {
                //While response from each player is not QuitGame
                while (player1Response[(int)PayloadIndexes.Command] != (byte)CommandProtocol.QuitGame ||
                       player2Response[(int)PayloadIndexes.Command] != (byte)CommandProtocol.QuitGame)
                {
                    //Compare cards from the players
                    switch (CompareCards(player1Response[(int)PayloadIndexes.Payload], player2Response[(int)PayloadIndexes.Payload]))
                    {
                        //Send response back to each player
                        case -1:
                            SocketServer.Send(Player1, new byte[] { (byte)2, (byte)CommandProtocol.PlayResult, (byte)PlayResult.Lose });
                            SocketServer.Send(Player2, new byte[] { (byte)2, (byte)CommandProtocol.PlayResult, (byte)PlayResult.Win });
                            break;
                        case 0:
                            SocketServer.Send(Player1, new byte[] { (byte)2, (byte)CommandProtocol.PlayResult, (byte)PlayResult.Draw });
                            SocketServer.Send(Player2, new byte[] { (byte)2, (byte)CommandProtocol.PlayResult, (byte)PlayResult.Draw });
                            break;
                        case 1:
                            SocketServer.Send(Player1, new byte[] { (byte)2, (byte)CommandProtocol.PlayResult, (byte)PlayResult.Win });
                            SocketServer.Send(Player2, new byte[] { (byte)2, (byte)CommandProtocol.PlayResult, (byte)PlayResult.Lose });
                            break;
                    } 
                    //Recieve next responses from players
                    player1Response = SocketServer.Receive(Player1);
                    player2Response = SocketServer.Receive(Player2);
                }

                //After game is over shutdown sockets
                Player1.Close();
                Player2.Close();
            }
            else
            {
                Player1.Close();
                Player2.Close();
            }
        }
    }
}
