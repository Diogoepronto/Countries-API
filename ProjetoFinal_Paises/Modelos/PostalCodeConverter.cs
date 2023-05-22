using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ProjetoFinal_Paises.Modelos;

public class PostalCodeConverter : JsonConverter<PostalCode>
{
    public override PostalCode ReadJson(
        JsonReader reader, Type objectType,
        PostalCode? existingValue, bool hasExistingValue,
        JsonSerializer serializer)
    {
        var jsonObject = JObject.Load(reader);
        var format = jsonObject["format"]?.ToString();
        var regex = jsonObject["regex"]?.ToString();

        return new PostalCode {Format = format, Regex = regex};
    }


    public override void WriteJson(
        JsonWriter writer, PostalCode? value,
        JsonSerializer serializer)
    {
        throw new NotImplementedException();
    }
}