using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using Newtonsoft.Json;

[DataContract]
public class QueueEntry
{
    [JsonProperty(PropertyName = "queueName")]
    public string QueueName { get; set; }

    /// <summary>
    /// Description
    /// </summary>
    [JsonProperty(PropertyName = "queueMessage")]
    public string QueueMessage { get; set; }

    /// <summary>
    /// Description
    /// </summary>
    [JsonProperty(PropertyName = "action")]
    public string Action { get; set; }
}