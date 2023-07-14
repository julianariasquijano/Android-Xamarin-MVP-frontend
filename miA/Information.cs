using System.Collections.Generic;
using System.Json;

namespace miA
{
    public class Information
    {

        public static ResourceDefinition mainRd;
        public static ResourceDefinition foreignRd;
        public static ClientDefinition mainCd;
        public static List<ForeignAgenda> foreignAgendas;
        public static ForeignAgenda seletedForeignAgenda;

        public static bool ServiceStopFlag = true;

        public static bool PopulateResources()
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

                return true;
            }
            else
            {

                return false;

            }

        }

        public static bool PopulateClients()
        {
            foreignAgendas = new List<ForeignAgenda>();
            JsonValue resultado = Datos.LlamarWsSync(Datos.sessionDataWebServiceUrl + "getUserClients", Datos.idUsuario);

            if ((string)resultado["status"] == "OK")
            {
                string cdJson = (string)resultado["clients"];
                if (cdJson == "" || cdJson == null)
                {
                    mainCd = new ClientDefinition(); mainCd.name = "Grupo Principal";
                }
                else mainCd = ClientDefinition.FromJson(cdJson);

                return true;
            }
            else return false;

        }

        public static bool PopulateForeignAgendas()
        {
            
            foreignAgendas = new List<ForeignAgenda>();

            JsonValue resultado = Datos.LlamarWsSync(Datos.sessionDataWebServiceUrl + "getUserForeignAgendas", Datos.idUsuario);

            if ((string)resultado["status"] == "OK")
            {
                //string json = (string)resultado["foreignAgendas"];
                JsonValue foreignAgendasJson = resultado["foreignAgendas"];

                if (foreignAgendasJson != null)
                {
                    foreach (JsonValue foreignAgendaData in foreignAgendasJson)
                    {
                        var foreignAgenda = new ForeignAgenda();
                        foreignAgenda.name = (string)foreignAgendaData["name"];
                        foreignAgenda.phone = (string)foreignAgendaData["phone"];
                        foreignAgenda.country = (int)foreignAgendaData["country"];
                        foreignAgenda.pdb = (string)foreignAgendaData["pdb"];
                        foreignAgenda.idPdb = (int)foreignAgendaData["idPdb"];

                        foreignAgendas.Add(foreignAgenda);

                    }
                }

                return true;

            }
            else return false;

        }

    }

    public class ForeignAgenda{
        public string name;
        public string phone;
        public int country;
        public string pdb;
        public int idPdb;

        public override string ToString()
        {
            return name;
        }

    }

}