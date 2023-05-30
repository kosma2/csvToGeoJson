using System;
using System.Text.Json;

namespace csvConvert
{
    public class Program
    {
        public class Feature
        {
            public String type { get; set; } = null!;
            public Geometry geometry { get; set; }= null!;
            public Properties properties { get; set; }= null!;
        }
        public class Properties
        {
            public int marketId { get; set; }
            public String countrCode { get; set; }= null!;
            public String city { get; set; }= null!;
            
        }
        public class Geometry
        {
            public String type { get; set; }= null!;
            public List<double> coordinates { get; set; }= null!;
        }
        public class geoJson
        {
            public String type { get; set; }= null!;
            public List<Feature> features { get; set; }= null!;
        }
        public static string ReadCsv(string filePath)
        {
           string csvString = File.ReadAllText(path:filePath);
           return csvString;
        }
        public static string[][] stringToArray(string csvString)
        {
            string normalStr = csvString.Normalize(System.Text.NormalizationForm.FormD);
            // split csv string into rows
            string[] splitReturn = normalStr.Split(
                new string[] { "\r\n", "\r", "\n" },
                StringSplitOptions.None
            );
            // split rows into individual column entries, put into a 2D string array
            string[][] geoReadyArray = new string[splitReturn.Count()][];
            for (int r = 0; r <= splitReturn.Count() - 1; r++)
            {
                String line = splitReturn[r];
                geoReadyArray[r] = line.Split(",");
            }
            return geoReadyArray;
        }

        public static geoJson csvArrayTogeoJson(string[][] geoReadyArray)
        {
                        geoJson geoJ = new geoJson();
            geoJ.type = "FeatureCollection";
            geoJ.features = new List<Feature>();

            for (int i = 2; i < geoReadyArray.GetLength(0) - 1; i++)
            {

                if (string.IsNullOrEmpty(geoReadyArray[i][5]))
                {
                    System.Console.WriteLine(geoReadyArray[i][1] + " skipped");
                    continue;
                }


                var coOrd = new Geometry();
                coOrd.coordinates = new List<double>();
                coOrd.coordinates.Add(Convert.ToDouble(geoReadyArray[i][5]));
                coOrd.coordinates.Add(Convert.ToDouble(geoReadyArray[i][6]));
                coOrd.type = "Point";

                var props = new Properties();
                props.marketId = Convert.ToInt16(geoReadyArray[i][0]);
                props.countrCode = geoReadyArray[i][2];

                string accentedStr = geoReadyArray[i][1];
                props.city = accentedStr;

                Feature feat = new Feature();
                feat.type = "Feature";
                feat.geometry = coOrd;
                feat.properties = props;

                geoJ.features.Add(feat);
                System.Console.WriteLine(geoReadyArray[i][1] + " added");

            }
            return geoJ;
        }

        public static void writeGeoJsonFile(geoJson writable)
        {
            JsonSerializerOptions options = new() { WriteIndented = true };
            string jsonString = JsonSerializer.Serialize<geoJson>(writable, options);
            File.WriteAllText(geoJsonFilePath, jsonString);
            Console.WriteLine("done");
        }
        public static string csvFilePath = "csv/mar.csv";
        public static string geoJsonFilePath = "geoJson/geoJson.json";

        public static void Main()
        {
            string csvString ="";
            try
            {
                csvString = ReadCsv(csvFilePath);
            }
            catch (Exception e) { System.Console.WriteLine(e.Message); }

            string[][] geoReadyArray = stringToArray(csvString);

            geoJson geoJsonWritable = csvArrayTogeoJson(geoReadyArray);

            writeGeoJsonFile(geoJsonWritable);

        }

    }
}