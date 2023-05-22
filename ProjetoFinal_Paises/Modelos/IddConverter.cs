using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ProjetoFinal_Paises.Modelos;

public class IddConverter : JsonConverter<Idd>
{
    public override Idd ReadJson(
        JsonReader reader, Type objectType, Idd? existingValue,
        bool hasExistingValue, JsonSerializer serializer)
    {
        var iddObject = serializer.Deserialize<JObject>(reader);
        var root = iddObject?["root"]?.ToString();
        var suffixes = iddObject?["suffixes"]?.ToObject<List<string>>();

        return new Idd
        {
            Root = root,
            Suffixes = suffixes
        };
    }

    public override void WriteJson(
        JsonWriter writer, Idd? value,
        JsonSerializer serializer)
    {
        throw new NotImplementedException();
    }
}