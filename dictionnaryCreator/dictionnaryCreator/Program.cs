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

        foreach (var line in lines)
        {
            if (string.IsNullOrWhiteSpace(line))
            {
                continue; // Ignorer les lignes vides
            }

            // Vérifier si la ligne commence par un mot (pas d'espace en début de ligne)
            if (!char.IsWhiteSpace(line, 0))
            {
                // Expression régulière pour détecter le début d'une entrée
                var match = Regex.Match(line, @"^(?<romansh>\S+)(\s+(?<abbr>(?:[^\s.]+\.)+))?\s*(?<definition>.*)$");

                if (match.Success)
                {
                    // Sauvegarder l'entrée précédente si elle existe
                    if (currentEntry != null)
                    {
                        currentEntry.German = CleanDefinition(definitionBuilder.ToString());
                        entries.Add(currentEntry);
                        definitionBuilder.Clear();
                    }

                    // Créer une nouvelle entrée
                    currentEntry = new WordEntry();
                    currentEntry.Romansh = match.Groups["romansh"].Value;

                    // Ajouter la première ligne de la définition
                    var def = match.Groups["definition"].Value;
                    if (!string.IsNullOrEmpty(def))
                    {
                        definitionBuilder.AppendLine(def.Trim());
                    }
                }
                else
                {
                    // Si la ligne ne correspond pas, elle fait partie de la définition précédente
                    if (currentEntry != null)
                    {
                        definitionBuilder.AppendLine(line.Trim());
                    }
                }
            }
            else
            {
                // Ligne indentée, continuation de la définition
                if (currentEntry != null)
                {
                    definitionBuilder.AppendLine(line.Trim());
                }
            }
        }

        // Ajouter la dernière entrée si elle existe
        if (currentEntry != null)
        {
            currentEntry.German = CleanDefinition(definitionBuilder.ToString());
            entries.Add(currentEntry);
        }

        return entries;
    }

    private static string CleanDefinition(string rawDefinition)
    {
        // Remplacer les sauts de ligne par des espaces
        rawDefinition = Regex.Replace(rawDefinition, @"\s+", " ").Trim();

        // Vous pouvez ajouter d'autres nettoyages si nécessaire

        return rawDefinition;
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
