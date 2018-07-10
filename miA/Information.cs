using System;
namespace miA
{
    public class Information
    {

        public static ResourceDefinition mainRd;

        public static void PopulateResources()
        {
            mainRd = new ResourceDefinition();
            mainRd.name = "Grupo Principal";
            mainRd.children.Add(new ResourceDefinition());
            mainRd.children[0].name = "RdTest1";
            mainRd.children[0].type = ResourceTypes.Element;


            mainRd.children.Add(new ResourceDefinition());
            mainRd.children[1].name = "RdTest2";
            mainRd.children[1].children.Add(new ResourceDefinition());
            mainRd.children[1].children[0].name = "RdTest2.1";
            mainRd.children[1].children.Add(new ResourceDefinition());
            mainRd.children[1].children[1].name = "RdTest2.2";
            mainRd.children[1].children[1].type = ResourceTypes.Element;
        }
    }
}
