using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using Newtonsoft.Json;

[DataContract]
public class BlobEntry
{
    [JsonProperty(PropertyName = "blobContainer")]
    public string BlobContainer { get; set; }

    /// <summary>
    /// Description
    /// </summary>
    [JsonProperty(PropertyName = "fileName")]
    public string FileName { get; set; }

    /// <summary>
    /// Description
    /// </summary>
    [JsonProperty(PropertyName = "action")]
    public string Action { get; set; }
}