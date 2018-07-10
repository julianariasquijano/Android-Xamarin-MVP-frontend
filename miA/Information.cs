using System;
using System.Json;

namespace miA
{
    public class Information
    {

        public static ResourceDefinition mainRd;

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


            //mainRd.name = "Grupo Principal";
            //mainRd.children.Add(new ResourceDefinition());
            //mainRd.children[0].name = "RdTest1";
            //mainRd.children[0].type = ResourceTypes.Element;


            //mainRd.children.Add(new ResourceDefinition());
            //mainRd.children[1].name = "RdTest2";
            //mainRd.children[1].children.Add(new ResourceDefinition());
            //mainRd.children[1].children[0].name = "RdTest2.1";
            //mainRd.children[1].children.Add(new ResourceDefinition());
            //mainRd.children[1].children[1].name = "RdTest2.2";
            //mainRd.children[1].children[1].type = ResourceTypes.Element;
        }
    }
}
