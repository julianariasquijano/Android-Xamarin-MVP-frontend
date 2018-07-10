using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;

namespace miA
{
    [DataContract]
    public class ResourceDefinition
    {
        [DataMember]
        public bool active = true;

        [DataMember]
        public string name = "";

        [DataMember]
        public ResourceTypes type = ResourceTypes.Group;

        [DataMember]
        public IList<ResourceDefinition> children { get; set; }

        public ResourceDefinition()
        {
            children = new List<ResourceDefinition>();
        }

        public static string ToJson(ResourceDefinition rd)
        {

            MemoryStream memoryStream = new MemoryStream();
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(ResourceDefinition));
            serializer.WriteObject(memoryStream, rd);
            memoryStream.Position = 0;
            StreamReader sr = new StreamReader(memoryStream);
            return sr.ReadToEnd();

        }

        public static ResourceDefinition FromJson(string json)
        {

            var memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(json));
            memoryStream.Position = 0;
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(ResourceDefinition));
            return (ResourceDefinition)serializer.ReadObject(memoryStream);

        }

        public static ResourceDefinition getNode(ResourceDefinition rd,string completeId,string partialId = null)
        {
            if (rd.name == completeId) return rd;// only for the base rd

            ResourceDefinition resultRd = null;
            if (partialId == null)
                partialId = rd.name;
            else
                partialId += "--" + rd.name;


            if (partialId == completeId) return rd;
            else {
                
                foreach (var child in rd.children)
                {
                    resultRd = getNode(child, completeId, partialId);
                    if (resultRd != null)
                    {
                        break;
                    }
                }

                return resultRd;
            }

        }

        public static string getParentNodeId(ResourceDefinition rd, string completeId, string partialId = null)
        {

            string parentId = null;

            string originalPartialId = partialId;
            if (partialId == null)
                partialId = rd.name;
            else
                partialId += "--" + rd.name;

            if (partialId == completeId) return originalPartialId;

            foreach (var child in rd.children)
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

    public enum ResourceTypes
    {

        Group = 0,
        Element = 1,
        None = 3

    }


}