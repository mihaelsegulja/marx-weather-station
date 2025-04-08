using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace WebApp.Model;

public partial class WeatherData
{
    [Key]
    public int Id { get; set; }

    public DateTime? Timestamp { get; set; }

    [Column("IAQ")]
    public double Iaq { get; set; }

    [Column("StaticIAQ")]
    public double StaticIaq { get; set; }

    [Column("CO2Equivalent")]
    public double Co2equivalent { get; set; }

    [Column("BreathVOCEquivalent")]
    public double BreathVocEquivalent { get; set; }

    public double CompensatedTemperature { get; set; }

    public double Pressure { get; set; }

    public double CompensatedHumidity { get; set; }

    public double GasResistance { get; set; }
}
