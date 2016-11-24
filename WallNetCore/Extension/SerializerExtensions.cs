using WallNetCore.Serialization;

namespace WallNetCore.Extension
{
    public static class SerializerExtensions
    {
        public static T Parse<T>(string toParse) => Serializer<T>.JsonDeserialize(toParse);

        public static string ToJson<T>(this T input)
        {
            byte[] json = Serializer<T>.JsonSerialize(input);
            string jsonAsText = SerializerEncoding.Encoding.GetString(json);
            return jsonAsText;
        }
    }
}