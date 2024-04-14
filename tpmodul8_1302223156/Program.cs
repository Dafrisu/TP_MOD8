using System;
using System.IO;
using System.Text.Json;

internal class Program
{
    private static void Main(string[] args)
    {
        double suhuBadan;
        int hariDemam;

        CovidData defaultConf = new CovidData();

        Console.Write($"Berapa suhu badan Anda saat ini? Dalam nilai {defaultConf.CovidConf.satuan_suhu}: ");
        if (!double.TryParse(Console.ReadLine(), out suhuBadan))
        {
            Console.WriteLine("Input tidak valid. Harap masukkan nilai numerik untuk suhu badan.");
            return;
        }

        Console.Write("Berapa hari yang lalu (perkiraan) anda terakhir memiliki gejala demam? ");
        if (!int.TryParse(Console.ReadLine(), out hariDemam))
        {
            Console.WriteLine("Input tidak valid. Harap masukkan nilai numerik untuk jumlah hari demam.");
            return;
        }

        bool tepatWaktu = hariDemam < defaultConf.CovidConf.batas_hari_demam;
        bool terimaFahrenheit = (defaultConf.CovidConf.satuan_suhu == "Fahrenheit") && (suhuBadan >= 97.7 && suhuBadan <= 99.5);
        bool terimaCelcius = (defaultConf.CovidConf.satuan_suhu == "Celcius") && (suhuBadan >= 36.5 && suhuBadan <= 37.5);


        if (tepatWaktu && (terimaCelcius || terimaFahrenheit))
        {
            Console.WriteLine(defaultConf.CovidConf.pesan_diterima);
        }
        else
        {
            Console.WriteLine(defaultConf.CovidConf.pesan_ditolak);

        }

    }
}

public class CovidConfig
{
    public string satuan_suhu { get; set; }
    public int batas_hari_demam { get; set; }
    public string pesan_ditolak { get; set; }
    public string pesan_diterima { get; set; }

    public CovidConfig() { }
    public CovidConfig(string satuan_suhu, int batas_hari_demam, string pesan_ditolak, string pesan_diterima)
    {
        this.satuan_suhu = satuan_suhu;
        this.batas_hari_demam = batas_hari_demam;
        this.pesan_ditolak = pesan_ditolak;
        this.pesan_diterima = pesan_diterima;
    }
}

public class CovidData
{
    public CovidConfig CovidConf { get; private set; }
    private const string filePath = @"C:\Users\daffa\Documents\File semester 4\TP KPL\TP_MOD_08_1302223156_DAFARAIMISUANDI\tpmodul8_1302223156\covid_config.json";

    public CovidData()
    {
        try
        {
            ReadConfig();
        }
        catch (FileNotFoundException)
        {
            WriteNewConfig();
        }
        catch (JsonException)
        {
            Console.WriteLine("File konfigurasi tidak valid. Membuat konfigurasi default.");
            WriteNewConfig();
        }
    }

    private void ReadConfig()
    {
        string jsonData = File.ReadAllText(filePath);
        CovidConf = JsonSerializer.Deserialize<CovidConfig>(jsonData);
    }

    

    private void WriteNewConfig()
    {
        JsonSerializerOptions opts = new JsonSerializerOptions()
        {
            WriteIndented = true,
        };

        string jsonString = JsonSerializer.Serialize(CovidConf, opts);
        File.WriteAllText(filePath, jsonString);
    }

    public void UbahSatuan(string satuanBaru)
    {
        bool satuanValid = (satuanBaru == "Celcius" || satuanBaru == "Fahrenheit");

        if (!satuanValid)
        {
            throw new ArgumentException("Satuan suhu tidak valid.");
        }

        CovidConf.satuan_suhu = satuanBaru;
        WriteNewConfig();
    }
}