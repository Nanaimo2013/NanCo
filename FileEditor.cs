using System;
using System.Collections.Generic;
using System.IO;

public class FileEditor
{
    public static void Start()
    {
        Console.Clear();
        Console.WriteLine("NanCo Simple Editor");
        Console.WriteLine("Commands: :w to save, :q to quit");
        Console.WriteLine("------------------------");

        List<string> lines = new List<string>();
        string currentLine;

        while (true)
        {
            currentLine = Console.ReadLine();
            
            if (currentLine == ":q")
                break;
                
            if (currentLine == ":w")
            {
                SaveFile(lines);
                continue;
            }
            
            lines.Add(currentLine);
        }
    }

    private static void SaveFile(List<string> lines)
    {
        Console.Write("Enter filename to save: ");
        string filename = Console.ReadLine();
        
        try
        {
            File.WriteAllLines(filename, lines);
            Console.WriteLine("File saved successfully!");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error saving file: {ex.Message}");
        }
    }
} 