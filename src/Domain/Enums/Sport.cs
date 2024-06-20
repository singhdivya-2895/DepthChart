﻿using System.Text.Json.Serialization;

namespace Domain.Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum Sport
    {
        NFL,
        MLB,
        NHL,
        NBA,
        TEST
    }
}
