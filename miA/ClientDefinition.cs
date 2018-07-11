using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;

namespace miA
{
    [DataContract]
    public class ClientDefinition
    {
        [DataMember]
        public string name;

        [DataMember]
        public string mail;

        [DataMember]
        public ClientTypes type = ClientTypes.Group;

        [DataMember]
        public string pdb;

        [DataMember]
        public string idPdb;

        [DataMember]
        public List<ClientDefinition> children { get; set; }


        public ClientDefinition()
        {
            children = new List<ClientDefinition>();
        }

        public static string ToJson(ClientDefinition cd)
        {

            MemoryStream memoryStream = new MemoryStream();
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(ClientDefinition));
            serializer.WriteObject(memoryStream, cd);
            memoryStream.Position = 0;
            StreamReader sr = new StreamReader(memoryStream);
            return sr.ReadToEnd();

        }

        public static ClientDefinition FromJson(string json)
        {

            var memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(json));
            memoryStream.Position = 0;
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(ClientDefinition));
            return (ClientDefinition)serializer.ReadObject(memoryStream);

        }

        public static ClientDefinition getNode(ClientDefinition cd, string completeId, string partialId = null)
        {
            if (cd.name == completeId) return cd;// only for the base rd

            ClientDefinition resultCd = null;
            if (partialId == null)
                partialId = cd.name;
            else
                partialId += "--" + cd.name;


            if (partialId == completeId) return cd;
            else
            {

                foreach (var child in cd.children)
                {
                    resultCd = getNode(child, completeId, partialId);
                    if (resultCd != null)
                    {
                        break;
                    }
                }

                return resultCd;
            }

        }

        public static string getParentNodeId(ClientDefinition cd, string completeId, string partialId = null)
        {

            string parentId = null;

            string originalPartialId = partialId;
            if (partialId == null)
                partialId = cd.name;
            else
                partialId += "--" + cd.name;

            if (partialId == completeId) return originalPartialId;

            foreach (var child in cd.children)
            {
                parentId = getParentNodeId(child, completeId, partialId);
                if (parentId != null)
                {
                    break;
                }
            }

            return parentId;


        }


    }

    public enum ClientTypes
    {

        Group = 0,
        Element = 1,
        None = 3

    }

}