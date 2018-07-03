﻿using System;
using System.IO;
using System.Json;
using System.Net;
using System.Text;
using System.Threading;
using System.Web;
using System.Security.Cryptography;
using System.Collections.Generic;

namespace miA
{
    public class Datos
    {


        public static string versionApp = "1"; //Habilita o inhabilita el uso de la app dependiendo de lo que el backend informe
        public static string mensajeVersion = "Por favor actualiza la versión de esta Aplicación.";
        public static string mensajeMantenimiento = "Aplicación en mantenimiento.";

        public static string registerAndAuthenticationWebServiceUrl = "http://www.enjoyframework.com?app=mia&mod=ra&act=";
        public static string sessionDataWebServiceUrl = "http://www.enjoyframework.com?app=mia&mod=ra&act=";
        public static string personalDataBaseWebServiceUrl = "http://www.enjoyframework.com?app=mia&mod=pdb&act=";
        public static string idUsuario = ""; //Usado cuando el usuario ha realizado un login satisfactorio
        public static string token = "";

        private static int intentosHttp = 3;
        private static int intentosHttpTimeOut = 3000;

        public Datos()
        {
            ServicePointManager.ServerCertificateValidationCallback += delegate { return true; };
        }

        public static bool EsCorreoElectronico(string correo)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(correo);
                return addr.Address == correo;
            }
            catch
            {
                return false;
            }
        }

        public static string Sha1Hash(string input)
        {
            using (SHA1Managed sha1 = new SHA1Managed())
            {
                var hash = sha1.ComputeHash(Encoding.UTF8.GetBytes(input));
                var sb = new StringBuilder(hash.Length * 2);

                foreach (byte b in hash)
                {
                    // can be "x2" if you want lowercase
                    sb.Append(b.ToString("X2"));
                }

                return sb.ToString();
            }
        }


        private static JsonValue HttpWsRequestSync(HttpWebRequest request)
        {

            JsonValue jsonDoc = new JsonObject
            {
                ["status"] = "ERROR",
                ["mensaje"] = "Error en la conexión",
            };


            request.ContentType = "application/json";
            request.Method = "GET";


            int contador = 1;
            while (contador <= intentosHttp)
            {
                request.Timeout = intentosHttpTimeOut * contador; // Cada intento espera mas.

                try
                {
                    // Send the request to the server and wait for the response:
                    using (WebResponse response = request.GetResponse())
                    {

                        // Get a stream representation of the HTTP web response:
                        using (Stream stream = response.GetResponseStream())
                        {
                            // Use this stream to build a JSON document object:
                            //jsonDoc = await Task.Run(() => JsonObject.Load(stream));
                            jsonDoc = JsonObject.Load(stream);
                            break;
                        }

                    }
                }
                catch (WebException ex)
                {
                    string status = ex.Status.ToString();
                    if (status == "RequestCanceled" || status == "ConnectFailure" || status == "NameResolutionFailure")
                    {
                        jsonDoc["mensaje"] = "Por favor verifica tu conexión a Internet";
                        break;
                    }
                    else if (status == "Timeout")
                    {
                        Thread.Sleep(1000);
                    }

                }
                catch (Exception ex)
                {
                    Thread.Sleep(1000);
                }

                contador++;
            }



            // Return the JSON document:
            return jsonDoc;

        }

        public static JsonValue LlamarWsSync(string ws, string var)
        {
            Encoding iso = Encoding.GetEncoding("ISO-8859-1");
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(new Uri( ws + "&var=" + HttpUtility.UrlEncode(var, iso) + "&versionApp=" + Datos.versionApp + "&token=" + Datos.token + "&idUsuario=" + Datos.idUsuario));
            return HttpWsRequestSync(request);
        }


        public static JsonValue verificarCredenciales(string email, string password)
        {

            // Create an HTTP web request using the URL:
            string cadenaDeParametros = "";
            cadenaDeParametros += "&correo=" + email;
            cadenaDeParametros += "&password=" + Sha1Hash(password);
            cadenaDeParametros += "&versionApp=" + Datos.versionApp;


            Encoding iso = Encoding.GetEncoding("ISO-8859-1");
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(new Uri(Datos.registerAndAuthenticationWebServiceUrl + "verificarCredenciales" + cadenaDeParametros));
            return HttpWsRequestSync(request);
        }

        public static JsonValue nuevoRegistro(Dictionary<string, string> datos)
        {

            // Create an HTTP web request using the URL:
            string cadenaDeParametros = "";
            cadenaDeParametros += "&nombre=" + datos["nombre"];
            cadenaDeParametros += "&correo=" + datos["correo"];
            cadenaDeParametros += "&telefono=" + datos["telefono"];
            cadenaDeParametros += "&password=" + datos["password"];

            Encoding iso = Encoding.GetEncoding("ISO-8859-1");
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(new Uri(Datos.registerAndAuthenticationWebServiceUrl + "nuevoRegistro" + cadenaDeParametros));
            return HttpWsRequestSync(request);
        }


    }
}
