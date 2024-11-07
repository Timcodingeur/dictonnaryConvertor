using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Text.Json;

public class WordEntry
{
    public string Romansh { get; set; }
    public string German { get; set; }
}

public class DictionaryParser
{
    public static List<WordEntry> Parse(string filePath)
    {
        var entries = new List<WordEntry>();
        var lines = File.ReadAllLines(filePath);

        WordEntry currentEntry = null;
        StringBuilder definitionBuilder = new StringBuilder();

        // Utiliser une expression régulière pour détecter une nouvelle entrée plus précisément
        foreach (var line in lines)
        {
            if (string.IsNullOrWhiteSpace(line))
            {
                continue; // Ignorer les lignes vides
            }

            var trimmedLine = line.Trim();

            // Si la ligne commence par une lettre majuscule suivie d'un point, il s'agit probablement d'une nouvelle entrée
            if (Regex.IsMatch(trimmedLine, @"^[A-ZÀ-ſ][a-zA-ZÀ-ſ']*\]"))
            {
                // Nouvelle entrée
                if (currentEntry != null)
                {
                    currentEntry.German = definitionBuilder.ToString().Trim();
                    entries.Add(currentEntry);
                    definitionBuilder.Clear();
                }

                // Extraire le mot en romanche
                var match = Regex.Match(trimmedLine, @"^(?<romansh>[A-Za-zÀ-ſ']+)\](?<german>.*)");

                if (match.Success)
                {
                    currentEntry = new WordEntry
                    {
                        Romansh = match.Groups["romansh"].Value,
                        German = match.Groups["german"].Value.Trim()
                    };
                }
                else
                {
                    currentEntry = null;
                    definitionBuilder.Clear();
                }
            }
            else
            {
                // Si la ligne est indentée ou continue la définition de la ligne précédente
                if (currentEntry != null)
                {
                    definitionBuilder.AppendLine(trimmedLine);
                }
            }
        }

        // Ajouter la dernière entrée
        if (currentEntry != null)
        {
            currentEntry.German = definitionBuilder.ToString().Trim();
            entries.Add(currentEntry);
        }

        return entries;
    }
}

class Program
{
    static void Main(string[] args)
    {
        // Chemin vers le fichier texte extrait
        string filePath = "./texte_extrait.txt"; // Assurez-vous que ce chemin est correct

        var entries = DictionaryParser.Parse(filePath);

        // Configurer les options du sérialiseur JSON pour gérer les caractères spéciaux
        var options = new JsonSerializerOptions
        {
            WriteIndented = true,
            Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        };

        // Exporter en JSON
        var result = new
        {
            words = entries
        };

        var json = JsonSerializer.Serialize(result, options);

        // Écrire le fichier JSON en UTF-8 sans BOM
        var utf8WithoutBom = new UTF8Encoding(false);
        File.WriteAllText("dictionnaire.json", json, utf8WithoutBom);

        Console.WriteLine("Conversion terminée.");
    }
}
