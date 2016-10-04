﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FingerClient
{
    static class Program
    {
        private static int numClients = 1;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {

            /*Thread[] server = new Thread[numClients];
            for (int i = 0; i < numClients; i++)
            {
                server[i] = new Thread(conexion);
                server[i].Start();
            }
            Thread.Sleep(250);*/

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1(this));
        }

        private static void conexion()
        {
            NamedPipeClientStream pipeClient = new NamedPipeClientStream(".", "testfinger", PipeDirection.InOut, PipeOptions.None, TokenImpersonationLevel.Impersonation);

            Console.WriteLine("\nConectando con el servidor...");
            pipeClient.Connect();

            ComunicacionStream cS = new ComunicacionStream(pipeClient);
            // Validate the server's signature string
            if (cS.leeCadena() == "Conectado al servidor"){
                // El token de seguridad del cliente se envía con la primera escritura.
                cS.enviaImagen(Image.FromFile(@"C:\Users\PC_STE_19\Documents\Visual Studio 2015\Projects\BiometricFinger\alterImages\020_2_2_muchas_lineas.jpg", true));

                if(cS.leeCadena() == "ENCONTRADO"){
                    string nombreUsuario = cS.leeCadena();
                    Console.WriteLine("Nombre usuario: "+nombreUsuario);
                }
                else{
                    Console.WriteLine("No ha podido ser identificado");
                }
            }
            else{
                Console.WriteLine("El servidor no pudo ser verificado. \n");
            }
            pipeClient.Close();
            // Dar al proceso cliente un tiempo para mostrar resultados antes de salir.
            Thread.Sleep(4000);
        }
    }
}