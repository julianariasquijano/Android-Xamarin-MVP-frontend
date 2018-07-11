using System;
using System.Json;

namespace miA
{
    public class Information
    {

        public static ResourceDefinition mainRd;
        public static ClientDefinition mainCd;

        public static void PopulateResources()
        {

            JsonValue resultado = Datos.LlamarWsSync(Datos.sessionDataWebServiceUrl + "getUserResources", Datos.idUsuario);

            if ((string)resultado["status"] == "OK")
            {
                string rdJson = (string)resultado["resources"];
                if (rdJson == "" || rdJson == null)
                {
                    mainRd = new ResourceDefinition();mainRd.name = "Grupo Principal";
                }
                else mainRd = ResourceDefinition.FromJson(rdJson);
            }

        }

        public static void PopulateClients()
        {

            JsonValue resultado = Datos.LlamarWsSync(Datos.sessionDataWebServiceUrl + "getUserClients", Datos.idUsuario);

            if ((string)resultado["status"] == "OK")
            {
                string cdJson = (string)resultado["clients"];
                if (cdJson == "" || cdJson == null)
                {
                    mainCd = new ClientDefinition(); mainCd.name = "Grupo Principal";
                }
                else mainCd = ClientDefinition.FromJson(cdJson);
            }

        }

    }
}
