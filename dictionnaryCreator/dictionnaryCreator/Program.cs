using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Text.Json;
using System.Text.Json.Serialization;
using UglyToad.PdfPig;
using UglyToad.PdfPig.Content;

public class WordEntry
{
    public string Romansh { get; set; }
    public string German { get; set; }
    public string French { get; set; }
    public string Italian { get; set; }
    public string SwissGerman { get; set; }
}

public class DictionaryParser
{
    public static List<WordEntry> Parse(string filePath)
    {
        var entries = new List<WordEntry>();
        var lines = new List<string>();

        // Lire le contenu du PDF et extraire le texte
        using (var document = PdfDocument.Open(filePath))
        {
            foreach (var page in document.GetPages())
            {
                var text = page.Text;
                // Diviser le texte en lignes
                var pageLines = text.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                lines.AddRange(pageLines);
            }
        }

        WordEntry currentEntry = null;
        StringBuilder definitionBuilder = new StringBuilder();

        // Expression régulière pour détecter le début d'une entrée
        var entryRegex = new Regex(@"^(?<romansh>[^\s]+(?:\s+[^\s]+)*)(?:\s+(?<abbrev>m\.|f\.|adj\.|v\.|adv\.|pron\.|part\.|n\.|interj\.))?\s*(?<definition>.*)$");

        foreach (var line in lines)
        {
            var trimmedLine = line.Trim();

            if (string.IsNullOrWhiteSpace(trimmedLine))
            {
                // Ligne vide, on l'ignore
                continue;
            }

            // Vérifier si la ligne correspond au début d'une nouvelle entrée
            if (!char.IsWhiteSpace(line, 0))
            {
                // Si nous avons une entrée en cours, nous la sauvegardons
                if (currentEntry != null)
                {
                    currentEntry.German = definitionBuilder.ToString().Trim();
                    entries.Add(currentEntry);
                    currentEntry = null;
                    definitionBuilder.Clear();
                }

                // Essayer de faire correspondre la ligne avec l'expression régulière
                var match = entryRegex.Match(trimmedLine);
                if (match.Success)
                {
                    currentEntry = new WordEntry();
                    currentEntry.Romansh = match.Groups["romansh"].Value.Trim();
                    // Vous pouvez également collecter l'abréviation si nécessaire

                    definitionBuilder.Append(match.Groups["definition"].Value.Trim());
                }
                else
                {
                    // La ligne ne correspond pas au format attendu, on l'ajoute à la définition en cours
                    if (currentEntry != null)
                    {
                        definitionBuilder.Append(" " + trimmedLine);
                    }
                }
            }
            else
            {
                // Ligne commençant par un espace, continuation de la définition
                if (currentEntry != null)
                {
                    definitionBuilder.Append(" " + trimmedLine);
                }
            }
        }

        // Ajouter la dernière entrée si elle existe
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
        // Chemin vers le fichier PDF contenant votre dictionnaire
        string filePath = "./Dizionari_dels_idioms_romauntschs_d_Engi.pdf"; // Assurez-vous que ce chemin est correct

        var entries = DictionaryParser.Parse(filePath);

        // Configurer les options du sérialiseur JSON pour gérer les caractères spéciaux
        var options = new JsonSerializerOptions
        {
            WriteIndented = true,
            Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        };

        // Exporter en JSON avec le format spécifié
        var result = new
        {
            words = entries.ConvertAll(entry => new
            {
                romanche = entry.Romansh,
                francais = entry.French ?? "",
                suisse_allemand = entry.SwissGerman ?? "",
                allemand = entry.German,
                italien = entry.Italian ?? ""
            })
        };

        var json = JsonSerializer.Serialize(result, options);

        // Écrire le fichier JSON en UTF-8 sans BOM
        var utf8WithoutBom = new UTF8Encoding(false);
        File.WriteAllText("dictionnaire.json", json, utf8WithoutBom);

        Console.WriteLine("Conversion terminée.");
    }
}
